using System;
using System.IO;

namespace HelloClipboard
{
	public static class Constants
	{
		public static readonly string AppName = "HelloClipboard";

		public static readonly string AppBaseDir = AppDomain.CurrentDomain.BaseDirectory;

		public static readonly string UserDataDir =
	   Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);


		public static TimeSpan applicationUpdateInterval = TimeSpan.FromHours(6);

		private const string _appSettingsFileName = "settings.json";
		private const string _tempConfigsFileName = "tempconfigs.json";

		public static string AppSettingsPath => Path.Combine(UserDataDir, _appSettingsFileName);
		public static string TempConfigsPath => Path.Combine(UserDataDir, _tempConfigsFileName);

		public static string AppSettingsFileName => _appSettingsFileName;
		public static string TempConfigsFileName => _tempConfigsFileName;
	}
}
