// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace IAPrinter.Util
{
	public enum FormatEnum
	{
		PlainText = 0,
		GoogleSheets = 1,
		Html = 2,
		Forum = 3
	}

	public static class FormatEnumHelpers
	{
		public static FormatEnum StringToFormatEnum(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return FormatEnum.PlainText;
			switch (input.Trim().ToLower())
			{
				case "googlesheets":
					return FormatEnum.GoogleSheets;
				case "html":
					return FormatEnum.Html;
				case "forum":
					return FormatEnum.Forum;
				case "plaintext":
				default:
					return FormatEnum.PlainText;
			}
		}
	}
}
