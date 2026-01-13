namespace HelloClipboard
{
	partial class ClipDetailFile
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
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            copySelectedTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            poisonButton1_copyPath = new ReaLTaiizor.Controls.PoisonButton();
            poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(components);
            poisonStyleExtender1 = new ReaLTaiizor.Controls.PoisonStyleExtender(components);
            contextMenuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).BeginInit();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            poisonStyleExtender1.SetApplyPoisonTheme(richTextBox1, true);
            richTextBox1.BackColor = System.Drawing.Color.LightBlue;
            richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            richTextBox1.Location = new System.Drawing.Point(22, 62);
            richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(316, 201);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { copySelectedTextToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(171, 26);
            // 
            // copySelectedTextToolStripMenuItem
            // 
            copySelectedTextToolStripMenuItem.Name = "copySelectedTextToolStripMenuItem";
            copySelectedTextToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            copySelectedTextToolStripMenuItem.Text = "Copy selected text";
            copySelectedTextToolStripMenuItem.Click += copySelectedTextToolStripMenuItem_Click;
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
            statusStrip1.TabIndex = 7;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // poisonButton1_copyPath
            // 
            poisonButton1_copyPath.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            poisonButton1_copyPath.Location = new System.Drawing.Point(22, 269);
            poisonButton1_copyPath.Name = "poisonButton1_copyPath";
            poisonButton1_copyPath.Size = new System.Drawing.Size(316, 30);
            poisonButton1_copyPath.TabIndex = 8;
            poisonButton1_copyPath.Text = "Copy (Path)";
            poisonButton1_copyPath.UseSelectable = true;
            poisonButton1_copyPath.Click += poisonButton1_copyPath_Click;
            // 
            // poisonStyleManager1
            // 
            poisonStyleManager1.Owner = this;
            // 
            // ClipDetailFile
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(360, 360);
            Controls.Add(poisonButton1_copyPath);
            Controls.Add(statusStrip1);
            Controls.Add(richTextBox1);
            Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(360, 360);
            Name = "ClipDetailFile";
            ShadowType = ReaLTaiizor.Enum.Poison.FormShadowType.AeroShadow;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Path Preview";
            contextMenuStrip1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copySelectedTextToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private ReaLTaiizor.Controls.PoisonButton poisonButton1_copyPath;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonStyleExtender poisonStyleExtender1;
    }
}