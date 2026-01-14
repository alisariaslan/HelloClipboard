using HelloClipboard.Constants;
using HelloClipboard.Utils;
using ReaLTaiizor.Forms;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard
{
    public partial class ClipDetailFile : PoisonForm
    {
        #region Fields & Constructor
        private readonly MainForm _mainForm;
        private float _textZoom = 1.0f;
        private const float BaseFontSize = 12f;
        public ClipDetailFile(MainForm mainForm, ClipboardItem item)
        {
            InitializeComponent();

            _mainForm = mainForm;

            InitRichTextBox();
            InitEvents();

            UpdateItem(item);
        }
        #endregion

        #region Init
        private void InitRichTextBox()
        {
            richTextBox1.ReadOnly = true;
            richTextBox1.DetectUrls = false;
            richTextBox1.WordWrap = false;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
            richTextBox1.ContextMenuStrip = contextMenuStrip1;
            richTextBox1.HideSelection = false;
            ApplySelectionColors();

            richTextBox1.Enter += (_, __) => ApplySelectionColors();
            richTextBox1.Leave += (_, __) => ApplySelectionColors();
        }
        private void InitEvents()
        {
            MouseWheel += ClipDetail_MouseWheel;
            richTextBox1.MouseWheel += ClipDetail_MouseWheel;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
        }
        #endregion

        #region Public API
        public void UpdateItem(ClipboardItem item)
        {
            SetupTextMode(item.Content);
            UpdateCopyFileButton(item.Content);
            _ = UpdateStatusInfoAsync(item.Content);
        }
        #endregion

        #region Text Mode
        private void SetupTextMode(string text)
        {
            richTextBox1.Visible = true;
            richTextBox1.Text = text;

            _textZoom = 0.8f;
            var oldFont = richTextBox1.Font;

            richTextBox1.Font = new Font(
                oldFont.FontFamily,
                BaseFontSize * _textZoom
            );

            oldFont.Dispose();

        }
        #endregion

        #region Mouse / Zoom
        private void ClipDetail_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                HandleZoom(e);
                return;
            }

            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                HandleHorizontalScroll(e);
            }
        }
        private void HandleZoom(MouseEventArgs e)
        {
            if (!richTextBox1.Visible) return;

            _textZoom = Math.Max(0.3f,
                _textZoom + (e.Delta > 0 ? 0.1f : -0.1f));

            richTextBox1.Font = new Font(
                richTextBox1.Font.FontFamily,
                BaseFontSize * _textZoom
            );
        }
        private void HandleHorizontalScroll(MouseEventArgs e)
        {
            if (!richTextBox1.Visible) return;

            int command = e.Delta > 0 ? SB_LINELEFT : SB_LINERIGHT;

            for (int i = 0; i < 10; i++)
            {
                SendMessage(
                    richTextBox1.Handle,
                    WM_HSCROLL,
                    (IntPtr)command,
                    IntPtr.Zero
                );
            }
        }
        #endregion

        #region Copy / Context Menu
        private void poisonButton1_copyPath_Click(object sender, EventArgs e)
        {
            _mainForm?.CopyCliked();
        }
        private string GetSelectedText()
        {
            if (richTextBox1 == null)
                return string.Empty;

            return string.IsNullOrWhiteSpace(richTextBox1.SelectedText)
                ? string.Empty
                : richTextBox1.SelectedText;
        }
        private void copySelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = GetSelectedText();
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
            }
        }
        private void ContextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(richTextBox1.SelectedText))
                e.Cancel = true;
        }
        #endregion

        #region File / Directory Info
        private void UpdateCopyFileButton(string path)
        {
            bool fileExists = File.Exists(path);
            bool dirExists = Directory.Exists(path);

            poisonButton1_copyPath.Enabled = fileExists || dirExists;

            if (dirExists)
                poisonButton1_copyPath.Text = "Copy (Folder)";
            else if (fileExists)
                poisonButton1_copyPath.Text = "Copy (File)";
            else
                poisonButton1_copyPath.Text = "Path not found";
        }
        private async Task UpdateStatusInfoAsync(string path)
        {
            if (!IsHandleCreated || IsDisposed) return;

            try
            {
                if (File.Exists(path))
                {
                    long size = new FileInfo(path).Length;

                    toolStripStatusLabel1.Text =
                        $"File Exists | Size: {FormatSize(size)}";
                    toolStripStatusLabel1.ForeColor = Color.DarkGreen;
                }
                else if (Directory.Exists(path))
                {
                    toolStripStatusLabel1.Text =
                        "Directory Exists | Calculating size...";
                    toolStripStatusLabel1.ForeColor = Color.Blue;

                    var result = await Task.Run(() => GetDirectoryInfo(path));

                    if (IsDisposed || !IsHandleCreated) return;

                    toolStripStatusLabel1.Text =
                        $"Directory Exists | Items: {result.count} | Total Size: {FormatSize(result.size)}";
                }
                else
                {
                    toolStripStatusLabel1.Text = "Path not found on disk";
                    toolStripStatusLabel1.ForeColor = Color.Red;
                }
            }
            catch
            {
                if (!IsDisposed)
                    toolStripStatusLabel1.Text = "Error reading path info";
            }
        }
        private (long size, int count) GetDirectoryInfo(string path)
        {
            long totalSize = 0;
            int count = 0;

            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                totalSize += new FileInfo(file).Length;
                count++;
            }

            count += Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Length;
            return (totalSize, count);
        }
        private string FormatSize(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            decimal size = bytes;
            int unit = 0;

            while (Math.Round(size / 1024) >= 1 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }

            return $"{size:n1} {units[unit]}";
        }
        #endregion

        #region WinAPI
        private const int WM_HSCROLL = 0x114;
        private const int SB_LINELEFT = 0;
        private const int SB_LINERIGHT = 1;
        [DllImport("user32.dll")]
        private static extern int SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam
        );
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
        private void ApplySelectionColors()
        {
            if (richTextBox1 == null) return;

            int start = richTextBox1.SelectionStart;
            int length = richTextBox1.SelectionLength;

            richTextBox1.SuspendLayout();

            richTextBox1.SelectionBackColor = AppColors.GetSelectionColor();
            richTextBox1.SelectionColor = AppColors.GetForeColor();

            richTextBox1.SelectionStart = start;
            richTextBox1.SelectionLength = length;

            richTextBox1.ResumeLayout();
        }
        public void RefreshTheme()
        {
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            richTextBox1.BackColor = AppColors.GetBackColor();
            richTextBox1.ForeColor = AppColors.GetForeColor();
            ApplySelectionColors();
        }
        #endregion

 
    }
}
