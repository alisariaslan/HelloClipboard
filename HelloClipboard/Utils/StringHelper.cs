

using System.Text.RegularExpressions;

namespace HelloClipboard.Utils
{
    public static class StringHelper
    {


        public static string CleanWhitespaces(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            string cleaned = Regex.Replace(input, @"[\p{C}\s]+", " ");
            var trimmed = cleaned.Trim();
            while (trimmed.Length > 0 && (trimmed[0] == '\r' || trimmed[0] == '\n' || char.IsWhiteSpace(trimmed[0])))
            {
                trimmed = trimmed.Substring(1);
            }
            while (trimmed.Length > 0 && (trimmed[trimmed.Length - 1] == '\r' || trimmed[trimmed.Length - 1] == '\n' || char.IsWhiteSpace(trimmed[trimmed.Length - 1])))
            {
                trimmed = trimmed.Substring(0, trimmed.Length - 1);
            }

            return trimmed;
        }
    }
}
