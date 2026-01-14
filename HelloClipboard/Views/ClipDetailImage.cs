using HelloClipboard.Constants;
using HelloClipboard.Utils;
using ReaLTaiizor.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace HelloClipboard
{
    public partial class ClipDetailImage : PoisonForm
    {
        #region  Fields & Constructor
        private readonly MainForm _mainForm;
        private Image _image;
        private string _cachedImageSize = "";
        private float _imageZoom = 1.0f;
        private float _minZoom = 1.0f;
        private Point _imageOffset = Point.Empty;
        private Point _dragStart;
        private bool _isDragging;
        private bool _isPenMode;
        private bool _isDrawing;
        private readonly List<(List<PointF> Points, Color Color)> _paths = new();
        private List<PointF> _currentPath;
        private Color _activeColor = Color.Red;
        public ClipDetailImage(MainForm mainForm, ClipboardItem item)
        {
            InitializeComponent();
            KeyPreview = true;
            _mainForm = mainForm;
            SetupShortcutHelp();
            HookEvents();
            pnl_selectedColor.BackColor = _activeColor;
            SetDoubleBuffered(panel1, true);
            if (item.ItemType == ClipboardItemType.Image)
                SetupImageMode(item.ImageContent);
        }
        #endregion

        #region Setup
        private void HookEvents()
        {
            poisonButton1_copyImage.Click += poisonButton1_copyImage_Click;
            pbox_pen.Click += pbox_pen_Click;
            pnl_selectedColor.Click += pnl_selectedColor_Click;

            panel1.Paint += panel1_Paint;
            panel1.MouseWheel += Panel1_MouseWheel;
            panel1.MouseDown += Panel1_MouseDown;
            panel1.MouseMove += Panel1_MouseMove;
            panel1.MouseUp += Panel1_MouseUp;
            panel1.Resize += Panel1_Resize;

            panel1.ContextMenuStrip = contextMenuStrip1;

            KeyDown += ClipDetailImage_KeyDown;
            KeyUp += ClipDetailImage_KeyUp;
        }

        private void SetupShortcutHelp()
        {
            string helpText =
                "Zoom: Ctrl + Mouse Wheel\n" +
                "Pan: Left Click + Drag\n" +
                "Scroll: Mouse Wheel (Shift = Horizontal)\n\n" +
                "Pen: Hold [D] or Click Pencil Icon\n" +
                "Color: Press [S] or Click Color Box";

            // Tooltip ayarları
            shortcutsToolTip.AutoPopDelay = 10000;   // 10 sn açık kalsın
            shortcutsToolTip.InitialDelay = 400;     // hover sonrası gecikme
            shortcutsToolTip.ReshowDelay = 200;
            shortcutsToolTip.ShowAlways = true;

            // Tooltip'i resim paneline bağla
            shortcutsToolTip.SetToolTip(panel1, helpText);
        }


        #endregion

        #region Keyboard

        private void ClipDetailImage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D && !_isPenMode)
                TogglePenMode(true);

            if (e.KeyCode == Keys.S)
                ShowColorPicker();
        }

        private void ClipDetailImage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
                TogglePenMode(false);
        }

        #endregion

        #region Image Setup

        public void UpdateItem(ClipboardItem item)
        {
            if (item.ItemType != ClipboardItemType.Image)
                return;

            _paths.Clear();
            _currentPath = null;

            _imageZoom = 1f;
            _minZoom = 1f;
            _imageOffset = Point.Empty;

            SetupImageMode(item.ImageContent);
        }

        private void SetupImageMode(Image img)
        {
            _image = img;
            CacheImageSize();

            CalculateInitialZoom();
            CenterImage();
            UpdateImageLabelInfo();

            panel1.Invalidate();
        }

        private void CacheImageSize()
        {
            try
            {
                using var ms = new System.IO.MemoryStream();
                _image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                _cachedImageSize = FormatByteSize(ms.Length);
            }
            catch
            {
                _cachedImageSize = "Error";
            }
        }

        private void CalculateInitialZoom()
        {
            if (_image == null) return;

            float zoomX = (float)panel1.ClientSize.Width / _image.Width;
            float zoomY = (float)panel1.ClientSize.Height / _image.Height;

            _minZoom = Math.Min(zoomX, zoomY);
            _imageZoom = _minZoom;
        }

        #endregion

        #region Mouse / Zoom / Pan

        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_image == null) return;

            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                float oldZoom = _imageZoom;
                _imageZoom = e.Delta > 0
                    ? _imageZoom + 0.2f
                    : Math.Max(_minZoom, _imageZoom - 0.2f);

                if (oldZoom != _imageZoom)
                {
                    float scale = _imageZoom / oldZoom;
                    _imageOffset.X = (int)(e.X - (e.X - _imageOffset.X) * scale);
                    _imageOffset.Y = (int)(e.Y - (e.Y - _imageOffset.Y) * scale);
                }

                ClampImageOffset();
                UpdateImageLabelInfo();
                panel1.Invalidate();
                return;
            }

            int scroll = panel1.ClientSize.Height / 10;
            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                _imageOffset.X += e.Delta > 0 ? scroll : -scroll;
            else
                _imageOffset.Y += e.Delta > 0 ? scroll : -scroll;

            ClampImageOffset();
            panel1.Invalidate();
        }
        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (_isPenMode)
            {
                _isDrawing = true;
                _currentPath = new List<PointF> { ScreenToImage(e.Location) };
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
            if (_isDrawing && _currentPath != null)
            {
                _currentPath.Add(ScreenToImage(e.Location));
                panel1.Invalidate();
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
            UpdateCopyButtonText();
        }
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

        #endregion

        #region Drawing

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (_image == null) return;

            e.Graphics.Clear(panel1.BackColor);
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int w = (int)(_image.Width * _imageZoom);
            int h = (int)(_image.Height * _imageZoom);

            e.Graphics.DrawImage(_image, _imageOffset.X, _imageOffset.Y, w, h);

            foreach (var path in _paths)
            {
                if (path.Points.Count < 2) continue;

                var pts = path.Points
                    .Select(p => new PointF(
                        p.X * _imageZoom + _imageOffset.X,
                        p.Y * _imageZoom + _imageOffset.Y))
                    .ToArray();

                using var pen = new Pen(path.Color, 2f)
                {
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round
                };

                e.Graphics.DrawLines(pen, pts);
            }
        }

        #endregion

        #region Helpers

        private PointF ScreenToImage(Point p) =>
            new(
                (p.X - _imageOffset.X) / _imageZoom,
                (p.Y - _imageOffset.Y) / _imageZoom
            );

        private void ClampImageOffset()
        {
            int w = (int)(_image.Width * _imageZoom);
            int h = (int)(_image.Height * _imageZoom);

            _imageOffset.X = w < panel1.Width
                ? (panel1.Width - w) / 2
                : Math.Min(0, Math.Max(panel1.Width - w, _imageOffset.X));

            _imageOffset.Y = h < panel1.Height
                ? (panel1.Height - h) / 2
                : Math.Min(0, Math.Max(panel1.Height - h, _imageOffset.Y));
        }

        private void CenterImage()
        {
            int w = (int)(_image.Width * _imageZoom);
            int h = (int)(_image.Height * _imageZoom);

            _imageOffset = new Point(
                (panel1.Width - w) / 2,
                (panel1.Height - h) / 2);
        }

        private void UpdateImageLabelInfo()
        {
            if (_image == null) return;
            toolStripStatusLabel1.Text =
                $"{_image.Width}x{_image.Height} | Size: {_cachedImageSize} | Zoom: {Math.Round(_imageZoom * 100)}%";
        }

        private static string FormatByteSize(long bytes)
        {
            string[] suf = { "B", "KB", "MB", "GB" };
            int i = 0;
            decimal size = bytes;

            while (Math.Round(size / 1024) >= 1 && i < suf.Length - 1)
            {
                size /= 1024;
                i++;
            }

            return $"{size:n1} {suf[i]}";
        }

        public static void SetDoubleBuffered(Control c, bool value)
        {
            typeof(Control)
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(c, value);
        }

        #endregion

        #region UI Actions

        private void TogglePenMode(bool active)
        {
            _isPenMode = active;
            pbox_pen.BackColor = active ? AppColors.GetButtonActiveColor() : Color.Transparent;
            panel1.Cursor = active ? Cursors.Cross : Cursors.Default;
        }

        private void ShowColorPicker()
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _activeColor = colorDialog1.Color;
                pnl_selectedColor.BackColor = _activeColor;
            }
        }

        private void UpdateCopyButtonText()
        {
            string text = _paths.Count > 0 ? "Copy (New Image)" : "Copy (Image)";
            poisonButton1_copyImage.Text = text;
            copyImageToolStripMenuItem.Text = text;
        }

        private void poisonButton1_copyImage_Click(object sender, EventArgs e)
        {
            if (_image == null) return;

            // Çizim yoksa normal kopyala
            if (_paths.Count == 0)
            {
                _mainForm?.CopyCliked();
                return;
            }

            // Çizim varsa flatten ederek kopyala
            using (Bitmap bmp = new Bitmap(_image.Width, _image.Height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(_image, 0, 0, _image.Width, _image.Height);

                foreach (var path in _paths)
                {
                    if (path.Points.Count < 2) continue;

                    using var pen = new Pen(path.Color, 2f)
                    {
                        StartCap = LineCap.Round,
                        EndCap = LineCap.Round
                    };

                    g.DrawLines(pen, path.Points.ToArray());
                }

                try
                {
                    Clipboard.SetImage(bmp);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Copy failed: " + ex.Message);
                }
            }
        }
        private void pbox_pen_Click(object sender, EventArgs e)
        {
            TogglePenMode(!_isPenMode);
        }
        private void pnl_selectedColor_Click(object sender, EventArgs e)
        {
            ShowColorPicker();
        }

        #endregion


        #region MANUAL RESIZE (BORDERLESS SUPPORT)
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Maximized)
            {
                var screen = Screen.FromControl(this).WorkingArea;
                this.Bounds = screen;
            }
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);

                if ((int)m.Result == HTCLIENT)
                {
                    Point cursor = PointToClient(Cursor.Position);

                    IntPtr hit = ResizeHitTestHelper.GetHitTest(this, cursor, 8);
                    if (hit != IntPtr.Zero)
                    {
                        m.Result = hit;
                        return;
                    }
                }
                return;
            }

            base.WndProc(ref m);
        }
        #endregion

        #region THEME
        public void RefreshTheme()
        {
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);

        }
        #endregion

    }
}
