using HelloClipboard.Utils;
using System.Windows.Forms;

namespace HelloClipboard.Html
{
    internal class AboutHtml
    {
        public static string GetTitle()
        {
            return "About";
        }
        public static string GetHtml(float fontSize = 12)
        {
            string aboutHtml = $@"
<!doctype html>
<html>
<head>
  <meta charset='utf-8'/>
  <style>
    body {{ font-size: {fontSize}px; font-family: Segoe UI, Tahoma, Arial; padding: 16px; }}
    h2 {{ margin-top:0; }}
    a {{ text-decoration:none; }}
    a:hover {{ text-decoration:underline; }}
    .meta {{ margin-top:12px; }}
    .footer {{ margin-top:20px; font-size:90%;  }}
  </style>
</head>
<body>
  <h1>HelloClipboard</h1>
  <div class='meta'>Version: {AppVersionHelper.GetAppVersion()}</div>
  <p>A modern, lightweight clipboard sharing tool for Windows.</p>
  <p>Synchronize your clipboard securely across platforms with ease.</p>
  <p>Developed by <strong>Ali SARIASLAN</strong> </p>
    <p>Contributions:
        <p>-<strong>Bahadır Düzcan</strong> <a href='https://github.com/bahadirduzcan'>github.com/bahadirduzcan</a></p>
    </p>
  <p>Contact: <a href='mailto:dev@alisariaslan.com'>dev@alisariaslan.com</a></p>
  <div class='footer'>
    GitHub: <a href='https://github.com/alisariaslan/HelloClipboard' target='_blank'>https://github.com/alisariaslan/HelloClipboard</a>
  </div>
</body>
</html>";
            return aboutHtml;
        }
    }
}
