using System;

namespace HelloClipboard.Utils
{
	public static class CharHelper
	{
		public static string GetLeadingChars(string text, char end)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			int lessThanIndex = text.IndexOf(end);

			// If the specified character is not found or is at the start, take the first 20 characters
			if (lessThanIndex <= 0)
			{
				int length = Math.Min(text.Length, 20);
				return text.Substring(0, length);
			}

			// Extract the substring up to the specified character
			string leadingPart = text.Substring(0, lessThanIndex);

			// Convert invisible characters to Unicode representations for better visibility
			var result = new System.Text.StringBuilder();
			foreach (char c in leadingPart)
			{
				// If it's not a standard printable character (control characters or whitespace)
				if (char.IsControl(c) || char.IsWhiteSpace(c))
				{
					// Show the Unicode code of the character in Hex format
					result.Append($"[U+{(int)c:X4}]");
				}
				else
				{
					// Append the character as-is (this case is rare given the logic)
					result.Append(c);
				}
			}
			return result.ToString();
		}
	}
}