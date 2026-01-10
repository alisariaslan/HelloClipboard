namespace HelloClipboard.Html
{
	internal class HelpHtml
	{
		public static string GetTitle()
		{
			return "Help - HelloClipboard";
		}
		public static string GetHtml(float fontsize = 12)
		{
			string helpHtml = $@"
<!doctype html>
<html>
<head>
  <meta charset='utf-8'/>
  <title>Help - HelloClipboard</title>
  <style>
    body {{  font-size: {fontsize}px; font-family: Segoe UI, Tahoma, Arial; padding:16px; color:#222; }}
    h2 {{ margin-top:0; }}
    ul {{ line-height:1.6; }}
    a {{ color:#1a73e8; text-decoration:none; }}
    .note {{ margin-top:12px; color:#555; }}
  </style>
</head>
<body>
  <h1>Help - HelloClipboard</h1>
  <ul>
    <li><strong>Copy</strong> — Copies the selected text to the system clipboard.</li>
    <li><strong>Settings</strong> — Configure basic application options such as starting with Windows and clipboard behavior.</li>
   <li><strong>Zoom</strong> — While viewing an image or text in the detail window, you can zoom in and out by holding <strong>Ctrl</strong> and using the mouse wheel.</li>
<li><strong>Pan</strong> — While viewing an image in the detail window, you can move the image by holding the left mouse button and dragging it around the detail window.</li>

  </ul>
  <p class='note'>If you experience any issues, please report them on the GitHub repository or contact <a href='mailto:dev@alisariaslan.com'>dev@alisariaslan.com</a>.</p>
  <p>GitHub: <a href='https://github.com/alisariaslan/HelloClipboard' target='_blank'>https://github.com/alisariaslan/HelloClipboard</a></p>
</body>
</html>";
			return helpHtml;
		}
	}
}
