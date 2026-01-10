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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.MessagesListBox = new System.Windows.Forms.ListBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pinUnpinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.delToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cancelStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.androidSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.textBox1_search = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pcbox_clearClipboard = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.pcbox_togglePrivacy = new System.Windows.Forms.PictureBox();
			this.pcbox_topMost = new System.Windows.Forms.PictureBox();
			this.panel4 = new System.Windows.Forms.Panel();
			this.checkBoxCaseSensitive = new System.Windows.Forms.CheckBox();
			this.checkBoxRegex = new System.Windows.Forms.CheckBox();
			this.contextMenuStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcbox_clearClipboard)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcbox_togglePrivacy)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcbox_topMost)).BeginInit();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// MessagesListBox
			// 
			this.MessagesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MessagesListBox.ContextMenuStrip = this.contextMenuStrip1;
			this.MessagesListBox.FormattingEnabled = true;
			this.MessagesListBox.ItemHeight = 17;
			this.MessagesListBox.Location = new System.Drawing.Point(0, 0);
			this.MessagesListBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MessagesListBox.Name = "MessagesListBox";
			this.MessagesListBox.Size = new System.Drawing.Size(345, 327);
			this.MessagesListBox.TabIndex = 2;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.pinUnpinToolStripMenuItem,
            this.toolStripSeparator1,
            this.delToolStripMenuItem,
            this.cancelStripMenuItem1});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(129, 142);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Visible = false;
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// pinUnpinToolStripMenuItem
			// 
			this.pinUnpinToolStripMenuItem.Name = "pinUnpinToolStripMenuItem";
			this.pinUnpinToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
			this.pinUnpinToolStripMenuItem.Text = "Pin/Unpin";
			this.pinUnpinToolStripMenuItem.Click += new System.EventHandler(this.pinUnpinToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(125, 6);
			// 
			// delToolStripMenuItem
			// 
			this.delToolStripMenuItem.Name = "delToolStripMenuItem";
			this.delToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
			this.delToolStripMenuItem.Text = "Delete";
			this.delToolStripMenuItem.Click += new System.EventHandler(this.delToolStripMenuItem_Click);
			// 
			// cancelStripMenuItem1
			// 
			this.cancelStripMenuItem1.Name = "cancelStripMenuItem1";
			this.cancelStripMenuItem1.Size = new System.Drawing.Size(128, 22);
			this.cancelStripMenuItem1.Text = "Cancel";
			this.cancelStripMenuItem1.Click += new System.EventHandler(this.cancelStripMenuItem1_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.checkUpdateToolStripMenuItem,
            this.androidSyncToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(344, 25);
			this.menuStrip1.TabIndex = 4;
			this.menuStrip1.Text = "Info";
			// 
			// infoToolStripMenuItem
			// 
			this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
			this.infoToolStripMenuItem.Size = new System.Drawing.Size(42, 21);
			this.infoToolStripMenuItem.Text = "Info";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
			this.helpToolStripMenuItem.Text = "Help";
			this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(66, 21);
			this.settingsToolStripMenuItem.Text = "Settings";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// checkUpdateToolStripMenuItem
			// 
			this.checkUpdateToolStripMenuItem.Name = "checkUpdateToolStripMenuItem";
			this.checkUpdateToolStripMenuItem.Size = new System.Drawing.Size(101, 21);
			this.checkUpdateToolStripMenuItem.Text = "Check Update";
			this.checkUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkUpdateToolStripMenuItem_Click);
			// 
			// androidSyncToolStripMenuItem
			// 
			this.androidSyncToolStripMenuItem.Name = "androidSyncToolStripMenuItem";
			this.androidSyncToolStripMenuItem.Size = new System.Drawing.Size(86, 21);
			this.androidSyncToolStripMenuItem.Text = "Phone Sync";
			this.androidSyncToolStripMenuItem.Click += new System.EventHandler(this.phoneSyncToolStripMenuItem_Click);
			// 
			// textBox1_search
			// 
			this.textBox1_search.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1_search.Location = new System.Drawing.Point(37, 6);
			this.textBox1_search.Margin = new System.Windows.Forms.Padding(2);
			this.textBox1_search.Name = "textBox1_search";
			this.textBox1_search.Size = new System.Drawing.Size(162, 25);
			this.textBox1_search.TabIndex = 1;
			this.textBox1_search.TextChanged += new System.EventHandler(this.textBox1_search_TextChanged);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::HelloClipboard.Properties.Resources.icons8_search_512px;
			this.pictureBox1.Location = new System.Drawing.Point(9, 6);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(23, 23);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 5;
			this.pictureBox1.TabStop = false;
			// 
			// pcbox_clearClipboard
			// 
			this.pcbox_clearClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pcbox_clearClipboard.Image = global::HelloClipboard.Properties.Resources.icons8_broom_480px;
			this.pcbox_clearClipboard.Location = new System.Drawing.Point(238, 6);
			this.pcbox_clearClipboard.Margin = new System.Windows.Forms.Padding(2);
			this.pcbox_clearClipboard.Name = "pcbox_clearClipboard";
			this.pcbox_clearClipboard.Size = new System.Drawing.Size(23, 23);
			this.pcbox_clearClipboard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pcbox_clearClipboard.TabIndex = 6;
			this.pcbox_clearClipboard.TabStop = false;
			this.pcbox_clearClipboard.Click += new System.EventHandler(this.pcbox_clearClipboard_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 25);
			this.panel1.Margin = new System.Windows.Forms.Padding(2);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(344, 416);
			this.panel1.TabIndex = 7;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.statusStrip1);
			this.panel3.Controls.Add(this.MessagesListBox);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 57);
			this.panel3.Margin = new System.Windows.Forms.Padding(2);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(344, 359);
			this.panel3.TabIndex = 8;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 337);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 11, 0);
			this.statusStrip1.Size = new System.Drawing.Size(344, 22);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.pcbox_togglePrivacy);
			this.panel2.Controls.Add(this.pcbox_topMost);
			this.panel2.Controls.Add(this.pictureBox1);
			this.panel2.Controls.Add(this.pcbox_clearClipboard);
			this.panel2.Controls.Add(this.textBox1_search);
			this.panel2.Controls.Add(this.panel4);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Margin = new System.Windows.Forms.Padding(2);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(344, 57);
			this.panel2.TabIndex = 7;
			// 
			// pcbox_togglePrivacy
			// 
			this.pcbox_togglePrivacy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pcbox_togglePrivacy.BackColor = System.Drawing.Color.Black;
			this.pcbox_togglePrivacy.Image = global::HelloClipboard.Properties.Resources.eye_50px;
			this.pcbox_togglePrivacy.Location = new System.Drawing.Point(310, 6);
			this.pcbox_togglePrivacy.Margin = new System.Windows.Forms.Padding(2);
			this.pcbox_togglePrivacy.Name = "pcbox_togglePrivacy";
			this.pcbox_togglePrivacy.Size = new System.Drawing.Size(23, 23);
			this.pcbox_togglePrivacy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pcbox_togglePrivacy.TabIndex = 11;
			this.pcbox_togglePrivacy.TabStop = false;
			this.pcbox_togglePrivacy.Click += new System.EventHandler(this.pcbox_togglePrivacy_Click);
			// 
			// pcbox_topMost
			// 
			this.pcbox_topMost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pcbox_topMost.Image = global::HelloClipboard.Properties.Resources.icons8_unlocked_192px;
			this.pcbox_topMost.Location = new System.Drawing.Point(274, 6);
			this.pcbox_topMost.Margin = new System.Windows.Forms.Padding(2);
			this.pcbox_topMost.Name = "pcbox_topMost";
			this.pcbox_topMost.Size = new System.Drawing.Size(23, 23);
			this.pcbox_topMost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pcbox_topMost.TabIndex = 7;
			this.pcbox_topMost.TabStop = false;
			this.pcbox_topMost.Click += new System.EventHandler(this.pcbox_topMost_Click);
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.checkBoxCaseSensitive);
			this.panel4.Controls.Add(this.checkBoxRegex);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel4.Location = new System.Drawing.Point(0, 33);
			this.panel4.Margin = new System.Windows.Forms.Padding(2);
			this.panel4.Name = "panel4";
			this.panel4.Padding = new System.Windows.Forms.Padding(9, 0, 0, 0);
			this.panel4.Size = new System.Drawing.Size(344, 24);
			this.panel4.TabIndex = 10;
			// 
			// checkBoxCaseSensitive
			// 
			this.checkBoxCaseSensitive.AutoSize = true;
			this.checkBoxCaseSensitive.Dock = System.Windows.Forms.DockStyle.Left;
			this.checkBoxCaseSensitive.Location = new System.Drawing.Point(158, 0);
			this.checkBoxCaseSensitive.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxCaseSensitive.Name = "checkBoxCaseSensitive";
			this.checkBoxCaseSensitive.Size = new System.Drawing.Size(195, 24);
			this.checkBoxCaseSensitive.TabIndex = 9;
			this.checkBoxCaseSensitive.Text = "Enable Case Sensitive Search";
			this.checkBoxCaseSensitive.UseVisualStyleBackColor = true;
			this.checkBoxCaseSensitive.CheckedChanged += new System.EventHandler(this.checkBoxCaseSensitive_CheckedChanged);
			// 
			// checkBoxRegex
			// 
			this.checkBoxRegex.AutoSize = true;
			this.checkBoxRegex.Dock = System.Windows.Forms.DockStyle.Left;
			this.checkBoxRegex.Location = new System.Drawing.Point(9, 0);
			this.checkBoxRegex.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxRegex.Name = "checkBoxRegex";
			this.checkBoxRegex.Size = new System.Drawing.Size(149, 24);
			this.checkBoxRegex.TabIndex = 8;
			this.checkBoxRegex.Text = "Enable Regex Search";
			this.checkBoxRegex.UseVisualStyleBackColor = true;
			this.checkBoxRegex.CheckedChanged += new System.EventHandler(this.checkBoxRegex_CheckedChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 441);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MinimumSize = new System.Drawing.Size(360, 480);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "HelloClipboard";
			this.contextMenuStrip1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcbox_clearClipboard)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcbox_togglePrivacy)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcbox_topMost)).EndInit();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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

