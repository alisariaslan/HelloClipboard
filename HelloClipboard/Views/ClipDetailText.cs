using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class ClipDetailText : Form
	{
		private readonly MainForm _mainForm;

		private float _textZoom = 1.0f;
		private string _fullText = string.Empty;
		private bool _fullyLoaded = false;
		private int _loadedUntilIndex = 0;
		private const int LinesPerChunk = 1000;
		private const int PreloadThresholdLines = 50;

		public ClipDetailText(MainForm mainForm, ClipboardItem item)
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
			richTextBox1.SelectionChanged += RichTextBox1_SelectionChanged;
			contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
			InitializeLineNumbers();
			SetupTextMode(item.Content);
		}

		private void RichTextBox1_SelectionChanged(object sender, EventArgs e)
		{
			UpdateStatusLabel();
		}

		private void UpdateStatusLabel()
		{
			if (toolStripStatusLabel1 == null) return;

			// 1. Satır Bilgileri
			int totalLines = _fullText.Split('\n').Length;
			int index = richTextBox1.SelectionStart;
			int line = richTextBox1.GetLineFromCharIndex(index);
			int firstCharOfLine = richTextBox1.GetFirstCharIndexFromLine(line);
			int column = index - firstCharOfLine;

			// 2. Boyut Hesaplama (Seçiliyse seçim, değilse tamamı)
			long sizeInBytes;
			bool isSelection = richTextBox1.SelectionLength > 0;

			if (isSelection)
			{
				// Seçili metnin UTF8 byte karşılığı
				sizeInBytes = System.Text.Encoding.UTF8.GetByteCount(richTextBox1.SelectedText);
			}
			else
			{
				// Tüm metnin UTF8 byte karşılığı
				sizeInBytes = System.Text.Encoding.UTF8.GetByteCount(_fullText);
			}

			string sizeText = FormatByteSize(sizeInBytes);

			// 3. Status Text Oluşturma
			// Format: Lines: 150 | Ln: 5, Col: 10 | Size: 1.2 KB (Total)
			string statusText = $"Lines: {totalLines} | Ln: {line + 1}, Col: {column + 1} | Size: {sizeText}";

			if (isSelection)
			{
				int selectedLines = 0;
				int startLine = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart);
				int endLine = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart + richTextBox1.SelectionLength);
				selectedLines = (endLine - startLine) + 1;

				statusText += $" (Selected: {selectedLines} line(s))";
			}
			else
			{
				statusText += " (Total)";
			}

			toolStripStatusLabel1.Text = statusText;
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
			UpdateStatusLabel();
		}

		public void UpdateItem(ClipboardItem item)
		{
			string shortTitle = item.Title.Length > Constants.MaxDetailFormTitleLength ? item.Title.Substring(0, Constants.MaxDetailFormTitleLength) + "…" : item.Title;
			this.Text = $"{shortTitle} - {Constants.AppName}";
			SetupTextMode(item.Content);
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
			_mainForm?.copyToolStripMenuItem_Click(sender, e);
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
			if(string.IsNullOrEmpty(richTextBox1.SelectedText))
			e.Cancel = true;

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
			}
			return string.Format("{0:n1} {1}", number, suffixes[counter]);
		}

		private void InitializeLineNumbers()
		{
			lineNumberPanel.Paint += LineNumberPanel_Paint;
			richTextBox1.VScroll += (s, e) => lineNumberPanel.Invalidate();
			richTextBox1.TextChanged += (s, e) => lineNumberPanel.Invalidate();
			richTextBox1.Resize += (s, e) => lineNumberPanel.Invalidate();
			// Zoom yapıldığında da numaralar güncellenmeli
			richTextBox1.SelectionChanged += (s, e) => lineNumberPanel.Invalidate();
		}

		private void LineNumberPanel_Paint(object sender, PaintEventArgs e)
		{
			if (string.IsNullOrEmpty(richTextBox1.Text)) return;

			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// RichTextBox üzerindeki ilk karakterin konumunu al
			int firstChar = richTextBox1.GetCharIndexFromPosition(new Point(0, 5));
			int firstLine = richTextBox1.GetLineFromCharIndex(firstChar);

			Point firstCharPos = richTextBox1.GetPositionFromCharIndex(firstChar);
			int y = firstCharPos.Y;

			// Fontu RichTextBox ile eşitle
			Font lineFont = richTextBox1.Font;
			Brush textBrush = new SolidBrush(Color.DimGray);

			int currentLine = firstLine;
			while (y < richTextBox1.Height)
			{
				string lineNum = (currentLine + 1).ToString();

				// Sayıyı sağa yaslı çizmek için genişlik hesabı
				SizeF textSize = e.Graphics.MeasureString(lineNum, lineFont);
				e.Graphics.DrawString(lineNum, lineFont, textBrush,
					lineNumberPanel.Width - textSize.Width - 5, y);

				currentLine++;
				int nextLineChar = richTextBox1.GetFirstCharIndexFromLine(currentLine);
				if (nextLineChar == -1) break;

				y = richTextBox1.GetPositionFromCharIndex(nextLineChar).Y;
				if (y > richTextBox1.Height) break;
			}
		}


		private void copySelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(richTextBox1.SelectedText))
			{
				try
				{
					// ClipboardMonitor'un kendi kopyaladığımız şeyi tekrar yakalamaması için
					// TrayApplicationContext üzerinden eventleri susturabiliriz.
					TrayApplicationContext.Instance?.SuppressClipboardEvents(true);

					Clipboard.SetText(richTextBox1.SelectedText);

					// Kısa bir süre sonra tekrar izlemeyi aç (MainFormViewModel'deki mantıkla aynı)
					System.Threading.Tasks.Task.Delay(150).ContinueWith(_ =>
						TrayApplicationContext.Instance?.SuppressClipboardEvents(false));
				}
				catch (Exception ex)
				{
					MessageBox.Show("Kopyalama hatası: " + ex.Message);
				}
			}
		}
	}
}
