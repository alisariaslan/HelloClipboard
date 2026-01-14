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
            components = new System.ComponentModel.Container();
            panel1 = new System.Windows.Forms.Panel();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            copyImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            panel2 = new System.Windows.Forms.Panel();
            pnl_selectedColor = new System.Windows.Forms.Panel();
            pbox_pen = new System.Windows.Forms.PictureBox();
            colorDialog1 = new System.Windows.Forms.ColorDialog();
            poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(components);
            poisonStyleExtender1 = new ReaLTaiizor.Controls.PoisonStyleExtender(components);
            poisonButton1_copyImage = new ReaLTaiizor.Controls.PoisonButton();
            shortcutsToolTip = new System.Windows.Forms.ToolTip(components);
            contextMenuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbox_pen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel1.AutoScroll = true;
            panel1.Location = new System.Drawing.Point(20, 90);
            panel1.Margin = new System.Windows.Forms.Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(320, 190);
            panel1.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { copyImageToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(103, 26);
            // 
            // copyImageToolStripMenuItem
            // 
            copyImageToolStripMenuItem.Name = "copyImageToolStripMenuItem";
            copyImageToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            copyImageToolStripMenuItem.Text = "Copy";
            // 
            // statusStrip1
            // 
            poisonStyleExtender1.SetApplyPoisonTheme(statusStrip1, true);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip1.Location = new System.Drawing.Point(20, 318);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 11, 0);
            statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            statusStrip1.Size = new System.Drawing.Size(320, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 6;
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
            panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel2.Controls.Add(pnl_selectedColor);
            panel2.Controls.Add(pbox_pen);
            panel2.Location = new System.Drawing.Point(20, 62);
            panel2.Margin = new System.Windows.Forms.Padding(2);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(320, 24);
            panel2.TabIndex = 7;
            // 
            // pnl_selectedColor
            // 
            pnl_selectedColor.BackColor = System.Drawing.Color.Red;
            pnl_selectedColor.Location = new System.Drawing.Point(32, 8);
            pnl_selectedColor.Margin = new System.Windows.Forms.Padding(2);
            pnl_selectedColor.Name = "pnl_selectedColor";
            pnl_selectedColor.Size = new System.Drawing.Size(16, 16);
            pnl_selectedColor.TabIndex = 1;
            // 
            // pbox_pen
            // 
            pbox_pen.Image = Properties.Resources.pencil_drawing_48px;
            pbox_pen.Location = new System.Drawing.Point(0, 0);
            pbox_pen.Margin = new System.Windows.Forms.Padding(2);
            pbox_pen.Name = "pbox_pen";
            pbox_pen.Size = new System.Drawing.Size(24, 24);
            pbox_pen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pbox_pen.TabIndex = 0;
            pbox_pen.TabStop = false;
            // 
            // poisonStyleManager1
            // 
            poisonStyleManager1.Owner = this;
            // 
            // poisonButton1_copyImage
            // 
            poisonButton1_copyImage.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            poisonButton1_copyImage.Location = new System.Drawing.Point(20, 285);
            poisonButton1_copyImage.Name = "poisonButton1_copyImage";
            poisonButton1_copyImage.Size = new System.Drawing.Size(320, 30);
            poisonButton1_copyImage.TabIndex = 8;
            poisonButton1_copyImage.Text = "Copy (Image)";
            poisonButton1_copyImage.UseSelectable = true;
            // 
            // ClipDetailImage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(360, 360);
            Controls.Add(poisonButton1_copyImage);
            Controls.Add(panel2);
            Controls.Add(statusStrip1);
            Controls.Add(panel1);
            Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(360, 360);
            Name = "ClipDetailImage";
            ShadowType = ReaLTaiizor.Enum.Poison.FormShadowType.AeroShadow;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Image Preview";
            contextMenuStrip1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbox_pen).EndInit();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copyImageToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.PictureBox pbox_pen;
		private System.Windows.Forms.Panel pnl_selectedColor;
		private System.Windows.Forms.ColorDialog colorDialog1;
        private ReaLTaiizor.Controls.PoisonStyleExtender poisonStyleExtender1;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonButton poisonButton1_copyImage;
        private System.Windows.Forms.ToolTip shortcutsToolTip;
    }
}