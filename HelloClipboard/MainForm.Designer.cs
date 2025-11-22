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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.androidSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.textBox1_search = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.contextMenuStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// MessagesListBox
			// 
			this.MessagesListBox.ContextMenuStrip = this.contextMenuStrip1;
			this.MessagesListBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MessagesListBox.FormattingEnabled = true;
			this.MessagesListBox.ItemHeight = 21;
			this.MessagesListBox.Location = new System.Drawing.Point(0, 68);
			this.MessagesListBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MessagesListBox.Name = "MessagesListBox";
			this.MessagesListBox.Size = new System.Drawing.Size(464, 613);
			this.MessagesListBox.TabIndex = 2;
			this.MessagesListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MessagesListBox_MouseClick);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.saveToFileToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(132, 48);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// saveToFileToolStripMenuItem
			// 
			this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
			this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
			this.saveToFileToolStripMenuItem.Text = "Save to file";
			this.saveToFileToolStripMenuItem.Click += new System.EventHandler(this.saveToFileToolStripMenuItem_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.updatesToolStripMenuItem,
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
			// updatesToolStripMenuItem
			// 
			this.updatesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkUpdateToolStripMenuItem});
			this.updatesToolStripMenuItem.Name = "updatesToolStripMenuItem";
			this.updatesToolStripMenuItem.Size = new System.Drawing.Size(79, 25);
			this.updatesToolStripMenuItem.Text = "Updates";
			// 
			// checkUpdateToolStripMenuItem
			// 
			this.checkUpdateToolStripMenuItem.Name = "checkUpdateToolStripMenuItem";
			this.checkUpdateToolStripMenuItem.Size = new System.Drawing.Size(180, 26);
			this.checkUpdateToolStripMenuItem.Text = "Check Update";
			this.checkUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkUpdateToolStripMenuItem_Click);
			// 
			// androidSyncToolStripMenuItem
			// 
			this.androidSyncToolStripMenuItem.Name = "androidSyncToolStripMenuItem";
			this.androidSyncToolStripMenuItem.Size = new System.Drawing.Size(103, 25);
			this.androidSyncToolStripMenuItem.Text = "Phone Sync";
			this.androidSyncToolStripMenuItem.Click += new System.EventHandler(this.androidSyncToolStripMenuItem_Click);
			// 
			// textBox1_search
			// 
			this.textBox1_search.Location = new System.Drawing.Point(46, 32);
			this.textBox1_search.Name = "textBox1_search";
			this.textBox1_search.Size = new System.Drawing.Size(373, 29);
			this.textBox1_search.TabIndex = 1;
			this.textBox1_search.TextChanged += new System.EventHandler(this.textBox1_search_TextChanged);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::HelloClipboard.Properties.Resources.icons8_search_512px;
			this.pictureBox1.Location = new System.Drawing.Point(13, 33);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(27, 28);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 5;
			this.pictureBox1.TabStop = false;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::HelloClipboard.Properties.Resources.icons8_broom_480px;
			this.pictureBox2.Location = new System.Drawing.Point(425, 32);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(27, 28);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox2.TabIndex = 6;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 681);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.textBox1_search);
			this.Controls.Add(this.MessagesListBox);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MinimumSize = new System.Drawing.Size(480, 720);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "HelloClipboard";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.Move += new System.EventHandler(this.MainForm_Move);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.contextMenuStrip1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox MessagesListBox;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem updatesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem checkUpdateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem androidSyncToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToFileToolStripMenuItem;
		private System.Windows.Forms.TextBox textBox1_search;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
	}
}

