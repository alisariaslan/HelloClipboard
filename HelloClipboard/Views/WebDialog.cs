using HelloClipboard.Constants;
using HelloClipboard.Utils;
using ReaLTaiizor.Forms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Threading.Tasks;

namespace HelloClipboard.Views
{
    public partial class WebDialog : PoisonForm
    {
        private WebView2 webView21;
        private readonly string _htmlContent;

        public WebDialog(string title, string htmlContent)
        {
            InitializeComponent();
            _htmlContent = htmlContent;

            // Setup UI
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            this.Text = title;

            InitializeWebViewContainer();
        }

        private void InitializeWebViewContainer()
        {
            try
            {
                webView21 = new WebView2
                {
                    Dock = DockStyle.Fill
                };
                this.Controls.Add(webView21);

                // Handle the Load event for async initialization
                this.Load += WebDialog_Load;

                // Ensure resources are released when form closes
                this.FormClosing += (s, e) => webView21?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize WebView2 component: {ex.Message}",
                    "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void WebDialog_Load(object sender, EventArgs e)
        {
            await InitializeAndLoadHtmlAsync(_htmlContent);
        }

        private async Task InitializeAndLoadHtmlAsync(string content)
        {
            try
            {
                // 1. Ensure WebView2 Runtime is installed and initialize the core
                if (webView21.CoreWebView2 == null)
                {
                    await webView21.EnsureCoreWebView2Async(null);
                }

                // 2. Register events safely after initialization
                webView21.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

                // 3. Inject theme and navigate
                string processedHtml = ApplyThemeToHtml(content);
                webView21.CoreWebView2.NavigateToString(processedHtml);
            }
            catch (Exception ex)
            {
                // Most common cause: WebView2 Runtime is not installed
                MessageBox.Show("The browser engine failed to start. Please ensure 'WebView2 Runtime' is installed.\n\n" +
                                $"Details: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            // Prevent opening a new browser window inside the app
            e.Handled = true;
            OpenLinkInDefaultBrowser(e.Uri);
        }

        private string ApplyThemeToHtml(string content)
        {
            try
            {
                string backColor = ColorTranslator.ToHtml(AppColors.GetDeeperBackColor());
                string foreColor = ColorTranslator.ToHtml(AppColors.GetForeColor());
                const string linkColor = "#2196F3";

                string style = $@"
                    <style>
                        body {{
                            background-color: {backColor};
                            color: {foreColor};
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            margin: 20px;
                            line-height: 1.6;
                            overflow-wrap: break-word;
                        }}
                        a {{ color: {linkColor}; text-decoration: none; }}
                        a:hover {{ text-decoration: underline; }}
                    </style>";

                if (string.IsNullOrWhiteSpace(content))
                    content = "<em>No content provided.</em>";

                if (content.Contains("<head>"))
                    return content.Replace("<head>", "<head>" + style);

                return $"<html><head>{style}</head><body>{content}</body></html>";
            }
            catch
            {
                // Return original content if theme application fails to prevent a blank screen
                return content;
            }
        }

        private void OpenLinkInDefaultBrowser(string url)
        {
            // Basic URL validation
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("http")) return;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region UI LOGIC (RESIZE & DRAG)
        protected override void OnResize(EventArgs e)
        {
            try
            {
                base.OnResize(e);
                if (this.WindowState == FormWindowState.Maximized)
                {
                    var screen = Screen.FromControl(this).WorkingArea;
                    this.Bounds = screen;
                }
            }
            catch { /* Non-critical error */ }
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
    }
}