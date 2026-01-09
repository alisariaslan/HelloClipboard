namespace HelloClipboard
{
	partial class ClipDetailImage
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
			this.button1_copy = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.lbl_help = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.pnl_selectedColor = new System.Windows.Forms.Panel();
			this.pbox_pen = new System.Windows.Forms.PictureBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.contextMenuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbox_pen)).BeginInit();
			this.SuspendLayout();
			// 
			// button1_copy
			// 
			this.button1_copy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button1_copy.Location = new System.Drawing.Point(13, 282);
			this.button1_copy.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.button1_copy.Name = "button1_copy";
			this.button1_copy.Size = new System.Drawing.Size(318, 32);
			this.button1_copy.TabIndex = 0;
			this.button1_copy.Text = "Copy image";
			this.button1_copy.UseVisualStyleBackColor = true;
			this.button1_copy.Click += new System.EventHandler(this.button1_copy_Click);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.Location = new System.Drawing.Point(0, 79);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(344, 195);
			this.panel1.TabIndex = 3;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyImageToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(139, 26);
			// 
			// copyImageToolStripMenuItem
			// 
			this.copyImageToolStripMenuItem.Name = "copyImageToolStripMenuItem";
			this.copyImageToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.copyImageToolStripMenuItem.Text = "Copy image";
			this.copyImageToolStripMenuItem.Click += new System.EventHandler(this.copyImageToolStripMenuItem_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 319);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(344, 22);
			this.statusStrip1.TabIndex = 6;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
			// 
			// lbl_help
			// 
			this.lbl_help.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbl_help.Font = new System.Drawing.Font("Segoe UI Semilight", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_help.Location = new System.Drawing.Point(0, 0);
			this.lbl_help.Name = "lbl_help";
			this.lbl_help.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
			this.lbl_help.Size = new System.Drawing.Size(344, 43);
			this.lbl_help.TabIndex = 0;
			this.lbl_help.Text = "label1";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.pnl_selectedColor);
			this.panel2.Controls.Add(this.pbox_pen);
			this.panel2.Location = new System.Drawing.Point(0, 46);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(344, 31);
			this.panel2.TabIndex = 7;
			// 
			// pnl_selectedColor
			// 
			this.pnl_selectedColor.BackColor = System.Drawing.Color.Red;
			this.pnl_selectedColor.Location = new System.Drawing.Point(43, 11);
			this.pnl_selectedColor.Name = "pnl_selectedColor";
			this.pnl_selectedColor.Size = new System.Drawing.Size(16, 16);
			this.pnl_selectedColor.TabIndex = 1;
			this.pnl_selectedColor.Click += new System.EventHandler(this.pnl_selectedColor_Click);
			// 
			// pbox_pen
			// 
			this.pbox_pen.Image = global::HelloClipboard.Properties.Resources.pencil_drawing_48px;
			this.pbox_pen.Location = new System.Drawing.Point(13, 3);
			this.pbox_pen.Name = "pbox_pen";
			this.pbox_pen.Size = new System.Drawing.Size(24, 24);
			this.pbox_pen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbox_pen.TabIndex = 0;
			this.pbox_pen.TabStop = false;
			this.pbox_pen.Click += new System.EventHandler(this.pbox_pen_Click);
			// 
			// ClipDetailImage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 341);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.lbl_help);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.button1_copy);
			this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(360, 380);
			this.Name = "ClipDetailImage";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Image Detail - HelloClipbaord";
			this.contextMenuStrip1.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pbox_pen)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1_copy;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copyImageToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Label lbl_help;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.PictureBox pbox_pen;
		private System.Windows.Forms.Panel pnl_selectedColor;
		private System.Windows.Forms.ColorDialog colorDialog1;
	}
}