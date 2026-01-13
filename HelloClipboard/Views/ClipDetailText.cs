using HelloClipboard.Constants;
using HelloClipboard.Utils;
using ReaLTaiizor.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace HelloClipboard
{
    public partial class ClipDetailText : PoisonForm
    {
        #region Fields & Constructor
        private readonly MainForm _mainForm;
        private List<string> _lines = new();
        private string _fullText = string.Empty;
        private float _baseFontSize = 0.75f;
        private float _textZoom = 0.9f;
        private const int LinePadding = 4;
        private Font _drawFont;
        private Color _textColor = Color.Black;
        private SolidBrush _selectionBrush = new(Color.FromArgb(100, Color.DeepSkyBlue));
        private SolidBrush _lineNumBrush = new(Color.DimGray);
        private SolidBrush _zebraBrushOdd = new(Color.White);
        private SolidBrush _zebraBrushEven = new(Color.FromArgb(245, 245, 245));
        private SolidBrush _lineNumberBackgroundBrush = new(Color.FromArgb(235, 235, 235));
        private int _maxLineWidth;
        private int _selStartLine = -1, _selStartChar = -1;
        private int _selEndLine = -1, _selEndChar = -1;
        private bool _isSelecting;
        private Timer _autoScrollTimer;
        public ClipDetailText(MainForm mainForm, ClipboardItem item)
        {
            InitializeComponent();

            _mainForm = mainForm;

            EnableDoubleBuffer(textDrawPanel);
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            InitializeTimers();
            HookEvents();

            SetupTextMode(item.Content);
        }
        #endregion

        #region Initialization
        private void InitializeTimers()
        {
            _autoScrollTimer = new Timer
            {
                Interval = 30
            };
            _autoScrollTimer.Tick += AutoScrollTimer_Tick;
        }
        private void HookEvents()
        {
            vScrollBar1.Scroll += (_, _) => textDrawPanel.Invalidate();
            hScrollBar1.Scroll += (_, _) => textDrawPanel.Invalidate();

            textDrawPanel.Paint += textDrawPanel_Paint;
            textDrawPanel.MouseWheel += TextDrawPanel_MouseWheel;
            textDrawPanel.MouseDown += TextDrawPanel_MouseDown;
            textDrawPanel.MouseMove += TextDrawPanel_MouseMove;
            textDrawPanel.MouseUp += TextDrawPanel_MouseUp;
            textDrawPanel.MouseEnter += (_, _) => textDrawPanel.Cursor = Cursors.IBeam;
            textDrawPanel.MouseLeave += (_, _) => textDrawPanel.Cursor = Cursors.Default;
            copySelectedTextToolStripMenuItem.Click += CopySelectedTextToolStripMenuItem_Click;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
        }
        public void UpdateItem(ClipboardItem item)
        {
            if (item == null)
                return;

            SetupTextMode(item.Content);
        }
        #endregion

        #region Keyboard
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

            bool handled = keyData switch
            {
                Keys.Up => ScrollVertical(-1),
                Keys.Down => ScrollVertical(1),
                Keys.Left => ScrollHorizontal(-20),
                Keys.Right => ScrollHorizontal(20),
                Keys.PageUp => ScrollVertical(-vScrollBar1.LargeChange),
                Keys.PageDown => ScrollVertical(vScrollBar1.LargeChange),
                _ => false
            };

            if (handled)
            {
                textDrawPanel.Invalidate();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region Scroll Helpers
        private bool ScrollVertical(int delta)
        {
            int max = vScrollBar1.Maximum - vScrollBar1.LargeChange + 1;
            vScrollBar1.Value = Math.Max(vScrollBar1.Minimum,
                Math.Min(vScrollBar1.Value + delta, Math.Max(0, max)));
            return true;
        }
        private bool ScrollHorizontal(int delta)
        {
            int max = hScrollBar1.Maximum - hScrollBar1.LargeChange + 1;
            hScrollBar1.Value = Math.Max(hScrollBar1.Minimum,
                Math.Min(hScrollBar1.Value + delta, Math.Max(0, max)));
            return true;
        }
        #endregion

        #region Text Setup
        private void SetupTextMode(string text)
        {
            _fullText = text ?? string.Empty;
            _lines = _fullText
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .ToList();

            _textZoom = 0.9f;
            vScrollBar1.Value = 0;
            hScrollBar1.Value = 0;

            ClearSelection();

            UpdateCachedResources();
            CalculateScrollLimits();
            UpdateStatusLabel();

            textDrawPanel.Invalidate();
        }
        private void UpdateCachedResources()
        {
            _drawFont?.Dispose();
            _drawFont = new Font(Font.FontFamily, 12 * _baseFontSize * _textZoom);
        }
        private void CalculateScrollLimits()
        {
            if (_drawFont == null) return;

            int lineHeight = GetLineHeight();

            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = _lines.Count;
            vScrollBar1.LargeChange = Math.Max(1, textDrawPanel.Height / lineHeight);

            _maxLineWidth = _lines.Take(500)
                .Select(l => TextRenderer.MeasureText(l, _drawFont).Width)
                .DefaultIfEmpty(0)
                .Max();

            hScrollBar1.Minimum = 0;
            hScrollBar1.Maximum = _maxLineWidth + 100;
            hScrollBar1.LargeChange = Math.Max(1, textDrawPanel.Width / 2);
        }
        #endregion

        #region Painting
        private void textDrawPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_lines.Count == 0 || _drawFont == null)
                return;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int lineHeight = GetLineHeight();
            int lineNumWidth = GetLineNumberWidth();
            int visibleLines = textDrawPanel.Height / lineHeight + 1;

            int vVal = vScrollBar1.Value;
            int hVal = hScrollBar1.Value;

            e.Graphics.Clear(textDrawPanel.BackColor);

            Rectangle textClip = new(lineNumWidth, 0,
                textDrawPanel.Width - lineNumWidth, textDrawPanel.Height);

            Region originalClip = e.Graphics.Clip;
            e.Graphics.SetClip(textClip);

            for (int i = 0; i < visibleLines; i++)
            {
                int lineIdx = vVal + i;
                if (lineIdx >= _lines.Count) break;

                int y = i * lineHeight;
                int textX = lineNumWidth + 10 - hVal;

                var zebraBrush = (lineIdx % 2 == 0) ? _zebraBrushEven : _zebraBrushOdd;
                e.Graphics.FillRectangle(zebraBrush,
                    new Rectangle(lineNumWidth, y, textDrawPanel.Width, lineHeight));

                DrawSelectionForLine(e.Graphics, lineIdx, y, textX, lineHeight);

                TextRenderer.DrawText(
                    e.Graphics,
                    _lines[lineIdx],
                    _drawFont,
                    new Point(textX, y),
                    _textColor);
            }

            e.Graphics.Clip = originalClip;

            DrawLineNumberSidebar(e.Graphics, lineNumWidth, lineHeight, visibleLines, vVal);
        }
        private void DrawLineNumberSidebar(Graphics g, int width, int lineHeight, int visibleLines, int vVal)
        {
            g.FillRectangle(_lineNumberBackgroundBrush,
                new Rectangle(0, 0, width, textDrawPanel.Height));

            g.DrawLine(SystemPens.ControlLight, width - 1, 0, width - 1, textDrawPanel.Height);

            for (int i = 0; i < visibleLines; i++)
            {
                int lineIdx = vVal + i;
                if (lineIdx >= _lines.Count) break;

                Rectangle rect = new(0, i * lineHeight, width - 5, lineHeight);
                TextRenderer.DrawText(g, (lineIdx + 1).ToString(),
                    _drawFont, rect, Color.DimGray,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }
        }
        #endregion

        #region Selection
        private void CopySelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelection();
        }
        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = string.IsNullOrEmpty(GetSelectedText());
            if (!e.Cancel)
                textDrawPanel.Focus();
        }
        private void DrawSelectionForLine(Graphics g, int lineIdx, int y, int textX, int h)
        {
            if (_selStartLine == -1) return;

            int startLine = Math.Min(_selStartLine, _selEndLine);
            int endLine = Math.Max(_selStartLine, _selEndLine);

            if (lineIdx < startLine || lineIdx > endLine)
                return;

            int startChar = 0;
            int endChar = _lines[lineIdx].Length;

            if (_selStartLine == _selEndLine)
            {
                startChar = Math.Min(_selStartChar, _selEndChar);
                endChar = Math.Max(_selStartChar, _selEndChar);
            }
            else if (lineIdx == startLine)
                startChar = (_selStartLine < _selEndLine) ? _selStartChar : _selEndChar;
            else if (lineIdx == endLine)
                endChar = (_selStartLine < _selEndLine) ? _selEndChar : _selStartChar;

            if (endChar <= startChar) return;

            string before = _lines[lineIdx][..Math.Min(startChar, _lines[lineIdx].Length)];
            string selected = _lines[lineIdx]
                .Substring(startChar, Math.Min(endChar - startChar, _lines[lineIdx].Length - startChar));

            int x = textX + TextRenderer.MeasureText(before, _drawFont).Width;
            int w = TextRenderer.MeasureText(selected, _drawFont).Width;

            g.FillRectangle(_selectionBrush, x, y, w, h);
        }
        #endregion

        #region Mouse & Clipboard
        private void TextDrawPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                _textZoom = Math.Max(0.5f, _textZoom + (e.Delta > 0 ? 0.1f : -0.1f));
                UpdateCachedResources();
                CalculateScrollLimits();
            }
            else if (ModifierKeys == Keys.Shift)
            {
                ScrollHorizontal(e.Delta > 0 ? -60 : 60);
            }
            else
            {
                ScrollVertical(e.Delta > 0 ? -3 : 3);
            }

            textDrawPanel.Invalidate();
        }
        private void TextDrawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _isSelecting = true;
            _autoScrollTimer.Start();

            var c = GetCharCoordsFromMouse(e.Location);
            _selStartLine = _selEndLine = c.line;
            _selStartChar = _selEndChar = c.ch;

            UpdateStatusLabel();
            textDrawPanel.Invalidate();
        }
        private void TextDrawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isSelecting) return;

            var c = GetCharCoordsFromMouse(e.Location);
            _selEndLine = c.line;
            _selEndChar = c.ch;

            UpdateStatusLabel();
            textDrawPanel.Invalidate();
        }
        private void TextDrawPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _isSelecting = false;
            _autoScrollTimer.Stop();
        }
        private void AutoScrollTimer_Tick(object sender, EventArgs e)
        {
            if (!Visible || IsDisposed || !_isSelecting)
                return;

            Point mouse = textDrawPanel.PointToClient(Cursor.Position);
            bool moved = false;

            if (mouse.Y < 0 && vScrollBar1.Value > vScrollBar1.Minimum)
            {
                vScrollBar1.Value--;
                moved = true;
            }
            else if (mouse.Y > textDrawPanel.Height &&
                     vScrollBar1.Value < vScrollBar1.Maximum - vScrollBar1.LargeChange + 1)
            {
                vScrollBar1.Value++;
                moved = true;
            }

            if (!moved) return;

            var c = GetCharCoordsFromMouse(mouse);
            _selEndLine = c.line;
            _selEndChar = c.ch;

            UpdateStatusLabel();
            textDrawPanel.Invalidate();
        }
        #endregion

        #region Helpers
        private int GetLineHeight() =>
            TextRenderer.MeasureText("Ag", _drawFont).Height + LinePadding;
        private int GetLineNumberWidth() =>
            TextRenderer.MeasureText(_lines.Count.ToString(), _drawFont).Width + 20;
        private (int line, int ch) GetCharCoordsFromMouse(Point p)
        {
            int lineHeight = GetLineHeight();
            int lineNumWidth = GetLineNumberWidth();

            int lineIdx = Math.Max(0,
                Math.Min(vScrollBar1.Value + (p.Y / lineHeight), _lines.Count - 1));

            int relX = p.X - (lineNumWidth + 10) + hScrollBar1.Value;
            int chIdx = 0;

            if (relX > 0)
            {
                string line = _lines[lineIdx];
                for (int i = 1; i <= line.Length; i++)
                {
                    if (TextRenderer.MeasureText(line[..i], _drawFont).Width > relX)
                        return (lineIdx, i - 1);
                }
                chIdx = line.Length;
            }

            return (lineIdx, chIdx);
        }
        private void SelectAll()
        {
            if (_lines.Count == 0) return;

            _selStartLine = 0;
            _selStartChar = 0;
            _selEndLine = _lines.Count - 1;
            _selEndChar = _lines[^1].Length;

            UpdateStatusLabel();
            textDrawPanel.Invalidate();
        }
        private void CopySelection()
        {
            string text = GetSelectedText();
            if (!string.IsNullOrEmpty(text))
                Clipboard.SetText(text);
        }
        private string GetSelectedText()
        {
            if (_selStartLine == -1) return string.Empty;

            int sL = Math.Min(_selStartLine, _selEndLine);
            int eL = Math.Max(_selStartLine, _selEndLine);

            StringBuilder sb = new();

            for (int i = sL; i <= eL; i++)
            {
                string line = _lines[i];
                int cS = 0, cE = line.Length;

                if (_selStartLine == _selEndLine)
                {
                    cS = Math.Min(_selStartChar, _selEndChar);
                    cE = Math.Max(_selStartChar, _selEndChar);
                }
                else if (i == sL)
                    cS = (_selStartLine < _selEndLine) ? _selStartChar : _selEndChar;
                else if (i == eL)
                    cE = (_selStartLine < _selEndLine) ? _selEndChar : _selStartChar;

                if (cE > cS)
                    sb.Append(line.Substring(cS, Math.Min(cE - cS, line.Length - cS)));

                if (i < eL) sb.AppendLine();
            }

            return sb.ToString();
        }
        private void ClearSelection()
        {
            _selStartLine = _selEndLine = -1;
            _selStartChar = _selEndChar = -1;
        }
        private void UpdateStatusLabel()
        {
            long size = Encoding.UTF8.GetByteCount(_fullText);
            string baseText = $"Lines: {_lines.Count} | Size: {FormatByteSize(size)}";

            if (_selStartLine != -1)
            {
                toolStripStatusLabel1.Text =
                    $"{baseText} | Ln: {_selEndLine + 1}, Col: {_selEndChar + 1} | Sel: {GetSelectedText().Length} chars";
            }
            else
            {
                toolStripStatusLabel1.Text = baseText;
            }
        }
        private static string FormatByteSize(long bytes)
        {
            string[] suffix = { "B", "KB", "MB", "GB" };
            int i = 0;
            decimal size = bytes;

            while (Math.Round(size / 1024) >= 1 && i < suffix.Length - 1)
            {
                size /= 1024;
                i++;
            }

            return $"{size:n1} {suffix[i]}";
        }
        private static void EnableDoubleBuffer(Control c)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, c, new object[] { true });
        }
        #endregion

        #region Theme
        public void RefreshTheme()
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

        #region Cleanup
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!Visible)
            {
                _isSelecting = false;
                _autoScrollTimer?.Stop();
                ClearSelection();
            }
        }
        #endregion

        #region Buttons
        private void poisonButton1_copyAsText_Click(object sender, EventArgs e) =>
            _mainForm?.CopyCliked(false);
        private void poisonButton1_copyAsObject_Click(object sender, EventArgs e) =>
            _mainForm?.CopyCliked(true);
        #endregion
    }
}
