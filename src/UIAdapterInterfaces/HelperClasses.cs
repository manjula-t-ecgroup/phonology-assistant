// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2003, SIL International. All Rights Reserved.   
// <copyright from='2003' to='2003' company='SIL International'>
//		Copyright (c) 2003, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: HelperClasses.cs
// Responsibility: TE Team
// Last reviewed: 
// 
// <remarks>
// </remarks>
// --------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using XCore;

namespace SIL.FieldWorks.Common.UIAdapters
{
	/// <summary>Handler for allowing applications to initialize a toolbar.</summary>
	public delegate void InitializeBarHandler(ref TMBarProperties barProps);

	/// <summary>Handler for allowing applications to initialize a toolbar item.</summary>
	public delegate void InitializeItemHandler(ref TMItemProperties itemProps);

	/// <summary>Handler for allowing applications a chance to set the contents of a combobox item.</summary>
	public delegate void InitializeComboItemHandler(string name, ComboBox cboItem);
	
	/// <summary>Handler for allowing applications to respond to a toolbar's request for a
	/// control container control.</summary>
	public delegate Control LoadControlContainerItemHandler(string name);

	/// <summary>Handler for requesting toolbar information from an application so a menu
	/// adapter can provide menu items to toggle the visibility of toolbars.</summary>
	public delegate TMItemProperties[] GetBarInfoForViewMenuHandler();

	/// <summary>Handler for allowing applications to respond
	/// to a recently used file being chosen.</summary>
	public delegate void RecentlyUsedItemChosenHandler(string filename);

