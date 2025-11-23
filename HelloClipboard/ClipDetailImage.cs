using System;
using System.Drawing;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class ClipDetailImage : Form
	{
		private readonly MainForm _mainForm;

		private float _imageZoom = 1.0f;
		private float _minZoom = 1.0f;

		private Point _dragStart;
		private bool _isDragging = false;

		public ClipDetailImage(MainForm mainForm, ClipboardItem item)
		{
			InitializeComponent();

			_mainForm = mainForm;
			this.Text = $"Row {item.Index + 1} Detail - {Constants.AppName}";

			this.MouseWheel += ClipDetail_MouseWheel;
			panel1.MouseWheel += ClipDetail_MouseWheel;
			pictureBox1.MouseWheel += ClipDetail_MouseWheel;

			if (item.ItemType == ClipboardItemType.Image)
				SetupImageMode(item.ImageContent);
		}

		// ---------------- IMAGE MODE ----------------
		private void SetupImageMode(Image img)
		{
			pictureBox1.Visible = true;
			panel1.Visible = true;

			pictureBox1.Image = img;

			SetDoubleBuffered(panel1, true);

			pictureBox1.MouseDown += PictureBox_MouseDown;
			pictureBox1.MouseMove += PictureBox_MouseMove;
			pictureBox1.MouseUp += PictureBox_MouseUp;

			panel1.MouseDown += Panel_MouseDown;
			panel1.MouseUp += Panel_MouseUp;

			CalculateInitialZoom();
			ApplyZoom();
			CenterImage();
		}

		private void Panel_MouseDown(object sender, MouseEventArgs e)
		{
			if (!pictureBox1.Bounds.Contains(e.Location))
			{
				_isDragging = true;
				_dragStart = e.Location;
				panel1.Cursor = Cursors.Hand;
			}
		}

		private void Panel_MouseUp(object sender, MouseEventArgs e)
		{
			_isDragging = false;
			panel1.Cursor = Cursors.Default;
		}

		private void CalculateInitialZoom()
		{
			float zoomX = (float)panel1.Width / pictureBox1.Image.Width;
			float zoomY = (float)panel1.Height / pictureBox1.Image.Height;

			_minZoom = Math.Min(zoomX, zoomY);
			_imageZoom = _minZoom;
		}

		public static void SetDoubleBuffered(Control c, bool value)
		{
			var property = typeof(Control).GetProperty("DoubleBuffered",
				System.Reflection.BindingFlags.Instance |
				System.Reflection.BindingFlags.NonPublic);
			property?.SetValue(c, value, null);
		}

		// ---------------- ZOOM ----------------
		private void ClipDetail_MouseWheel(object sender, MouseEventArgs e)
		{
			if ((ModifierKeys & Keys.Control) != Keys.Control)
				return;

			if (pictureBox1.Visible)
			{
				if (e.Delta > 0)
					_imageZoom += 0.1f;
				else
					_imageZoom = Math.Max(_minZoom, _imageZoom - 0.1f);

				ApplyZoom();

				if (_imageZoom == _minZoom)
					CenterImage();
				else
					ZoomBasedOnMousePixel(e);
			}
		}

		private void ZoomBasedOnMousePixel(MouseEventArgs e)
		{
			if (pictureBox1.Image == null) return;

			// Mouse pozisyonu panel scroll ile normalize
			var mouseX = e.X + panel1.AutoScrollPosition.X;
			var mouseY = e.Y + panel1.AutoScrollPosition.Y;

			float oldZoom = _imageZoom;
			float newWidth = pictureBox1.Image.Width * _imageZoom;
			float newHeight = pictureBox1.Image.Height * _imageZoom;

			pictureBox1.Width = (int)newWidth;
			pictureBox1.Height = (int)newHeight;

			// Scroll pozisyonunu ayarla, böylece mouse noktası sabit kalır
			int scrollX = (int)((mouseX / oldZoom) * _imageZoom - e.X);
			int scrollY = (int)((mouseY / oldZoom) * _imageZoom - e.Y);

			panel1.AutoScrollPosition = new Point(scrollX, scrollY);
		}


		private void ApplyZoom()
		{
			if (pictureBox1.Image == null) return;

			pictureBox1.Width = (int)(pictureBox1.Image.Width * _imageZoom);
			pictureBox1.Height = (int)(pictureBox1.Image.Height * _imageZoom);

			// Scroll her zaman açık kalsın
			panel1.AutoScroll = true;

			if (_imageZoom <= _minZoom)
			{
				CenterImage();
			}
		}


		private void CenterImage()
		{
			pictureBox1.Left = Math.Max((panel1.ClientSize.Width - pictureBox1.Width) / 2, 0);
			pictureBox1.Top = Math.Max((panel1.ClientSize.Height - pictureBox1.Height) / 2, 0);
		}


		// ---------------- PAN ----------------
		private void PictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			_isDragging = true;
			_dragStart = e.Location;
			pictureBox1.Cursor = Cursors.Hand;
		}

		private void PictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (_isDragging)
			{
				int newX = panel1.HorizontalScroll.Value - (e.X - _dragStart.X);
				int newY = panel1.VerticalScroll.Value - (e.Y - _dragStart.Y);
				panel1.AutoScrollPosition = new Point(newX, newY);

				panel1.Invalidate();
				panel1.Update();
			}
		}

		private void PictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			_isDragging = false;
			pictureBox1.Cursor = Cursors.Default;
		}

	
		private void panel1_Resize(object sender, EventArgs e)
		{
			if (pictureBox1.Image == null) return;

			float zoomX = (float)panel1.Width / pictureBox1.Image.Width;
			float zoomY = (float)panel1.Height / pictureBox1.Image.Height;
			float newMinZoom = Math.Min(zoomX, zoomY);

			bool wasAtMinZoom = Math.Abs(_imageZoom - _minZoom) < 0.0001f;

			_minZoom = newMinZoom;

			if (wasAtMinZoom)
			{
				_imageZoom = _minZoom;
				ApplyZoom();
			}
			else
			{
				ApplyZoom();
			}

		}

		// ---------------- COPY ----------------
		private void button1_copy_Click(object sender, EventArgs e)
		{
			_mainForm?.copyToolStripMenuItem_Click(sender, e);
		}
	}
}
