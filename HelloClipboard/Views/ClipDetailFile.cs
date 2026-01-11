using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard
{
    public partial class ClipDetailFile : Form
    {
        private readonly MainForm _mainForm;

        private float _textZoom = 1.0f;
        private string _fullText = string.Empty;
        private bool _fullyLoaded = false;
        private int _loadedUntilIndex = 0;
        private const int LinesPerChunk = 1000;
        private const int PreloadThresholdLines = 50;

        public ClipDetailFile(MainForm mainForm, ClipboardItem item)
        {
            InitializeComponent();

            _mainForm = mainForm;
            string shortTitle = item.Title.Length > Constants.MaxDetailFormTitleLength ? item.Title.Substring(0, Constants.MaxDetailFormTitleLength) + "…" : item.Title;
            this.Text = $"{shortTitle} - {Constants.AppName}";

            this.MouseWheel += ClipDetail_MouseWheel;
            richTextBox1.MouseWheel += ClipDetail_MouseWheel;
            richTextBox1.DetectUrls = false;
            richTextBox1.ReadOnly = true;
            richTextBox1.VScroll += RichTextBox1_VScroll;
            richTextBox1.Resize += RichTextBox1_Resize;
            richTextBox1.ContextMenuStrip = contextMenuStrip1;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            UpdateCopyFileButton(item.Content);
            SetupTextMode(item.Content);
            UpdateStatusInfo(item.Content);
        }

        // ---------------- TEXT MODE ----------------
        private void SetupTextMode(string text)
        {
            _fullText = text ?? string.Empty;
            _fullyLoaded = false;
            _loadedUntilIndex = 0;

            richTextBox1.Visible = true;
            richTextBox1.WordWrap = false;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
            LoadNextChunk(reset: true);

            float baseFontSize = 12;

            _textZoom = 0.8f;

            richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, baseFontSize * _textZoom);
        }

        public void UpdateItem(ClipboardItem item)
        {
            string shortTitle = item.Title.Length > Constants.MaxDetailFormTitleLength ? item.Title.Substring(0, Constants.MaxDetailFormTitleLength) + "…" : item.Title;
            this.Text = $"{shortTitle} - {Constants.AppName}";
            SetupTextMode(item.Content);
            UpdateCopyFileButton(item.Content);
            UpdateStatusInfo(item.Content);
        }


        // ---------------- ZOOM ----------------
        private void ClipDetail_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (richTextBox1.Visible)
                {
                    _textZoom = Math.Max(0.3f, _textZoom + (e.Delta > 0 ? 0.1f : -0.1f));
                    richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, 12 * _textZoom);
                }
            }
            else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (richTextBox1.Visible)
                {
                    int scrollAmount = e.Delta > 0 ? -10 : 10;
                    int steps = 10;
                    for (int i = 0; i < steps; i++)
                    {
                        SendMessage(richTextBox1.Handle, WM_HSCROLL, (IntPtr)(e.Delta > 0 ? SB_LINELEFT : SB_LINERIGHT), IntPtr.Zero);
                    }
                }
            }
        }

        // ---------------- P/Invoke ----------------
        private const int WM_HSCROLL = 0x114;
        private const int SB_LINELEFT = 0;
        private const int SB_LINERIGHT = 1;
        private const int WM_SETREDRAW = 0x000B;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, bool wParam, int lParam);


        // ---------------- COPY ----------------
        private void button1_copy_Click(object sender, EventArgs e)
        {
            _mainForm?.CopyCliked();
        }

        // Büyük metinleri hızlı yüklemek için yeniden çizimi geçici kapat
        private void SetTextFast(string textToDisplay, bool resetSelection)
        {
            if (richTextBox1.IsHandleCreated)
            {
                SendMessage(richTextBox1.Handle, WM_SETREDRAW, false, 0);
            }
            richTextBox1.SuspendLayout();
            richTextBox1.Clear();
            richTextBox1.Text = textToDisplay;
            if (resetSelection)
            {
                richTextBox1.SelectionStart = 0;
                richTextBox1.SelectionLength = 0;
            }
            richTextBox1.ResumeLayout();
            if (richTextBox1.IsHandleCreated)
            {
                SendMessage(richTextBox1.Handle, WM_SETREDRAW, true, 0);
                richTextBox1.Invalidate();
            }
        }

        private void AppendTextFast(string chunk)
        {
            if (string.IsNullOrEmpty(chunk))
                return;

            if (richTextBox1.IsHandleCreated)
            {
                SendMessage(richTextBox1.Handle, WM_SETREDRAW, false, 0);
            }
            int selectionStart = richTextBox1.SelectionStart;
            richTextBox1.SuspendLayout();
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectedText = chunk;
            richTextBox1.SelectionStart = selectionStart;
            richTextBox1.SelectionLength = 0;
            richTextBox1.ResumeLayout();
            if (richTextBox1.IsHandleCreated)
            {
                SendMessage(richTextBox1.Handle, WM_SETREDRAW, true, 0);
                richTextBox1.Invalidate();
            }
        }

        private void LoadNextChunk(bool reset)
        {
            if (_fullyLoaded)
                return;

            int startIndex = _loadedUntilIndex;
            int targetLines = 0;
            int length = _fullText.Length;
            int end = startIndex;

            while (end < length && targetLines < LinesPerChunk)
            {
                if (_fullText[end] == '\n')
                    targetLines++;
                end++;
            }

            if (end >= length)
            {
                end = length;
                _fullyLoaded = true;
            }

            string chunk = _fullText.Substring(startIndex, end - startIndex);
            if (reset)
            {
                SetTextFast(chunk, resetSelection: true);
            }
            else
            {
                AppendTextFast(chunk);
            }

            _loadedUntilIndex = end;
        }

        private void TryLoadMoreIfNearBottom()
        {
            if (_fullyLoaded)
                return;

            int lastChar = richTextBox1.GetCharIndexFromPosition(new Point(1, richTextBox1.ClientSize.Height - 1));
            int lastLine = richTextBox1.GetLineFromCharIndex(lastChar);
            int totalLines = richTextBox1.Lines.Length;

            while (!_fullyLoaded && (totalLines - lastLine) < PreloadThresholdLines)
            {
                LoadNextChunk(reset: false);
                lastChar = richTextBox1.GetCharIndexFromPosition(new Point(1, richTextBox1.ClientSize.Height - 1));
                lastLine = richTextBox1.GetLineFromCharIndex(lastChar);
                totalLines = richTextBox1.Lines.Length;
            }
        }

        private void RichTextBox1_VScroll(object sender, EventArgs e)
        {
            TryLoadMoreIfNearBottom();
        }

        private void RichTextBox1_Resize(object sender, EventArgs e)
        {
            TryLoadMoreIfNearBottom();
        }

        private void ContextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(richTextBox1.SelectedText))
                e.Cancel = true;

        }

        private void UpdateCopyFileButton(string filePath)
        {
            bool fileExists = File.Exists(filePath);
            bool dirExists = Directory.Exists(filePath);
            bool exists = fileExists || dirExists;

            button1_copy.Enabled = exists;

            if (dirExists)
            {
                button1_copy.Text = "Copy folder"; // Klasör ise metni güncelle
            }
            else if (fileExists)
            {
                button1_copy.Text = "Copy file";
            }
            else
            {
                button1_copy.Text = "Path not found";
            }

            // Status label bilgisini güncelle
            UpdateStatusInfo(filePath);
        }

        private async void UpdateStatusInfo(string filePath)
        {
            if (toolStripStatusLabel1 == null) return;

            if (File.Exists(filePath))
            {
                long bytes = new FileInfo(filePath).Length;
                toolStripStatusLabel1.Text = $"File Exists | Size: {FormatSize(bytes)}";
                toolStripStatusLabel1.ForeColor = Color.DarkGreen;
            }
            else if (Directory.Exists(filePath))
            {
                toolStripStatusLabel1.Text = "Directory Exists | Calculating size...";
                toolStripStatusLabel1.ForeColor = Color.Blue;

                // Performansı korumak için hesaplamayı arka plana atıyoruz
                try
                {
                    string pathSnapshot = filePath; // Thread güvenliği için
                    var (size, count) = await Task.Run(() => GetDirectoryInfo(pathSnapshot));

                    // Form kapanmışsa veya yol değişmişse güncelleme yapma
                    if (this.IsDisposed) return;

                    toolStripStatusLabel1.Text = $"Directory Exists | Items: {count} | Total Size: {FormatSize(size)}";
                }
                catch (Exception)
                {
                    toolStripStatusLabel1.Text = "Directory Exists | Size unknown (Access Denied)";
                }
            }
            else
            {
                toolStripStatusLabel1.Text = "Path not found on disk";
                toolStripStatusLabel1.ForeColor = Color.Red;
            }
        }



        // Klasör tarama işlemini yapan yardımcı metod
        private (long size, int count) GetDirectoryInfo(string path)
        {
            long totalSize = 0;
            int count = 0;

            // Recursive tarama yaparak dosyaları topla
            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                totalSize += new FileInfo(file).Length;
                count++;
            }

            // Klasörlerin kendisini de saymak istersen:
            count += Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Length;

            return (totalSize, count);
        }

        private string FormatSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }

        private void copySelectedTextToolStripMenuItem_Click(object sender, EventArgs e) => _mainForm?.CopyCliked();
    }
}