	#region TMItemProperties Class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// This class is used to pass information back and forth between a ToolBarAdapter object
	/// and toolbar item command handlers for click and update.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class TMItemProperties
	{
		private bool m_update = false;
		private string m_name = string.Empty;
		private string m_text = string.Empty;
		private string m_category = string.Empty;
		private string m_tooltip = string.Empty;
		private string m_commandId = string.Empty;
		private string m_message = string.Empty;
		private string m_originalText = string.Empty;
		//private string m_statusMsg = string.Empty;
		//private string m_shortcut = "None";
		private bool m_enabled = true;
		private bool m_checked = false;
		private bool m_visible = true;
		private bool m_beginGroup = false;
		//private bool m_isImageAppSpecific = false;
		//private int m_imageIndex = -1;
		private Control m_ctrl = null;
		private Image m_image = null;
		private ArrayList m_list = null;
		private Control m_parentCtrl = null;
		private object m_tag = null;
		private Size m_size = Size.Empty;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a TMItemProperties object.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public TMItemProperties()
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the name for a menu/toolbar item. Setting this value should not change
		/// an item's name within the XML definition. It is only used to be able to pass
		/// an item's name to instantiators of a toolbar adapter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Name
		{
			get {return m_name;}
			set {m_name = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the text for a menu/toolbar item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Text
		{
			get {return m_text;}
			set {m_text = value;}
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the value of the item's text as it came from the resources. Setting this
		/// value in your appication has no effect. It is ignored.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string OriginalText
		{
			get {return m_originalText;}
			set {m_originalText = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the category (e.g. "Bold" button's category would be "Format") for a
		/// menu/toolbar item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Category
		{
			get {return m_category;}
			set {m_category = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the tooltip for a menu/toolbar item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Tooltip
		{
			get {return m_tooltip;}
			set {m_tooltip = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the command Id for a menu/toolbar item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string CommandId
		{
			get {return m_commandId;}
			set {m_commandId = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the command id for a menu/toolbar item. Setting this value in your
		/// application has no effect. It is ignored.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Message
		{
			get {return m_message;}
			set {m_message = value;}
		}

//		/// ------------------------------------------------------------------------------------
//		/// <summary>
//		/// Gets or sets the status message for a menu/toolbar item.
//		/// </summary>
//		/// ------------------------------------------------------------------------------------
//		public string StatusMessage
//		{
//			get {return m_statusMsg;}
//			set {m_statusMsg = value;}
//		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the enabled state for a menu/toolbar item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Enabled
		{
			get {return m_enabled;}
			set {m_enabled = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the checked state for a menu/toolbar item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Checked
		{
			get {return m_checked;}
			set {m_checked = value;}
		}
	
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the visible state for a menu/toolbar item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Visible
		{
			get {return m_visible;}
			set {m_visible = value;}
		}
	
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether or not the item begins a group (for menu
		/// items, this would be a separator before this item. For toolbar items it would
		/// be a dividing line.)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool BeginGroup
		{
			get {return m_beginGroup;}
			set {m_beginGroup = value;}
		}

//		/// ------------------------------------------------------------------------------------
//		/// <summary>
//		/// Gets or sets the image index for a menu/toolbar item.
//		/// </summary>
//		/// ------------------------------------------------------------------------------------
//		public int ImageIndex
//		{
//			get {return m_imageIndex;}
//			set {m_imageIndex = value;}
//		}
	
//		/// ------------------------------------------------------------------------------------
//		/// <summary>
//		/// Gets or sets a value indicating whether or not the menu/toolbar image is application
//		/// specific or is defined in the FW framework. e.g. Undo, Redo, Cut, Copy, Paste, etc.
//		/// are defined in the framework whereas the image for Insert verse number would be
//		/// an application specific image.
//		/// </summary>
//		/// ------------------------------------------------------------------------------------
//		public bool IsImageAppSpecific
//		{
//			get {return m_isImageAppSpecific;}
//			set {m_isImageAppSpecific = value;}
//		}

//		/// ------------------------------------------------------------------------------------
//		/// <summary>
//		/// Gets or sets the shortcut key for a menu/toolbar item.
//		/// </summary>
//		/// ------------------------------------------------------------------------------------
//		public string Shortcut
//		{
//			get {return m_shortcut;}
//			set {m_shortcut = value;}
//		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the image for a menu/toolbar item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Image Image
		{
			get {return m_image;}
			set {m_image = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the control hosted by toolbar items whose type is ComboBox or control container.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Control Control
		{
			get {return m_ctrl;}
			set {m_ctrl = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a list of items associated with a toolbar item. For example, if the
		/// toolbar item is a combobox, this list may contain all the item's in the combobox.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public ArrayList List
		{
			get {return m_list;}
			set {m_list = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the form on which the menu/toolbar item is located. Setting this
		/// property does not set the parent form of an item. The setter is only for toolbar
		/// adapters to use.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Control ParentControl
		{
			get {return m_parentCtrl;}
			set {m_parentCtrl = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether or not the menu/toolbar item will be updated
		/// with the properties specified in the <see cref="TMItemProperties"/> after
		/// control leaves the command handler and is returned to the toolbar adapter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Update
		{
			get {return m_update;}
			set {m_update = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets any extra information needed to be passed between the application and
		/// an adapter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public object Tag
		{
			get {return m_tag;}
			set {m_tag = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the item's size
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Size Size
		{
			get { return m_size; }
			set { m_size = value; }
		}
	}

	#endregion

	#region TMBarProperties Class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// This class is used to pass information back and forth between a ToolBarAdapter object
	/// and an application.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class TMBarProperties
	{
		private bool m_update;
		private string m_name;
		private string m_text;
		private bool m_enabled;
		private bool m_visible;
		private Control m_parentCtrl;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a TMBarProperties object.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public TMBarProperties()
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a TMBarProperties object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="text"></param>
		/// <param name="enabled"></param>
		/// <param name="visible"></param>
		/// <param name="parentCtrl">The form on which the toolbar is located.</param>
		/// ------------------------------------------------------------------------------------
		public TMBarProperties(string name, string text, bool enabled, bool visible, Control parentCtrl)
		{
			m_name = name;
			m_text = text;
			m_enabled = enabled;
			m_visible = visible;
			m_parentCtrl = parentCtrl;
			m_update = false;
		}
	
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the name for a toolbar. Setting this value should not change
		/// a bar's name within the XML definition. It is only used to pass a bar's name to
		/// instantiators of a toolbar adapter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Name
		{
			get {return m_name;}
			set {m_name = value;}
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the display text for a toolbar.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Text
		{
			get {return m_text;}
			set {m_text = value;}
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the enabled state for a toolbar.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Enabled
		{
			get {return m_enabled;}
			set {m_enabled = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the visible state for a toolbar.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Visible
		{
			get {return m_visible;}
			set {m_visible = value;}
		}
	
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the control or form on which the toolbar item is located. Setting
		/// this property does not set the parent form of an item. The setter is only for
		/// toolbar adapters to use.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Control ParentControl
		{
			get {return m_parentCtrl;}
			set {m_parentCtrl = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether or not the toolbar will be updated
		/// with the properties specified in the <see cref="TMBarProperties"/> after
		/// control leaves the command handler and is returned to the toolbar adapter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Update
		{
			get {return m_update;}
			set {m_update = value;}
		}
	}

	#endregion

	#region ToolBarPopupInfo Class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Objects of this class type are used to pass information between a toolbar adapter and
	/// handlers of "DropDown" commands. DropDown commands are issued via the message mediator
	/// when a popup-type toolbar button's popup arrow is clicked.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class ToolBarPopupInfo
	{
		private string m_name;
		private Control m_ctrl;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Instantiates a new, uninitialized ToolBarPopupInfo object.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public ToolBarPopupInfo()
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Instantiates a new, initialized ToolBarPopupInfo object.
		/// </summary>
		/// <param name="name">Toolbar item's name.</param>
		/// ------------------------------------------------------------------------------------
		public ToolBarPopupInfo(string name)
		{
			m_name = name;
		}
	
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the name of the toolbar popup item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Name
		{
			get {return m_name;}
			set {m_name = value;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the control to be popped-up.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Control Control
		{
			get {return m_ctrl;}
			set {m_ctrl = value;}
		}
	}

	#endregion

	#region WindowListInfo Class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class used to send information to the update command handler for the window list
	/// item on a menu.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class WindowListInfo
	{
		/// <summary>The TMItemProperties of the first window list menu item. When the window
		/// list item's update handler is called, the update handler should fill an
		/// ArrayList of strings that will be used by the menu adapter to build each window
		/// list menu item. The array list should contain the desired menu texts (without the
		/// numbers, however). Once the ArrayList is built, it should be assigned to the List
		/// property of WindowListItemProperties.</summary>
		public TMItemProperties WindowListItemProperties;
		/// <summary>Index of the item in the ArrayList that represents the active window.
		/// This tells the menu adapter what item in the window list gets checked.</summary>
		public int CheckedItemIndex;
	}

	#endregion

	#region SBTabProperties Class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class SBTabProperties
	{
		/// <summary>Gets or sets the internal name for the tab.</summary>
		public string Name;
		/// <summary>Gets or sets the text displayed in the tab.</summary>
		public string Text = string.Empty;
		/// <summary>Gets or sets a value indicating whether or not the tab is enabled.</summary>
		public bool Enabled = true;
		/// <summary>
		/// Gets or sets the message used by the message mediator to inform IxCoreColleagues
		/// that the tab is becoming the current tab in the sidebar.
		/// </summary>
		public string Message;
		/// <summary>Gets or sets the image index used for the tab.</summary>
		public int ImageIndex = -1;
		/// <summary>Gets the collection properties for each item on the tab.</summary>
		public SBTabItemProperties[] Items;
		/// <summary>Gets or sets whether or not the tab's items should appear as small icons.</summary>
		public bool SmallIconMode = false;
		/// <summary></summary>
		public object Tag;
		/// <summary>Gets the name of the current item on the tab or null when there isn't one.</summary>
		public string CurrentTabItem = null;

		/// <summary>
		/// This string stores the format string used to build the tooltip (when the tab is
		/// current) for a tab's information bar button. When the tab is current, the tooltip
		/// will display the tab's text followed by the text of the tab's current item. When
		/// the tab isn't current then this format string isn't used and the tooltip will
		/// display the tab's text only.
		/// </summary>
		public string InfoBarButtonToolTipFormat;
		/// <summary>
		/// Gets or sets the message used by the message mediator to inform IxCoreColleagues
		/// the user clicked on the tab's configure menu item shown on the sidebar tab's
		/// context menu.
		/// </summary>
		public string ConfigureMessage;
		/// <summary>
		/// Gets or sets the tab's configure menu item text that appears on the sidebar's
		/// context menu.
		/// </summary>
		public string ConfigureMenuText;
		/// <summary>
		/// This is only used so when an application handles the update message for the
		/// configure menu, it can return whether or not the configure menu should be enabled.
		/// </summary>
		public bool ConfigureMenuEnabled = true;
		/// <summary>
		/// This is only used so when an application handles the update message for the
		/// configure menu, it can return whether or not the configure menu should be visible.
		/// </summary>
		public bool ConfigureMenuVisible = true;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SBTabProperties()
		{
			// Initialize the Id to a guid in case a more human comprehensible one isn't
			// assigned later.
			Name = Guid.NewGuid().ToString();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the properties for a single tab item.
		/// </summary>
		/// <param name="index">Index of tab item whose properties are being requested.</param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public SBTabItemProperties GetTabItemProperties(int index)
		{
			if (index < 0 || Items == null || index >= Items.Length)
				return null;

			return Items[index];
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the properties for a single tab item.
		/// </summary>
		/// <param name="name">Name of tab item whose properties are being requested.</param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public SBTabItemProperties GetTabItemProperties(string name)
		{
			foreach (SBTabItemProperties itemProps in Items)
			{
				if (itemProps.Name == name)
					return itemProps;
			}
			
			return null;
		}
	}

	#endregion

    #region SBTabItemProperties Class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class SBTabItemProperties
	{
		/// <summary></summary>
		public string Name;
		/// <summary></summary>
		public string Text = string.Empty;
		/// <summary></summary>
		public string Tooltip = string.Empty;
		/// <summary></summary>
		public string Message;
		/// <summary></summary>
		public int ImageIndex = -1;
		/// <summary></summary>
		public string OwningTabName = null;
		/// <summary></summary>
		public object Tag;
		/// <summary>true indicates that event should always be fired, even if the button
		/// was the last one clicked, false if button fires only once.</summary>
		public bool ClickAlways = false;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SBTabItemProperties()
		{
			// Initialize the Id to a guid in case a more human comprehensible one isn't
			// assigned later.
			Name = Guid.NewGuid().ToString();
		}
	}

	#endregion

	#region AdapterHelper
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class AdapterHelper
	{
		private	static Assembly m_uiAdapterAssembly = null;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the UI adapter library DLL.
		/// </summary>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		private static bool LoadUIAdapterAssembly()
		{
			if (m_uiAdapterAssembly != null)
				return true;

			string appPath = Application.StartupPath;

			try
			{
				// Load an adapter library .dll
				m_uiAdapterAssembly = Assembly.LoadFrom(Path.Combine(appPath, "UIAdapters.dll"));
			}
			catch
			{
				MessageBox.Show("Trying to load " + Path.Combine(appPath, "UIAdapters.dll") +
					". Your installation may be corrupt.", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
					MessageBoxOptions.ServiceNotification);
			}

			Debug.Assert(m_uiAdapterAssembly != null, "Could not find the adapter library DLL");
			return (m_uiAdapterAssembly != null);
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <returns>An instance of a sidebar/information bar adapter.</returns>
		/// ------------------------------------------------------------------------------------
		public static ISIBInterface CreateSideBarInfoBarAdapter()
		{
			if (RunningTests() || !LoadUIAdapterAssembly())
				return null;
			
			ISIBInterface sibAdapter = (ISIBInterface)m_uiAdapterAssembly.CreateInstance(
				"SIL.FieldWorks.Common.UIAdapters.SIBAdapter");

			Debug.Assert(sibAdapter != null, "Could not create a side bar/info. bar adapter.");
			return sibAdapter;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <returns>An instance of a menu adapter.</returns>
		/// ------------------------------------------------------------------------------------
		public static ITMAdapter CreateTMAdapter()
		{
			if (RunningTests() || !LoadUIAdapterAssembly())
				return null;

			ITMAdapter tmAdapter = (ITMAdapter)m_uiAdapterAssembly.CreateInstance(
				"SIL.FieldWorks.Common.UIAdapters.TMAdapter");
			
			Debug.Assert(tmAdapter != null, "Could not create a toolbar/menu adapter.");
			return tmAdapter;
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Usually <c>false</c>, but can be set to <c>true</c> while running unit tests
		/// </summary>
		/// <returns><c>true</c> if running tests, otherwise <c>false</c>.</returns>
		/// ------------------------------------------------------------------------------------
		private static bool RunningTests()
		{
			// If the real application is ever installed in a path that includes nunit, then
			// this will return true and the app. won't run properly. But what are the chances
			// of that?...
			return (Application.ExecutablePath.ToLower().IndexOf("nunit") != -1);
		}
	}

	#endregion
}
