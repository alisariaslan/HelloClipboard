using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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

		// Çizim için gerekli değişkenler


		private readonly Pen _redPen = new Pen(Color.Red, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };

		private bool _isPenMode = false;

		private bool _isDrawing = false;

		private List<(List<PointF> Points, Color Color)> _paths = new List<(List<PointF> Points, Color Color)>();
		private List<PointF> _currentPath;

		private Color _activeColor = Color.Red;
		private Pen _drawPen = new Pen(Color.Red, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };

		public ClipDetailImage(MainForm mainForm, ClipboardItem item)
		{
			InitializeComponent();
			this.KeyPreview = true;

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
			pnl_selectedColor.BackColor = _activeColor; // Başlangıç rengi

			SetDoubleBuffered(panel1, true);

			if (item.ItemType == ClipboardItemType.Image)
				SetupImageMode(item.ImageContent);

			this.KeyDown += ClipDetailImage_KeyDown;
			this.KeyUp += ClipDetailImage_KeyUp;
		}

		private void ClipDetailImage_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.D && !_isPenMode) // Sadece mod kapalıyken tetikle
			{
				TogglePenMode(true);
			}
			if (e.KeyCode == Keys.S)
			{
				ShowColorPicker();
			}
		}

		public void ClearDrawings()
		{
			_paths.Clear();
			_currentPath = null;
			UpdateCopyButtonText();
			panel1.Invalidate();
		}

		private void ClipDetailImage_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.D)
			{
				TogglePenMode(false);
			}
		}

		public void UpdateItem(ClipboardItem item)
		{
			if (item.ItemType != ClipboardItemType.Image)
				return;

			// Yeni resim geldiğinde eski çizimleri temizle
			_paths.Clear();
			_currentPath = null;

			string shortTitle = item.Title.Length > Constants.MaxDetailFormTitleLength ? item.Title.Substring(0, Constants.MaxDetailFormTitleLength) + "…" : item.Title;
			this.Text = $"{shortTitle} - {Constants.AppName}";

			_imageOffset = Point.Empty;
			_imageZoom = 1.0f;
			_minZoom = 1.0f;

			SetupImageMode(item.ImageContent);
		}

		private void SetupShortcutHelp()
		{
			// Yardım metnini yeni kısayollarla güncelledik
			lbl_help.Text = "• Zoom: Ctrl+Wheel | Pan: Left Click+Drag | Scroll: Wheel (Shift for Horz.)\n" +
							"• Pen: Hold [D] or Click Pencil Icon | Color: Press [S] or Click Color Box";
		}
		private void ShowColorPicker()
		{
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				_activeColor = colorDialog1.Color;
				pnl_selectedColor.BackColor = _activeColor;
				_drawPen.Color = _activeColor;
			}
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
			if (_isPenMode && e.Button == MouseButtons.Left)
			{
				_isDrawing = true;
				_currentPath = new List<PointF>();
				_currentPath.Add(ScreenToImage(e.Location));

				// Mevcut aktif renk ile yeni bir yol ekle
				_paths.Add((_currentPath, _activeColor));
			}
			else
			{
				_isDragging = true;
				_dragStart = e.Location;
				panel1.Cursor = Cursors.Hand;
			}
		}

		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (_isDrawing)
			{
				_currentPath.Add(ScreenToImage(e.Location));
				panel1.Invalidate(); // Çizimi güncelle
			}
			else if (_isDragging)
			{
				_imageOffset.X += e.X - _dragStart.X;
				_imageOffset.Y += e.Y - _dragStart.Y;
				_dragStart = e.Location;
				ClampImageOffset();
				panel1.Invalidate();
			}
		}

		private void Panel1_MouseUp(object sender, MouseEventArgs e)
		{
			_isDrawing = false;
			_isDragging = false;
			panel1.Cursor = _isPenMode ? Cursors.Cross : Cursors.Default;
			UpdateCopyButtonText(); // Eklen
		}

		// Yardımcı Metod: Panel üzerindeki Mouse koordinatını resim üzerindeki gerçek koordinata çevirir
		private PointF ScreenToImage(Point p)
		{
			return new PointF(
				(p.X - _imageOffset.X) / _imageZoom,
				(p.Y - _imageOffset.Y) / _imageZoom
			);
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

			e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			int drawWidth = (int)(_image.Width * _imageZoom);
			int drawHeight = (int)(_image.Height * _imageZoom);

			e.Graphics.Clear(panel1.BackColor);
			e.Graphics.DrawImage(_image, _imageOffset.X, _imageOffset.Y, drawWidth, drawHeight);

			// Her yolu kendi rengiyle çiziyoruz
			foreach (var path in _paths)
			{
				if (path.Points.Count < 2) continue;

				PointF[] pts = path.Points.Select(p => new PointF(
					p.X * _imageZoom + _imageOffset.X,
					p.Y * _imageZoom + _imageOffset.Y
				)).ToArray();

				using (Pen p = new Pen(path.Color, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
				{
					e.Graphics.DrawLines(p, pts);
				}
			}
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
			if (_image == null) return;

			// Eğer çizim yoksa standart kopyalama yap
			if (_paths.Count == 0)
			{
				_mainForm?.copyToolStripMenuItem_Click(sender, e);
				return;
			}

			// Çizim varsa: Resmin üzerine çizimleri "düzleştir" (Flatten)
			using (Bitmap bmp = new Bitmap(_image.Width, _image.Height))
			{
				using (Graphics g = Graphics.FromImage(bmp))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;

					// 1. Orijinal resmi çiz
					g.DrawImage(_image, 0, 0, _image.Width, _image.Height);

					// 2. Tüm yolları (çizimleri) resim koordinatlarına göre çiz
					foreach (var path in _paths)
					{
						if (path.Points.Count < 2) continue;
						using (Pen p = new Pen(path.Color, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
						{
							g.DrawLines(p, path.Points.ToArray());
						}
					}
				}

				// 3. Oluşan yeni resmi panoya gönder
				try
				{
					Clipboard.SetImage(bmp);
					// İsteğe bağlı: Kopyalandı uyarısı veya efekt
				}
				catch (Exception ex) { MessageBox.Show("Copy failed: " + ex.Message); }
			}
		}

		private void copyImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			button1_copy_Click(this, e);
		}

		private void pnl_selectedColor_Paint(object sender, PaintEventArgs e)
		{
			ControlPaint.DrawBorder(e.Graphics, pnl_selectedColor.ClientRectangle, Color.Gray, ButtonBorderStyle.Solid);
		}

		private void pbox_pen_Click(object sender, EventArgs e)
		{
			TogglePenMode(!_isPenMode);
		}
		private void UpdateCopyButtonText()
		{
			// Eğer çizim varsa (path sayısı > 0) metni değiştir
			string text = _paths.Count > 0 ? "Copy edited image" : "Copy image";
			button1_copy.Text = text;
			copyImageToolStripMenuItem.Text = text;
		}

		private void TogglePenMode(bool active)
		{
			_isPenMode = active;
			pbox_pen.BackColor = _isPenMode ? Color.LightBlue : Color.Transparent;
			panel1.Cursor = _isPenMode ? Cursors.Cross : Cursors.Default;
		}

		private void pnl_selectedColor_Click(object sender, EventArgs e)
		{
			ShowColorPicker();
		}
	}
}
