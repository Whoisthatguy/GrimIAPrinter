// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace IAPrinter.Util
{
	public static class ResultFormatter
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ResultFormatter));

		public static string FormatResult(ItemContainer container, PrinterConfig config)
		{
			try
			{
				switch (config.Format)
				{
					case FormatEnum.GoogleSheets:
						return FormatResultSheets(container);
					case FormatEnum.Html:
						return FormatResultHtml(container);
					case FormatEnum.Forum:
						return FormatResultForum(container);
					case FormatEnum.PlainText:
					default:
						return FormatResultPlainText(container);
				}
			}
			catch (Exception ex)
			{
				log.Error("Error while formatting results:", ex);
			}
			return "";
		}

		private static string FormatResultSheets(ItemContainer container)
		{
			StringBuilder result = new StringBuilder();
			foreach (var rarity in container.GetContainedRarities().OrderBy(e => e))
			{
				bool rarityFirst = true;
				foreach (var itemType in container.GetContainedItemTypes().OrderBy(e => e))
				{
					bool itemTypeFirst = true;
					foreach (var item in container.Get(itemType, rarity).GroupBy(e => e.Name).OrderBy(e => e.Key))
					{
						if (rarityFirst)
						{
							result.AppendLine($"{rarity.ToString()}:");
							result.AppendLine();
							rarityFirst = false;
						}
						if (itemTypeFirst)
						{
							result.AppendLine($"{itemType.ToString()}:");
							result.AppendLine();
							itemTypeFirst = false;
						}
						string multipleString = item.Count() > 1 ? $" x{item.Count()}" : "";
						result.Append($"=HYPERLINK(\"{BuildGrimToolsItemUrl(item.Key)}\"; \"{item.Key}{multipleString}\")");
						result.AppendLine();
					}
					if (!itemTypeFirst)
						result.AppendLine();
				}
				if (!rarityFirst)
					result.AppendLine();
			}
			return result.ToString();
		}

		private static string FormatResultPlainText(ItemContainer container)
		{
			StringBuilder result = new StringBuilder();
			foreach (var rarity in container.GetContainedRarities().OrderBy(e => e))
			{
				bool rarityFirst = true;
				foreach (var itemType in container.GetContainedItemTypes().OrderBy(e => e))
				{
					bool itemTypeFirst = true;
					foreach (var item in container.Get(itemType, rarity).GroupBy(e => e.Name).OrderBy(e => e.Key))
					{
						if (rarityFirst)
						{
							result.AppendLine($"{rarity.ToString()}:");
							result.AppendLine();
							rarityFirst = false;
						}
						if (itemTypeFirst)
						{
							result.AppendLine($"{itemType.ToString()}:");
							result.AppendLine();
							itemTypeFirst = false;
						}
						result.Append(item.Key);
						if (item.Count() > 1)
							result.Append($" x{item.Count()}");
						result.AppendLine();
					}
					if (!itemTypeFirst)
						result.AppendLine();
				}
				if (!rarityFirst)
					result.AppendLine();
			}
			return result.ToString();
		}

		private static string FormatResultForum(ItemContainer container)
		{
			StringBuilder result = new StringBuilder();
			string textColor = "";
			foreach (var rarity in container.GetContainedRarities().OrderBy(e => e))
			{
				textColor = RarityEnumHelpers.RarityToTextColorString(rarity);
				bool rarityFirst = true;
				foreach (var itemType in container.GetContainedItemTypes().OrderBy(e => e))
				{
					bool itemTypeFirst = true;
					foreach (var item in container.Get(itemType, rarity).GroupBy(e => e.Name).OrderBy(e => e.Key))
					{
						if (rarityFirst)
						{
							result.AppendLine($"[SIZE=\"3\"]{ColorStringForum($"{rarity.ToString()}:", textColor)}[/SIZE]");
							result.AppendLine();
							rarityFirst = false;
						}
						if (itemTypeFirst)
						{
							result.AppendLine(ColorStringForum($"{itemType.ToString()}:", textColor));
							itemTypeFirst = false;
							result.AppendLine("[SPOILER2]");
						}
						result.Append($"[URL=\"{BuildGrimToolsItemUrl(item.Key)}\"]{ColorStringForum(item.Key, textColor)}[/URL]");
						if (item.Count() > 1)
							result.Append($" x{item.Count()}");
						result.AppendLine();
					}
					if (!itemTypeFirst)
					{
						result.AppendLine("[/SPOILER2]");
						result.AppendLine();
					}
				}
				if (!rarityFirst)
				{
					result.AppendLine();
				}

			}
			return result.ToString();
		}

		private static string FormatResultHtml(ItemContainer container)
		{
			StringWriter stringWriter = new StringWriter();
			using (var result = new HtmlTextWriter(stringWriter))
			{
				result.AddAttribute(HtmlTextWriterAttribute.Class, "grimIAPrinterResultHtml");
				result.RenderBeginTag(HtmlTextWriterTag.Html);
				result.RenderBeginTag(HtmlTextWriterTag.Head);
				result.RenderBeginTag(HtmlTextWriterTag.Style);
				GenerateCssStyle(result);
				result.RenderEndTag(); //style
				result.RenderEndTag(); //head
				result.WriteLine();

				result.AddAttribute(HtmlTextWriterAttribute.Class, "grimIAPrinterResultBody");
				result.RenderBeginTag(HtmlTextWriterTag.Body);
				result.AddAttribute(HtmlTextWriterAttribute.Class, "grimIAPrinterResult");
				result.RenderBeginTag(HtmlTextWriterTag.Div);
				foreach (var rarity in container.GetContainedRarities().OrderBy(e => e))
				{
					bool rarityFirst = true;
					foreach (var itemType in container.GetContainedItemTypes().OrderBy(e => e))
					{
						bool itemTypeFirst = true;
						foreach (var item in container.Get(itemType, rarity).GroupBy(e => e.Name).OrderBy(e => e.Key))
						{
							if (!itemTypeFirst) //fix html line breaks
								result.WriteLine();
							else if (!rarityFirst)
								result.WriteLine();

							if (rarityFirst)
							{
								result.AddAttribute(HtmlTextWriterAttribute.Class, $"rarity rarity{rarity.ToString()}");							
								result.RenderBeginTag(HtmlTextWriterTag.Div);
								result.RenderBeginTag(HtmlTextWriterTag.H3);
								result.Write(rarity.ToString());
								result.RenderEndTag(); // h3
								result.WriteLine();
								rarityFirst = false;
							}
							if (itemTypeFirst)
							{
								result.AddAttribute(HtmlTextWriterAttribute.Class, $"itemType itemType{itemType.ToString()}");
								result.RenderBeginTag(HtmlTextWriterTag.Div);
								result.RenderBeginTag(HtmlTextWriterTag.H4);
								result.Write(itemType.ToString());
								result.RenderEndTag(); // h4
								result.WriteLine();
								result.AddAttribute(HtmlTextWriterAttribute.Class, "itemList");
								result.RenderBeginTag(HtmlTextWriterTag.Ul);
								itemTypeFirst = false;
							}
							result.AddAttribute(HtmlTextWriterAttribute.Class, "itemListItem");
							result.RenderBeginTag(HtmlTextWriterTag.Li);
							result.AddAttribute(HtmlTextWriterAttribute.Class, $"itemLink rarity{rarity.ToString()}");
							result.AddAttribute(HtmlTextWriterAttribute.Href, BuildGrimToolsItemUrl(item.Key));
							result.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
							result.RenderBeginTag(HtmlTextWriterTag.A);
							result.Write(item.Key);
							if (item.Count() > 1)
								result.Write($" x{item.Count()}");
							result.RenderEndTag(); // a
							result.RenderEndTag(); // li
						}
						if (!itemTypeFirst)
						{
							result.RenderEndTag(); // ul;
							result.RenderEndTag(); // div itemType
						}
					}
					if (!rarityFirst)
						result.RenderEndTag(); // div rarity
				}
				result.RenderEndTag(); //div grimIAPrinterResult
				result.RenderEndTag(); //body
				result.RenderEndTag(); //html

				return result.InnerWriter.ToString();
			}
		}

		private static void GenerateCssStyle(HtmlTextWriter writer)
		{
			string purple = System.Configuration.ConfigurationManager.AppSettings["ColorPurpleItems"];
			if (String.IsNullOrWhiteSpace(purple))
				purple = RarityEnumHelpers.RarityToTextColorString(RarityEnum.Purple);
			string green = System.Configuration.ConfigurationManager.AppSettings["ColorGreenItems"];
			if (String.IsNullOrWhiteSpace(green))
				green = RarityEnumHelpers.RarityToTextColorString(RarityEnum.Green);
			string blue = System.Configuration.ConfigurationManager.AppSettings["ColorBlueItems"];
			if (String.IsNullOrWhiteSpace(blue))
				blue = RarityEnumHelpers.RarityToTextColorString(RarityEnum.Blue);
			string white = System.Configuration.ConfigurationManager.AppSettings["ColorWhiteItems"];
			if (String.IsNullOrWhiteSpace(white))
				white = RarityEnumHelpers.RarityToTextColorString(RarityEnum.White);
			string backgroundColor = System.Configuration.ConfigurationManager.AppSettings["ColorBackground"];
			if (String.IsNullOrWhiteSpace(backgroundColor))
				backgroundColor = "#ccc";
			
			int itemLineHeight = 17;
			Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["ItemLineHeight"], out itemLineHeight);
			int hoverSpace = 5;
			Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["HoverSpace"], out hoverSpace);
			int itemTypeColumnWidth = 300;
			Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["ItemTypeColumnWidth"], out itemTypeColumnWidth);

			writer.WriteLine($".grimIAPrinterResultHtml {{ background-color: {backgroundColor}; }}");
			writer.WriteLine(".grimIAPrinterResultBody { margin: 0px; }");
			writer.WriteLine($".grimIAPrinterResult {{ font-family: monospace; background-color: {backgroundColor}; }}");
			writer.WriteLine(".grimIAPrinterResult h4 { margin-block-start: 0px; }");
			writer.WriteLine(".grimIAPrinterResult .rarity { margin:10px; }");
			writer.WriteLine(".grimIAPrinterResult .itemList { padding-inline-start: 0px; list-style: none; }");
			writer.WriteLine($".grimIAPrinterResult .itemType {{ display: inline-block; vertical-align: top; margin-left: 10px; margin-bottom: 30px; width: {itemTypeColumnWidth}px; }}");
			writer.WriteLine($".grimIAPrinterResult .itemType li {{ height: {itemLineHeight}px; }}");
			writer.WriteLine(".grimIAPrinterResult .itemLink { overflow: hidden; white-space: nowrap; text-overflow: ellipsis; display: block; }");
			writer.WriteLine($".grimIAPrinterResult .itemLink:hover {{ overflow: visible; white-space: normal; width: auto; position: absolute; display: inline; min-width: {itemTypeColumnWidth}px; padding-right: {hoverSpace}px; background-color: {backgroundColor}; }}");
			writer.WriteLine($".grimIAPrinterResult .itemLink:hover+li {{ margin-top: {itemLineHeight}px; }}");
			writer.WriteLine($".grimIAPrinterResult .rarityPurple {{ color: {purple}; }}");
			writer.WriteLine($".grimIAPrinterResult .rarityGreen {{ color: {green}; }}");
			writer.WriteLine($".grimIAPrinterResult .rarityBlue {{ color: {blue}; }}");
			writer.Write($".grimIAPrinterResult .rarityWhite {{ color: {white}; }}"); //last css entry no new line to avoid ugly empty line in style
		}

		private static string ColorStringForum(string input, string textColor)
		{
			if (String.IsNullOrWhiteSpace(textColor) || textColor == "default")
				return input;
			else
				return $"[COLOR=\"{textColor}\"]{input}[/COLOR]";
		}

		private static string BuildGrimToolsItemUrl(PlayerItem item)
		{
			if (item == null)
				return "";
			return BuildGrimToolsItemUrl(item.Name);
		}

		private static string BuildGrimToolsItemUrl(string itemName)
		{
			string value = HttpUtility.HtmlDecode(System.Configuration.ConfigurationManager.AppSettings["GrimToolsItemBaseUrl"]);
			if (String.IsNullOrWhiteSpace(value))
				return "";
			return value.Replace("[ITEMNAME]", HttpUtility.UrlEncode(itemName));
		}
	}
}
