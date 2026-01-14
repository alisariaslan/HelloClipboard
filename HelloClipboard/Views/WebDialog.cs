using HelloClipboard.Constants;
using HelloClipboard.Utils;
using ReaLTaiizor.Forms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HelloClipboard.Views
{
    public partial class WebDialog : PoisonForm
    {
        public WebDialog(string title, string htmlContent)
        {
            InitializeComponent();
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            this.Text = title;
            string styledHtml = ApplyThemeToHtml(htmlContent);
            webBrowser1.DocumentText = styledHtml;
        }
        private string ApplyThemeToHtml(string content)
        {
            // Renkleri Hex formatına çeviriyoruz
            string backColor = ColorTranslator.ToHtml(AppColors.GetDeeperBackColor());
            string foreColor = ColorTranslator.ToHtml(AppColors.GetForeColor());
            string linkColor = "#2196F3"; // Varsayılan mavi link

            // HTML içeriğine CSS enjekte ediyoruz
            string style = $@"
                <style>
                    body {{
                        background-color: {backColor};
                        color: {foreColor};
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        margin: 20px;
                        line-height: 1.6;
                    }}
                    a {{
                        color: {linkColor};
                        text-decoration: none;
                    }}
                    a:hover {{
                        text-decoration: underline;
                    }}
                    /* Scrollbar'ı da karanlık temaya uyduralım (IE/Edge tabanlı) */
                    html {{
                        scrollbar-face-color: {backColor};
                        scrollbar-track-color: {backColor};
                    }}
                </style>";

            // Eğer içerikte <body> yoksa basitçe başına ekle, varsa içine enjekte et
            if (content.Contains("<body"))
            {
                return content.Replace("<head>", "<head>" + style);
            }
            else
            {
                return $"<html><head>{style}</head><body>{content}</body></html>";
            }
        }
        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url != null && e.Url.Scheme != "about")
            {
                e.Cancel = true;
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = e.Url.ToString(),
                        UseShellExecute = true
                    });
                }
                catch { }
            }
        }

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
    }
}
