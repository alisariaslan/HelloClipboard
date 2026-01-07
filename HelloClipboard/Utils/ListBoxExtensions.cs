using System.Windows.Forms;

namespace HelloClipboard.Utils
{
	public static class ListBoxExtensions
	{
		/// <summary>
		/// ListBox içerisinde seçimi yukarı veya aşağı kaydırır.
		/// </summary>
		public static void MoveSelection(this ListBox listBox, int direction)
		{
			if (listBox.Items.Count == 0) return;

			int currentIndex = listBox.SelectedIndex;
			int newIndex = currentIndex + direction;

			// Eğer hiçbir şey seçili değilse ve yukarı/aşağı basıldıysa ilk öğeyi seç
			if (currentIndex == -1)
			{
				listBox.SelectedIndex = 0;
				return;
			}

			// Sınır kontrolü
			if (newIndex >= 0 && newIndex < listBox.Items.Count)
			{
				listBox.SelectedIndex = newIndex;
			}
		}

	}
}
