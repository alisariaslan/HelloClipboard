using System;
using System.Threading.Tasks;

namespace HelloClipboard.Services
{
    /// <summary>
    /// Manages the Privacy Mode state, which temporarily suspends clipboard monitoring.
    /// Provides a countdown mechanism and notifies the UI of state changes.
    /// </summary>
    public class PrivacyService
    {
        private bool _isActive;
        private DateTime _until;

        public bool IsActive => _isActive;
        public DateTime Until => _until;

        public event Action<bool> StateChanged;
        public event Action Tick; // Used to update the countdown timer in the UI

        /// <summary>
        /// Toggles the Privacy Mode state based on the provided duration.
        /// </summary>
        public void Toggle(int minutes)
        {
            if (_isActive) Disable();
            else Enable(TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// Enables Privacy Mode for a specified duration and starts the background monitor.
        /// </summary>
        public void Enable(TimeSpan duration)
        {
            if (_isActive) return;

            _isActive = true;
            _until = DateTime.UtcNow.Add(duration);
            StateChanged?.Invoke(true);

            // Background task to monitor the remaining duration
            Task.Run(async () =>
            {
                while (_isActive)
                {
                    if (DateTime.UtcNow >= _until)
                    {
                        Disable();
                        break;
                    }

                    // Notify UI to refresh the countdown display
                    Tick?.Invoke();

                    // Check state and update every second
                    await Task.Delay(1000);
                }
            });
        }

        /// <summary>
        /// Retrieves the default privacy duration from settings, enforced between 1 and 99 minutes.
        /// </summary>
        public int GetPrivacyDurationMinutes()
        {
            int minutes = SettingsLoader.Current?.PrivacyModeDurationMinutes ?? 10;

            // Safety bounds
            if (minutes < 1) minutes = 10;
            if (minutes > 99) minutes = 99;

            return minutes;
        }

        /// <summary>
        /// Disables Privacy Mode and notifies subscribers.
        /// </summary>
        public void Disable()
        {
            if (!_isActive) return;
            _isActive = false;
            StateChanged?.Invoke(false);
        }
    }
}