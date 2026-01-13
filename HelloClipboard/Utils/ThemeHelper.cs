using ReaLTaiizor.Enum.Poison;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using System;
using System.Drawing;
using System.Windows.Forms;
using HelloClipboard.Constants; // AppColors

namespace HelloClipboard.Utils
{
    public static class ThemeHelper
    {
        public static void SaveTheme(ThemeStyle theme)
        {
            SettingsLoader.Current.SelectedTheme = theme;
            SettingsLoader.Save();
        }

        public static ThemeStyle GetTheme()
        {
            return SettingsLoader.Current.SelectedTheme;
        }

        public static void ApplySavedThemeToForm(Form form, PoisonStyleManager poisonStyleManager)
        {
            if (form is null) throw new Exception("Form is null");
            if (poisonStyleManager is null) throw new Exception("PoisonStyleManager is null");

            var theme = GetTheme();
            if (form is PoisonForm poisonForm)
            {
                poisonForm.Theme = theme;
                poisonStyleManager.Theme = theme;
            }
            else
                throw new Exception("Form is not a PoisonForm");

            switch (theme)
            {
                case ThemeStyle.Light:
                    form.BackColor = AppColors.LightBackColor;
                    form.ForeColor = AppColors.LightForeColor;
                    break;
                case ThemeStyle.Dark:
                    form.BackColor = AppColors.DarkBackColor;
                    form.ForeColor = AppColors.DarkForeColor;
                    break;
                default:
                    form.BackColor = SystemColors.Control;
                    form.ForeColor = SystemColors.ControlText;
                    break;
            }
        }

        public static void ApplyThemeToControl(Control control, ThemeStyle theme)
        {
            if (control is null) return;

            switch (theme)
            {
                case ThemeStyle.Light:
                    control.BackColor = AppColors.LightBackColor;
                    control.ForeColor = AppColors.LightForeColor;
                    break;
                case ThemeStyle.Dark:
                    control.BackColor = AppColors.DarkBackControlColor;
                    control.ForeColor = AppColors.DarkForeColor;
                    break;
                default:
                    control.BackColor = SystemColors.Control;
                    control.ForeColor = SystemColors.ControlText;
                    break;
            }

            foreach (Control child in control.Controls)
                ApplyThemeToControl(child, theme);
        }
    }
}
