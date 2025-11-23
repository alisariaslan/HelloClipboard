using System;
using System.Drawing;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class ClipDetailImage : Form
	{
		private readonly MainForm _mainForm;

		private Image _image;
		private float _imageZoom = 1.0f;
		private float _minZoom = 1.0f;

		private Point _dragStart;
		private bool _isDragging = false;
		private Point _imageOffset = Point.Empty;

		public ClipDetailImage(MainForm mainForm, ClipboardItem item)
		{
			InitializeComponent();

			_mainForm = mainForm;
			this.Text = $"Row {item.Index + 1} Detail - {Constants.AppName}";

			// Paint olayını bağla
			panel1.Paint += panel1_Paint;

			panel1.MouseWheel += Panel1_MouseWheel;
			panel1.MouseDown += Panel1_MouseDown;
			panel1.MouseMove += Panel1_MouseMove;
			panel1.MouseUp += Panel1_MouseUp;
			panel1.Resize += Panel1_Resize;

			SetDoubleBuffered(panel1, true);

			if (item.ItemType == ClipboardItemType.Image)
				SetupImageMode(item.ImageContent);
		}


		private void CalculateInitialZoom()
		{
			if (_image == null) return;

			float zoomX = (float)panel1.ClientSize.Width / _image.Width;
			float zoomY = (float)panel1.ClientSize.Height / _image.Height;

			_minZoom = Math.Min(zoomX, zoomY);
			_imageZoom = _minZoom;

			CenterImage();
		}


		private void SetupImageMode(Image img)
		{
			_image = img;
			CalculateInitialZoom();
			CenterImage();
			panel1.Invalidate(); // paneli tekrar çiz
		}

		// ---------------- DOUBLE BUFFER ----------------
		public static void SetDoubleBuffered(Control c, bool value)
		{
			var property = typeof(Control).GetProperty("DoubleBuffered",
				System.Reflection.BindingFlags.Instance |
				System.Reflection.BindingFlags.NonPublic);
			property?.SetValue(c, value, null);
		}

		// ---------------- ZOOM ----------------
		private void Panel1_MouseWheel(object sender, MouseEventArgs e)
		{
			if ((ModifierKeys & Keys.Control) != Keys.Control)
				return;

			if (_image == null) return;

			float oldZoom = _imageZoom;

			if (e.Delta > 0)
				_imageZoom += 0.1f;
			else
				_imageZoom = Math.Max(_minZoom, _imageZoom - 0.1f);

			// Mouse pozisyonunu koruyacak şekilde offseti ayarla
			float scale = _imageZoom / oldZoom;
			_imageOffset.X = (int)(e.X - (e.X - _imageOffset.X) * scale);
			_imageOffset.Y = (int)(e.Y - (e.Y - _imageOffset.Y) * scale);

			panel1.Invalidate();
		}

		// ---------------- PAN ----------------
		private void Panel1_MouseDown(object sender, MouseEventArgs e)
		{
			_isDragging = true;
			_dragStart = e.Location;
			panel1.Cursor = Cursors.Hand;
		}

		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!_isDragging) return;

			_imageOffset.X += e.X - _dragStart.X;
			_imageOffset.Y += e.Y - _dragStart.Y;
			_dragStart = e.Location;

			panel1.Invalidate();
		}

		private void Panel1_MouseUp(object sender, MouseEventArgs e)
		{
			_isDragging = false;
			panel1.Cursor = Cursors.Default;
		}

		// ---------------- RESIZE ----------------
		private void Panel1_Resize(object sender, EventArgs e)
		{
			if (_image == null) return;

			float zoomX = (float)panel1.Width / _image.Width;
			float zoomY = (float)panel1.Height / _image.Height;
			float newMinZoom = Math.Min(zoomX, zoomY);

			bool wasAtMinZoom = Math.Abs(_imageZoom - _minZoom) < 0.0001f;

			_minZoom = newMinZoom;

			if (wasAtMinZoom)
			{
				_imageZoom = _minZoom;
				CenterImage();
			}

			panel1.Invalidate();
		}

		// ---------------- DRAW IMAGE ----------------
		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			if (_image == null) return;

			e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

			int drawWidth = (int)(_image.Width * _imageZoom);
			int drawHeight = (int)(_image.Height * _imageZoom);

			e.Graphics.Clear(panel1.BackColor);
			e.Graphics.DrawImage(_image, _imageOffset.X, _imageOffset.Y, drawWidth, drawHeight);
		}

		// ---------------- CENTER IMAGE ----------------
		private void CenterImage()
		{
			if (_image == null) return;

			int drawWidth = (int)(_image.Width * _imageZoom);
			int drawHeight = (int)(_image.Height * _imageZoom);

			_imageOffset.X = (panel1.ClientSize.Width - drawWidth) / 2;
			_imageOffset.Y = (panel1.ClientSize.Height - drawHeight) / 2;
		}

		private void button1_copy_Click(object sender, EventArgs e)
		{
			_mainForm?.copyToolStripMenuItem_Click(sender, e);
		}
	}
}
