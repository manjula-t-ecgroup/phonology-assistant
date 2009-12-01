using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using SIL.FieldWorks.Common.UIAdapters;
using SIL.Pa.Controls;
using SIL.Pa.Data;
using SIL.Pa.Dialogs;
using SIL.Pa.FFSearchEngine;
using SIL.Pa.Resources;
using SilUtils;
using XCore;

namespace SIL.Pa
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Form in which search patterns are defined and used for searching.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class XYChartVw : UserControl, IxCoreColleague, ITabView, ISearchResultsViewHost
	{
		private const string kSavedChartsFile = "XYCharts.xml";

		private bool m_activeView;
		private bool m_initialDock = true;
		private bool m_editingSavedChartName;
		private SlidingPanel m_slidingPanel;
		private SearchResultsViewManager m_rsltVwMngr;
		private List<XYChartLayout> m_savedCharts;
		private ITMAdapter m_tmAdapter;
		private readonly string m_openClass = ResourceHelper.GetString("kstidOpenClassSymbol");
		private readonly string m_closeClass = ResourceHelper.GetString("kstidCloseClassSymbol");
		private readonly SplitterPanel m_dockedSidePanel;
		private readonly XYGrid m_xyGrid;
		private readonly Keys m_saveChartHotKey = Keys.None;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public XYChartVw()
		{
			PaApp.InitializeProgressBarForLoadingView(Properties.Resources.kstidXYChartsViewText, 6);
			InitializeComponent();
			Name = "XYChartVw";
			PaApp.IncProgressBar();

			hlblSavedCharts.TextFormatFlags &= ~TextFormatFlags.HidePrefix;

			m_xyGrid = new XYGrid();
			m_xyGrid.OwningView = this;
			m_xyGrid.Dock = DockStyle.Fill;
			m_xyGrid.TabIndex = lblChartName.TabIndex + 1;
			m_xyGrid.KeyDown += m_xyGrid_KeyDown;
			m_xyGrid.CellMouseDoubleClick += m_xyGrid_CellMouseDoubleClick;
			PaApp.IncProgressBar();

			LoadToolbarAndContextMenus();
			m_xyGrid.TMAdapter = m_tmAdapter;
			m_xyGrid.OwnersNameLabelControl = lblChartNameValue;
			PaApp.IncProgressBar();

			SetupSidePanelContents();
			PaApp.IncProgressBar();
			SetupSlidingPanel();
			OnPaFontsChanged(null);
			PaApp.IncProgressBar();

			m_dockedSidePanel = (m_slidingPanel.SlideFromLeft ? splitOuter.Panel1 : splitOuter.Panel2);
			
			LoadSettings();
			m_xyGrid.Reset();
			splitChart.Panel1.Controls.Add(m_xyGrid);
			m_xyGrid.BringToFront();
			splitChart.Panel1MinSize = m_xyGrid.Top + 10;
			splitChart.Panel2Collapsed = true;
			PaApp.IncProgressBar();

			UpdateButtons();
			PaApp.UninitializeProgressBar();

			base.DoubleBuffered = true;
			Disposed += ViewDisposed;

			TMItemProperties itemProps = m_tmAdapter.GetItemProperties("tbbSaveChartOnMenu");
			if (itemProps != null)
				m_saveChartHotKey = itemProps.ShortcutKey;

			lblChartNameValue.Left = lblChartName.Right + 10;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		void ViewDisposed(object sender, EventArgs e)
		{
			Disposed -= ViewDisposed;

			if (m_xyGrid != null && !m_xyGrid.IsDisposed)
				m_xyGrid.Dispose();

			if (ptrnBldrComponent != null && !ptrnBldrComponent.IsDisposed)
				ptrnBldrComponent.Dispose();

			if (m_rsltVwMngr != null)
				m_rsltVwMngr.Dispose();

			if (splitOuter != null && !splitOuter.IsDisposed)
				splitOuter.Dispose();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadToolbarAndContextMenus()
		{
			if (m_tmAdapter != null)
				m_tmAdapter.Dispose();

			m_tmAdapter = AdapterHelper.CreateTMAdapter();

			if (m_rsltVwMngr != null)
				m_rsltVwMngr.TMAdapter = m_tmAdapter;
			else
				m_rsltVwMngr = new SearchResultsViewManager(this, m_tmAdapter, splitResults, rtfRecVw);

			if (m_tmAdapter == null)
				return;

			m_tmAdapter.LoadControlContainerItem += m_tmAdapter_LoadControlContainerItem;

			string[] defs = new string[1];
			defs[0] = Path.Combine(Application.StartupPath, "XYChartsTMDefinition.xml");
			m_tmAdapter.Initialize(this, PaApp.MsgMediator, PaApp.ApplicationRegKeyPath, defs);
			m_tmAdapter.AllowUpdates = true;
			m_tmAdapter.SetContextMenuForControl(m_xyGrid, "cmnuXYChart");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Hand off a couple of drop-down controls to the toobar/menu adapter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private Control m_tmAdapter_LoadControlContainerItem(string name)
		{
			if (name == "tbbSearchOptionsDropDown")
				return m_xyGrid.SearchOptionsDropDown;

			if (name == "tbbAdjustPlaybackSpeed")
				return m_rsltVwMngr.PlaybackSpeedAdjuster;

			return null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// There's a problem with Ctrl+S (save chart) getting recognized when there is a
		/// search result word list showing. Therefore, we trap it at a lower level.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			// Make sure the user pressed the hotkey for saving a chart and
			// that he isn't in the middle of editing a saved chart name.
			if (msg.Msg == 0x100 && keyData == m_saveChartHotKey && !m_editingSavedChartName)
			{
				// Make sure the button is enabled.
				TMItemProperties itemProps = m_tmAdapter.GetItemProperties("tbbSaveChart");
				if (itemProps != null && itemProps.Enabled)
				{
					PaApp.MsgMediator.SendMessage("SaveChart", null);
					return true;
				}
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the view's result view manager.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SearchResultsViewManager ResultViewManger
		{
			get { return m_rsltVwMngr; }
		}

		#region Loading and saving charts
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadSavedChartsList()
		{
			string filename = PaApp.Project.ProjectPathFilePrefix + kSavedChartsFile;

			m_savedCharts = SilUtils.Utils.DeserializeData(filename,
				typeof(List<XYChartLayout>)) as List<XYChartLayout>;

			if (m_savedCharts == null)
				return;

			foreach (XYChartLayout layout in m_savedCharts)
			{
				ListViewItem item = new ListViewItem(layout.Name);
				item.Tag = layout;
				lvSavedCharts.Items.Add(item);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SaveCharts()
		{
			if (m_savedCharts != null)
			{
				string filename = PaApp.Project.ProjectPathFilePrefix + kSavedChartsFile;
				SilUtils.Utils.SerializeData(filename, m_savedCharts);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Searches the collection of saved chart layouts and returns the one whose name
		/// is that of the one specified. If the layout cannot be found, null is returned.
		/// </summary>
		/// <param name="layoutToSkip">Layout to skip as the collection of saved
		/// layouts is searched for the one having the specified name.</param>
		/// <param name="nameToCheck">The name of the saved layout to search for.</param>
		/// <param name="showMsg">true to show a message box if the layout cannot be
		/// found. Otherwise, false.</param>
		/// ------------------------------------------------------------------------------------
		private XYChartLayout GetExistingLayoutByName(XYChartLayout layoutToSkip,
			string nameToCheck, bool showMsg)
		{
			// Check if chart name already exists. If it does,
			// tell the user and don't cancel the current edit.
			foreach (XYChartLayout savedLayout in m_savedCharts)
			{
				if (savedLayout != layoutToSkip && savedLayout.Name == nameToCheck)
				{
					if (showMsg)
					{
						string msg = string.Format(
							Properties.Resources.kstidSavedChartNameAlreadyExistsMsg, nameToCheck);
						SilUtils.Utils.STMsgBox(msg);
					}
					
					return savedLayout;
				}
			}

			return null;
		}

		#endregion

		#region Methods for setting up side panel
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SetupSidePanelContents()
		{
			ptrnBldrComponent.ConPickerClickedHandler = HandleVowConClicked;
			ptrnBldrComponent.ConPickerDragHandler = HandleItemDrag;
			ptrnBldrComponent.VowPickerClickedHandler = HandleVowConClicked;
			ptrnBldrComponent.VowPickerDragHandler = HandleItemDrag;
			ptrnBldrComponent.OtherCharDragHandler = HandleItemDrag;
			ptrnBldrComponent.OtherCharPickedHandler = HandleCharExplorerCharPicked;
			ptrnBldrComponent.FeatureListsItemDragHandler = HandleItemDrag;
			ptrnBldrComponent.FeatureListsKeyPressHandler = HandleFeatureListKeyPress;
			ptrnBldrComponent.FeatureListDoubleClickHandler = HandleFeatureListCustomDoubleClick;
			ptrnBldrComponent.ClassListItemDragHandler = HandleItemDrag;
			ptrnBldrComponent.ClassListKeyPressHandler = HandleClassListKeyPress;
			ptrnBldrComponent.ClassListDoubleClickHandler = HandleClassListDoubleClick;

			ptrnBldrComponent.Initialize();

			LoadSavedChartsList();

			btnAutoHide.Left = btnDock.Left = (pnlSideBarCaption.Width - btnDock.Width - 6);
			btnAutoHide.Visible = false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SetupSlidingPanel()
		{
			pnlSideBarCaption.Height = FontHelper.UIFont.Height + 7;
			pnlSideBarCaption.Font = FontHelper.UIFont;
			pnlSideBarCaption.Text =
				Properties.Resources.kstidXYChartsSliderPanelText.Replace(" & ", " && ");

			btnAutoHide.Top = ((pnlSideBarCaption.Height - btnAutoHide.Height) / 2) - 1;
			btnDock.Top = btnAutoHide.Top;

			m_slidingPanel = new SlidingPanel(Properties.Resources.kstidXYChartsSliderPanelText,
				this, splitSideBarOuter, pnlSliderPlaceholder, Name);

			Controls.Add(m_slidingPanel);
			splitOuter.BringToFront();
		}

		#endregion

		#region Loading and saving settings
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadSettings()
		{
			ptrnBldrComponent.LoadSettings(Name);

			bool sidePanelDocked = PaApp.SettingsHandler.GetBoolSettingsValue(Name,
				"sidepaneldocked", true);

			if (sidePanelDocked)
				btnDock_Click(null, null);
			else
				btnAutoHide_Click(null, null);

			OnViewDocked(this);
			m_initialDock = true;

			m_rsltVwMngr.RecordViewOn = PaApp.SettingsHandler.GetBoolSettingsValue(Name,
				"recordpanevisible", true);

			// Hide the record view pane until the first search, at which time the value of
			// m_histogramOn will determine whether or not to show the record view pane.
			splitResults.Panel2Collapsed = true;
		}

		#endregion

		#region ITabView Members
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool ActiveView
		{
			get { return m_activeView; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void SetViewActive(bool makeActive, bool isDocked)
		{
			m_activeView = makeActive;

			if (m_activeView && isDocked && m_rsltVwMngr.CurrentViewsGrid != null &&
				m_rsltVwMngr.CurrentViewsGrid.Focused)
			{
				m_rsltVwMngr.CurrentViewsGrid.SetStatusBarText();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Form OwningForm
		{
			get { return FindForm(); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the view's toolbar/menu adapter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public ITMAdapter TMAdapter
		{
			get { return m_tmAdapter; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnBeginViewUnDocking(object args)
		{
			if (args == this)
				SaveSettings();

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void SaveSettings()
		{
			if (m_slidingPanel.SlideFromLeft)
			{
				PaApp.SettingsHandler.SaveSettingsValue(Name, "sidepaneldocked",
					!splitOuter.Panel1Collapsed);
			}
			else
			{
				PaApp.SettingsHandler.SaveSettingsValue(Name, "sidepaneldocked",
					!splitOuter.Panel2Collapsed);
			}

			ptrnBldrComponent.SaveSettings(Name);
			PaApp.SettingsHandler.SaveSettingsValue(Name, "recordpanevisible",
				m_rsltVwMngr.RecordViewOn);

			try
			{
				// These are in a try/catch because sometimes they might throw an exception
				// in rare cases. The exception has to do with a condition in the underlying
				// .Net framework that I haven't been able to make sense of. Anyway, if an
				// exception is thrown, no big deal, the splitter distances will just be set
				// to their default values.
				float splitRatio = splitOuter.SplitterDistance / (float)splitOuter.Width;
				PaApp.SettingsHandler.SaveSettingsValue(Name, "splitratio1", splitRatio);

				splitRatio = splitResults.SplitterDistance / (float)splitResults.Height;
				PaApp.SettingsHandler.SaveSettingsValue(Name, "splitratio2", splitRatio);

				splitRatio = splitSideBarOuter.SplitterDistance / (float)splitSideBarOuter.Height;
				PaApp.SettingsHandler.SaveSettingsValue(Name, "splitratio3", splitRatio);

				splitRatio = splitChart.SplitterDistance / (float)splitChart.Height;
				PaApp.SettingsHandler.SaveSettingsValue(Name, "splitratio4", splitRatio);
			}
			catch { }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnBeginViewClosing(object args)
		{
			if (args == this)
				SaveSettings();

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnBeginViewDocking(object args)
		{
			if (args == this && IsHandleCreated)
				SaveSettings();

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnViewDocked(object args)
		{
			if (args == this)
			{
				try
				{
					// These are in a try/catch because sometimes they might throw an exception
					// in rare cases. The exception has to do with a condition in the underlying
					// .Net framework that I haven't been able to make sense of. Anyway, if an
					// exception is thrown, no big deal, the splitter distances will just be set
					// to their default values.
					float splitRatio = PaApp.SettingsHandler.GetFloatSettingsValue(Name, "splitratio1", 0.25f);
					splitOuter.SplitterDistance = (int)(splitOuter.Width * splitRatio);

					splitRatio = PaApp.SettingsHandler.GetFloatSettingsValue(Name, "splitratio2", 0.8f);
					splitResults.SplitterDistance = (int)(splitResults.Height * splitRatio);

					splitRatio = PaApp.SettingsHandler.GetFloatSettingsValue(Name, "splitratio3", 0.5f);
					splitSideBarOuter.SplitterDistance = (int)(splitSideBarOuter.Height * splitRatio);

					splitRatio = PaApp.SettingsHandler.GetFloatSettingsValue(Name, "splitratio4", 0.4f);
					splitChart.SplitterDistance = (int)(splitChart.Height * splitRatio);
				}
				catch { }

				// Don't need to load the tool bar or menus if this is the first time
				// the view was docked since that all gets done during construction.
				if (m_initialDock)
				{
					m_initialDock = false;
					m_xyGrid.Focus();
				}
				else
				{
					// The toolbar has to be recreated each time the view is removed from it's
					// (undocked) form and docked back into the main form. The reason has to
					// do with tooltips. They seem to form an attachment, somehow, with the
					// form that owns the controls the tooltip is extending. When that form
					// gets pulled out from under the tooltips, sometimes the program will crash.
					LoadToolbarAndContextMenus();
					SetToolTips();
				}
			}

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnViewUndocked(object args)
		{
			if (args == this)
				SetToolTips();

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SetToolTips()
		{
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(GetType());

			m_tooltip = new ToolTip(components);
			m_tooltip.SetToolTip(btnRemoveSavedChart, resources.GetString("btnRemoveSavedChart.ToolTip"));

			btnAutoHide.SetToolTips();
			btnDock.SetToolTips();
		}

		#endregion

		#region Side Panel button click and update handlers
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Dock the side panel.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnDock_Click(object sender, EventArgs e)
		{
			// Swap which buttons are visible in the side panel's caption area.
			btnAutoHide.Visible = true;
			btnDock.Visible = false;

			// Uncollapse the panel in which the side panel will be docked.
			if (m_slidingPanel.SlideFromLeft)
				splitOuter.Panel1Collapsed = false;
			else
				splitOuter.Panel2Collapsed = false;

			// Let the sliding panel control handle the rest of the docking procedure.
			m_slidingPanel.DockControl(m_dockedSidePanel);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Undock the side panel.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnAutoHide_Click(object sender, EventArgs e)
		{
			// Swap which buttons are visible in the side panel's caption area.
			btnAutoHide.Visible = false;
			btnDock.Visible = true;

			// Let the sliding panel control handle most of the undocking/hiding procedure.
			m_slidingPanel.UnDockControl(m_dockedSidePanel);

			// Collapse the panel in which the side panel was docked.
			if (m_slidingPanel.SlideFromLeft)
				splitOuter.Panel1Collapsed = true;
			else
				splitOuter.Panel2Collapsed = true;
		}

		#endregion

		#region XYGrid event methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Perform a search when the user clicks on a cell containing a count.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		void m_xyGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			int col = e.ColumnIndex;
			int row = e.RowIndex;

			if (m_xyGrid.IsEmpty || col <= 0 || row <= 0 || m_xyGrid[col, row].Value is XYChartException)
				return;

			if (m_xyGrid.IsCurrentCellValidForSearch)
				Search(row, col, SearchResultLocation.CurrentTabGroup);
			else
				SilUtils.Utils.STMsgBox(Properties.Resources.kstidXYChartFillChartMsg);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Perform a search when the user presses enter on a cell.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		void m_xyGrid_KeyDown(object sender, KeyEventArgs e)
		{
			Point pt = m_xyGrid.CurrentCellAddress;

			if (e.KeyCode != Keys.Enter || e.Modifiers != Keys.None || m_xyGrid.IsEmpty ||
				pt.X <= 0 || pt.Y <= 0 || pt.X == m_xyGrid.Columns.Count - 1 ||
				pt.Y == m_xyGrid.NewRowIndex)
			{
				return;
			}

			if (m_xyGrid.IsCurrentCellValidForSearch)
				Search(m_xyGrid.CurrentCell, SearchResultLocation.CurrentTabGroup);
			else
				SilUtils.Utils.STMsgBox(Properties.Resources.kstidXYChartFillChartMsg);

			e.Handled = true;
		}

		#endregion

		#region Non DragDrop keyboard and mouse events for inserting text into grid
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleFeatureListCustomDoubleClick(object sender, string feature)
		{
			m_xyGrid.InsertTextInCell(feature);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the user pressing Enter on a feature.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void HandleFeatureListKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter && sender is FeatureListView)
			{	
			    string text = ((FeatureListView)sender).CurrentFormattedFeature;
				m_xyGrid.InsertTextInCell(text);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Inserts phones into the search pattern when they're clicked on in the character
		/// explorer on the "Other" tab.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleCharExplorerCharPicked(CharPicker picker, ToolStripButton item)
		{
			m_xyGrid.InsertTextInCell(item.Text.Replace(DataUtils.kDottedCircle, string.Empty));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Inserts phones into the search pattern when they're clicked on from the vowel or
		/// consonant tab.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleVowConClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			m_xyGrid.InsertTextInCell(e.ClickedItem.Text);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// When user double-clicks a class name, then put user in insert mode to insert that
		/// class in the pattern.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleClassListDoubleClick(object sender, MouseEventArgs e)
		{
			ClassListView lv = ptrnBldrComponent.ClassListView;

			if (lv.SelectedItems.Count > 0)
			{
				ClassListViewItem item = lv.SelectedItems[0] as ClassListViewItem;
				if (item != null)
				{
					m_xyGrid.InsertTextInCell((
						item.Pattern == null || PaApp.Project.ShowClassNamesInSearchPatterns ?
						m_openClass + item.Text + m_closeClass : item.Pattern));
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// When the user presses enter on a class, then put the user in the insert mode to
		/// insert that class in the pattern.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleClassListKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
				HandleClassListDoubleClick(null, null);
		}

		#endregion

		#region Methods for dragging and dropping items to search pattern panels
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Begin dragging something to insert into a search item or environment cell.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleItemDrag(object sender, ItemDragEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			string dragText = null;

			if (e.Item is string)
				dragText = e.Item as string;
			else if (e.Item is CharGridCell)
				dragText = ((CharGridCell)e.Item).Phone;
			else if (sender is FeatureListView)
				dragText = ((FeatureListView)sender).CurrentFormattedFeature;
			else if (e.Item is ClassListViewItem)
			{
				ClassListViewItem item = e.Item as ClassListViewItem;
				if (item != null)
				{
					dragText = (item.Pattern == null || PaApp.Project.ShowClassNamesInSearchPatterns ?
						m_openClass + item.Text + m_closeClass : item.Pattern);
				}
			}

			// Any text we begin dragging.
			if (dragText != null)
			{
				if (m_slidingPanel.Visible)
					m_slidingPanel.Close(true);
	
				DoDragDrop(dragText.Replace(DataUtils.kDottedCircle, string.Empty),
					DragDropEffects.Copy);
			}
		}

		#endregion
		
		#region Event methods for saved charts list view
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Remove the selected saved chart when the remove button is pressed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnRemoveSavedChart_Click(object sender, EventArgs e)
		{
			if (lvSavedCharts.SelectedItems == null || lvSavedCharts.SelectedItems.Count == 0)
				return;

			string msg = Properties.Resources.kstidConfirmSavedChartRemoval;
			if (SilUtils.Utils.STMsgBox(msg, MessageBoxButtons.YesNo) == DialogResult.No)
				return;

			XYChartLayout layout = lvSavedCharts.SelectedItems[0].Tag as XYChartLayout;

			if (layout != null)
			{
				if (layout == m_xyGrid.ChartLayout ||
					(m_xyGrid.ChartLayout != null && layout.Name == m_xyGrid.ChartLayout.Name))
				{
					// Don't delete the m_xyGrid.ChartLayout if the saved chart name is in edited mode
					if (layout.Name != null)
					{
						m_xyGrid.Reset();
						m_xyGrid.ChartLayout = null;
					}
				}

				m_savedCharts.Remove(layout);
				int index = lvSavedCharts.SelectedIndices[0];
				lvSavedCharts.Items.Remove(lvSavedCharts.SelectedItems[0]);
				if (index >= lvSavedCharts.Items.Count)
					index--;

				if (index >= 0)
					lvSavedCharts.Items[index].Selected = true;
	
				SaveCharts();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Remove the selected saved chart when the delete key is pressed. Load the saved
		/// chart into the grid when pressing enter and put the user into the edit mode
		/// in the saved charts list when he presses F2.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void lvSavedCharts_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers != Keys.None)
				return;

			if (lvSavedCharts.SelectedItems != null && lvSavedCharts.SelectedItems.Count > 0)
			{
				if (e.KeyCode == Keys.Delete)
				{
					btnRemoveSavedChart_Click(null, null);
					e.Handled = true;
				}
				else if (e.KeyCode == Keys.F2)
				{
					lvSavedCharts.SelectedItems[0].BeginEdit();
					e.Handled = true;
				}
				else if (e.KeyCode == Keys.Enter)
				{
					LoadSavedLayout(lvSavedCharts.SelectedItems[0]);
					e.Handled = true;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// When double-clicking on a saved chart, that chart will be loaded in the grid.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void lvSavedCharts_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// When a saved chart is double-clicked on, load it into the grid.
			ListViewHitTestInfo hinfo = lvSavedCharts.HitTest(e.Location);
			LoadSavedLayout(hinfo.Item);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the saved item represented by the item in the saved item's list view.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadSavedLayout(ListViewItem item)
		{
			if (item == null)
				return;
			
			if (m_slidingPanel.Visible)
				m_slidingPanel.Close(true);

			m_xyGrid.LoadFromLayout(item.Tag as XYChartLayout);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void lvSavedCharts_BeforeLabelEdit(object sender, LabelEditEventArgs e)
		{
			m_editingSavedChartName = true;
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Verify the new saved chart's name is not a duplicate. If not, then save the
		/// change to disk.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void lvSavedCharts_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			string newName = (e.Label != null ? e.Label.Trim() : null);
			if (string.IsNullOrEmpty(newName))
				newName = null;

			XYChartLayout layout = lvSavedCharts.Items[e.Item].Tag as XYChartLayout;
			if (layout == null || layout.Name == newName || newName == null)
			{
				e.CancelEdit = true;
				m_editingSavedChartName = false;
				return;
			}

			// Check if chart name already exists. If it does, cancel the current edit.
			if (GetExistingLayoutByName(layout, newName, true) != null)
			{
				e.CancelEdit = true;
				return;
			}

			// If the chart loaded in the grid is the one whose name was just edited,
			// then update the loaded name and the label above the grid.
			if (m_xyGrid.ChartLayout != null &&
				m_xyGrid.ChartLayout.Name == lvSavedCharts.Items[e.Item].Text)
			{
				m_xyGrid.ChartLayout.Name = newName;
				lblChartNameValue.Text = newName;
			}

			// Keep the new name and save the change to disk.
			layout.Name = newName;
			SaveCharts();

			lvSavedCharts.Items[e.Item].Text = newName;
			e.CancelEdit = true;
			m_editingSavedChartName = false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void lvSavedCharts_ItemDrag(object sender, ItemDragEventArgs e)
		{
			ListViewItem item = e.Item as ListViewItem;
			if (e.Button != MouseButtons.Left || item == null || item.Tag == null ||
				!(item.Tag is XYChartLayout))
				return;

			if (m_slidingPanel.Visible)
				m_slidingPanel.Close(true);

			DoDragDrop(item.Tag as XYChartLayout, DragDropEffects.Move);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Makes sure the list has a selected item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void lvSavedCharts_Enter(object sender, EventArgs e)
		{
			// Make sure an item is selected when the list gets focus. Probably the only
			// time the list will get focus and not have a selected item is the first
			// time the list gains focus after the view has been loaded.
			if (lvSavedCharts.SelectedIndices.Count == 0 && lvSavedCharts.Items.Count > 0)
				lvSavedCharts.Items[0].Selected = true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Make sure the only column in the saved charts list is the same with as its
		/// owning list view. Also make sure the list view's size fills the panel underneath
		/// the header label.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void pnlSavedCharts_Resize(object sender, EventArgs e)
		{
			// Save the index of the selected item.
			int i = (lvSavedCharts.SelectedIndices != null &&
				lvSavedCharts.SelectedIndices.Count > 0 ? lvSavedCharts.SelectedIndices[0] : 0);

			// This is sort of a kludge, but it's necessary due to the possiblity that
			// the list view's column header will change size. It turns out that if there
			// were any items scrolled off the top of the list and the column header is
			// resized and the new size of the list view will cause the vertical scroll
			// bar to go away, there will be one or more blank lines at the top of the
			// list view. Making sure the first item is visible before changing the column
			// header's size will prevent this. See PA-676.
			if (lvSavedCharts.Items.Count > 0)
				lvSavedCharts.EnsureVisible(0);

			// Make sure the list view fills the panel it's (accounting for the fact that
			// it's also in the panel underneath the hlblSaveCharts control).
			lvSavedCharts.Size = new Size(pnlSavedCharts.ClientSize.Width,
				pnlSavedCharts.ClientSize.Height - hlblSavedCharts.Size.Height);

			// Resize the list view's colum header so it fits just
			// inside the list view's client area.
			if (hdrSavedCharts.Width != lvSavedCharts.ClientSize.Width - 3)
				hdrSavedCharts.Width = lvSavedCharts.ClientSize.Width - 3;

			// Make sure the previously selected item is visible.
			if (i >= 0 && lvSavedCharts.Items.Count > 0)
				lvSavedCharts.EnsureVisible(i);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void lvSavedCharts_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			UpdateButtons();
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void UpdateButtons()
		{
			btnRemoveSavedChart.Enabled = (lvSavedCharts.SelectedItems != null &&
				lvSavedCharts.SelectedItems.Count > 0);
		}
		
		#endregion

		#region Searching methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Perform a search for the specified cell.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void Search(DataGridViewCell cell, SearchResultLocation resultLocation)
		{
			if (cell != null)
				Search(cell.RowIndex, cell.ColumnIndex, resultLocation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Perform a search for the cell specified by row and col.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void Search(int row, int col, SearchResultLocation resultLocation)
		{
			if (!m_xyGrid.IsCellValidForSearch(row, col))
				return;

			SearchQuery query = m_xyGrid.GetCellsFullSearchQuery(row, col);
			if (query != null)
				m_rsltVwMngr.PerformSearch(query, resultLocation);
		}

		#endregion

		#region ISearchResultsViewHost Members
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void BeforeSearchPerformed(SearchQuery query, WordListCache resultCache)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void AfterSearchPerformed(SearchQuery query, WordListCache resultCache)
		{
			if (resultCache != null && splitChart.Panel2Collapsed)
			{
				splitChart.Panel2Collapsed = false;
				if (splitChart.SplitterDistance < splitChart.Panel1.Padding.Top * 2.5)
					splitChart.SplitterDistance = (int)(splitChart.Panel1.Padding.Top * 2.5);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool ShouldMenuBeEnabled(string menuName)
		{
			return m_xyGrid.IsCurrentCellValidForSearch;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SearchQuery GetQueryForMenu(string menuName)
		{
			return m_xyGrid.CurrentCellsFullSearchQuery;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This gets called when all the tabs or tab groups have been closed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void NotifyAllTabsClosed()
		{
			// When there are no more search results showing,
			// then close the pane that holds them.
			splitChart.Panel2Collapsed = true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This gets called when the current tab has changed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void NotifyCurrentTabChanged(SearchResultTab newTab)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the find dialog.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void ShowFindDlg(PaWordListGrid grid)
		{
			if (!FindInfo.FindDlgIsOpen)
			{
				FindDlg findDlg = new FindDlg(grid);
				findDlg.Show();
			}
		}

		#endregion

		#region Message mediator message handler and update handler methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Compares the grid sent in args with the current result view grid.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnCompareGrid(object args)
		{
			PaWordListGrid grid = args as PaWordListGrid;
			return (grid != null && m_rsltVwMngr.CurrentViewsGrid == grid);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This method gets called when the font(s) get changed in the options dialog.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnPaFontsChanged(object args)
		{
			lvSavedCharts.Font = FontHelper.PhoneticFont;
			hlblSavedCharts.Font = FontHelper.UIFont;
			lblChartName.Font = FontHelper.UIFont;
			lblChartNameValue.Font = FontHelper.MakeFont(FontHelper.PhoneticFont, FontStyle.Bold);

			int lblHeight = Math.Max(lblChartName.Height, lblChartNameValue.Height);
			int padding = lblHeight + 14;

			splitChart.Panel1.Padding = new Padding(splitChart.Panel1.Padding.Left, padding,
				splitChart.Panel1.Padding.Right, splitChart.Panel1.Padding.Bottom);

			// Center the labels vertically.
			lblChartName.Top =
				(int)Math.Ceiling((padding - lblChartName.Height) / 2f) + 1;

			lblChartNameValue.Top =
				(int)Math.Ceiling((padding - lblChartNameValue.Height) / 2f);

			rtfRecVw.UpdateFonts();
			ptrnBldrComponent.RefreshFonts();
			m_slidingPanel.RefreshFonts();

			// Return false to allow other windows to update their fonts.
			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnDataSourcesModified(object args)
		{
			ptrnBldrComponent.RefreshComponents();
			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Fills the grid with search result totals.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnFillInChart(object args)
		{
			if (!m_activeView)
				return false;

			m_xyGrid.FillChart();
			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Fills the grid with search result totals.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnUpdateFillInChart(object args)
		{
			TMItemProperties itemProps = args as TMItemProperties;
			if (itemProps == null || !m_activeView)
				return false;

			if (itemProps.Enabled == m_xyGrid.IsEmpty)
			{
				itemProps.Visible = true;
				itemProps.Enabled = !m_xyGrid.IsEmpty;
				itemProps.Update = true;
			}
			
			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnSaveChart(object args)
		{
			if (!m_activeView)
				return false;

			// Commit changes and end the edit mode if necessary. Fixes PA-714
			if (m_xyGrid.IsCurrentCellInEditMode)
				m_xyGrid.EndEdit();

			if (!m_xyGrid.IsDirty)
				return false;

			// If the name isn't specified, then use the save as dialog.
			if (string.IsNullOrEmpty(m_xyGrid.ChartName))
				return OnSaveChartAs(args);

			SaveCurrentChart(m_xyGrid.ChartLayout);
			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnSaveChartAs(object args)
		{
			if (!m_activeView)
				return false;

			// Commit changes and end the edit mode if necessary. Fixes PA-714
			if (m_xyGrid.IsCurrentCellInEditMode)
				m_xyGrid.EndEdit();

			using (SaveXYChartDlg dlg = new SaveXYChartDlg(m_xyGrid, m_savedCharts))
			{
				if (dlg.ShowDialog() == DialogResult.OK)
					SaveCurrentChart(dlg.LayoutToOverwrite);
			}

			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Saves the current layout to the project file.
		/// </summary>
		/// <param name="layoutToOverwrite">The layout to overwrite when saving. This should
		/// be null if the layout is to be added to the list of saved layouts.</param>
		/// ------------------------------------------------------------------------------------
		private void SaveCurrentChart(XYChartLayout layoutToOverwrite)
		{
			ListViewItem item = null;

			// When the user wants to overwrite an existing layout, we need to find the
			// item in the saved list that corresponds to the one being overwritten.
			if (layoutToOverwrite != null)
			{
				foreach (ListViewItem lvi in lvSavedCharts.Items)
				{
					XYChartLayout tmpLayout = lvi.Tag as XYChartLayout;
					if (tmpLayout != null && tmpLayout.Name == layoutToOverwrite.Name)
					{
						item = lvi;
						break;
					}
				}
			}

			if (m_savedCharts == null)
				m_savedCharts = new List<XYChartLayout>();
			
			XYChartLayout layoutCopy = m_xyGrid.ChartLayout.Clone();

			if (item != null)
			{
				// Overwrite an existing layout.
				int i = m_savedCharts.IndexOf(item.Tag as XYChartLayout);
				m_savedCharts[i] = layoutCopy;
				item.Tag = layoutCopy;
				item.Text = layoutCopy.Name;
			}
			else
			{
				// Save a new layout.
				m_savedCharts.Add(layoutCopy);
				item = new ListViewItem(layoutCopy.Name);
				item.Tag = layoutCopy;
				lvSavedCharts.Items.Add(item);
			}

			SaveCharts();
			m_xyGrid.LoadFromLayout(layoutCopy);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnUpdateSaveChart(object args)
		{
			TMItemProperties itemProps = args as TMItemProperties;
			if (itemProps == null || !m_activeView)
				return false;

			if (itemProps.Enabled != (m_xyGrid.ChartLayout != null))
			{
				itemProps.Visible = true;
				itemProps.Enabled = (m_xyGrid.ChartLayout != null);
				itemProps.Update = true;
			}
			
			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Performs a search.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnBeginSearch(object args)
		{
			if (!m_activeView)
				return false;

			TMItemProperties itemProps = args as TMItemProperties;
			if (itemProps != null && itemProps.Name.StartsWith("cmnu"))
				return false;

			Search(m_xyGrid.CurrentCell, SearchResultLocation.CurrentTabGroup);
			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnUpdateBeginSearch(object args)
		{
			if (!m_activeView)
				return false;

			TMItemProperties itemProps = args as TMItemProperties;
			if (itemProps != null && !itemProps.Name.StartsWith("cmnu"))
			{
				if (itemProps.Enabled != m_xyGrid.IsCurrentCellValidForSearch)
				{
					itemProps.Visible = true;
					itemProps.Enabled = m_xyGrid.IsCurrentCellValidForSearch;
					itemProps.Update = true;
				}
				
				return true;
			}

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Display the RtfExportDlg form.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnExportAsRTF(object args)
		{
			if (!m_activeView)
				return false;

			RtfExportDlg rtfExp = new RtfExportDlg(m_rsltVwMngr.CurrentViewsGrid);
			rtfExp.ShowDialog(this);
			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnUpdateExportAsRTF(object args)
		{
			TMItemProperties itemProps = args as TMItemProperties;
			if (itemProps == null || !m_activeView)
				return false;

			bool enable = (m_rsltVwMngr.CurrentViewsGrid != null &&
				m_rsltVwMngr.CurrentViewsGrid.Focused);

			if (itemProps.Enabled != enable)
			{
				itemProps.Update = true;
				itemProps.Visible = true;
				itemProps.Enabled = enable;
			}

			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnExportAsHTML(object args)
		{
			if (!m_activeView)
				return false;

			string outputFileName;
			object objForExport = ObjectForHTMLExport;

			// Determine whether to export the XY Chart or a search result word list.
			if (!(objForExport is XYGrid))
				outputFileName = m_rsltVwMngr.HTMLExport();
			else
			{
				string defaultHTMLFileName = string.Format(
					Properties.Resources.kstidXYChartHTMLFileName,
					PaApp.Project.Language, m_xyGrid.ChartName);

				outputFileName = HTMLXYChartWriter.Export(m_xyGrid, defaultHTMLFileName,
					Properties.Resources.kstidXYChartHTMLChartType, m_xyGrid.ChartName);
			}

			if (outputFileName == null)
				return false;

			if (File.Exists(outputFileName))
				LaunchHTMLDlg.PostExportProcess(FindForm(), outputFileName);

			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnUpdateExportAsHTML(object args)
		{
			TMItemProperties itemProps = args as TMItemProperties;
			if (itemProps == null || !m_activeView)
				return false;

			bool enable = (ObjectForHTMLExport != null);
			itemProps.Enabled = enable;
			itemProps.Visible = true;
			itemProps.Update = true;
			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines which object in the view should be exported to HTML, the XY chart grid,
		/// or one of the search result word lists, if there are any.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private object ObjectForHTMLExport
		{
			get
			{
				// If a search result grid has focus, it wins the contest.
				if (m_rsltVwMngr.CurrentViewsGrid != null && m_rsltVwMngr.CurrentViewsGrid.Focused)
					return m_rsltVwMngr.CurrentViewsGrid;

				// Otherwise the grid does if it's not empty.
				return (!m_xyGrid.IsEmpty ? m_xyGrid : null);
			}
		}

		#endregion

		#region IxCoreColleague Members
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Never used in PA.
		/// </summary>
		/// <param name="mediator"></param>
		/// <param name="configurationParameters"></param>
		/// ------------------------------------------------------------------------------------
		public void Init(Mediator mediator, XmlNode configurationParameters)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the message target.
		/// </summary>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public IxCoreColleague[] GetMessageTargets()
		{
			return new IxCoreColleague[] { this };
		}

		#endregion

	}
}