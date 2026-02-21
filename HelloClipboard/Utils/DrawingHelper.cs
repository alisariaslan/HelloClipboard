using HelloClipboard.Constants;
using HelloClipboard.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HelloClipboard.Utils
{
    public static class DrawingHelper
    {
        private static Color GetZebraColor(int index)
        {
            return (index % 2 == 0) ? AppColors.GetBackColor() : AppColors.GetAlternateColor();
        }

        public static void RenderClipboardItem(
            DrawItemEventArgs e,
            ListBox listBox,
            string searchTerm,
            MainFormViewModel viewModel)
        {
            if (e.Index < 0 || e.Index >= listBox.Items.Count) return;

            var item = listBox.Items[e.Index] as ClipboardItem;
            if (item == null) return;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;


            // Arka plan rengi
            Color backColor = selected ? AppColors.GetSelectionColor() : GetZebraColor(e.Index);
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            // Metin rengi
            Color textColor = AppColors.GetForeColor();

            // Metin oluşturma
            string displayText = item.Title ?? string.Empty;
            if (SettingsLoader.Current.EnableTimeStamps)
                displayText = $"[{item.Timestamp:HH:mm:ss}] " + displayText;
            if (item.IsPinned)
                displayText = "[PIN] " + displayText;
            if (item.Tags != null && item.Tags.Count > 0)
                displayText = $"[{string.Join(",", item.Tags)}] " + displayText;

            // Metni çiz
            DrawTextWithHighlight(
                e.Graphics,
                displayText,
                e.Font,
                textColor,
                e.Bounds,
                selected,
                searchTerm,
                viewModel.GetHighlightRegex(searchTerm),
                viewModel.CaseSensitive);

            e.DrawFocusRectangle();
        }

        public static void RenderSnippetItem(DrawItemEventArgs e, ListBox listBox)
        {
            if (e.Index < 0 || e.Index >= listBox.Items.Count) return;
            var item = listBox.Items[e.Index] as SnippetItem;
            if (item == null) return;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = selected ? AppColors.GetSelectionColor() : GetZebraColor(e.Index);
            using (SolidBrush brush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(brush, e.Bounds);

            string displayText = item.Name ?? "Untitled";
            if (item.Tags != null && item.Tags.Count > 0)
                displayText = $"[{string.Join(",", item.Tags)}] " + displayText;

            var format = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            TextRenderer.DrawText(e.Graphics, displayText, e.Font, e.Bounds, AppColors.GetForeColor(), format);
            e.DrawFocusRectangle();
        }

        public static void DrawTextWithHighlight(
            Graphics g,
            string text,
            Font font,
            Color textColor,
            Rectangle bounds,
            bool selected,
            string searchTerm,
            Regex highlightRegex,
            bool caseSensitive)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var format = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                TextRenderer.DrawText(g, text, font, bounds, textColor, format);
                return;
            }

            List<(string part, bool highlight)> parts = new List<(string, bool)>();

            if (highlightRegex != null)
            {
                int lastIndex = 0;
                foreach (Match m in highlightRegex.Matches(text))
                {
                    if (m.Index > lastIndex)
                        parts.Add((text.Substring(lastIndex, m.Index - lastIndex), false));
                    parts.Add((text.Substring(m.Index, m.Length), true));
                    lastIndex = m.Index + m.Length;
                }
                if (lastIndex < text.Length)
                    parts.Add((text.Substring(lastIndex), false));
            }
            else
            {
                StringComparison comp = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                int start = 0;
                while (true)
                {
                    int idx = text.IndexOf(searchTerm, start, comp);
                    if (idx < 0)
                    {
                        parts.Add((text.Substring(start), false));
                        break;
                    }
                    if (idx > start)
                        parts.Add((text.Substring(start, idx - start), false));
                    parts.Add((text.Substring(idx, searchTerm.Length), true));
                    start = idx + searchTerm.Length;
                    if (start >= text.Length) break;
                }
            }

            int x = bounds.Left;
            foreach (var (part, highlight) in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                var size = TextRenderer.MeasureText(g, part, font, new Size(int.MaxValue, int.MaxValue), format);
                var rect = new Rectangle(x, bounds.Top, size.Width, bounds.Height);

                if (highlight)
                {
                    Color highlightBack = AppColors.GetHighlightColor(selected);
                    using (var brush = new SolidBrush(highlightBack))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }

                TextRenderer.DrawText(g, part, font, rect, textColor, format);
                x += size.Width;

                if (x > bounds.Right) break;
            }
        }

    }
}
