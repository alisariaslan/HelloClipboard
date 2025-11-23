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
			string shortTitle = item.Title.Length > Constants.MaxDetailFormTitleLength ? item.Title.Substring(0, Constants.MaxDetailFormTitleLength) + "…" : item.Title;
			this.Text = $"{shortTitle} - {Constants.AppName}";

			this.MouseWheel += ClipDetail_MouseWheel;
			richTextBox1.MouseWheel += ClipDetail_MouseWheel;

			SetupTextMode(item.Content);
		}

		// ---------------- TEXT MODE ----------------
		private void SetupTextMode(string text)
		{
			richTextBox1.Visible = true;
			richTextBox1.WordWrap = false;
			richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
			richTextBox1.Text = text;

			float baseFontSize = 12;

			_textZoom = 0.8f;

			richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, baseFontSize * _textZoom);
		}


		// ---------------- ZOOM ----------------
		private void ClipDetail_MouseWheel(object sender, MouseEventArgs e)
		{
			if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (richTextBox1.Visible)
				{
					_textZoom = Math.Max(0.3f, _textZoom + (e.Delta > 0 ? 0.1f : -0.1f));
					richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, 12 * _textZoom);
				}
			}
			else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				if (richTextBox1.Visible)
				{
					int scrollAmount = e.Delta > 0 ? -10 : 10; 
					int steps = 10; 
					for (int i = 0; i < steps; i++)
					{
						SendMessage(richTextBox1.Handle, WM_HSCROLL, (IntPtr)(e.Delta > 0 ? SB_LINELEFT : SB_LINERIGHT), IntPtr.Zero);
					}
				}
			}
		}

		// ---------------- P/Invoke ----------------
		private const int WM_HSCROLL = 0x114;
		private const int SB_LINELEFT = 0;
		private const int SB_LINERIGHT = 1;

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);


		// ---------------- COPY ----------------
		private void button1_copy_Click(object sender, EventArgs e)
		{
			_mainForm?.copyToolStripMenuItem_Click(sender, e);
		}
	}
}
