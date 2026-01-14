namespace HelloClipboard
{
	partial class MainForm
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
				_viewModel.Dispose();
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pinUnpinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            delToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cancelStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            checkUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            androidSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(components);
            poisonStyleExtender1 = new ReaLTaiizor.Controls.PoisonStyleExtender(components);
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            panel5 = new System.Windows.Forms.Panel();
            pcbox_clearClipboard = new System.Windows.Forms.PictureBox();
            pcbox_topMost = new System.Windows.Forms.PictureBox();
            panel6 = new System.Windows.Forms.Panel();
            panel3 = new System.Windows.Forms.Panel();
            poisonTextBox1_search = new ReaLTaiizor.Controls.PoisonTextBox();
            pictureBox2_searchSettings = new System.Windows.Forms.PictureBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            pcbox_togglePrivacy = new System.Windows.Forms.PictureBox();
            panel_searchSettings = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            poisonLabel2 = new ReaLTaiizor.Controls.PoisonLabel();
            poisonToggle2_caseSens = new ReaLTaiizor.Controls.PoisonToggle();
            panel1 = new System.Windows.Forms.Panel();
            poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            poisonToggle1_regex = new ReaLTaiizor.Controls.PoisonToggle();
            poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            panel4 = new System.Windows.Forms.Panel();
            MessagesListBox = new System.Windows.Forms.ListBox();
            contextMenuStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).BeginInit();
            statusStrip1.SuspendLayout();
            panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pcbox_clearClipboard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pcbox_topMost).BeginInit();
            panel6.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2_searchSettings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pcbox_togglePrivacy).BeginInit();
            panel_searchSettings.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            poisonPanel1.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { copyToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, pinUnpinToolStripMenuItem, toolStripSeparator1, delToolStripMenuItem, cancelStripMenuItem1 });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(129, 142);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Visible = false;
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // pinUnpinToolStripMenuItem
            // 
            pinUnpinToolStripMenuItem.Name = "pinUnpinToolStripMenuItem";
            pinUnpinToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            pinUnpinToolStripMenuItem.Text = "Pin/Unpin";
            pinUnpinToolStripMenuItem.Click += pinUnpinToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(125, 6);
            // 
            // delToolStripMenuItem
            // 
            delToolStripMenuItem.Name = "delToolStripMenuItem";
            delToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            delToolStripMenuItem.Text = "Delete";
            delToolStripMenuItem.Click += delToolStripMenuItem_Click;
            // 
            // cancelStripMenuItem1
            // 
            cancelStripMenuItem1.Name = "cancelStripMenuItem1";
            cancelStripMenuItem1.Size = new System.Drawing.Size(128, 22);
            cancelStripMenuItem1.Text = "Cancel";
            cancelStripMenuItem1.Click += cancelStripMenuItem1_Click;
            // 
            // menuStrip1
            // 
            poisonStyleExtender1.SetApplyPoisonTheme(menuStrip1, true);
            menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { infoToolStripMenuItem, settingsToolStripMenuItem, checkUpdateToolStripMenuItem, androidSyncToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(20, 60);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            menuStrip1.Size = new System.Drawing.Size(380, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "Info";
            // 
            // infoToolStripMenuItem
            // 
            infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aboutToolStripMenuItem });
            infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            infoToolStripMenuItem.Size = new System.Drawing.Size(42, 24);
            infoToolStripMenuItem.Text = "Info";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // checkUpdateToolStripMenuItem
            // 
            checkUpdateToolStripMenuItem.Name = "checkUpdateToolStripMenuItem";
            checkUpdateToolStripMenuItem.Size = new System.Drawing.Size(101, 24);
            checkUpdateToolStripMenuItem.Text = "Check Update";
            checkUpdateToolStripMenuItem.Click += checkUpdateToolStripMenuItem_Click;
            // 
            // androidSyncToolStripMenuItem
            // 
            androidSyncToolStripMenuItem.Name = "androidSyncToolStripMenuItem";
            androidSyncToolStripMenuItem.Size = new System.Drawing.Size(86, 24);
            androidSyncToolStripMenuItem.Text = "Phone Sync";
            androidSyncToolStripMenuItem.Click += phoneSyncToolStripMenuItem_Click;
            // 
            // poisonStyleManager1
            // 
            poisonStyleManager1.Owner = this;
            // 
            // statusStrip1
            // 
            poisonStyleExtender1.SetApplyPoisonTheme(statusStrip1, true);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip1.Location = new System.Drawing.Point(0, 415);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 11, 0);
            statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            statusStrip1.Size = new System.Drawing.Size(380, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.ForeColor = System.Drawing.Color.DeepSkyBlue;
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // panel5
            // 
            panel5.AutoSize = true;
            panel5.Controls.Add(pcbox_clearClipboard);
            panel5.Controls.Add(pcbox_topMost);
            panel5.Controls.Add(panel6);
            panel5.Controls.Add(pcbox_togglePrivacy);
            panel5.Dock = System.Windows.Forms.DockStyle.Top;
            panel5.Location = new System.Drawing.Point(20, 84);
            panel5.Margin = new System.Windows.Forms.Padding(0);
            panel5.Name = "panel5";
            panel5.Padding = new System.Windows.Forms.Padding(0, 8, 0, 8);
            panel5.Size = new System.Drawing.Size(380, 43);
            panel5.TabIndex = 13;
            // 
            // pcbox_clearClipboard
            // 
            pcbox_clearClipboard.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pcbox_clearClipboard.Image = Properties.Resources.trash_can_40px;
            pcbox_clearClipboard.Location = new System.Drawing.Point(292, 8);
            pcbox_clearClipboard.Margin = new System.Windows.Forms.Padding(0);
            pcbox_clearClipboard.Name = "pcbox_clearClipboard";
            pcbox_clearClipboard.Size = new System.Drawing.Size(24, 24);
            pcbox_clearClipboard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pcbox_clearClipboard.TabIndex = 6;
            pcbox_clearClipboard.TabStop = false;
            pcbox_clearClipboard.Click += pcbox_clearClipboard_Click;
            // 
            // pcbox_topMost
            // 
            pcbox_topMost.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pcbox_topMost.Image = Properties.Resources.unlock_40px;
            pcbox_topMost.Location = new System.Drawing.Point(324, 8);
            pcbox_topMost.Margin = new System.Windows.Forms.Padding(0);
            pcbox_topMost.Name = "pcbox_topMost";
            pcbox_topMost.Size = new System.Drawing.Size(24, 24);
            pcbox_topMost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pcbox_topMost.TabIndex = 7;
            pcbox_topMost.TabStop = false;
            pcbox_topMost.Click += pcbox_topMost_Click;
            // 
            // panel6
            // 
            panel6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel6.Controls.Add(panel3);
            panel6.Controls.Add(pictureBox2_searchSettings);
            panel6.Controls.Add(pictureBox1);
            panel6.Location = new System.Drawing.Point(0, 8);
            panel6.Name = "panel6";
            panel6.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            panel6.Size = new System.Drawing.Size(262, 24);
            panel6.TabIndex = 15;
            // 
            // panel3
            // 
            panel3.AutoSize = true;
            panel3.Controls.Add(poisonTextBox1_search);
            panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            panel3.Location = new System.Drawing.Point(36, 0);
            panel3.Name = "panel3";
            panel3.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            panel3.Size = new System.Drawing.Size(202, 24);
            panel3.TabIndex = 14;
            // 
            // poisonTextBox1_search
            // 
            // 
            // 
            // 
            poisonTextBox1_search.CustomButton.Image = null;
            poisonTextBox1_search.CustomButton.Location = new System.Drawing.Point(164, 2);
            poisonTextBox1_search.CustomButton.Name = "";
            poisonTextBox1_search.CustomButton.Size = new System.Drawing.Size(19, 19);
            poisonTextBox1_search.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            poisonTextBox1_search.CustomButton.TabIndex = 1;
            poisonTextBox1_search.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            poisonTextBox1_search.CustomButton.UseSelectable = true;
            poisonTextBox1_search.CustomButton.Visible = false;
            poisonTextBox1_search.Dock = System.Windows.Forms.DockStyle.Fill;
            poisonTextBox1_search.Location = new System.Drawing.Point(8, 0);
            poisonTextBox1_search.MaxLength = 32767;
            poisonTextBox1_search.MinimumSize = new System.Drawing.Size(155, 24);
            poisonTextBox1_search.Name = "poisonTextBox1_search";
            poisonTextBox1_search.PasswordChar = '\0';
            poisonTextBox1_search.PromptText = "Search...";
            poisonTextBox1_search.ScrollBars = System.Windows.Forms.ScrollBars.None;
            poisonTextBox1_search.SelectedText = "";
            poisonTextBox1_search.SelectionLength = 0;
            poisonTextBox1_search.SelectionStart = 0;
            poisonTextBox1_search.ShortcutsEnabled = true;
            poisonTextBox1_search.ShowClearButton = true;
            poisonTextBox1_search.Size = new System.Drawing.Size(186, 24);
            poisonTextBox1_search.TabIndex = 12;
            poisonTextBox1_search.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            poisonTextBox1_search.UseSelectable = true;
            poisonTextBox1_search.WaterMark = "Search...";
            poisonTextBox1_search.WaterMarkColor = System.Drawing.Color.FromArgb(109, 109, 109);
            poisonTextBox1_search.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // pictureBox2_searchSettings
            // 
            pictureBox2_searchSettings.BackColor = System.Drawing.Color.Transparent;
            pictureBox2_searchSettings.Dock = System.Windows.Forms.DockStyle.Right;
            pictureBox2_searchSettings.Image = Properties.Resources.settings_40px;
            pictureBox2_searchSettings.Location = new System.Drawing.Point(238, 0);
            pictureBox2_searchSettings.Margin = new System.Windows.Forms.Padding(2);
            pictureBox2_searchSettings.Name = "pictureBox2_searchSettings";
            pictureBox2_searchSettings.Size = new System.Drawing.Size(24, 24);
            pictureBox2_searchSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox2_searchSettings.TabIndex = 13;
            pictureBox2_searchSettings.TabStop = false;
            pictureBox2_searchSettings.Click += pictureBox2_searchSettings_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            pictureBox1.Image = Properties.Resources.search_40px;
            pictureBox1.Location = new System.Drawing.Point(12, 0);
            pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(24, 24);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // pcbox_togglePrivacy
            // 
            pcbox_togglePrivacy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pcbox_togglePrivacy.BackColor = System.Drawing.Color.Transparent;
            pcbox_togglePrivacy.Image = Properties.Resources.eye_40px;
            pcbox_togglePrivacy.Location = new System.Drawing.Point(356, 8);
            pcbox_togglePrivacy.Margin = new System.Windows.Forms.Padding(0);
            pcbox_togglePrivacy.Name = "pcbox_togglePrivacy";
            pcbox_togglePrivacy.Size = new System.Drawing.Size(24, 24);
            pcbox_togglePrivacy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pcbox_togglePrivacy.TabIndex = 11;
            pcbox_togglePrivacy.TabStop = false;
            pcbox_togglePrivacy.Click += pcbox_togglePrivacy_Click;
            // 
            // panel_searchSettings
            // 
            panel_searchSettings.AutoSize = true;
            panel_searchSettings.Controls.Add(panel2);
            panel_searchSettings.Controls.Add(panel1);
            panel_searchSettings.Dock = System.Windows.Forms.DockStyle.Top;
            panel_searchSettings.Location = new System.Drawing.Point(20, 127);
            panel_searchSettings.Margin = new System.Windows.Forms.Padding(2);
            panel_searchSettings.Name = "panel_searchSettings";
            panel_searchSettings.Size = new System.Drawing.Size(380, 56);
            panel_searchSettings.TabIndex = 14;
            panel_searchSettings.Visible = false;
            // 
            // panel2
            // 
            panel2.AutoSize = true;
            panel2.Controls.Add(poisonLabel2);
            panel2.Controls.Add(poisonToggle2_caseSens);
            panel2.Dock = System.Windows.Forms.DockStyle.Top;
            panel2.Location = new System.Drawing.Point(0, 28);
            panel2.Name = "panel2";
            panel2.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            panel2.Size = new System.Drawing.Size(380, 28);
            panel2.TabIndex = 5;
            // 
            // poisonLabel2
            // 
            poisonLabel2.AutoSize = true;
            poisonLabel2.Location = new System.Drawing.Point(27, 4);
            poisonLabel2.Name = "poisonLabel2";
            poisonLabel2.Size = new System.Drawing.Size(97, 19);
            poisonLabel2.TabIndex = 2;
            poisonLabel2.Text = "Case Sensetive:";
            poisonLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // poisonToggle2_caseSens
            // 
            poisonToggle2_caseSens.AutoSize = true;
            poisonToggle2_caseSens.Location = new System.Drawing.Point(130, 4);
            poisonToggle2_caseSens.Name = "poisonToggle2_caseSens";
            poisonToggle2_caseSens.Size = new System.Drawing.Size(80, 21);
            poisonToggle2_caseSens.TabIndex = 3;
            poisonToggle2_caseSens.Text = "Off";
            poisonToggle2_caseSens.UseSelectable = true;
            poisonToggle2_caseSens.CheckedChanged += poisonToggle2_caseSens_CheckedChanged;
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.Controls.Add(poisonLabel1);
            panel1.Controls.Add(poisonToggle1_regex);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            panel1.Size = new System.Drawing.Size(380, 28);
            panel1.TabIndex = 4;
            // 
            // poisonLabel1
            // 
            poisonLabel1.AutoSize = true;
            poisonLabel1.Location = new System.Drawing.Point(27, 4);
            poisonLabel1.Name = "poisonLabel1";
            poisonLabel1.Size = new System.Drawing.Size(91, 19);
            poisonLabel1.TabIndex = 0;
            poisonLabel1.Text = "Regex Search:";
            poisonLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // poisonToggle1_regex
            // 
            poisonToggle1_regex.AutoSize = true;
            poisonToggle1_regex.Location = new System.Drawing.Point(130, 4);
            poisonToggle1_regex.Name = "poisonToggle1_regex";
            poisonToggle1_regex.Size = new System.Drawing.Size(80, 21);
            poisonToggle1_regex.TabIndex = 1;
            poisonToggle1_regex.Text = "Off";
            poisonToggle1_regex.UseSelectable = true;
            poisonToggle1_regex.CheckedChanged += poisonToggle1_regex_CheckedChanged;
            // 
            // poisonPanel1
            // 
            poisonPanel1.Controls.Add(panel4);
            poisonPanel1.Controls.Add(statusStrip1);
            poisonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            poisonPanel1.HorizontalScrollbarBarColor = true;
            poisonPanel1.HorizontalScrollbarHighlightOnWheel = false;
            poisonPanel1.HorizontalScrollbarSize = 10;
            poisonPanel1.Location = new System.Drawing.Point(20, 183);
            poisonPanel1.Name = "poisonPanel1";
            poisonPanel1.Size = new System.Drawing.Size(380, 437);
            poisonPanel1.TabIndex = 4;
            poisonPanel1.VerticalScrollbarBarColor = true;
            poisonPanel1.VerticalScrollbarHighlightOnWheel = false;
            poisonPanel1.VerticalScrollbarSize = 10;
            // 
            // panel4
            // 
            panel4.Controls.Add(MessagesListBox);
            panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            panel4.Location = new System.Drawing.Point(0, 0);
            panel4.Name = "panel4";
            panel4.Size = new System.Drawing.Size(380, 415);
            panel4.TabIndex = 4;
            // 
            // MessagesListBox
            // 
            MessagesListBox.ContextMenuStrip = contextMenuStrip1;
            MessagesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            MessagesListBox.FormattingEnabled = true;
            MessagesListBox.Location = new System.Drawing.Point(0, 0);
            MessagesListBox.Margin = new System.Windows.Forms.Padding(0);
            MessagesListBox.Name = "MessagesListBox";
            MessagesListBox.Size = new System.Drawing.Size(380, 415);
            MessagesListBox.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(420, 640);
            Controls.Add(poisonPanel1);
            Controls.Add(panel_searchSettings);
            Controls.Add(panel5);
            Controls.Add(menuStrip1);
            Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MinimumSize = new System.Drawing.Size(420, 480);
            Name = "MainForm";
            ShadowType = ReaLTaiizor.Enum.Poison.FormShadowType.AeroShadow;
            Text = "HelloClipboard";
            contextMenuStrip1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pcbox_clearClipboard).EndInit();
            ((System.ComponentModel.ISupportInitialize)pcbox_topMost).EndInit();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2_searchSettings).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pcbox_togglePrivacy).EndInit();
            panel_searchSettings.ResumeLayout(false);
            panel_searchSettings.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            poisonPanel1.ResumeLayout(false);
            poisonPanel1.PerformLayout();
            panel4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem checkUpdateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem androidSyncToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pinUnpinToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cancelStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem delToolStripMenuItem;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonStyleExtender poisonStyleExtender1;
        private System.Windows.Forms.Panel panel_searchSettings;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ReaLTaiizor.Controls.PoisonTextBox poisonTextBox1_search;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel2;
        private ReaLTaiizor.Controls.PoisonToggle poisonToggle1_regex;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
        private ReaLTaiizor.Controls.PoisonToggle poisonToggle2_caseSens;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pictureBox2_searchSettings;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ListBox MessagesListBox;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.PictureBox pcbox_clearClipboard;
        private System.Windows.Forms.PictureBox pcbox_topMost;
        private System.Windows.Forms.PictureBox pcbox_togglePrivacy;
    }
}

