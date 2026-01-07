using System;
using System.Threading.Tasks;

namespace HelloClipboard.Services
{
	public class PrivacyService
	{
		private bool _isActive;
		private DateTime _until;

		public bool IsActive => _isActive;
		public DateTime Until => _until;

		public event Action<bool> StateChanged;
		public event Action Tick;

		public void Toggle(int minutes)
		{
			if (_isActive) Disable();
			else Enable(TimeSpan.FromMinutes(minutes));
		}

		public void Enable(TimeSpan duration)
		{
			if (_isActive) return;

			_isActive = true;
			_until = DateTime.UtcNow.Add(duration);
			StateChanged?.Invoke(true);

			// Arka planda süreyi takip eden tek görev
			Task.Run(async () =>
			{
				while (_isActive)
				{
					if (DateTime.UtcNow >= _until)
					{
						Disable();
						break;
					}
					Tick?.Invoke(); // UI'daki geri sayım yazısı için
					await Task.Delay(1000); // Saniyede bir kontrol
				}
			});
		}

		public int GetPrivacyDurationMinutes()
		{
			int minutes = SettingsLoader.Current?.PrivacyModeDurationMinutes ?? 10;
			if (minutes < 1)
				minutes = 10;
			if (minutes > 99)
				minutes = 99;
			return minutes;
		}

		public void Disable()
		{
			if (!_isActive) return;
			_isActive = false;
			StateChanged?.Invoke(false);
		}
	}
}