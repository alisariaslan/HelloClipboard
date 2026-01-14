namespace HelloClipboard.Views
{
    partial class WebDialog
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
            webBrowser1 = new System.Windows.Forms.WebBrowser();
            poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(components);
            poisonStyleExtender1 = new ReaLTaiizor.Controls.PoisonStyleExtender(components);
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).BeginInit();
            SuspendLayout();
            // 
            // webBrowser1
            // 
            webBrowser1.AllowWebBrowserDrop = false;
            webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            webBrowser1.Location = new System.Drawing.Point(20, 60);
            webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            webBrowser1.Name = "webBrowser1";
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Size = new System.Drawing.Size(500, 340);
            webBrowser1.TabIndex = 1;
            // 
            // poisonStyleManager1
            // 
            poisonStyleManager1.Owner = this;
            // 
            // WebDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(540, 420);
            Controls.Add(webBrowser1);
            MinimumSize = new System.Drawing.Size(540, 420);
            Name = "WebDialog";
            ShadowType = ReaLTaiizor.Enum.Poison.FormShadowType.AeroShadow;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Title";
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonStyleExtender poisonStyleExtender1;
    }
}