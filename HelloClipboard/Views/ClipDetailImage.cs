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

			SetupShortcutHelp();
			_mainForm = mainForm;
			string shortTitle = item.Title.Length > Constants.MaxDetailFormTitleLength ? item.Title.Substring(0, Constants.MaxDetailFormTitleLength) + "…" : item.Title;
			this.Text = $"{shortTitle} - {Constants.AppName}";

			panel1.Paint += panel1_Paint;
			panel1.MouseWheel += Panel1_MouseWheel;
			panel1.MouseDown += Panel1_MouseDown;
			panel1.MouseMove += Panel1_MouseMove;
			panel1.MouseUp += Panel1_MouseUp;
			panel1.Resize += Panel1_Resize;
			panel1.ContextMenuStrip = contextMenuStrip1;

			SetDoubleBuffered(panel1, true);

			if (item.ItemType == ClipboardItemType.Image)
				SetupImageMode(item.ImageContent);
		}

		public void UpdateItem(ClipboardItem item)
		{
			if (item.ItemType != ClipboardItemType.Image)
				return;

			string shortTitle = item.Title.Length > Constants.MaxDetailFormTitleLength ? item.Title.Substring(0, Constants.MaxDetailFormTitleLength) + "…" : item.Title;
			this.Text = $"{shortTitle} - {Constants.AppName}";

			_imageOffset = Point.Empty;
			_imageZoom = 1.0f;
			_minZoom = 1.0f;

			SetupImageMode(item.ImageContent);
		}

		private void SetupShortcutHelp()
		{
			lbl_help.Text = "• Zoom: Ctrl + Mouse Wheel\n" +
						  "• Pan: Left Click & Drag\n" +
						  "• Scroll: Mouse Wheel (Shift for Horizontal)";

		}
		private string FormatByteSize(long bytes)
		{
			string[] suffixes = { "B", "KB", "MB", "GB" };
			int counter = 0;
			decimal number = bytes;
			while (Math.Round(number / 1024) >= 1)
			{
				number /= 1024;
				counter++;
				if (counter == suffixes.Length - 1) break;
			}
			return string.Format("{0:n1} {1}", number, suffixes[counter]);
		}

		private void UpdateImageLabelInfo()
		{
			if (_image == null) return;
			toolStripStatusLabel1.Text = $"{_image.Width}x{_image.Height} | Size: {_cachedImageSize} | Zoom: {Math.Round(_imageZoom * 100)}%";
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

		private string _cachedImageSize = "";

		private void SetupImageMode(Image img)
		{
			_image = img;

			// Boyutu burada bir kez hesapla
			try
			{
				using (var ms = new System.IO.MemoryStream())
				{
					_image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					_cachedImageSize = FormatByteSize(ms.Length);
				}
			}
			catch { _cachedImageSize = "Error"; }

			CalculateInitialZoom();
			CenterImage();
			UpdateImageLabelInfo();
			panel1.Invalidate();
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
			if (_image == null) return;

			if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				float oldZoom = _imageZoom;

				if (e.Delta > 0)
					_imageZoom += 0.2f;
				else
					_imageZoom = Math.Max(_minZoom, _imageZoom - 0.2f);

				if (oldZoom > _minZoom || _imageZoom > _minZoom)
				{
					float scale = _imageZoom / oldZoom;
					_imageOffset.X = (int)(e.X - (e.X - _imageOffset.X) * scale);
					_imageOffset.Y = (int)(e.Y - (e.Y - _imageOffset.Y) * scale);
				}
				else
				{
					CenterImage();
				}

				ClampImageOffset();
				UpdateImageLabelInfo();
				panel1.Invalidate();
				return;
			}

			if ((ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				int scrollAmount = panel1.ClientSize.Width / 10;
				scrollAmount = e.Delta > 0 ? scrollAmount : -scrollAmount;
				_imageOffset.X += scrollAmount;
			}
			else
			{
				int scrollAmount = panel1.ClientSize.Height / 10;
				scrollAmount = e.Delta > 0 ? scrollAmount : -scrollAmount;
				_imageOffset.Y += scrollAmount;
			}

			ClampImageOffset();
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

			ClampImageOffset();
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
				ClampImageOffset();
				UpdateImageLabelInfo();
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

		private void ClampImageOffset()
		{
			if (_image == null) return;

			int drawWidth = (int)(_image.Width * _imageZoom);
			int drawHeight = (int)(_image.Height * _imageZoom);

			if (drawWidth < panel1.ClientSize.Width)
				_imageOffset.X = (panel1.ClientSize.Width - drawWidth) / 2;
			else
			{
				int maxX = 0;
				int minX = panel1.ClientSize.Width - drawWidth;
				_imageOffset.X = Math.Min(maxX, Math.Max(minX, _imageOffset.X));
			}

			if (drawHeight < panel1.ClientSize.Height)
				_imageOffset.Y = (panel1.ClientSize.Height - drawHeight) / 2;
			else
			{
				int maxY = 0;
				int minY = panel1.ClientSize.Height - drawHeight;
				_imageOffset.Y = Math.Min(maxY, Math.Max(minY, _imageOffset.Y));
			}
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

		private void copyImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			button1_copy_Click(this, e);
		}
	}
}
