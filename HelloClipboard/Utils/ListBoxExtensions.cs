using System.Windows.Forms;

namespace HelloClipboard.Utils
{
	public static class ListBoxExtensions
	{
		/// <summary>
		/// Moves the selection within the ListBox up or down.
		/// </summary>
		public static void MoveSelection(this ListBox listBox, int direction)
		{
			if (listBox.Items.Count == 0) return;

			int currentIndex = listBox.SelectedIndex;
			int newIndex = currentIndex + direction;

			// If nothing is selected and up/down is pressed, select the first item
			if (currentIndex == -1)
			{
				listBox.SelectedIndex = 0;
				return;
			}

			// Bounds check
			if (newIndex >= 0 && newIndex < listBox.Items.Count)
			{
				listBox.SelectedIndex = newIndex;
			}
		}
	}
}