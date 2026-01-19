using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
    public class TaskService
    {


      
        public async System.Threading.Tasks.Task SetStartWithWindowsAsync(bool enable)
        {
            if (!await PrivilegesHelper.EnsureAdministrator())
                return;

            string appName = AppConstants.AppName;
            string exePath = $"\"{Application.ExecutablePath}\"";

            using RegistryKey key = Registry.CurrentUser.OpenSubKey(
                AppConstants.RunKeyPath,
                writable: true
            );

            if (key == null)
                throw new InvalidOperationException("Startup registry key not found.");

            if (enable)
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

    
}
