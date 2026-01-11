using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HelloClipboard.Utils
{
    public static class AppVersionHelper
    {
        /// <summary>
        /// 1.2.3 formatında base version döner
        /// </summary>
        public static string GetBaseVersion()
        {
            try
            {
                var entry = Assembly.GetEntryAssembly() ?? typeof(MainForm).Assembly;
                var location = entry.Location;

                // 1) FileVersionInfo öncelikli
                if (!string.IsNullOrEmpty(location))
                {
                    var fvi = FileVersionInfo.GetVersionInfo(location);

                    if (fvi.FileMajorPart != 0 || fvi.FileMinorPart != 0 || fvi.FileBuildPart != 0)
                        return $"{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}";

                    if (!string.IsNullOrEmpty(fvi.ProductVersion))
                    {
                        var m = Regex.Match(fvi.ProductVersion, @"^(\d+\.\d+\.\d+)");
                        if (m.Success)
                            return m.Groups[1].Value;
                    }
                }

                // 2) AssemblyInformationalVersion
                var infoAttr = entry.GetCustomAttributes(false)
                    .OfType<AssemblyInformationalVersionAttribute>()
                    .FirstOrDefault();

                if (infoAttr != null && !string.IsNullOrEmpty(infoAttr.InformationalVersion))
                {
                    var m = Regex.Match(infoAttr.InformationalVersion, @"^(\d+\.\d+\.\d+)");
                    if (m.Success)
                        return m.Groups[1].Value;
                }

                // 3) Assembly version fallback
                var ver = entry.GetName().Version;
                if (ver != null && ver.Build >= 0)
                    return $"{ver.Major}.{ver.Minor}.{ver.Build}";
            }
            catch { }

            return "0.0.0";
        }

        /// <summary>
        /// Sadece build numarasını döner → 13
        /// </summary>
        public static int GetBuildNumber()
        {
            try
            {
                var entry = Assembly.GetEntryAssembly() ?? typeof(MainForm).Assembly;
                var location = entry.Location;

                if (!string.IsNullOrEmpty(location))
                {
                    var fvi = FileVersionInfo.GetVersionInfo(location);
                    if (fvi.FilePrivatePart > 0)
                        return fvi.FilePrivatePart;
                }

                var ver = entry.GetName().Version;
                if (ver != null && ver.Revision >= 0)
                    return ver.Revision;
            }
            catch { }

            return 0;
        }

        /// <summary>
        /// 1.2.3 (13) formatında final versiyon
        /// </summary>
        public static string GetAppVersion()
        {
            var baseVersion = GetBaseVersion();
            var build = GetBuildNumber();

            return build > 0
                ? $"{baseVersion} ({build})"
                : baseVersion;
        }

        private static Version FixVersion(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
                return new Version(0, 0, 0, 0);

            // 3+d18836f... → 3
            // 1.2.3+hash   → 1.2.3
            var clean = v
                .Split('+')[0]
                .Split('-')[0]
                .Trim();

            return Version.TryParse(clean, out var ver)
                ? ver
                : new Version(0, 0, 0, 0);
        }
    }
}
