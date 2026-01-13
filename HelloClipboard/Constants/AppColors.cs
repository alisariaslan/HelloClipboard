using HelloClipboard.Utils;
using ReaLTaiizor.Enum.Poison;
using System;
using System.Drawing;

namespace HelloClipboard.Constants
{
    internal static class AppColors
    {
        // Light Theme
        public static Color LightBackColor { get; } = Color.White;
        public static Color LightForeColor { get; } = Color.Black;
        public static Color LightAlternateColor { get; } = Color.FromArgb(230, 245, 255); // Örnek zebra rengi
        public static Color LightButtonActive { get; } = Color.LightBlue;

        // Dark Theme
        public static Color DarkBackColor { get; } = Color.FromArgb(30, 30, 30);
        public static Color DarkForeColor { get; } = Color.White;
        public static Color DarkBackControlColor { get; } = Color.FromArgb(45, 45, 45);
        public static Color DarkAlternateColor { get; } = Color.FromArgb(70, 70, 70); // Örnek zebra rengi
        public static Color DarkButtonActive { get; } = Color.DodgerBlue;

        // Selected / Highlight
        public static Color HighlightColor { get; } = Color.Yellow;
        public static Color SelectedHighlightColor { get; } = Color.Gold;

        public static Color GetBackColor() =>
         ThemeHelper.GetTheme() == ThemeStyle.Dark ? DarkBackColor : LightBackColor;
        public static Color GetForeColor() =>
                ThemeHelper.GetTheme() == ThemeStyle.Dark ? DarkForeColor : LightForeColor;
        public static Color GetAlternateColor() =>
              ThemeHelper.GetTheme() == ThemeStyle.Dark ? DarkAlternateColor : LightAlternateColor;
        public static Color GetBackControlColor() =>
             ThemeHelper.GetTheme() == ThemeStyle.Dark ? DarkBackControlColor : LightBackColor;
        public static Color GetButtonActiveColor() =>
               ThemeHelper.GetTheme() == ThemeStyle.Dark ? DarkButtonActive : LightButtonActive;
        public static Color GetHighlightColor(bool selected) =>
            selected ? SelectedHighlightColor : HighlightColor;
    }
}
