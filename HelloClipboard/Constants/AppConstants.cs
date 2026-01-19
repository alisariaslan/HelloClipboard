using System;
using System.IO;

namespace HelloClipboard
{
    public static class AppConstants
    {
        public static readonly string AppName = "HelloClipboard";

        public static readonly string AppBaseDir = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string UserDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);

        public static readonly string HistoryDirectory = Path.Combine(UserDataDir, "ClipboardHistory");

        public static readonly string RegistryKeyPath = @"Software\HelloClipboard";

        public static readonly string RegistryKeyValueName = "AppSecret";

        public static readonly TimeSpan ApplicationUpdateInterval = TimeSpan.FromHours(2);

        public const int MaxDetailFormTitleLength = 15;

        private const string _appSettingsFileName = "settings.json";
        private const string _tempConfigsFileName = "tempconfigs.json";

        public const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";


        public static string AppSettingsPath => Path.Combine(UserDataDir, _appSettingsFileName);
        public static string TempConfigsPath => Path.Combine(UserDataDir, _tempConfigsFileName);

    }
}
