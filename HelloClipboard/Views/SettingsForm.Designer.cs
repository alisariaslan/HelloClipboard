namespace HelloClipboard
{
	partial class SettingsForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.checkBox1_startWithWindows = new System.Windows.Forms.CheckBox();
			this.checkBox2_hideToSystemTray = new System.Windows.Forms.CheckBox();
			this.checkBox3_checkUpdates = new System.Windows.Forms.CheckBox();
			this.textBox1_maxHistoryCount = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBox4_preventClipboardDuplication = new System.Windows.Forms.CheckBox();
			this.button2_Defaults = new System.Windows.Forms.Button();
			this.checkBox1_invertClipboardHistoryListing = new System.Windows.Forms.CheckBox();
			this.checkBox1_clipboardHistory = new System.Windows.Forms.CheckBox();
			this.checkBox1_alwaysTopMost = new System.Windows.Forms.CheckBox();
			this.checkBox1_showInTaskbar = new System.Windows.Forms.CheckBox();
			this.checkBox2_enableTimeStamps = new System.Windows.Forms.CheckBox();
			this.checkBox1_autoHideWhenUnfocus = new System.Windows.Forms.CheckBox();
			this.label2_hotkey = new System.Windows.Forms.Label();
			this.textBox_hotkey = new System.Windows.Forms.TextBox();
			this.checkBox_enableHotkey = new System.Windows.Forms.CheckBox();
			this.label_privacyDuration = new System.Windows.Forms.Label();
			this.textBox_privacyDuration = new System.Windows.Forms.TextBox();
			this.checkBox1_suppressClipboardEvents = new System.Windows.Forms.CheckBox();
			this.chbox_focusDetailWindow = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkBox1_startWithWindows
			// 
			this.checkBox1_startWithWindows.AutoSize = true;
			this.checkBox1_startWithWindows.Location = new System.Drawing.Point(10, 38);
			this.checkBox1_startWithWindows.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox1_startWithWindows.Name = "checkBox1_startWithWindows";
			this.checkBox1_startWithWindows.Size = new System.Drawing.Size(138, 21);
			this.checkBox1_startWithWindows.TabIndex = 0;
			this.checkBox1_startWithWindows.Text = "Start with Windows";
			this.checkBox1_startWithWindows.UseVisualStyleBackColor = true;
			this.checkBox1_startWithWindows.CheckedChanged += new System.EventHandler(this.checkBox1_startWithWindows_CheckedChanged);
			// 
			// checkBox2_hideToSystemTray
			// 
			this.checkBox2_hideToSystemTray.AutoSize = true;
			this.checkBox2_hideToSystemTray.Location = new System.Drawing.Point(10, 66);
			this.checkBox2_hideToSystemTray.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox2_hideToSystemTray.Name = "checkBox2_hideToSystemTray";
			this.checkBox2_hideToSystemTray.Size = new System.Drawing.Size(143, 21);
			this.checkBox2_hideToSystemTray.TabIndex = 1;
			this.checkBox2_hideToSystemTray.Text = "Hide to System Tray";
			this.checkBox2_hideToSystemTray.UseVisualStyleBackColor = true;
			this.checkBox2_hideToSystemTray.CheckedChanged += new System.EventHandler(this.checkBox2_hideToSystemTray_CheckedChanged);
			// 
			// checkBox3_checkUpdates
			// 
			this.checkBox3_checkUpdates.AutoSize = true;
			this.checkBox3_checkUpdates.Location = new System.Drawing.Point(10, 9);
			this.checkBox3_checkUpdates.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox3_checkUpdates.Name = "checkBox3_checkUpdates";
			this.checkBox3_checkUpdates.Size = new System.Drawing.Size(114, 21);
			this.checkBox3_checkUpdates.TabIndex = 2;
			this.checkBox3_checkUpdates.Text = "Check Updates";
			this.checkBox3_checkUpdates.UseVisualStyleBackColor = true;
			this.checkBox3_checkUpdates.CheckedChanged += new System.EventHandler(this.checkBox3_checkUpdates_CheckedChanged);
			// 
			// textBox1_maxHistoryCount
			// 
			this.textBox1_maxHistoryCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1_maxHistoryCount.Location = new System.Drawing.Point(378, 82);
			this.textBox1_maxHistoryCount.Name = "textBox1_maxHistoryCount";
			this.textBox1_maxHistoryCount.Size = new System.Drawing.Size(78, 25);
			this.textBox1_maxHistoryCount.TabIndex = 4;
			this.textBox1_maxHistoryCount.Text = "1000";
			this.textBox1_maxHistoryCount.TextChanged += new System.EventHandler(this.textBox1_maxHistoryCount_TextChanged);
			this.textBox1_maxHistoryCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_maxHistoryCount_KeyPress);
			this.textBox1_maxHistoryCount.Leave += new System.EventHandler(this.textBox1_maxHistoryCount_Leave);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(340, 62);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 17);
			this.label1.TabIndex = 5;
			this.label1.Text = "Max History Count";
			// 
			// checkBox4_preventClipboardDuplication
			// 
			this.checkBox4_preventClipboardDuplication.AutoSize = true;
			this.checkBox4_preventClipboardDuplication.Location = new System.Drawing.Point(10, 94);
			this.checkBox4_preventClipboardDuplication.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox4_preventClipboardDuplication.Name = "checkBox4_preventClipboardDuplication";
			this.checkBox4_preventClipboardDuplication.Size = new System.Drawing.Size(139, 21);
			this.checkBox4_preventClipboardDuplication.TabIndex = 6;
			this.checkBox4_preventClipboardDuplication.Text = "Prevent Duplication";
			this.checkBox4_preventClipboardDuplication.UseVisualStyleBackColor = true;
			this.checkBox4_preventClipboardDuplication.CheckedChanged += new System.EventHandler(this.checkBox4_preventClipboardDuplication_CheckedChanged);
			// 
			// button2_Defaults
			// 
			this.button2_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button2_Defaults.Location = new System.Drawing.Point(10, 406);
			this.button2_Defaults.Name = "button2_Defaults";
			this.button2_Defaults.Size = new System.Drawing.Size(446, 26);
			this.button2_Defaults.TabIndex = 9;
			this.button2_Defaults.Text = "Reset to Defaults";
			this.button2_Defaults.UseVisualStyleBackColor = true;
			this.button2_Defaults.Click += new System.EventHandler(this.button2_Defaults_Click);
			// 
			// checkBox1_invertClipboardHistoryListing
			// 
			this.checkBox1_invertClipboardHistoryListing.AutoSize = true;
			this.checkBox1_invertClipboardHistoryListing.Location = new System.Drawing.Point(10, 148);
			this.checkBox1_invertClipboardHistoryListing.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox1_invertClipboardHistoryListing.Name = "checkBox1_invertClipboardHistoryListing";
			this.checkBox1_invertClipboardHistoryListing.Size = new System.Drawing.Size(121, 21);
			this.checkBox1_invertClipboardHistoryListing.TabIndex = 11;
			this.checkBox1_invertClipboardHistoryListing.Text = "Invert Clipboard";
			this.checkBox1_invertClipboardHistoryListing.UseVisualStyleBackColor = true;
			this.checkBox1_invertClipboardHistoryListing.CheckedChanged += new System.EventHandler(this.checkBox1_invertClipboardHistoryListing_CheckedChanged);
			// 
			// checkBox1_clipboardHistory
			// 
			this.checkBox1_clipboardHistory.AutoSize = true;
			this.checkBox1_clipboardHistory.Location = new System.Drawing.Point(10, 177);
			this.checkBox1_clipboardHistory.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox1_clipboardHistory.Name = "checkBox1_clipboardHistory";
			this.checkBox1_clipboardHistory.Size = new System.Drawing.Size(111, 21);
			this.checkBox1_clipboardHistory.TabIndex = 12;
			this.checkBox1_clipboardHistory.Text = "Enable History";
			this.checkBox1_clipboardHistory.UseVisualStyleBackColor = true;
			this.checkBox1_clipboardHistory.CheckedChanged += new System.EventHandler(this.checkBox1_clipboardHistory_CheckedChanged);
			// 
			// checkBox1_alwaysTopMost
			// 
			this.checkBox1_alwaysTopMost.AutoSize = true;
			this.checkBox1_alwaysTopMost.Location = new System.Drawing.Point(10, 205);
			this.checkBox1_alwaysTopMost.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox1_alwaysTopMost.Name = "checkBox1_alwaysTopMost";
			this.checkBox1_alwaysTopMost.Size = new System.Drawing.Size(126, 21);
			this.checkBox1_alwaysTopMost.TabIndex = 13;
			this.checkBox1_alwaysTopMost.Text = "Always Top Most";
			this.checkBox1_alwaysTopMost.UseVisualStyleBackColor = true;
			this.checkBox1_alwaysTopMost.CheckedChanged += new System.EventHandler(this.checkBox1_alwaysTopMost_CheckedChanged);
			// 
			// checkBox1_showInTaskbar
			// 
			this.checkBox1_showInTaskbar.AutoSize = true;
			this.checkBox1_showInTaskbar.Location = new System.Drawing.Point(10, 233);
			this.checkBox1_showInTaskbar.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox1_showInTaskbar.Name = "checkBox1_showInTaskbar";
			this.checkBox1_showInTaskbar.Size = new System.Drawing.Size(121, 21);
			this.checkBox1_showInTaskbar.TabIndex = 14;
			this.checkBox1_showInTaskbar.Text = "Show In Taskbar";
			this.checkBox1_showInTaskbar.UseVisualStyleBackColor = true;
			this.checkBox1_showInTaskbar.CheckedChanged += new System.EventHandler(this.checkBox1_showInTaskbar_CheckedChanged);
			// 
			// checkBox2_enableTimeStamps
			// 
			this.checkBox2_enableTimeStamps.AutoSize = true;
			this.checkBox2_enableTimeStamps.Location = new System.Drawing.Point(10, 262);
			this.checkBox2_enableTimeStamps.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox2_enableTimeStamps.Name = "checkBox2_enableTimeStamps";
			this.checkBox2_enableTimeStamps.Size = new System.Drawing.Size(145, 21);
			this.checkBox2_enableTimeStamps.TabIndex = 15;
			this.checkBox2_enableTimeStamps.Text = "Enable Time Stamps";
			this.checkBox2_enableTimeStamps.UseVisualStyleBackColor = true;
			this.checkBox2_enableTimeStamps.CheckedChanged += new System.EventHandler(this.checkBox2_enableTimeStamps_CheckedChanged);
			// 
			// checkBox1_autoHideWhenUnfocus
			// 
			this.checkBox1_autoHideWhenUnfocus.AutoSize = true;
			this.checkBox1_autoHideWhenUnfocus.Location = new System.Drawing.Point(10, 290);
			this.checkBox1_autoHideWhenUnfocus.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox1_autoHideWhenUnfocus.Name = "checkBox1_autoHideWhenUnfocus";
			this.checkBox1_autoHideWhenUnfocus.Size = new System.Drawing.Size(185, 21);
			this.checkBox1_autoHideWhenUnfocus.TabIndex = 16;
			this.checkBox1_autoHideWhenUnfocus.Text = "Auto Hide when Unfocused";
			this.checkBox1_autoHideWhenUnfocus.UseVisualStyleBackColor = true;
			this.checkBox1_autoHideWhenUnfocus.CheckedChanged += new System.EventHandler(this.checkBox1_autoHideWhenUnfocus_CheckedChanged);
			// 
			// label2_hotkey
			// 
			this.label2_hotkey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2_hotkey.AutoSize = true;
			this.label2_hotkey.Location = new System.Drawing.Point(322, 13);
			this.label2_hotkey.Name = "label2_hotkey";
			this.label2_hotkey.Size = new System.Drawing.Size(134, 17);
			this.label2_hotkey.TabIndex = 17;
			this.label2_hotkey.Text = "Show Window Hotkey";
			// 
			// textBox_hotkey
			// 
			this.textBox_hotkey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_hotkey.Location = new System.Drawing.Point(288, 33);
			this.textBox_hotkey.Name = "textBox_hotkey";
			this.textBox_hotkey.ReadOnly = true;
			this.textBox_hotkey.Size = new System.Drawing.Size(168, 25);
			this.textBox_hotkey.TabIndex = 18;
			this.textBox_hotkey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_hotkey_KeyDown);
			// 
			// checkBox_enableHotkey
			// 
			this.checkBox_enableHotkey.AutoSize = true;
			this.checkBox_enableHotkey.Location = new System.Drawing.Point(10, 316);
			this.checkBox_enableHotkey.Name = "checkBox_enableHotkey";
			this.checkBox_enableHotkey.Size = new System.Drawing.Size(152, 21);
			this.checkBox_enableHotkey.TabIndex = 19;
			this.checkBox_enableHotkey.Text = "Enable Global Hotkey";
			this.checkBox_enableHotkey.UseVisualStyleBackColor = true;
			this.checkBox_enableHotkey.CheckedChanged += new System.EventHandler(this.checkBox_enableHotkey_CheckedChanged);
			// 
			// label_privacyDuration
			// 
			this.label_privacyDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_privacyDuration.AutoSize = true;
			this.label_privacyDuration.Location = new System.Drawing.Point(283, 110);
			this.label_privacyDuration.Name = "label_privacyDuration";
			this.label_privacyDuration.Size = new System.Drawing.Size(173, 17);
			this.label_privacyDuration.TabIndex = 20;
			this.label_privacyDuration.Text = "Private Mode Duration (min)";
			// 
			// textBox_privacyDuration
			// 
			this.textBox_privacyDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_privacyDuration.Location = new System.Drawing.Point(378, 130);
			this.textBox_privacyDuration.MaxLength = 2;
			this.textBox_privacyDuration.Name = "textBox_privacyDuration";
			this.textBox_privacyDuration.Size = new System.Drawing.Size(78, 25);
			this.textBox_privacyDuration.TabIndex = 21;
			// 
			// checkBox1_suppressClipboardEvents
			// 
			this.checkBox1_suppressClipboardEvents.AutoSize = true;
			this.checkBox1_suppressClipboardEvents.Location = new System.Drawing.Point(10, 122);
			this.checkBox1_suppressClipboardEvents.Name = "checkBox1_suppressClipboardEvents";
			this.checkBox1_suppressClipboardEvents.Size = new System.Drawing.Size(184, 21);
			this.checkBox1_suppressClipboardEvents.TabIndex = 22;
			this.checkBox1_suppressClipboardEvents.Text = "Suppress Clipboard Events";
			this.checkBox1_suppressClipboardEvents.UseVisualStyleBackColor = true;
			this.checkBox1_suppressClipboardEvents.CheckedChanged += new System.EventHandler(this.checkBox1_suppressClipboardEvents_CheckedChanged);
			// 
			// chbox_focusDetailWindow
			// 
			this.chbox_focusDetailWindow.AutoSize = true;
			this.chbox_focusDetailWindow.Location = new System.Drawing.Point(10, 343);
			this.chbox_focusDetailWindow.Name = "chbox_focusDetailWindow";
			this.chbox_focusDetailWindow.Size = new System.Drawing.Size(148, 21);
			this.chbox_focusDetailWindow.TabIndex = 23;
			this.chbox_focusDetailWindow.Text = "Focus Detail Window";
			this.chbox_focusDetailWindow.UseVisualStyleBackColor = true;
			this.chbox_focusDetailWindow.CheckedChanged += new System.EventHandler(this.chbox_focusDetailWindow_CheckedChanged);
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 441);
			this.Controls.Add(this.chbox_focusDetailWindow);
			this.Controls.Add(this.checkBox1_suppressClipboardEvents);
			this.Controls.Add(this.textBox_privacyDuration);
			this.Controls.Add(this.label_privacyDuration);
			this.Controls.Add(this.checkBox_enableHotkey);
			this.Controls.Add(this.textBox_hotkey);
			this.Controls.Add(this.label2_hotkey);
			this.Controls.Add(this.checkBox1_autoHideWhenUnfocus);
			this.Controls.Add(this.checkBox2_enableTimeStamps);
			this.Controls.Add(this.checkBox1_showInTaskbar);
			this.Controls.Add(this.checkBox1_alwaysTopMost);
			this.Controls.Add(this.checkBox1_clipboardHistory);
			this.Controls.Add(this.checkBox1_invertClipboardHistoryListing);
			this.Controls.Add(this.button2_Defaults);
			this.Controls.Add(this.checkBox4_preventClipboardDuplication);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1_maxHistoryCount);
			this.Controls.Add(this.checkBox3_checkUpdates);
			this.Controls.Add(this.checkBox2_hideToSystemTray);
			this.Controls.Add(this.checkBox1_startWithWindows);
			this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(480, 480);
			this.Name = "SettingsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Settings - HelloClipboard";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox1_startWithWindows;
		private System.Windows.Forms.CheckBox checkBox2_hideToSystemTray;
		private System.Windows.Forms.CheckBox checkBox3_checkUpdates;
		private System.Windows.Forms.TextBox textBox1_maxHistoryCount;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBox4_preventClipboardDuplication;
		private System.Windows.Forms.Button button2_Defaults;
		private System.Windows.Forms.CheckBox checkBox1_invertClipboardHistoryListing;
		private System.Windows.Forms.CheckBox checkBox1_clipboardHistory;
		private System.Windows.Forms.CheckBox checkBox1_alwaysTopMost;
		private System.Windows.Forms.CheckBox checkBox1_showInTaskbar;
		private System.Windows.Forms.CheckBox checkBox2_enableTimeStamps;
		private System.Windows.Forms.CheckBox checkBox1_autoHideWhenUnfocus;
		private System.Windows.Forms.Label label2_hotkey;
		private System.Windows.Forms.TextBox textBox_hotkey;
		private System.Windows.Forms.CheckBox checkBox_enableHotkey;
		private System.Windows.Forms.Label label_privacyDuration;
		private System.Windows.Forms.TextBox textBox_privacyDuration;
		private System.Windows.Forms.CheckBox checkBox1_suppressClipboardEvents;
		private System.Windows.Forms.CheckBox chbox_focusDetailWindow;
	}
}
