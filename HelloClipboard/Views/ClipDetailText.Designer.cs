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
            components = new System.ComponentModel.Container();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            copySelectedTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            textDrawPanel = new System.Windows.Forms.Panel();
            vScrollBar1 = new System.Windows.Forms.VScrollBar();
            hScrollBar1 = new System.Windows.Forms.HScrollBar();
            poisonButton1_copyAsText = new ReaLTaiizor.Controls.PoisonButton();
            poisonButton1_copyAsObject = new ReaLTaiizor.Controls.PoisonButton();
            poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(components);
            poisonStyleExtender1 = new ReaLTaiizor.Controls.PoisonStyleExtender(components);
            contextMenuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            textDrawPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).BeginInit();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { copySelectedTextToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(103, 26);
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            // 
            // copySelectedTextToolStripMenuItem
            // 
            copySelectedTextToolStripMenuItem.Name = "copySelectedTextToolStripMenuItem";
            copySelectedTextToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            copySelectedTextToolStripMenuItem.Text = "Copy";
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
            statusStrip1.TabIndex = 8;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // textDrawPanel
            // 
            textDrawPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textDrawPanel.BackColor = System.Drawing.Color.White;
            textDrawPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textDrawPanel.ContextMenuStrip = contextMenuStrip1;
            textDrawPanel.Controls.Add(vScrollBar1);
            textDrawPanel.Controls.Add(hScrollBar1);
            textDrawPanel.Location = new System.Drawing.Point(22, 62);
            textDrawPanel.Margin = new System.Windows.Forms.Padding(2);
            textDrawPanel.Name = "textDrawPanel";
            textDrawPanel.Size = new System.Drawing.Size(316, 201);
            textDrawPanel.TabIndex = 9;
            textDrawPanel.Paint += textDrawPanel_Paint;
            // 
            // vScrollBar1
            // 
            vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            vScrollBar1.Location = new System.Drawing.Point(297, 0);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new System.Drawing.Size(17, 182);
            vScrollBar1.TabIndex = 0;
            // 
            // hScrollBar1
            // 
            hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            hScrollBar1.Location = new System.Drawing.Point(0, 182);
            hScrollBar1.Name = "hScrollBar1";
            hScrollBar1.Size = new System.Drawing.Size(314, 17);
            hScrollBar1.TabIndex = 1;
            // 
            // poisonButton1_copyAsText
            // 
            poisonButton1_copyAsText.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            poisonButton1_copyAsText.Location = new System.Drawing.Point(138, 269);
            poisonButton1_copyAsText.Name = "poisonButton1_copyAsText";
            poisonButton1_copyAsText.Size = new System.Drawing.Size(199, 30);
            poisonButton1_copyAsText.TabIndex = 11;
            poisonButton1_copyAsText.Text = "Copy (Text)";
            poisonButton1_copyAsText.UseSelectable = true;
            poisonButton1_copyAsText.Click += poisonButton1_copyAsText_Click;
            // 
            // poisonButton1_copyAsObject
            // 
            poisonButton1_copyAsObject.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            poisonButton1_copyAsObject.Location = new System.Drawing.Point(22, 269);
            poisonButton1_copyAsObject.Name = "poisonButton1_copyAsObject";
            poisonButton1_copyAsObject.Size = new System.Drawing.Size(110, 30);
            poisonButton1_copyAsObject.TabIndex = 12;
            poisonButton1_copyAsObject.Text = "Copy (Object)";
            poisonButton1_copyAsObject.UseSelectable = true;
            poisonButton1_copyAsObject.Click += poisonButton1_copyAsObject_Click;
            // 
            // poisonStyleManager1
            // 
            poisonStyleManager1.Owner = this;
            // 
            // ClipDetailText
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(360, 360);
            Controls.Add(poisonButton1_copyAsObject);
            Controls.Add(poisonButton1_copyAsText);
            Controls.Add(textDrawPanel);
            Controls.Add(statusStrip1);
            Font = new System.Drawing.Font("Segoe UI", 10F);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(360, 360);
            Name = "ClipDetailText";
            ShadowType = ReaLTaiizor.Enum.Poison.FormShadowType.AeroShadow;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Text Preview";
            contextMenuStrip1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            textDrawPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copySelectedTextToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Panel textDrawPanel;
        private ReaLTaiizor.Controls.PoisonButton poisonButton1_copyAsText;
        private ReaLTaiizor.Controls.PoisonButton poisonButton1_copyAsObject;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonStyleExtender poisonStyleExtender1;
    }
}