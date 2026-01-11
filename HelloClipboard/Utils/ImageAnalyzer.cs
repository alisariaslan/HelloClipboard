using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace HelloClipboard.Utils
{
    internal class ImageAnalizer
    {
        public static bool AreImagesEqual(Image img1, Image img2)
        {
            if (img1 == null || img2 == null)
                return false;

            if (img1.Width != img2.Width || img1.Height != img2.Height)
                return false;

            try
            {
                using (var ms1 = new MemoryStream())
                using (var ms2 = new MemoryStream())
                {
                    img1.Save(ms1, ImageFormat.Png);
                    img2.Save(ms2, ImageFormat.Png);

                    byte[] bytes1 = ms1.ToArray();
                    byte[] bytes2 = ms2.ToArray();

                    if (bytes1.Length != bytes2.Length)
                        return false;

                    for (int i = 0; i < bytes1.Length; i++)
                    {
                        if (bytes1[i] != bytes2[i])
                            return false;
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
