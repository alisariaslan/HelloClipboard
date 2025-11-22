
using Microsoft.Win32;
using System;
using System.Net.Http;
using System.Windows.Forms;

namespace HelloClipboard
{
	public class MainFormViewModel : IDisposable
	{
		public event Action<string> OnMessage;


		private readonly HttpClient _httpClient = new HttpClient();
	
		private bool _isDisposed = false;

		public MainFormViewModel(MainForm mainForm)
		{
		
		}

		public void LoadSettings()
		{
			SettingsLoader.LoadSettings();
			OnMessage?.Invoke("Settings loaded.");
		}



		public void ToggleStartWithWindows(bool newState)
		{
			string appName = Constants.AppName;
			string exePath = $"\"{Application.ExecutablePath}\"";
			try
			{
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
						   @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true))
				{
					if (newState)
					{
						key.SetValue(appName, exePath);
					}
					else
					{
						if (key.GetValue(appName) != null)
							key.DeleteValue(appName);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to update Windows startup.\nError: {ex.Message}",
								"Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void Dispose()
		{
			_httpClient.Dispose();
			_isDisposed = true;
		}
	}

}
