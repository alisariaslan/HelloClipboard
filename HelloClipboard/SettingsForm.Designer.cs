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
			this.checkBox5_enableBetterHistoryVisualization = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button2_Defaults = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// checkBox1_startWithWindows
			// 
			this.checkBox1_startWithWindows.AutoSize = true;
			this.checkBox1_startWithWindows.Location = new System.Drawing.Point(13, 47);
			this.checkBox1_startWithWindows.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.checkBox1_startWithWindows.Name = "checkBox1_startWithWindows";
			this.checkBox1_startWithWindows.Size = new System.Drawing.Size(164, 25);
			this.checkBox1_startWithWindows.TabIndex = 0;
			this.checkBox1_startWithWindows.Text = "Start with Windows";
			this.checkBox1_startWithWindows.UseVisualStyleBackColor = true;
			this.checkBox1_startWithWindows.CheckedChanged += new System.EventHandler(this.checkBox1_startWithWindows_CheckedChanged);
			// 
			// checkBox2_hideToSystemTray
			// 
			this.checkBox2_hideToSystemTray.AutoSize = true;
			this.checkBox2_hideToSystemTray.Location = new System.Drawing.Point(13, 82);
			this.checkBox2_hideToSystemTray.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.checkBox2_hideToSystemTray.Name = "checkBox2_hideToSystemTray";
			this.checkBox2_hideToSystemTray.Size = new System.Drawing.Size(167, 25);
			this.checkBox2_hideToSystemTray.TabIndex = 1;
			this.checkBox2_hideToSystemTray.Text = "Hide to System Tray";
			this.checkBox2_hideToSystemTray.UseVisualStyleBackColor = true;
			this.checkBox2_hideToSystemTray.CheckedChanged += new System.EventHandler(this.checkBox2_hideToSystemTray_CheckedChanged);
			// 
			// checkBox3_checkUpdates
			// 
			this.checkBox3_checkUpdates.AutoSize = true;
			this.checkBox3_checkUpdates.Location = new System.Drawing.Point(13, 12);
			this.checkBox3_checkUpdates.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.checkBox3_checkUpdates.Name = "checkBox3_checkUpdates";
			this.checkBox3_checkUpdates.Size = new System.Drawing.Size(132, 25);
			this.checkBox3_checkUpdates.TabIndex = 2;
			this.checkBox3_checkUpdates.Text = "Check Updates";
			this.checkBox3_checkUpdates.UseVisualStyleBackColor = true;
			this.checkBox3_checkUpdates.CheckedChanged += new System.EventHandler(this.checkBox3_checkUpdates_CheckedChanged);
			// 
			// textBox1_maxHistoryCount
			// 
			this.textBox1_maxHistoryCount.Location = new System.Drawing.Point(160, 308);
			this.textBox1_maxHistoryCount.Name = "textBox1_maxHistoryCount";
			this.textBox1_maxHistoryCount.Size = new System.Drawing.Size(100, 29);
			this.textBox1_maxHistoryCount.TabIndex = 4;
			this.textBox1_maxHistoryCount.Text = "1000";
			this.textBox1_maxHistoryCount.TextChanged += new System.EventHandler(this.textBox1_maxHistoryCount_TextChanged);
			this.textBox1_maxHistoryCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_maxHistoryCount_KeyPress);
			this.textBox1_maxHistoryCount.Leave += new System.EventHandler(this.textBox1_maxHistoryCount_Leave);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 311);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(142, 21);
			this.label1.TabIndex = 5;
			this.label1.Text = "Max History Count:";
			// 
			// checkBox4_preventClipboardDuplication
			// 
			this.checkBox4_preventClipboardDuplication.AutoSize = true;
			this.checkBox4_preventClipboardDuplication.Location = new System.Drawing.Point(12, 169);
			this.checkBox4_preventClipboardDuplication.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.checkBox4_preventClipboardDuplication.Name = "checkBox4_preventClipboardDuplication";
			this.checkBox4_preventClipboardDuplication.Size = new System.Drawing.Size(237, 25);
			this.checkBox4_preventClipboardDuplication.TabIndex = 6;
			this.checkBox4_preventClipboardDuplication.Text = "Prevent Clipboard Duplication";
			this.checkBox4_preventClipboardDuplication.UseVisualStyleBackColor = true;
			this.checkBox4_preventClipboardDuplication.CheckedChanged += new System.EventHandler(this.checkBox4_preventClipboardDuplication_CheckedChanged);
			// 
			// checkBox5_enableBetterHistoryVisualization
			// 
			this.checkBox5_enableBetterHistoryVisualization.AutoSize = true;
			this.checkBox5_enableBetterHistoryVisualization.Location = new System.Drawing.Point(13, 204);
			this.checkBox5_enableBetterHistoryVisualization.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.checkBox5_enableBetterHistoryVisualization.Name = "checkBox5_enableBetterHistoryVisualization";
			this.checkBox5_enableBetterHistoryVisualization.Size = new System.Drawing.Size(266, 25);
			this.checkBox5_enableBetterHistoryVisualization.TabIndex = 7;
			this.checkBox5_enableBetterHistoryVisualization.Text = "Enable Better History Visualization";
			this.checkBox5_enableBetterHistoryVisualization.UseVisualStyleBackColor = true;
			this.checkBox5_enableBetterHistoryVisualization.CheckedChanged += new System.EventHandler(this.checkBox5_enableBetterHistoryVisualization_CheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(12, 234);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(407, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "When data is added to the history, spaces and new lines are visually removed.";
			// 
			// button2_Defaults
			// 
			this.button2_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button2_Defaults.Location = new System.Drawing.Point(12, 396);
			this.button2_Defaults.Name = "button2_Defaults";
			this.button2_Defaults.Size = new System.Drawing.Size(440, 33);
			this.button2_Defaults.TabIndex = 9;
			this.button2_Defaults.Text = "Reset to Defaults";
			this.button2_Defaults.UseVisualStyleBackColor = true;
			this.button2_Defaults.Click += new System.EventHandler(this.button2_Defaults_Click);
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 441);
			this.Controls.Add(this.button2_Defaults);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkBox5_enableBetterHistoryVisualization);
			this.Controls.Add(this.checkBox4_preventClipboardDuplication);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1_maxHistoryCount);
			this.Controls.Add(this.checkBox3_checkUpdates);
			this.Controls.Add(this.checkBox2_hideToSystemTray);
			this.Controls.Add(this.checkBox1_startWithWindows);
			this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(480, 480);
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
		private System.Windows.Forms.CheckBox checkBox5_enableBetterHistoryVisualization;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button2_Defaults;
	}
}