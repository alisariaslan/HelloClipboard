using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard
{
	public class UpdateInfo
	{
		public string Version { get; set; }
		public string Notes { get; set; }
	}

	public class UpdateService
	{
		private const string UpdateCheckUrl = "https://raw.githubusercontent.com/alisariaslan/HelloClipboard/main/latest_version.json";
		private string UpdateDownloadUrl { get; set; }

		public void SetUpdateDownloadUrl(string partialVersionStr)
		{
			UpdateDownloadUrl = "https://github.com/alisariaslan/HelloClipboard/releases/download/latest/HelloClipboard_Installer.exe";
		}
		// Olay Tanımı: Güncelleme bulunduğunda UI'ı haberdar eder
		public event EventHandler<UpdateInfo> UpdateAvailable;

		private static CancellationTokenSource _cts;
		private bool _isChecking;

		/// <summary>
		/// Arka planda periyodik kontrolü başlatır.
		/// </summary>
		public void StartPeriodicCheck(string currentVersion)
		{
			if (_isChecking) return;
			_isChecking = true;
			_cts = new CancellationTokenSource();

			Task.Run(async () =>
			{
				try
				{
					while (!_cts.Token.IsCancellationRequested)
					{
						// Ayar kontrolü
						if (SettingsLoader.Current.CheckUpdates)
						{
							var now = DateTime.UtcNow;
							var last = TempConfigLoader.Current.LastUpdateCheck;
							if (last == default || (now - last) >= Constants.ApplicationUpdateInterval)
							{
								await CheckForUpdateAsync(currentVersion, true);
								// Not: CheckForUpdateAsync zaten başarılı olursa event fırlatmalı 
								// veya burada sonucu kontrol edip fırlatmalısın.
								// Mevcut CheckForUpdateAsync içinde event fırlatma yoksa ekleyebiliriz.
								var update = await CheckForUpdateAsync(currentVersion, true);
								if (update != null) UpdateAvailable?.Invoke(this, update);
							}
						}
						await Task.Delay(TimeSpan.FromMinutes(5), _cts.Token); // 5 dakikada bir uyanıp kontrol et
					}
				}
				catch (OperationCanceledException) { }
				finally { _isChecking = false; }
			}, _cts.Token);
		}

		public void StopPeriodicCheck() // static kaldırıldı
		{
			_cts?.Cancel();
			_cts?.Dispose();
		}

		public async Task<UpdateInfo> CheckForUpdateAsync(string currentVersion, bool silent)
		{
			try
			{
				using (HttpClient client = new HttpClient())
				{
					var json = await client.GetStringAsync(UpdateCheckUrl);
					var info = JsonSerializer.Deserialize<UpdateInfo>(json);
					if (info == null) throw new Exception("Failed to parse update information.");

					SetUpdateDownloadUrl(info.Version);
					Version latest = new Version(info.Version);
					Version current = new Version(currentVersion);

					TempConfigLoader.Current.LastUpdateCheck = DateTime.UtcNow;
					TempConfigLoader.Save();

					if (latest > current) return info;
				}
			}
			catch (Exception ex)
			{
				if (!silent)
					MessageBox.Show($"Failed to check updates.\nError: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return null;
		}

		public async Task DownloadAndRunUpdateAsync()
		{
			try
			{
				MessageBox.Show(
					"Before proceeding, please temporarily disable your antivirus software.\n" +
					"Some antivirus programs may falsely block the update installer.",
					"Antivirus Warning",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);

				using (HttpClient client = new HttpClient())
				{
					var bytes = await client.GetByteArrayAsync(UpdateDownloadUrl);

					string fileName = Path.GetFileName(new Uri(UpdateDownloadUrl).LocalPath);

					string downloadsPath = Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
						"Downloads"
					);
					string targetPath = Path.Combine(downloadsPath, fileName);

					File.WriteAllBytes(targetPath, bytes);

					MessageBox.Show(
						$"Update downloaded to:\n{targetPath}\n\nThe application will now close and start the installer.",
						"Update Ready",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information
					);

					Process.Start(new ProcessStartInfo
					{
						FileName = targetPath,
						UseShellExecute = true
					});

					TrayApplicationContext.Instance?.ExitApplication();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Failed to download or run the update.\n\nError: {ex.Message}",
					"Update Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}
	}
}
