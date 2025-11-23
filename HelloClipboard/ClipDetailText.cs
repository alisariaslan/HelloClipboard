using System;
using System.Drawing;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class ClipDetailText : Form
	{
		private readonly MainForm _mainForm;

		private float _textZoom = 1.0f;

		public ClipDetailText(MainForm mainForm, ClipboardItem item)
		{
			InitializeComponent();

			_mainForm = mainForm;
			this.Text = $"Row {item.Index + 1} Detail - {Constants.AppName}";

			this.MouseWheel += ClipDetail_MouseWheel;
			richTextBox1.MouseWheel += ClipDetail_MouseWheel;

			// Sadece metin modu
			SetupTextMode(item.Content);
		}

		// ---------------- TEXT MODE ----------------
		private void SetupTextMode(string text)
		{
			richTextBox1.Visible = true;
			richTextBox1.WordWrap = false;
			richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
			richTextBox1.Text = text;
			richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, 12);
		}

		// ---------------- ZOOM ----------------
		private void ClipDetail_MouseWheel(object sender, MouseEventArgs e)
		{
			if ((ModifierKeys & Keys.Control) != Keys.Control)
				return;

			if (richTextBox1.Visible)
			{
				_textZoom = Math.Max(0.3f, _textZoom + (e.Delta > 0 ? 0.1f : -0.1f));
				richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, 12 * _textZoom);
			}
		}

		// ---------------- COPY ----------------
		private void button1_copy_Click(object sender, EventArgs e)
		{
			_mainForm?.copyToolStripMenuItem_Click(sender, e);
		}
	}
}
