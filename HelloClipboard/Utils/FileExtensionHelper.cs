namespace HelloClipboard.Utils
{
    public static class FileExtensionHelper
    {
        public static string GetFileExtension(ClipboardItemType type)
        {
            switch (type)
            {
                case ClipboardItemType.Text:
                    return ".txt";
                case ClipboardItemType.Path:
                    return ".path";
                case ClipboardItemType.Image:
                    return ".png";
                default:
                    return ".txt";
            }
        }

        public static ClipboardItemType GetItemTypeFromExtension(string extension)
        {
            string lowerExtension = extension.ToLowerInvariant();

            switch (lowerExtension)
            {
                case ".txt":
                    return ClipboardItemType.Text;
                case ".path":
                    return ClipboardItemType.Path;
                case ".png":
                    return ClipboardItemType.Image;
                default:
                    return ClipboardItemType.Text;
            }
        }
    }
}
