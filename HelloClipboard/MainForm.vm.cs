using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard
{
	public class MainFormViewModel : IDisposable
	{
		private readonly TrayApplicationContext _trayApplicationContext;
		private readonly MainForm _mainForm;
		private bool _isDisposed = false;

		public MainFormViewModel(MainForm mainForm, TrayApplicationContext trayApplicationContext)
		{
			_mainForm = mainForm;
			_trayApplicationContext = trayApplicationContext;
		}

		public void LoadSettings()
		{
			SettingsLoader.LoadSettings();
		}

		public void CopyClicked(ClipboardItem selectedItem)
		{
			_trayApplicationContext.SuppressClipboardEvents(true);

			if (selectedItem.ItemType == ClipboardItemType.Image)
			{
				Clipboard.SetImage(selectedItem.ImageContent);
			}
			else
			{
				Clipboard.SetText(string.Join(Environment.NewLine, selectedItem.Content));
			}

			Task.Delay(100).ContinueWith(_ =>
			{
				_trayApplicationContext.SuppressClipboardEvents(false);
			});
		}


		public void Dispose()
		{
			_isDisposed = true;
		}
	}

}
