namespace SIL.Pa.Controls
{
	partial class SortOptionsDropDown
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SortOptionsDropDown));
			this.rbUnicodeOrder = new System.Windows.Forms.RadioButton();
			this.rbPlaceArticulation = new System.Windows.Forms.RadioButton();
			this.rbMannerArticulation = new System.Windows.Forms.RadioButton();
			this.grpAdvSortOptions = new System.Windows.Forms.GroupBox();
			this.tblAdvSorting = new System.Windows.Forms.TableLayoutPanel();
			this.pnlAdvSort2 = new System.Windows.Forms.Panel();
			this.rbItem3rd = new System.Windows.Forms.RadioButton();
			this.rbBefore3rd = new System.Windows.Forms.RadioButton();
			this.rbAfter3rd = new System.Windows.Forms.RadioButton();
			this.pnlAdvSort0 = new System.Windows.Forms.Panel();
			this.rbItem1st = new System.Windows.Forms.RadioButton();
			this.rbBefore1st = new System.Windows.Forms.RadioButton();
			this.rbAfter1st = new System.Windows.Forms.RadioButton();
			this.pnlAdvSort1 = new System.Windows.Forms.Panel();
			this.rbAfter2nd = new System.Windows.Forms.RadioButton();
			this.rbBefore2nd = new System.Windows.Forms.RadioButton();
			this.rbItem2nd = new System.Windows.Forms.RadioButton();
			this.cbAfterRL = new System.Windows.Forms.CheckBox();
			this.cbItemRL = new System.Windows.Forms.CheckBox();
			this.cbBeforeRL = new System.Windows.Forms.CheckBox();
			this.lblBefore = new System.Windows.Forms.Label();
			this.lblThird = new System.Windows.Forms.Label();
			this.lblAfter = new System.Windows.Forms.Label();
			this.lblItem = new System.Windows.Forms.Label();
			this.lblFirst = new System.Windows.Forms.Label();
			this.lblRL = new System.Windows.Forms.Label();
			this.lblSecond = new System.Windows.Forms.Label();
			this.lnkHelp = new System.Windows.Forms.LinkLabel();
			this.grpAdvSortOptions.SuspendLayout();
			this.tblAdvSorting.SuspendLayout();
			this.pnlAdvSort2.SuspendLayout();
			this.pnlAdvSort0.SuspendLayout();
			this.pnlAdvSort1.SuspendLayout();
			this.SuspendLayout();
			// 
			// rbUnicodeOrder
			// 
			resources.ApplyResources(this.rbUnicodeOrder, "rbUnicodeOrder");
			this.rbUnicodeOrder.BackColor = System.Drawing.Color.Transparent;
			this.rbUnicodeOrder.Name = "rbUnicodeOrder";
			this.rbUnicodeOrder.TabStop = true;
			this.rbUnicodeOrder.UseVisualStyleBackColor = false;
			this.rbUnicodeOrder.Click += new System.EventHandler(this.HandleSortTypeChecked);
			// 
			// rbPlaceArticulation
			// 
			resources.ApplyResources(this.rbPlaceArticulation, "rbPlaceArticulation");
			this.rbPlaceArticulation.BackColor = System.Drawing.Color.Transparent;
			this.rbPlaceArticulation.Name = "rbPlaceArticulation";
			this.rbPlaceArticulation.TabStop = true;
			this.rbPlaceArticulation.UseVisualStyleBackColor = false;
			this.rbPlaceArticulation.Click += new System.EventHandler(this.HandleSortTypeChecked);
			// 
			// rbMannerArticulation
			// 
			resources.ApplyResources(this.rbMannerArticulation, "rbMannerArticulation");
			this.rbMannerArticulation.BackColor = System.Drawing.Color.Transparent;
			this.rbMannerArticulation.Name = "rbMannerArticulation";
			this.rbMannerArticulation.TabStop = true;
			this.rbMannerArticulation.UseVisualStyleBackColor = false;
			this.rbMannerArticulation.Click += new System.EventHandler(this.HandleSortTypeChecked);
			// 
			// grpAdvSortOptions
			// 
			resources.ApplyResources(this.grpAdvSortOptions, "grpAdvSortOptions");
			this.grpAdvSortOptions.Controls.Add(this.tblAdvSorting);
			this.grpAdvSortOptions.Name = "grpAdvSortOptions";
			this.grpAdvSortOptions.TabStop = false;
			// 
			// tblAdvSorting
			// 
			resources.ApplyResources(this.tblAdvSorting, "tblAdvSorting");
			this.tblAdvSorting.Controls.Add(this.pnlAdvSort2, 3, 1);
			this.tblAdvSorting.Controls.Add(this.pnlAdvSort0, 1, 1);
			this.tblAdvSorting.Controls.Add(this.pnlAdvSort1, 2, 1);
			this.tblAdvSorting.Controls.Add(this.cbAfterRL, 4, 3);
			this.tblAdvSorting.Controls.Add(this.cbItemRL, 4, 2);
			this.tblAdvSorting.Controls.Add(this.cbBeforeRL, 4, 1);
			this.tblAdvSorting.Controls.Add(this.lblBefore, 0, 1);
			this.tblAdvSorting.Controls.Add(this.lblThird, 3, 0);
			this.tblAdvSorting.Controls.Add(this.lblAfter, 0, 3);
			this.tblAdvSorting.Controls.Add(this.lblItem, 0, 2);
			this.tblAdvSorting.Controls.Add(this.lblFirst, 1, 0);
			this.tblAdvSorting.Controls.Add(this.lblRL, 4, 0);
			this.tblAdvSorting.Controls.Add(this.lblSecond, 2, 0);
			this.tblAdvSorting.Name = "tblAdvSorting";
			// 
			// pnlAdvSort2
			// 
			this.pnlAdvSort2.Controls.Add(this.rbItem3rd);
			this.pnlAdvSort2.Controls.Add(this.rbBefore3rd);
			this.pnlAdvSort2.Controls.Add(this.rbAfter3rd);
			resources.ApplyResources(this.pnlAdvSort2, "pnlAdvSort2");
			this.pnlAdvSort2.Name = "pnlAdvSort2";
			this.tblAdvSorting.SetRowSpan(this.pnlAdvSort2, 3);
			// 
			// rbItem3rd
			// 
			resources.ApplyResources(this.rbItem3rd, "rbItem3rd");
			this.rbItem3rd.BackColor = System.Drawing.Color.Transparent;
			this.rbItem3rd.Name = "rbItem3rd";
			this.rbItem3rd.UseVisualStyleBackColor = false;
			this.rbItem3rd.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.rbItem3rd.Click += new System.EventHandler(this.HandleCheckedColumn2);
			this.rbItem3rd.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.rbItem3rd.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// rbBefore3rd
			// 
			resources.ApplyResources(this.rbBefore3rd, "rbBefore3rd");
			this.rbBefore3rd.BackColor = System.Drawing.Color.Transparent;
			this.rbBefore3rd.Name = "rbBefore3rd";
			this.rbBefore3rd.UseVisualStyleBackColor = false;
			this.rbBefore3rd.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.rbBefore3rd.Click += new System.EventHandler(this.HandleCheckedColumn2);
			this.rbBefore3rd.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.rbBefore3rd.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// rbAfter3rd
			// 
			resources.ApplyResources(this.rbAfter3rd, "rbAfter3rd");
			this.rbAfter3rd.BackColor = System.Drawing.Color.Transparent;
			this.rbAfter3rd.Name = "rbAfter3rd";
			this.rbAfter3rd.UseVisualStyleBackColor = false;
			this.rbAfter3rd.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.rbAfter3rd.Click += new System.EventHandler(this.HandleCheckedColumn2);
			this.rbAfter3rd.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.rbAfter3rd.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// pnlAdvSort0
			// 
			this.pnlAdvSort0.Controls.Add(this.rbItem1st);
			this.pnlAdvSort0.Controls.Add(this.rbBefore1st);
			this.pnlAdvSort0.Controls.Add(this.rbAfter1st);
			resources.ApplyResources(this.pnlAdvSort0, "pnlAdvSort0");
			this.pnlAdvSort0.Name = "pnlAdvSort0";
			this.tblAdvSorting.SetRowSpan(this.pnlAdvSort0, 3);
			// 
			// rbItem1st
			// 
			resources.ApplyResources(this.rbItem1st, "rbItem1st");
			this.rbItem1st.BackColor = System.Drawing.Color.Transparent;
			this.rbItem1st.Name = "rbItem1st";
			this.rbItem1st.UseVisualStyleBackColor = false;
			this.rbItem1st.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.rbItem1st.Click += new System.EventHandler(this.HandleCheckedColumn0);
			this.rbItem1st.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.rbItem1st.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// rbBefore1st
			// 
			resources.ApplyResources(this.rbBefore1st, "rbBefore1st");
			this.rbBefore1st.BackColor = System.Drawing.Color.Transparent;
			this.rbBefore1st.Name = "rbBefore1st";
			this.rbBefore1st.Tag = "";
			this.rbBefore1st.UseVisualStyleBackColor = false;
			this.rbBefore1st.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.rbBefore1st.Click += new System.EventHandler(this.HandleCheckedColumn0);
			this.rbBefore1st.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.rbBefore1st.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// rbAfter1st
			// 
			resources.ApplyResources(this.rbAfter1st, "rbAfter1st");
			this.rbAfter1st.BackColor = System.Drawing.Color.Transparent;
			this.rbAfter1st.Name = "rbAfter1st";
			this.rbAfter1st.UseVisualStyleBackColor = false;
			this.rbAfter1st.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.rbAfter1st.Click += new System.EventHandler(this.HandleCheckedColumn0);
			this.rbAfter1st.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.rbAfter1st.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// pnlAdvSort1
			// 
			this.pnlAdvSort1.Controls.Add(this.rbAfter2nd);
			this.pnlAdvSort1.Controls.Add(this.rbBefore2nd);
			this.pnlAdvSort1.Controls.Add(this.rbItem2nd);
			resources.ApplyResources(this.pnlAdvSort1, "pnlAdvSort1");
			this.pnlAdvSort1.Name = "pnlAdvSort1";
			this.tblAdvSorting.SetRowSpan(this.pnlAdvSort1, 3);
			// 
			// rbAfter2nd
			// 
			resources.ApplyResources(this.rbAfter2nd, "rbAfter2nd");
			this.rbAfter2nd.BackColor = System.Drawing.Color.Transparent;
			this.rbAfter2nd.Name = "rbAfter2nd";
			this.rbAfter2nd.UseVisualStyleBackColor = false;
			this.rbAfter2nd.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.rbAfter2nd.Click += new System.EventHandler(this.HandleCheckedColumn1);
			this.rbAfter2nd.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.rbAfter2nd.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// rbBefore2nd
			// 
			resources.ApplyResources(this.rbBefore2nd, "rbBefore2nd");
			this.rbBefore2nd.BackColor = System.Drawing.Color.Transparent;
			this.rbBefore2nd.Name = "rbBefore2nd";
			this.rbBefore2nd.UseVisualStyleBackColor = false;
			this.rbBefore2nd.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.rbBefore2nd.Click += new System.EventHandler(this.HandleCheckedColumn1);
			this.rbBefore2nd.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.rbBefore2nd.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// rbItem2nd
			// 
			resources.ApplyResources(this.rbItem2nd, "rbItem2nd");
			this.rbItem2nd.BackColor = System.Drawing.Color.Transparent;
			this.rbItem2nd.Name = "rbItem2nd";
			this.rbItem2nd.UseVisualStyleBackColor = false;
			this.rbItem2nd.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.rbItem2nd.Click += new System.EventHandler(this.HandleCheckedColumn1);
			this.rbItem2nd.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.rbItem2nd.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// cbAfterRL
			// 
			resources.ApplyResources(this.cbAfterRL, "cbAfterRL");
			this.cbAfterRL.BackColor = System.Drawing.Color.Transparent;
			this.cbAfterRL.Name = "cbAfterRL";
			this.cbAfterRL.UseVisualStyleBackColor = false;
			this.cbAfterRL.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.cbAfterRL.Click += new System.EventHandler(this.HandleRightToLeftCheckBoxChecked);
			this.cbAfterRL.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.cbAfterRL.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// cbItemRL
			// 
			resources.ApplyResources(this.cbItemRL, "cbItemRL");
			this.cbItemRL.BackColor = System.Drawing.Color.Transparent;
			this.cbItemRL.Name = "cbItemRL";
			this.cbItemRL.UseVisualStyleBackColor = false;
			this.cbItemRL.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.cbItemRL.Click += new System.EventHandler(this.HandleRightToLeftCheckBoxChecked);
			this.cbItemRL.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.cbItemRL.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// cbBeforeRL
			// 
			resources.ApplyResources(this.cbBeforeRL, "cbBeforeRL");
			this.cbBeforeRL.BackColor = System.Drawing.Color.Transparent;
			this.cbBeforeRL.Name = "cbBeforeRL";
			this.cbBeforeRL.UseVisualStyleBackColor = false;
			this.cbBeforeRL.Enter += new System.EventHandler(this.HandleAdvancedOptionItemEnter);
			this.cbBeforeRL.Click += new System.EventHandler(this.HandleRightToLeftCheckBoxChecked);
			this.cbBeforeRL.Leave += new System.EventHandler(this.HandleAdvancedOptionItemLeave);
			this.cbBeforeRL.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleAdvancedOptionItemPaint);
			// 
			// lblBefore
			// 
			this.lblBefore.AutoEllipsis = true;
			this.lblBefore.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblBefore, "lblBefore");
			this.lblBefore.Name = "lblBefore";
			// 
			// lblThird
			// 
			this.lblThird.AutoEllipsis = true;
			this.lblThird.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblThird, "lblThird");
			this.lblThird.Name = "lblThird";
			// 
			// lblAfter
			// 
			this.lblAfter.AutoEllipsis = true;
			this.lblAfter.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblAfter, "lblAfter");
			this.lblAfter.Name = "lblAfter";
			// 
			// lblItem
			// 
			this.lblItem.AutoEllipsis = true;
			this.lblItem.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblItem, "lblItem");
			this.lblItem.Name = "lblItem";
			// 
			// lblFirst
			// 
			this.lblFirst.AutoEllipsis = true;
			this.lblFirst.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblFirst, "lblFirst");
			this.lblFirst.Name = "lblFirst";
			// 
			// lblRL
			// 
			this.lblRL.AutoEllipsis = true;
			this.lblRL.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblRL, "lblRL");
			this.lblRL.Name = "lblRL";
			// 
			// lblSecond
			// 
			this.lblSecond.AutoEllipsis = true;
			this.lblSecond.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblSecond, "lblSecond");
			this.lblSecond.Name = "lblSecond";
			// 
			// lnkHelp
			// 
			resources.ApplyResources(this.lnkHelp, "lnkHelp");
			this.lnkHelp.Name = "lnkHelp";
			this.lnkHelp.TabStop = true;
			this.lnkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HandleHelpClicked);
			// 
			// SortOptionsDropDown
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.lnkHelp);
			this.Controls.Add(this.grpAdvSortOptions);
			this.Controls.Add(this.rbMannerArticulation);
			this.Controls.Add(this.rbPlaceArticulation);
			this.Controls.Add(this.rbUnicodeOrder);
			this.Name = "SortOptionsDropDown";
			this.grpAdvSortOptions.ResumeLayout(false);
			this.tblAdvSorting.ResumeLayout(false);
			this.pnlAdvSort2.ResumeLayout(false);
			this.pnlAdvSort0.ResumeLayout(false);
			this.pnlAdvSort1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton rbUnicodeOrder;
		private System.Windows.Forms.RadioButton rbPlaceArticulation;
		private System.Windows.Forms.RadioButton rbMannerArticulation;
		private System.Windows.Forms.CheckBox cbAfterRL;
		private System.Windows.Forms.CheckBox cbItemRL;
		private System.Windows.Forms.CheckBox cbBeforeRL;
		private System.Windows.Forms.RadioButton rbAfter3rd;
		private System.Windows.Forms.RadioButton rbAfter2nd;
		private System.Windows.Forms.RadioButton rbAfter1st;
		private System.Windows.Forms.RadioButton rbItem3rd;
		private System.Windows.Forms.RadioButton rbItem2nd;
		private System.Windows.Forms.RadioButton rbItem1st;
		private System.Windows.Forms.Label lblRL;
		private System.Windows.Forms.Label lblThird;
		private System.Windows.Forms.Label lblSecond;
		private System.Windows.Forms.Label lblFirst;
		private System.Windows.Forms.Label lblAfter;
		private System.Windows.Forms.Label lblItem;
		private System.Windows.Forms.Label lblBefore;
		private System.Windows.Forms.GroupBox grpAdvSortOptions;
		private System.Windows.Forms.RadioButton rbBefore3rd;
		private System.Windows.Forms.RadioButton rbBefore2nd;
		private System.Windows.Forms.RadioButton rbBefore1st;
		private System.Windows.Forms.LinkLabel lnkHelp;
		private System.Windows.Forms.TableLayoutPanel tblAdvSorting;
		private System.Windows.Forms.Panel pnlAdvSort0;
		private System.Windows.Forms.Panel pnlAdvSort2;
		private System.Windows.Forms.Panel pnlAdvSort1;
	}
}
