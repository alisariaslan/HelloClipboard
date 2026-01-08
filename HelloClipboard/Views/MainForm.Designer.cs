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
			this.saveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.androidSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.textBox1_search = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.pictureBox3_topMost = new System.Windows.Forms.PictureBox();
			this.checkBoxRegex = new System.Windows.Forms.CheckBox();
			this.checkBoxCaseSensitive = new System.Windows.Forms.CheckBox();
			this.contextMenuStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3_topMost)).BeginInit();
			this.SuspendLayout();
			// 
			// MessagesListBox
			// 
			this.MessagesListBox.ContextMenuStrip = this.contextMenuStrip1;
			this.MessagesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MessagesListBox.FormattingEnabled = true;
			this.MessagesListBox.ItemHeight = 21;
			this.MessagesListBox.Location = new System.Drawing.Point(0, 0);
			this.MessagesListBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MessagesListBox.Name = "MessagesListBox";
			this.MessagesListBox.Size = new System.Drawing.Size(464, 609);
			this.MessagesListBox.TabIndex = 2;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.saveToFileToolStripMenuItem,
            this.openUrlToolStripMenuItem,
            this.pinToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(181, 92);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// saveToFileToolStripMenuItem
			// 
			this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
			this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.saveToFileToolStripMenuItem.Text = "Save to file";
			this.saveToFileToolStripMenuItem.Click += new System.EventHandler(this.saveToFileToolStripMenuItem_Click);
			// 
			// openUrlToolStripMenuItem
			// 
			this.openUrlToolStripMenuItem.Name = "openUrlToolStripMenuItem";
			this.openUrlToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.openUrlToolStripMenuItem.Text = "Open in browser";
			this.openUrlToolStripMenuItem.Visible = false;
			this.openUrlToolStripMenuItem.Click += new System.EventHandler(this.openUrlToolStripMenuItem_Click);
			// 
			// pinToolStripMenuItem
			// 
			this.pinToolStripMenuItem.Name = "pinToolStripMenuItem";
			this.pinToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.pinToolStripMenuItem.Text = "Pin/Unpin";
			this.pinToolStripMenuItem.Click += new System.EventHandler(this.pinToolStripMenuItem_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.checkUpdateToolStripMenuItem,
            this.androidSyncToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(464, 29);
			this.menuStrip1.TabIndex = 4;
			this.menuStrip1.Text = "Info";
			// 
			// infoToolStripMenuItem
			// 
			this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
			this.infoToolStripMenuItem.Size = new System.Drawing.Size(49, 25);
			this.infoToolStripMenuItem.Text = "Info";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 26);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(122, 26);
			this.helpToolStripMenuItem.Text = "Help";
			this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(78, 25);
			this.settingsToolStripMenuItem.Text = "Settings";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// checkUpdateToolStripMenuItem
			// 
			this.checkUpdateToolStripMenuItem.Name = "checkUpdateToolStripMenuItem";
			this.checkUpdateToolStripMenuItem.Size = new System.Drawing.Size(118, 25);
			this.checkUpdateToolStripMenuItem.Text = "Check Update";
			this.checkUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkUpdateToolStripMenuItem_Click);
			// 
			// androidSyncToolStripMenuItem
			// 
			this.androidSyncToolStripMenuItem.Name = "androidSyncToolStripMenuItem";
			this.androidSyncToolStripMenuItem.Size = new System.Drawing.Size(103, 25);
			this.androidSyncToolStripMenuItem.Text = "Phone Sync";
			this.androidSyncToolStripMenuItem.Click += new System.EventHandler(this.phoneSyncToolStripMenuItem_Click);
			// 
			// textBox1_search
			// 
			this.textBox1_search.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1_search.Location = new System.Drawing.Point(47, 8);
			this.textBox1_search.Name = "textBox1_search";
			this.textBox1_search.Size = new System.Drawing.Size(335, 29);
			this.textBox1_search.TabIndex = 1;
			this.textBox1_search.TextChanged += new System.EventHandler(this.textBox1_search_TextChanged);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::HelloClipboard.Properties.Resources.icons8_search_512px;
			this.pictureBox1.Location = new System.Drawing.Point(12, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(29, 29);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 5;
			this.pictureBox1.TabStop = false;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox2.Image = global::HelloClipboard.Properties.Resources.icons8_broom_480px;
			this.pictureBox2.Location = new System.Drawing.Point(388, 8);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(29, 29);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox2.TabIndex = 6;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 29);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(464, 652);
			this.panel1.TabIndex = 7;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.MessagesListBox);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 71);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(464, 581);
			this.panel3.TabIndex = 8;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.checkBoxCaseSensitive);
			this.panel2.Controls.Add(this.checkBoxRegex);
			this.panel2.Controls.Add(this.pictureBox3_topMost);
			this.panel2.Controls.Add(this.pictureBox1);
			this.panel2.Controls.Add(this.pictureBox2);
			this.panel2.Controls.Add(this.textBox1_search);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(464, 71);
			this.panel2.TabIndex = 7;
			// 
			// pictureBox3_topMost
			// 
			this.pictureBox3_topMost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox3_topMost.Image = global::HelloClipboard.Properties.Resources.icons8_unlocked_192px;
			this.pictureBox3_topMost.Location = new System.Drawing.Point(423, 8);
			this.pictureBox3_topMost.Name = "pictureBox3_topMost";
			this.pictureBox3_topMost.Size = new System.Drawing.Size(29, 29);
			this.pictureBox3_topMost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox3_topMost.TabIndex = 7;
			this.pictureBox3_topMost.TabStop = false;
			this.pictureBox3_topMost.Click += new System.EventHandler(this.pictureBox3_topMost_Click);
			// 
			// checkBoxRegex
			// 
			this.checkBoxRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxRegex.AutoSize = true;
			this.checkBoxRegex.Location = new System.Drawing.Point(213, 43);
			this.checkBoxRegex.Name = "checkBoxRegex";
			this.checkBoxRegex.Size = new System.Drawing.Size(69, 25);
			this.checkBoxRegex.TabIndex = 8;
			this.checkBoxRegex.Text = "Regex";
			this.checkBoxRegex.UseVisualStyleBackColor = true;
			this.checkBoxRegex.CheckedChanged += new System.EventHandler(this.checkBoxRegex_CheckedChanged);
			// 
			// checkBoxCaseSensitive
			// 
			this.checkBoxCaseSensitive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxCaseSensitive.AutoSize = true;
			this.checkBoxCaseSensitive.Location = new System.Drawing.Point(288, 43);
			this.checkBoxCaseSensitive.Name = "checkBoxCaseSensitive";
			this.checkBoxCaseSensitive.Size = new System.Drawing.Size(123, 25);
			this.checkBoxCaseSensitive.TabIndex = 9;
			this.checkBoxCaseSensitive.Text = "Case sensitive";
			this.checkBoxCaseSensitive.UseVisualStyleBackColor = true;
			this.checkBoxCaseSensitive.CheckedChanged += new System.EventHandler(this.checkBoxCaseSensitive_CheckedChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 681);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MinimumSize = new System.Drawing.Size(480, 720);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "HelloClipboard";
			this.contextMenuStrip1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3_topMost)).EndInit();
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
		private System.Windows.Forms.ToolStripMenuItem saveToFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openUrlToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pinToolStripMenuItem;
		private System.Windows.Forms.TextBox textBox1_search;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.PictureBox pictureBox3_topMost;
		private System.Windows.Forms.CheckBox checkBoxRegex;
		private System.Windows.Forms.CheckBox checkBoxCaseSensitive;
	}
}

