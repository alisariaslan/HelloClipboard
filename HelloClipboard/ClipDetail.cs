using System;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class ClipDetail : Form
	{
		private readonly MainForm _mainForm;

		public ClipDetail(MainForm mainForm,ClipboardItem item)
		{
			InitializeComponent();
			richTextBox1.WordWrap = false;
			richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
			richTextBox1.Text = item.Text;
			this.Text = $"Row {item.Index} - {Constants.AppName}";
		}

		private void button1_copy_Click(object sender, EventArgs e)
		{
			_mainForm?.copyToolStripMenuItem_Click(sender, e);
			this.Close(); 
		}
	}
}
