namespace HelloClipboard
{
	partial class ClipDetailText
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
			this.components = new System.ComponentModel.Container();
			this.btn_copyAsText = new System.Windows.Forms.Button();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copySelectedTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.textDrawPanel = new System.Windows.Forms.Panel();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.btn_copyAsObject = new System.Windows.Forms.Button();
			this.contextMenuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.textDrawPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_copyAsText
			// 
			this.btn_copyAsText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_copyAsText.Location = new System.Drawing.Point(138, 269);
			this.btn_copyAsText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btn_copyAsText.Name = "btn_copyAsText";
			this.btn_copyAsText.Size = new System.Drawing.Size(196, 28);
			this.btn_copyAsText.TabIndex = 0;
			this.btn_copyAsText.Text = "Copy as text";
			this.btn_copyAsText.UseVisualStyleBackColor = true;
			this.btn_copyAsText.Click += new System.EventHandler(this.btn_copyAsText_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copySelectedTextToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(103, 26);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
			// 
			// copySelectedTextToolStripMenuItem
			// 
			this.copySelectedTextToolStripMenuItem.Name = "copySelectedTextToolStripMenuItem";
			this.copySelectedTextToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
			this.copySelectedTextToolStripMenuItem.Text = "Copy";
			this.copySelectedTextToolStripMenuItem.Click += new System.EventHandler(this.copySelectedTextToolStripMenuItem_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 299);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 11, 0);
			this.statusStrip1.Size = new System.Drawing.Size(344, 22);
			this.statusStrip1.TabIndex = 8;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
			// 
			// textDrawPanel
			// 
			this.textDrawPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDrawPanel.BackColor = System.Drawing.Color.White;
			this.textDrawPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textDrawPanel.ContextMenuStrip = this.contextMenuStrip1;
			this.textDrawPanel.Controls.Add(this.vScrollBar1);
			this.textDrawPanel.Controls.Add(this.hScrollBar1);
			this.textDrawPanel.Location = new System.Drawing.Point(0, 0);
			this.textDrawPanel.Margin = new System.Windows.Forms.Padding(2);
			this.textDrawPanel.Name = "textDrawPanel";
			this.textDrawPanel.Size = new System.Drawing.Size(344, 263);
			this.textDrawPanel.TabIndex = 9;
			this.textDrawPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.textDrawPanel_Paint);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar1.Location = new System.Drawing.Point(325, 0);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 244);
			this.vScrollBar1.TabIndex = 0;
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar1.Location = new System.Drawing.Point(0, 244);
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(342, 17);
			this.hScrollBar1.TabIndex = 1;
			// 
			// btn_copyAsObject
			// 
			this.btn_copyAsObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btn_copyAsObject.Location = new System.Drawing.Point(12, 269);
			this.btn_copyAsObject.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btn_copyAsObject.Name = "btn_copyAsObject";
			this.btn_copyAsObject.Size = new System.Drawing.Size(120, 28);
			this.btn_copyAsObject.TabIndex = 10;
			this.btn_copyAsObject.Text = "Copy as object";
			this.btn_copyAsObject.UseVisualStyleBackColor = true;
			this.btn_copyAsObject.Click += new System.EventHandler(this.btn_copyAsObject_Click);
			// 
			// ClipDetailText
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 321);
			this.Controls.Add(this.btn_copyAsObject);
			this.Controls.Add(this.textDrawPanel);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.btn_copyAsText);
			this.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(360, 360);
			this.Name = "ClipDetailText";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Text Detail - HelloClipboard";
			this.contextMenuStrip1.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.textDrawPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.Button btn_copyAsText;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copySelectedTextToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Panel textDrawPanel;
		private System.Windows.Forms.Button btn_copyAsObject;
	}
}