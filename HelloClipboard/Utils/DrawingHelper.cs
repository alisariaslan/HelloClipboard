using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HelloClipboard.Utils
{
	public static class DrawingHelper
	{
		public static void RenderClipboardItem(
	DrawItemEventArgs e,
	ListBox listBox,
	string searchTerm,
	MainFormViewModel viewModel)
		{
			e.DrawBackground();
			if (e.Index < 0 || e.Index >= listBox.Items.Count)
				return;

			var item = listBox.Items[e.Index] as ClipboardItem;
			if (item == null)
				return;

			bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
			Color textColor = selected ? SystemColors.HighlightText : SystemColors.ControlText;
			var bounds = e.Bounds;

			string displayText = item.Title ?? string.Empty;
			if (item.IsPinned)
				displayText = "[PIN] " + displayText;

			DrawingHelper.DrawTextWithHighlight(e.Graphics, displayText, e.Font, textColor, bounds, selected, searchTerm, viewModel.GetHighlightRegex(searchTerm), viewModel.CaseSensitive);

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

			// Arama terimi yoksa normal çizim yap
			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				TextRenderer.DrawText(g, text, font, bounds, textColor, format);
				return;
			}

			List<(string part, bool highlight)> parts = new List<(string, bool)>();

			// Regex varsa Regex ile parçala
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
			// Regex yoksa normal string IndexOf ile parçala
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
					if (start >= text.Length)
						break;
				}
			}

			// Parçaları yan yana çiz
			int x = bounds.Left;
			foreach (var (part, highlight) in parts)
			{
				if (string.IsNullOrEmpty(part))
					continue;

				var size = TextRenderer.MeasureText(g, part, font, new Size(int.MaxValue, int.MaxValue), format);
				var rect = new Rectangle(x, bounds.Top, size.Width, bounds.Height);

				if (highlight)
				{
					Color back = selected ? Color.Gold : Color.Yellow;
					using (var brush = new SolidBrush(back))
					{
						g.FillRectangle(brush, rect);
					}
				}

				TextRenderer.DrawText(g, part, font, rect, textColor, format);
				x += size.Width;

				// Sınır dışına çıktıysa daha fazla çizme (Performans ve Ellipsis için)
				if (x > bounds.Right)
					break;
			}
		}
	}
}