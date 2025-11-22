namespace HelloClipboard
{
	partial class ClipDetail
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
			this.button1_copy = new System.Windows.Forms.Button();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// button1_copy
			// 
			this.button1_copy.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.button1_copy.Location = new System.Drawing.Point(0, 284);
			this.button1_copy.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.button1_copy.Name = "button1_copy";
			this.button1_copy.Size = new System.Drawing.Size(344, 37);
			this.button1_copy.TabIndex = 0;
			this.button1_copy.Text = "Copy";
			this.button1_copy.UseVisualStyleBackColor = true;
			this.button1_copy.Click += new System.EventHandler(this.button1_copy_Click);
			// 
			// richTextBox1
			// 
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(344, 284);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "";
			// 
			// ClipDetail
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 321);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.button1_copy);
			this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(360, 360);
			this.Name = "ClipDetail";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Detail - HelloClipbaord";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button1_copy;
		private System.Windows.Forms.RichTextBox richTextBox1;
	}
}