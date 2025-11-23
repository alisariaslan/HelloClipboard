using System.Windows.Forms;

namespace HelloClipboard.Html
{
	internal class AboutHtml
	{
		public static string GetTitle()
		{
			return "About - HelloClipboard";
		}
		public static string GetHtml()
		{
			string aboutHtml = $@"
<!doctype html>
<html>
<head>
  <meta charset='utf-8'/>
  <title>About - HelloClipboard</title>
  <style>
    body {{ font-family: Segoe UI, Tahoma, Arial; padding: 16px; color:#222; }}
    h1 {{ margin-top:0; }}
    a {{ color:#1a73e8; text-decoration:none; }}
    a:hover {{ text-decoration:underline; }}
    .meta {{ margin-top:12px; color:#555; }}
    .footer {{ margin-top:20px; font-size:90%; color:#666; }}
  </style>
</head>
<body>
  <h1>HelloClipboard</h1>
  <div class='meta'>Version: {Application.ProductVersion}</div>
  <p>A modern, lightweight clipboard sharing tool for Windows.</p>
  <p>Synchronize your clipboard securely across platforms with ease.</p>
  <p>Developed by <strong>Ali SARIASLAN</strong></p>
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
