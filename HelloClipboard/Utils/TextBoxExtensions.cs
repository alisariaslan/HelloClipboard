using System.Windows.Forms;

namespace HelloClipboard.Utils
{
    public static class TextBoxExtensions
    {
        /// <summary>
        /// Deletes the word before the cursor (Ctrl + Backspace behavior).
        /// </summary>
        public static void DeletePreviousWord(this TextBox tb)
        {
            int pos = tb.SelectionStart;
            if (pos == 0) return;

            string text = tb.Text;
            int start = pos;

            // First skip whitespaces
            while (start > 0 && char.IsWhiteSpace(text[start - 1]))
                start--;
            // Then skip the word
            while (start > 0 && !char.IsWhiteSpace(text[start - 1]))
                start--;

            int length = pos - start;
            if (length <= 0) return;

            tb.Text = text.Remove(start, length);
            tb.SelectionStart = start;
        }

        /// <summary>
        /// Deletes the word after the cursor (Ctrl + Delete behavior).
        /// </summary>
        public static void DeleteNextWord(this TextBox tb)
        {
            int pos = tb.SelectionStart;
            string text = tb.Text;
            if (pos >= text.Length) return;

            int end = pos;
            // First skip whitespaces
            while (end < text.Length && char.IsWhiteSpace(text[end]))
                end++;
            // Then skip the word
            while (end < text.Length && !char.IsWhiteSpace(text[end]))
                end++;

            int length = end - pos;
            if (length <= 0) return;

            tb.Text = text.Remove(pos, length);
            tb.SelectionStart = pos;
        }

        /// <summary>
        /// Handles Ctrl+Backspace and Ctrl+Delete (word deletion) logic for the TextBox.
        /// </summary>
        public static bool HandleWordDeletion(this TextBox textBox, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Back)
            {
                textBox.DeletePreviousWord(); // Call the deletion method
                return true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                textBox.DeleteNextWord(); // Call the deletion method
                return true;
            }
            return false;
        }
    }
}