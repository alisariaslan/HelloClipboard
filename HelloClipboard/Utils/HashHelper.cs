using System;
using System.Drawing;

namespace HelloClipboard.Utils
{
    public static class HashHelper
    {
        public static string CalculateMd5Hash(string text)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                var hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public static string HashImageBytes(Image image)
        {
            if (image == null) return null;

            using (var ms = new System.IO.MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    ms.Position = 0;
                    byte[] hashBytes = md5.ComputeHash(ms);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
