using System;
using System.Diagnostics;

namespace HelloClipboard.Utils
{
	public static class UrlHelper
	{ 
		public static bool IsValidUrl(string text)
		{
			if (string.IsNullOrWhiteSpace(text)) return false;
			text = text.Trim();

			if (Uri.TryCreate(text, UriKind.Absolute, out var uri))
			{
				return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
			}
			return false;
		}

		public static void OpenUrl(string url)
		{
			if (!IsValidUrl(url)) return;

			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = url.Trim(),
					UseShellExecute = true
				});
			}
			catch (Exception ex)
			{
				// Hata durumunda Form'a bilgi fırlatabiliriz veya burada loglayabiliriz
				throw new Exception($"URL açılırken hata oluştu: {ex.Message}");
			}
		}
	}
}
