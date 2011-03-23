namespace SIL.Pa.UI.Controls
{
	partial class PlaybackSpeedAdjuster
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
			this.components = new System.ComponentModel.Container();
			this.trkSpeed = new System.Windows.Forms.TrackBar();
			this.lnkPlay = new System.Windows.Forms.LinkLabel();
			this.lblValue = new System.Windows.Forms.Label();
			this.lbl100 = new System.Windows.Forms.Label();
			this.lbl50 = new System.Windows.Forms.Label();
			this.lbl75 = new System.Windows.Forms.Label();
			this.lbl25 = new System.Windows.Forms.Label();
			this.lbl125 = new System.Windows.Forms.Label();
			this.lbl150 = new System.Windows.Forms.Label();
			this.lbl175 = new System.Windows.Forms.Label();
			this.lbl200 = new System.Windows.Forms.Label();
			this.lblZero = new System.Windows.Forms.Label();
			this.locExtender = new Localization.UI.LocalizationExtender(this.components);
			((System.ComponentModel.ISupportInitialize)(this.trkSpeed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.locExtender)).BeginInit();
			this.SuspendLayout();
			// 
			// trkSpeed
			// 
			this.trkSpeed.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.trkSpeed.AutoSize = false;
			this.trkSpeed.BackColor = System.Drawing.Color.White;
			this.locExtender.SetLocalizableToolTip(this.trkSpeed, null);
			this.locExtender.SetLocalizationComment(this.trkSpeed, null);
			this.locExtender.SetLocalizationPriority(this.trkSpeed, Localization.LocalizationPriority.NotLocalizable);
			this.locExtender.SetLocalizingId(this.trkSpeed, "PlaybackSpeedAdjuster.trkSpeed");
			this.trkSpeed.Location = new System.Drawing.Point(34, 24);
			this.trkSpeed.Maximum = 200;
			this.trkSpeed.Name = "trkSpeed";
			this.trkSpeed.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trkSpeed.Size = new System.Drawing.Size(45, 249);
			this.trkSpeed.TabIndex = 0;
			this.trkSpeed.TickFrequency = 10;
			this.trkSpeed.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trkSpeed.Value = 100;
			this.trkSpeed.ValueChanged += new System.EventHandler(this.trkSpeed_ValueChanged);
			// 
			// lnkPlay
			// 
			this.lnkPlay.BackColor = System.Drawing.Color.Transparent;
			this.lnkPlay.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lnkPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.locExtender.SetLocalizableToolTip(this.lnkPlay, null);
			this.locExtender.SetLocalizationComment(this.lnkPlay, null);
			this.locExtender.SetLocalizingId(this.lnkPlay, "PlaybackSpeedAdjuster.lnkPlay");
			this.lnkPlay.Location = new System.Drawing.Point(0, 269);
			this.lnkPlay.Name = "lnkPlay";
			this.lnkPlay.Size = new System.Drawing.Size(73, 23);
			this.lnkPlay.TabIndex = 1;
			this.lnkPlay.TabStop = true;
			this.lnkPlay.Text = "Play";
			this.lnkPlay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblValue
			// 
			this.lblValue.BackColor = System.Drawing.Color.Transparent;
			this.lblValue.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.locExtender.SetLocalizableToolTip(this.lblValue, null);
			this.locExtender.SetLocalizationComment(this.lblValue, null);
			this.locExtender.SetLocalizingId(this.lblValue, "PlaybackSpeedAdjuster.lblValue");
			this.lblValue.Location = new System.Drawing.Point(0, 3);
			this.lblValue.Name = "lblValue";
			this.lblValue.Size = new System.Drawing.Size(73, 21);
			this.lblValue.TabIndex = 2;
			this.lblValue.Text = "{0}%";
			this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl100
			// 
			this.lbl100.BackColor = System.Drawing.Color.Transparent;
			this.locExtender.SetLocalizableToolTip(this.lbl100, null);
			this.locExtender.SetLocalizationComment(this.lbl100, null);
			this.locExtender.SetLocalizingId(this.lbl100, "PlaybackSpeedAdjuster.lbl100");
			this.lbl100.Location = new System.Drawing.Point(0, 141);
			this.lbl100.Name = "lbl100";
			this.lbl100.Size = new System.Drawing.Size(35, 15);
			this.lbl100.TabIndex = 3;
			this.lbl100.Text = "100%";
			this.lbl100.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbl100.UseCompatibleTextRendering = true;
			// 
			// lbl50
			// 
			this.lbl50.BackColor = System.Drawing.Color.Transparent;
			this.locExtender.SetLocalizableToolTip(this.lbl50, null);
			this.locExtender.SetLocalizationComment(this.lbl50, null);
			this.locExtender.SetLocalizingId(this.lbl50, "PlaybackSpeedAdjuster.lbl50");
			this.lbl50.Location = new System.Drawing.Point(0, 197);
			this.lbl50.Name = "lbl50";
			this.lbl50.Size = new System.Drawing.Size(35, 15);
			this.lbl50.TabIndex = 4;
			this.lbl50.Text = "50%";
			this.lbl50.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbl50.UseCompatibleTextRendering = true;
			// 
			// lbl75
			// 
			this.lbl75.BackColor = System.Drawing.Color.Transparent;
			this.locExtender.SetLocalizableToolTip(this.lbl75, null);
			this.locExtender.SetLocalizationComment(this.lbl75, null);
			this.locExtender.SetLocalizingId(this.lbl75, "PlaybackSpeedAdjuster.lbl75");
			this.lbl75.Location = new System.Drawing.Point(0, 169);
			this.lbl75.Name = "lbl75";
			this.lbl75.Size = new System.Drawing.Size(35, 15);
			this.lbl75.TabIndex = 5;
			this.lbl75.Text = "75%";
			this.lbl75.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbl75.UseCompatibleTextRendering = true;
			// 
			// lbl25
			// 
			this.lbl25.BackColor = System.Drawing.Color.Transparent;
			this.locExtender.SetLocalizableToolTip(this.lbl25, null);
			this.locExtender.SetLocalizationComment(this.lbl25, null);
			this.locExtender.SetLocalizingId(this.lbl25, "PlaybackSpeedAdjuster.lbl25");
			this.lbl25.Location = new System.Drawing.Point(0, 225);
			this.lbl25.Name = "lbl25";
			this.lbl25.Size = new System.Drawing.Size(35, 15);
			this.lbl25.TabIndex = 6;
			this.lbl25.Text = "25%";
			this.lbl25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbl25.UseCompatibleTextRendering = true;
			// 
			// lbl125
			// 
			this.lbl125.BackColor = System.Drawing.Color.Transparent;
			this.locExtender.SetLocalizableToolTip(this.lbl125, null);
			this.locExtender.SetLocalizationComment(this.lbl125, null);
			this.locExtender.SetLocalizingId(this.lbl125, "PlaybackSpeedAdjuster.lbl125");
			this.lbl125.Location = new System.Drawing.Point(0, 114);
			this.lbl125.Name = "lbl125";
			this.lbl125.Size = new System.Drawing.Size(35, 15);
			this.lbl125.TabIndex = 7;
			this.lbl125.Text = "125%";
			this.lbl125.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbl125.UseCompatibleTextRendering = true;
			// 
			// lbl150
			// 
			this.lbl150.BackColor = System.Drawing.Color.Transparent;
			this.locExtender.SetLocalizableToolTip(this.lbl150, null);
			this.locExtender.SetLocalizationComment(this.lbl150, null);
			this.locExtender.SetLocalizingId(this.lbl150, "PlaybackSpeedAdjuster.lbl150");
			this.lbl150.Location = new System.Drawing.Point(0, 86);
			this.lbl150.Name = "lbl150";
			this.lbl150.Size = new System.Drawing.Size(35, 15);
			this.lbl150.TabIndex = 8;
			this.lbl150.Text = "150%";
			this.lbl150.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbl150.UseCompatibleTextRendering = true;
			// 
			// lbl175
			// 
			this.lbl175.BackColor = System.Drawing.Color.Transparent;
			this.locExtender.SetLocalizableToolTip(this.lbl175, null);
			this.locExtender.SetLocalizationComment(this.lbl175, null);
			this.locExtender.SetLocalizingId(this.lbl175, "PlaybackSpeedAdjuster.lbl175");
			this.lbl175.Location = new System.Drawing.Point(0, 58);
			this.lbl175.Name = "lbl175";
			this.lbl175.Size = new System.Drawing.Size(35, 15);
			this.lbl175.TabIndex = 9;
			this.lbl175.Text = "175%";
			this.lbl175.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbl175.UseCompatibleTextRendering = true;
			// 
			// lbl200
			// 
			this.lbl200.BackColor = System.Drawing.Color.Transparent;
			this.locExtender.SetLocalizableToolTip(this.lbl200, null);
			this.locExtender.SetLocalizationComment(this.lbl200, null);
			this.locExtender.SetLocalizingId(this.lbl200, "PlaybackSpeedAdjuster.lbl200");
			this.lbl200.Location = new System.Drawing.Point(0, 30);
			this.lbl200.Name = "lbl200";
			this.lbl200.Size = new System.Drawing.Size(35, 15);
			this.lbl200.TabIndex = 10;
			this.lbl200.Text = "200%";
			this.lbl200.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbl200.UseCompatibleTextRendering = true;
			// 
			// lblZero
			// 
			this.lblZero.BackColor = System.Drawing.Color.Transparent;
			this.locExtender.SetLocalizableToolTip(this.lblZero, null);
			this.locExtender.SetLocalizationComment(this.lblZero, null);
			this.locExtender.SetLocalizingId(this.lblZero, "PlaybackSpeedAdjuster.lblZero");
			this.lblZero.Location = new System.Drawing.Point(0, 252);
			this.lblZero.Name = "lblZero";
			this.lblZero.Size = new System.Drawing.Size(35, 15);
			this.lblZero.TabIndex = 11;
			this.lblZero.Text = "0%";
			this.lblZero.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lblZero.UseCompatibleTextRendering = true;
			// 
			// locExtender
			// 
			this.locExtender.LocalizationGroup = global::SIL.Pa.ResourceStuff.PaTMStrings.kstidDoNothingToolTip;
			this.locExtender.LocalizationManagerId = "Pa";
			// 
			// PlaybackSpeedAdjuster
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.lblZero);
			this.Controls.Add(this.lbl200);
			this.Controls.Add(this.lbl175);
			this.Controls.Add(this.lbl150);
			this.Controls.Add(this.lbl125);
			this.Controls.Add(this.lbl25);
			this.Controls.Add(this.lbl75);
			this.Controls.Add(this.lbl50);
			this.Controls.Add(this.lbl100);
			this.Controls.Add(this.trkSpeed);
			this.Controls.Add(this.lnkPlay);
			this.Controls.Add(this.lblValue);
			this.DoubleBuffered = true;
			this.locExtender.SetLocalizableToolTip(this, null);
			this.locExtender.SetLocalizationComment(this, null);
			this.locExtender.SetLocalizationPriority(this, Localization.LocalizationPriority.NotLocalizable);
			this.locExtender.SetLocalizingId(this, "PlaybackSpeedAdjuster.PlaybackSpeedAdjuster");
			this.Name = "PlaybackSpeedAdjuster";
			this.Padding = new System.Windows.Forms.Padding(0, 3, 0, 5);
			this.Size = new System.Drawing.Size(73, 297);
			((System.ComponentModel.ISupportInitialize)(this.trkSpeed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.locExtender)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TrackBar trkSpeed;
		private System.Windows.Forms.Label lblValue;
		private System.Windows.Forms.Label lbl100;
		private System.Windows.Forms.Label lbl50;
		private System.Windows.Forms.Label lbl75;
		private System.Windows.Forms.Label lbl25;
		private System.Windows.Forms.Label lbl125;
		private System.Windows.Forms.Label lbl150;
		private System.Windows.Forms.Label lbl175;
		private System.Windows.Forms.Label lbl200;
		private System.Windows.Forms.Label lblZero;
		public System.Windows.Forms.LinkLabel lnkPlay;
		private Localization.UI.LocalizationExtender locExtender;
	}
}
