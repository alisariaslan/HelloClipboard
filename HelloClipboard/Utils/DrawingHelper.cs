using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HelloClipboard.Utils
{

	public static class DrawingHelper
	{

		private static readonly Color ZebraOddRowColor = Color.FromArgb(245, 245, 245); // Açık gri
		private static readonly Color ZebraEvenRowColor = Color.White; // Beyaz (Liste kutusu rengi de olabilir)

		public static void RenderClipboardItem(
	DrawItemEventArgs e,
	ListBox listBox,
	string searchTerm,
	MainFormViewModel viewModel)
		{

			bool selectedd = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
			Color backColor;

			if (selectedd)
			{
				// Seçili öğe için varsayılan vurgu rengi
				backColor = SystemColors.Highlight;
			}
			else
			{
				// Index'e göre zebra rengini seç
				if (e.Index % 2 == 0) // Çift indeksler (0, 2, 4...)
				{
					backColor = ZebraEvenRowColor;
				}
				else // Tek indeksler (1, 3, 5...)
				{
					backColor = ZebraOddRowColor;
				}
			}

			// Arka planı çiz
			using (SolidBrush brush = new SolidBrush(backColor))
			{
				e.Graphics.FillRectangle(brush, e.Bounds);
			}

			if (e.Index < 0 || e.Index >= listBox.Items.Count)
				return;

			var item = listBox.Items[e.Index] as ClipboardItem;
			if (item == null)
				return;

			bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
			Color textColor = selected ? SystemColors.HighlightText : SystemColors.ControlText;
			var bounds = e.Bounds;

			// --- METİN OLUŞTURMA BAŞLANGICI ---
			string displayText = item.Title ?? string.Empty;

			// 1. Önce Zaman Damgasını ekleyelim (Eğer ayar aktifse)
			if (SettingsLoader.Current.EnableTimeStamps)
			{
				displayText = $"[{item.Timestamp:HH:mm:ss}] " + displayText;
			}

			// 2. Sonra Pin bilgisini ekleyelim
			if (item.IsPinned)
				displayText = "[PIN] " + displayText;
			// --- METİN OLUŞTURMA SONU ---

			DrawingHelper.DrawTextWithHighlight(
				e.Graphics,
				displayText,
				e.Font,
				textColor,
				bounds,
				selected,
				searchTerm,
				viewModel.GetHighlightRegex(searchTerm),
				viewModel.CaseSensitive);

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

			// If there is no search term, perform normal drawing
			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				TextRenderer.DrawText(g, text, font, bounds, textColor, format);
				return;
			}

			List<(string part, bool highlight)> parts = new List<(string, bool)>();

			// If a Regex is provided, split the text using Regex
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
			// If no Regex is provided, split using standard string IndexOf
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

			// Draw the parts side-by-side
			int x = bounds.Left;
			foreach (var (part, highlight) in parts)
			{
				if (string.IsNullOrEmpty(part))
					continue;

				var size = TextRenderer.MeasureText(g, part, font, new Size(int.MaxValue, int.MaxValue), format);
				var rect = new Rectangle(x, bounds.Top, size.Width, bounds.Height);

				if (highlight)
				{
					// Use Gold for selected items and Yellow for unselected items for better visibility
					Color back = selected ? Color.Gold : Color.Yellow;
					using (var brush = new SolidBrush(back))
					{
						g.FillRectangle(brush, rect);
					}
				}

				TextRenderer.DrawText(g, part, font, rect, textColor, format);
				x += size.Width;

				// Stop drawing if the text exceeds bounds (for performance and Ellipsis handling)
				if (x > bounds.Right)
					break;
			}
		}
	}
}