using System;
using System.Drawing;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class ClipDetail : Form
	{
		private readonly MainForm _mainForm;

		private float _textZoom = 1.0f;
		private float _imageZoom = 1.0f;
		private Point _imageDragStart;
		private bool _isDragging = false;

		public ClipDetail(MainForm mainForm,ClipboardItem item)
		{
			InitializeComponent();
			_mainForm = mainForm;
			this.Text = $"Row {item.Index + 1} Detail - {Constants.AppName}";
			this.MouseWheel += ClipDetail_MouseWheel;
			panel1.MouseWheel += ClipDetail_MouseWheel;
			pictureBox1.MouseWheel += ClipDetail_MouseWheel;
			richTextBox1.MouseWheel += ClipDetail_MouseWheel;

			if (item.ItemType == ClipboardItemType.Image)
			{
				SetupImageMode(item.ImageContent);
			}
			else
			{
				SetupTextMode(item.Content);
			}
		}

		private void SetupTextMode(string text)
		{
			richTextBox1.Visible = true;
			pictureBox1.Visible = false;
			panel1.Visible = false;

			richTextBox1.WordWrap = false;
			richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
			richTextBox1.Text = text;
			richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, 12);
		}

		private void SetupImageMode(Image img)
		{
			richTextBox1.Visible = false;
			pictureBox1.Visible = true;
			panel1.Visible = true;

			// PictureBox autosize moduna alınır
			pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox1.Image = img;

			_imageZoom = 1.0f;
			ApplyPictureZoom();

			pictureBox1.MouseDown += PictureBox1_MouseDown;
			pictureBox1.MouseMove += PictureBox1_MouseMove;
			pictureBox1.MouseUp += PictureBox1_MouseUp;
		}


		private void ClipDetail_MouseWheel(object sender, MouseEventArgs e)
		{
			if ((ModifierKeys & Keys.Control) != Keys.Control)
				return;

			// TEXT MODE ZOOM
			if (richTextBox1.Visible)
			{
				if (e.Delta > 0) _textZoom += 0.1f;
				else if (_textZoom > 0.3f) _textZoom -= 0.1f;

				richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, 12 * _textZoom);
			}

			// IMAGE MODE ZOOM
			if (pictureBox1.Visible)
			{
				if (e.Delta > 0) _imageZoom += 0.1f;
				else if (_imageZoom > 0.1f) _imageZoom -= 0.1f;

				ApplyPictureZoom(); 
			}
		}

		private void ApplyPictureZoom()
		{
			if (pictureBox1.Image == null) return;

			// Resmin orijinal boyutlarını al
			int originalWidth = pictureBox1.Image.Width;
			int originalHeight = pictureBox1.Image.Height;

			// Zoom faktörüne göre yeni boyutları hesapla
			pictureBox1.Width = (int)(originalWidth * _imageZoom);
			pictureBox1.Height = (int)(originalHeight * _imageZoom);
		}

		// ------------------ IMAGE DRAG/PAN ------------------

		private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			_isDragging = true;
			_imageDragStart = e.Location;
			pictureBox1.Cursor = Cursors.Hand;
		}

		private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
		{
			if (_isDragging)
			{
				pictureBox1.Left += e.X - _imageDragStart.X;
				pictureBox1.Top += e.Y - _imageDragStart.Y;
			}
		}

		private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			_isDragging = false;
			pictureBox1.Cursor = Cursors.Default;
		}

		// ------------------ COPY ------------------

		private void button1_copy_Click(object sender, EventArgs e)
		{
			_mainForm?.copyToolStripMenuItem_Click(sender, e);
		}
	
}
}
