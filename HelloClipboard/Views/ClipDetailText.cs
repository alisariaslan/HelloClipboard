using HelloClipboard.Constants;
using HelloClipboard.Utils;
using ReaLTaiizor.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace HelloClipboard
{
    public partial class ClipDetailText : PoisonForm
    {
        private readonly MainForm _mainForm;
        private List<string> _lines = new List<string>();
        private string _fullText = string.Empty;
        private float _baseFontSize = 0.75f;
        private float _textZoom = 0.9f;
        private const int LinePadding = 4;
        private Font _drawFont;
        private Color _textColor = Color.Black;
        private SolidBrush _selectionBrush = new SolidBrush(Color.FromArgb(100, Color.DeepSkyBlue));
        private SolidBrush _lineNumBrush = new SolidBrush(Color.DimGray);
        private SolidBrush _zebraBrushOdd = new SolidBrush(Color.White);
        private SolidBrush _zebraBrushEven = new SolidBrush(Color.FromArgb(245, 245, 245));
        private int _maxLineWidth = 0;
        private int _selStartLine = -1, _selStartChar = -1;
        private int _selEndLine = -1, _selEndChar = -1;
        private bool _isSelecting = false;
        private Timer _autoScrollTimer;
        private Rectangle _lastNormalBounds;

        public ClipDetailText(MainForm mainForm, ClipboardItem item)
        {
            InitializeComponent();
            _mainForm = mainForm;

            EnableDoubleBuffer(textDrawPanel);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            _autoScrollTimer = new Timer();
            _autoScrollTimer.Interval = 30;
            _autoScrollTimer.Tick += AutoScrollTimer_Tick;

            vScrollBar1.Scroll += (s, e) => textDrawPanel.Invalidate();
            hScrollBar1.Scroll += (s, e) => textDrawPanel.Invalidate();
            textDrawPanel.MouseWheel += TextDrawPanel_MouseWheel;
            textDrawPanel.MouseDown += TextDrawPanel_MouseDown;
            textDrawPanel.MouseMove += TextDrawPanel_MouseMove;
            textDrawPanel.MouseUp += TextDrawPanel_MouseUp;
            textDrawPanel.Paint += textDrawPanel_Paint;
            textDrawPanel.MouseEnter += TextDrawPanel_MouseEnter;
            textDrawPanel.MouseLeave += TextDrawPanel_MouseLeave;


            SetupTextMode(item.Content);
        }





        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.A))
            {
                SelectAll();
                return true;
            }
            if (keyData == (Keys.Control | Keys.C))
            {
                CopySelection();
                return true;
            }
            bool handled = false;
            int vStep = 1;
            int hStep = 20;

            switch (keyData)
            {
                case Keys.Up:
                    ScrollVertical(-vStep);
                    handled = true;
                    break;
                case Keys.Down:
                    ScrollVertical(vStep);
                    handled = true;
                    break;
                case Keys.Left:
                    ScrollHorizontal(-hStep);
                    handled = true;
                    break;
                case Keys.Right:
                    ScrollHorizontal(hStep);
                    handled = true;
                    break;
                case Keys.PageUp:
                    ScrollVertical(-vScrollBar1.LargeChange);
                    handled = true;
                    break;
                case Keys.PageDown:
                    ScrollVertical(vScrollBar1.LargeChange);
                    handled = true;
                    break;
            }
            if (handled)
            {
                textDrawPanel.Invalidate();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ScrollVertical(int delta)
        {
            int newVal = vScrollBar1.Value + delta;
            int max = vScrollBar1.Maximum - vScrollBar1.LargeChange + 1;
            vScrollBar1.Value = Math.Max(vScrollBar1.Minimum, Math.Min(newVal, Math.Max(0, max)));
        }

        private void ScrollHorizontal(int delta)
        {
            int newVal = hScrollBar1.Value + delta;
            int max = hScrollBar1.Maximum - hScrollBar1.LargeChange + 1;
            hScrollBar1.Value = Math.Max(hScrollBar1.Minimum, Math.Min(newVal, Math.Max(0, max)));
        }

        private void SelectAll()
        {
            if (_lines.Count == 0) return;

            _selStartLine = 0;
            _selStartChar = 0;
            _selEndLine = _lines.Count - 1;
            _selEndChar = _lines[_selEndLine].Length;

            UpdateStatusLabel();
            textDrawPanel.Invalidate();
        }

        private void CopySelection()
        {
            string text = GetSelectedText();
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
            }
        }

        private void AutoScrollTimer_Tick(object sender, EventArgs e)
        {
            if (!_isSelecting) return;
            Point clientMouse = textDrawPanel.PointToClient(Cursor.Position);
            bool moved = false;

            if (clientMouse.Y < 0)
            {
                if (vScrollBar1.Value > vScrollBar1.Minimum) { vScrollBar1.Value -= 1; moved = true; }
            }
            else if (clientMouse.Y > textDrawPanel.Height)
            {
                if (vScrollBar1.Value < vScrollBar1.Maximum - vScrollBar1.LargeChange + 1) { vScrollBar1.Value += 1; moved = true; }
            }

            if (clientMouse.X < 0)
            {
                if (hScrollBar1.Value > hScrollBar1.Minimum) { hScrollBar1.Value = Math.Max(hScrollBar1.Minimum, hScrollBar1.Value - 20); moved = true; }
            }
            else if (clientMouse.X > textDrawPanel.Width)
            {
                if (hScrollBar1.Value < hScrollBar1.Maximum - hScrollBar1.LargeChange + 1) { hScrollBar1.Value = Math.Min(hScrollBar1.Maximum - hScrollBar1.LargeChange + 1, hScrollBar1.Value + 20); moved = true; }
            }

            if (moved)
            {
                var c = GetCharCoordsFromMouse(clientMouse);
                _selEndLine = c.line;
                _selEndChar = c.ch;
                UpdateStatusLabel();
                textDrawPanel.Invalidate();
            }
        }

        private void EnableDoubleBuffer(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, control, new object[] { true });
        }

        private void SetupTextMode(string text)
        {
            _fullText = text ?? string.Empty;
            _lines = _fullText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();

            _textZoom = 0.9f;
            vScrollBar1.Value = 0;
            hScrollBar1.Value = 0;
            _selStartLine = _selEndLine = -1;
            _selStartChar = _selEndChar = -1;

            UpdateCachedResources();
            CalculateScrollLimits();
            UpdateStatusLabel();
            textDrawPanel.Invalidate();
        }

        private void UpdateCachedResources()
        {
            _drawFont?.Dispose();
            _drawFont = new Font(this.Font.FontFamily, 12 * _baseFontSize * _textZoom);
        }

        private void CalculateScrollLimits()
        {
            if (_drawFont == null) return;
            int lineHeight = TextRenderer.MeasureText("Ag", _drawFont).Height + LinePadding;
            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = _lines.Count;
            vScrollBar1.LargeChange = Math.Max(1, textDrawPanel.Height / lineHeight);

            _maxLineWidth = 0;
            int checkCount = Math.Min(_lines.Count, 500);
            for (int i = 0; i < checkCount; i++)
            {
                int w = TextRenderer.MeasureText(_lines[i], _drawFont).Width;
                if (w > _maxLineWidth) _maxLineWidth = w;
            }
            hScrollBar1.Minimum = 0;
            hScrollBar1.Maximum = _maxLineWidth + 100;
            hScrollBar1.LargeChange = Math.Max(1, textDrawPanel.Width / 2);
        }


        private SolidBrush _lineNumberBackgroundBrush = new SolidBrush(Color.FromArgb(235, 235, 235)); // Açık ama sabit bir gri tonu
        private void textDrawPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_lines.Count == 0 || _drawFont == null) return;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int lineHeight = TextRenderer.MeasureText("Ag", _drawFont).Height + LinePadding;
            int lineNumWidth = TextRenderer.MeasureText(_lines.Count.ToString(), _drawFont).Width + 20;

            int vVal = vScrollBar1.Value;
            int hVal = hScrollBar1.Value;
            int visibleLines = (textDrawPanel.Height / lineHeight) + 1;

            // 1. Ekranı temizle
            e.Graphics.Clear(textDrawPanel.BackColor);

            // --- AŞAMA 1: METİN KATMANI (TEXT LAYER) ---
            // Burada çizim alanını sadece metin bölgesiyle (sol gri alan hariç) sınırlandırıyoruz.
            // Bu sayede yazılar sola kaysa bile, lineNumWidth koordinatından öncesine çizilemezler.

            Rectangle textAreaClip = new Rectangle(lineNumWidth, 0, textDrawPanel.Width - lineNumWidth, textDrawPanel.Height);
            Region originalRegion = e.Graphics.Clip; // Orijinal clip bölgesini sakla
            e.Graphics.SetClip(textAreaClip);

            for (int i = 0; i < visibleLines; i++)
            {
                int lineIdx = vVal + i;
                if (lineIdx >= _lines.Count) break;

                int yPos = i * lineHeight;
                int textX = lineNumWidth + 10 - hVal; // Scroll durumuna göre X konumu

                // Zebra Arkaplan (Sadece metin alanı için)
                SolidBrush textBackgroundBrush = (lineIdx % 2 == 0) ? _zebraBrushEven : _zebraBrushOdd;
                Rectangle textRect = new Rectangle(lineNumWidth, yPos, textDrawPanel.Width - lineNumWidth, lineHeight);
                e.Graphics.FillRectangle(textBackgroundBrush, textRect);

                // Seçim Alanı
                DrawSelectionForLine(e.Graphics, lineIdx, yPos, textX, lineHeight);

                // Metnin Kendisi
                TextRenderer.DrawText(e.Graphics, _lines[lineIdx], _drawFont, new Point(textX, yPos), _textColor);
            }

            // --- AŞAMA 2: ARAYÜZ KATMANI (UI LAYER) ---
            // Clip'i (sınırlamayı) kaldırıyoruz ki sol tarafa çizim yapabilelim.
            e.Graphics.Clip = originalRegion;

            // Sol taraftaki gri sütunu EN SON çiziyoruz. 
            // Böylece altta yanlışlıkla taşan yazı parçası kalsa bile bu gri kutu onların üzerini örter.
            Rectangle sidebarRect = new Rectangle(0, 0, lineNumWidth, textDrawPanel.Height);
            e.Graphics.FillRectangle(_lineNumberBackgroundBrush, sidebarRect);

            // İsteğe bağlı: Satır numarası ile metin arasına dikey bir çizgi
            e.Graphics.DrawLine(SystemPens.ControlLight, lineNumWidth - 1, 0, lineNumWidth - 1, textDrawPanel.Height);

            // Satır Numaralarını Çiz
            for (int i = 0; i < visibleLines; i++)
            {
                int lineIdx = vVal + i;
                if (lineIdx >= _lines.Count) break;

                int yPos = i * lineHeight;
                Rectangle numRect = new Rectangle(0, yPos, lineNumWidth - 5, lineHeight); // Sağdan biraz boşluk bırak

                // Numarayı çiz
                TextRenderer.DrawText(e.Graphics, (lineIdx + 1).ToString(), _drawFont, numRect, Color.DimGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }
        }

        private void DrawSelectionForLine(Graphics g, int lineIdx, int y, int textX, int h)
        {
            if (_selStartLine == -1) return;
            int sL = Math.Min(_selStartLine, _selEndLine);
            int eL = Math.Max(_selStartLine, _selEndLine);
            if (lineIdx >= sL && lineIdx <= eL)
            {
                int cStart = 0, cEnd = _lines[lineIdx].Length;
                if (_selStartLine == _selEndLine) { cStart = Math.Min(_selStartChar, _selEndChar); cEnd = Math.Max(_selStartChar, _selEndChar); }
                else if (lineIdx == sL) cStart = (_selStartLine < _selEndLine) ? _selStartChar : _selEndChar;
                else if (lineIdx == eL) cEnd = (_selStartLine < _selEndLine) ? _selEndChar : _selStartChar;

                if (cEnd > cStart)
                {
                    string pre = _lines[lineIdx].Substring(0, Math.Min(cStart, _lines[lineIdx].Length));
                    string sel = _lines[lineIdx].Substring(cStart, Math.Min(cEnd - cStart, _lines[lineIdx].Length - cStart));
                    int x = textX + TextRenderer.MeasureText(pre, _drawFont).Width;
                    int w = TextRenderer.MeasureText(sel, _drawFont).Width;
                    g.FillRectangle(_selectionBrush, x, y, w, h);
                }
            }
        }

        private void TextDrawPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control) { _textZoom = Math.Max(0.5f, _textZoom + (e.Delta > 0 ? 0.1f : -0.1f)); UpdateCachedResources(); CalculateScrollLimits(); }
            else if (ModifierKeys == Keys.Shift) { int newVal = hScrollBar1.Value + (e.Delta > 0 ? -60 : 60); hScrollBar1.Value = Math.Max(hScrollBar1.Minimum, Math.Min(newVal, hScrollBar1.Maximum - hScrollBar1.LargeChange + 1)); }
            else { int newVal = vScrollBar1.Value + (e.Delta > 0 ? -3 : 3); vScrollBar1.Value = Math.Max(vScrollBar1.Minimum, Math.Min(newVal, vScrollBar1.Maximum - vScrollBar1.LargeChange + 1)); }
            textDrawPanel.Invalidate();
        }

        private (int line, int ch) GetCharCoordsFromMouse(Point p)
        {
            if (_drawFont == null) return (0, 0);
            int lineHeight = TextRenderer.MeasureText("Ag", _drawFont).Height + LinePadding;
            int lineNumWidth = TextRenderer.MeasureText(_lines.Count.ToString(), _drawFont).Width + 20;
            int lineIdx = Math.Max(0, Math.Min(vScrollBar1.Value + (p.Y / lineHeight), _lines.Count - 1));
            int relX = p.X - (lineNumWidth + 10) + hScrollBar1.Value;
            int chIdx = 0;
            if (relX > 0)
            {
                string line = _lines[lineIdx];
                for (int i = 1; i <= line.Length; i++) { if (TextRenderer.MeasureText(line.Substring(0, i), _drawFont).Width > relX) return (lineIdx, i - 1); }
                chIdx = line.Length;
            }
            return (lineIdx, chIdx);
        }

        private string GetSelectedText()
        {
            if (_selStartLine == -1) return "";
            int sL = Math.Min(_selStartLine, _selEndLine);
            int eL = Math.Max(_selStartLine, _selEndLine);
            StringBuilder sb = new StringBuilder();
            for (int i = sL; i <= eL; i++)
            {
                string line = _lines[i];
                int cS = 0, cE = line.Length;
                if (_selStartLine == _selEndLine) { cS = Math.Min(_selStartChar, _selEndChar); cE = Math.Max(_selStartChar, _selEndChar); }
                else if (i == sL) cS = (_selStartLine < _selEndLine) ? _selStartChar : _selEndChar;
                else if (i == eL) cE = (_selStartLine < _selEndLine) ? _selEndChar : _selStartChar;
                if (cE > cS) sb.Append(line.Substring(cS, Math.Min(cE - cS, line.Length - cS)));
                if (i < eL) sb.AppendLine();
            }
            return sb.ToString();
        }

        private void TextDrawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isSelecting = true;
                _autoScrollTimer.Start();
                var c = GetCharCoordsFromMouse(e.Location);
                _selStartLine = _selEndLine = c.line;
                _selStartChar = _selEndChar = c.ch;
                UpdateStatusLabel();
                textDrawPanel.Invalidate();
            }
        }

        private void TextDrawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSelecting)
            {
                var c = GetCharCoordsFromMouse(e.Location);
                _selEndLine = c.line;
                _selEndChar = c.ch;
                UpdateStatusLabel();
                textDrawPanel.Invalidate();
            }
        }

        private void TextDrawPanel_MouseUp(object sender, MouseEventArgs e) { _isSelecting = false; _autoScrollTimer.Stop(); }

        private void UpdateStatusLabel()
        {
            long size = Encoding.UTF8.GetByteCount(_fullText);
            string baseStatus = $"Lines: {_lines.Count} | Size: {FormatByteSize(size)}";
            if (_selStartLine != -1)
            {
                int currentLine = _selEndLine + 1;
                int currentCol = _selEndChar + 1;
                toolStripStatusLabel1.Text = $"{baseStatus} | Ln: {currentLine}, Col: {currentCol} | Sel: {GetSelectedText().Length} chars";
            }
            else toolStripStatusLabel1.Text = baseStatus;
        }

        private string FormatByteSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0; decimal number = bytes;
            while (Math.Round(number / 1024) >= 1) { number /= 1024; counter++; }
            return $"{number:n1} {suffixes[counter]}";
        }

        #region THEME
        public void ThemeChanged()
        {
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);

            textDrawPanel.BackColor = AppColors.GetBackColor();
            _zebraBrushOdd.Color = AppColors.GetBackColor();
            _zebraBrushEven.Color = AppColors.GetAlternateColor();
            _lineNumberBackgroundBrush.Color = AppColors.GetLineNumberBackground();
            _selectionBrush.Color = AppColors.GetSelectionColor();
            _textColor = AppColors.GetForeColor();
            textDrawPanel.Invalidate();
        }

        #endregion

        public void UpdateItem(ClipboardItem item)
        {
            SetupTextMode(item.Content);
        }

        private void copySelectedTextToolStripMenuItem_Click(object sender, EventArgs e) => CopySelection();
        
        private void ContextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e) => e.Cancel = string.IsNullOrEmpty(GetSelectedText());

        private void TextDrawPanel_MouseEnter(object sender, EventArgs e)
        {
            textDrawPanel.Cursor = Cursors.IBeam;
        }

        private void TextDrawPanel_MouseLeave(object sender, EventArgs e)
        {
            textDrawPanel.Cursor = Cursors.Default;

        }

        #region MANUAL RESIZE (BORDERLESS SUPPORT)
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Normal)
            {
                _lastNormalBounds = this.Bounds;
            }
            if (this.WindowState == FormWindowState.Maximized)
            {
                var screen = Screen.FromControl(this).WorkingArea;
                this.Bounds = screen;
            }
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;
            if (m.Msg == WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xFFF0;
                if (command == SC_MOVE && this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.Bounds = _lastNormalBounds;
                    Point cursorPos = Cursor.Position;
                    int dx = cursorPos.X - this.Width / 2;
                    int dy = cursorPos.Y - 15;
                    this.Location = new Point(dx, dy);
                }
            }
            base.WndProc(ref m);
        }
        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _autoScrollTimer?.Dispose();
            _drawFont?.Dispose();
            _selectionBrush?.Dispose();
            _lineNumBrush?.Dispose();
            _zebraBrushOdd?.Dispose();
            _zebraBrushEven?.Dispose();
            _lineNumberBackgroundBrush?.Dispose();
            base.OnFormClosing(e);
        }

        private void poisonButton1_copyAsText_Click(object sender, EventArgs e) => _mainForm?.CopyCliked(asObject: false);

        private void poisonButton1_copyAsObject_Click(object sender, EventArgs e) => _mainForm?.CopyCliked(asObject: true);
    }
}