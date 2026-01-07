using System.Windows.Forms;

namespace HelloClipboard.Utils
{
	public static class TextBoxExtensions
	{
		/// <summary>
		/// İmleçten önceki kelimeyi siler (Ctrl + Backspace davranışı).
		/// </summary>
		public static void DeletePreviousWord(this TextBox tb)
		{
			int pos = tb.SelectionStart;
			if (pos == 0) return;

			string text = tb.Text;
			int start = pos;

			// Önce boşlukları geç
			while (start > 0 && char.IsWhiteSpace(text[start - 1]))
				start--;
			// Sonra kelimeyi geç
			while (start > 0 && !char.IsWhiteSpace(text[start - 1]))
				start--;

			int length = pos - start;
			if (length <= 0) return;

			tb.Text = text.Remove(start, length);
			tb.SelectionStart = start;
		}

		/// <summary>
		/// İmleçten sonraki kelimeyi siler (Ctrl + Delete davranışı).
		/// </summary>
		public static void DeleteNextWord(this TextBox tb)
		{
			int pos = tb.SelectionStart;
			string text = tb.Text;
			if (pos >= text.Length) return;

			int end = pos;
			// Önce boşlukları geç
			while (end < text.Length && char.IsWhiteSpace(text[end]))
				end++;
			// Sonra kelimeyi geç
			while (end < text.Length && !char.IsWhiteSpace(text[end]))
				end++;

			int length = end - pos;
			if (length <= 0) return;

			tb.Text = text.Remove(pos, length);
			tb.SelectionStart = pos;
		}
	}
}