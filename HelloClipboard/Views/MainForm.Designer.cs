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
            MessagesListBox = new System.Windows.Forms.ListBox();
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
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            checkUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            androidSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            textBox1_search = new System.Windows.Forms.TextBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            pcbox_clearClipboard = new System.Windows.Forms.PictureBox();
            panel1 = new System.Windows.Forms.Panel();
            panel3 = new System.Windows.Forms.Panel();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            panel2 = new System.Windows.Forms.Panel();
            pcbox_togglePrivacy = new System.Windows.Forms.PictureBox();
            pcbox_topMost = new System.Windows.Forms.PictureBox();
            panel4 = new System.Windows.Forms.Panel();
            checkBoxCaseSensitive = new System.Windows.Forms.CheckBox();
            checkBoxRegex = new System.Windows.Forms.CheckBox();
            contextMenuStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pcbox_clearClipboard).BeginInit();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            statusStrip1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pcbox_togglePrivacy).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pcbox_topMost).BeginInit();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // MessagesListBox
            // 
            MessagesListBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            MessagesListBox.ContextMenuStrip = contextMenuStrip1;
            MessagesListBox.FormattingEnabled = true;
            MessagesListBox.Location = new System.Drawing.Point(0, 0);
            MessagesListBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MessagesListBox.Name = "MessagesListBox";
            MessagesListBox.Size = new System.Drawing.Size(345, 344);
            MessagesListBox.TabIndex = 2;
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
            menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { infoToolStripMenuItem, settingsToolStripMenuItem, checkUpdateToolStripMenuItem, androidSyncToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(344, 25);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "Info";
            // 
            // infoToolStripMenuItem
            // 
            infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aboutToolStripMenuItem, helpToolStripMenuItem });
            infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            infoToolStripMenuItem.Size = new System.Drawing.Size(42, 21);
            infoToolStripMenuItem.Text = "Info";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            helpToolStripMenuItem.Text = "Help";
            helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new System.Drawing.Size(66, 21);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // checkUpdateToolStripMenuItem
            // 
            checkUpdateToolStripMenuItem.Name = "checkUpdateToolStripMenuItem";
            checkUpdateToolStripMenuItem.Size = new System.Drawing.Size(101, 21);
            checkUpdateToolStripMenuItem.Text = "Check Update";
            checkUpdateToolStripMenuItem.Click += checkUpdateToolStripMenuItem_Click;
            // 
            // androidSyncToolStripMenuItem
            // 
            androidSyncToolStripMenuItem.Name = "androidSyncToolStripMenuItem";
            androidSyncToolStripMenuItem.Size = new System.Drawing.Size(86, 21);
            androidSyncToolStripMenuItem.Text = "Phone Sync";
            androidSyncToolStripMenuItem.Click += phoneSyncToolStripMenuItem_Click;
            // 
            // textBox1_search
            // 
            textBox1_search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBox1_search.Location = new System.Drawing.Point(37, 6);
            textBox1_search.Margin = new System.Windows.Forms.Padding(2);
            textBox1_search.Name = "textBox1_search";
            textBox1_search.Size = new System.Drawing.Size(162, 25);
            textBox1_search.TabIndex = 1;
            textBox1_search.TextChanged += textBox1_search_TextChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_search_512px;
            pictureBox1.Location = new System.Drawing.Point(9, 6);
            pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(23, 23);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // pcbox_clearClipboard
            // 
            pcbox_clearClipboard.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pcbox_clearClipboard.Image = Properties.Resources.icons8_broom_480px;
            pcbox_clearClipboard.Location = new System.Drawing.Point(238, 6);
            pcbox_clearClipboard.Margin = new System.Windows.Forms.Padding(2);
            pcbox_clearClipboard.Name = "pcbox_clearClipboard";
            pcbox_clearClipboard.Size = new System.Drawing.Size(23, 23);
            pcbox_clearClipboard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pcbox_clearClipboard.TabIndex = 6;
            pcbox_clearClipboard.TabStop = false;
            pcbox_clearClipboard.Click += pcbox_clearClipboard_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 25);
            panel1.Margin = new System.Windows.Forms.Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(344, 422);
            panel1.TabIndex = 7;
            // 
            // panel3
            // 
            panel3.Controls.Add(statusStrip1);
            panel3.Controls.Add(MessagesListBox);
            panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            panel3.Location = new System.Drawing.Point(0, 57);
            panel3.Margin = new System.Windows.Forms.Padding(2);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(344, 365);
            panel3.TabIndex = 8;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new System.Drawing.Point(0, 343);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 11, 0);
            statusStrip1.Size = new System.Drawing.Size(344, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // panel2
            // 
            panel2.Controls.Add(pcbox_togglePrivacy);
            panel2.Controls.Add(pcbox_topMost);
            panel2.Controls.Add(pictureBox1);
            panel2.Controls.Add(pcbox_clearClipboard);
            panel2.Controls.Add(textBox1_search);
            panel2.Controls.Add(panel4);
            panel2.Dock = System.Windows.Forms.DockStyle.Top;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Margin = new System.Windows.Forms.Padding(2);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(344, 57);
            panel2.TabIndex = 7;
            // 
            // pcbox_togglePrivacy
            // 
            pcbox_togglePrivacy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pcbox_togglePrivacy.BackColor = System.Drawing.Color.Black;
            pcbox_togglePrivacy.Image = Properties.Resources.eye_50px;
            pcbox_togglePrivacy.Location = new System.Drawing.Point(310, 6);
            pcbox_togglePrivacy.Margin = new System.Windows.Forms.Padding(2);
            pcbox_togglePrivacy.Name = "pcbox_togglePrivacy";
            pcbox_togglePrivacy.Size = new System.Drawing.Size(23, 23);
            pcbox_togglePrivacy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pcbox_togglePrivacy.TabIndex = 11;
            pcbox_togglePrivacy.TabStop = false;
            pcbox_togglePrivacy.Click += pcbox_togglePrivacy_Click;
            // 
            // pcbox_topMost
            // 
            pcbox_topMost.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pcbox_topMost.Image = Properties.Resources.icons8_unlocked_192px;
            pcbox_topMost.Location = new System.Drawing.Point(274, 6);
            pcbox_topMost.Margin = new System.Windows.Forms.Padding(2);
            pcbox_topMost.Name = "pcbox_topMost";
            pcbox_topMost.Size = new System.Drawing.Size(23, 23);
            pcbox_topMost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pcbox_topMost.TabIndex = 7;
            pcbox_topMost.TabStop = false;
            pcbox_topMost.Click += pcbox_topMost_Click;
            // 
            // panel4
            // 
            panel4.Controls.Add(checkBoxCaseSensitive);
            panel4.Controls.Add(checkBoxRegex);
            panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel4.Location = new System.Drawing.Point(0, 33);
            panel4.Margin = new System.Windows.Forms.Padding(2);
            panel4.Name = "panel4";
            panel4.Padding = new System.Windows.Forms.Padding(9, 0, 0, 0);
            panel4.Size = new System.Drawing.Size(344, 24);
            panel4.TabIndex = 10;
            // 
            // checkBoxCaseSensitive
            // 
            checkBoxCaseSensitive.AutoSize = true;
            checkBoxCaseSensitive.Dock = System.Windows.Forms.DockStyle.Left;
            checkBoxCaseSensitive.Location = new System.Drawing.Point(158, 0);
            checkBoxCaseSensitive.Margin = new System.Windows.Forms.Padding(2);
            checkBoxCaseSensitive.Name = "checkBoxCaseSensitive";
            checkBoxCaseSensitive.Size = new System.Drawing.Size(195, 24);
            checkBoxCaseSensitive.TabIndex = 9;
            checkBoxCaseSensitive.Text = "Enable Case Sensitive Search";
            checkBoxCaseSensitive.UseVisualStyleBackColor = true;
            checkBoxCaseSensitive.CheckedChanged += checkBoxCaseSensitive_CheckedChanged;
            // 
            // checkBoxRegex
            // 
            checkBoxRegex.AutoSize = true;
            checkBoxRegex.Dock = System.Windows.Forms.DockStyle.Left;
            checkBoxRegex.Location = new System.Drawing.Point(9, 0);
            checkBoxRegex.Margin = new System.Windows.Forms.Padding(2);
            checkBoxRegex.Name = "checkBoxRegex";
            checkBoxRegex.Size = new System.Drawing.Size(149, 24);
            checkBoxRegex.TabIndex = 8;
            checkBoxRegex.Text = "Enable Regex Search";
            checkBoxRegex.UseVisualStyleBackColor = true;
            checkBoxRegex.CheckedChanged += checkBoxRegex_CheckedChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(344, 447);
            Controls.Add(panel1);
            Controls.Add(menuStrip1);
            Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MinimumSize = new System.Drawing.Size(360, 480);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "HelloClipboard";
            contextMenuStrip1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pcbox_clearClipboard).EndInit();
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pcbox_togglePrivacy).EndInit();
            ((System.ComponentModel.ISupportInitialize)pcbox_topMost).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox MessagesListBox;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem checkUpdateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem androidSyncToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pinUnpinToolStripMenuItem;
		private System.Windows.Forms.TextBox textBox1_search;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pcbox_clearClipboard;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.PictureBox pcbox_topMost;
		private System.Windows.Forms.CheckBox checkBoxRegex;
		private System.Windows.Forms.CheckBox checkBoxCaseSensitive;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripMenuItem cancelStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem delToolStripMenuItem;
		private System.Windows.Forms.PictureBox pcbox_togglePrivacy;
    }
}

