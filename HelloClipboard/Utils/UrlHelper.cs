using System;
using System.Diagnostics;

namespace HelloClipboard.Utils
{
    public static class UrlHelper
    {
        /// <summary>
        /// Validates if the provided string is a valid HTTP or HTTPS URL.
        /// </summary>
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

        /// <summary>
        /// Opens the specified URL using the system's default browser.
        /// </summary>
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
                // Re-throw with an English error message or log the exception
                throw new Exception($"An error occurred while opening the URL: {ex.Message}");
            }
        }
    }
}