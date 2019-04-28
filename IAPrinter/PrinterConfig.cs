// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using IAPrinter.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAPrinter
{
	public class PrinterConfig
	{
		public RarityEnum Rarity { get; set; }
		public int ItemLevel { get; set; }
		public string OutputPath { get; set; }
		public FormatEnum Format { get; set; }
		public HashSet<string> ExcludedGreens { get; set; }

		public PrinterConfig()
		{
			Rarity = RarityEnumHelpers.StringToRarityEnum(System.Configuration.ConfigurationManager.AppSettings["Rarity"]);

			string itemLevelConfig = System.Configuration.ConfigurationManager.AppSettings["ItemLevel"];
			int tempILvl;
			if (Int32.TryParse(itemLevelConfig, out tempILvl))
				ItemLevel = tempILvl;
			else
				ItemLevel = 84; //fallback

			string outputPath = System.Configuration.ConfigurationManager.AppSettings["OutputPath"];
			OutputPath = !String.IsNullOrWhiteSpace(outputPath) ? outputPath : "";

			Format = FormatEnumHelpers.StringToFormatEnum(System.Configuration.ConfigurationManager.AppSettings["OutputFormat"]);
			ExcludedGreens = PrinterConfigHelpers.ProcessExcludeGreensInput(System.Configuration.ConfigurationManager.AppSettings["ExcludedGreens"]);
		}
	}

	public static class PrinterConfigHelpers
	{
		public static HashSet<string> ProcessExcludeGreensInput(string input)
		{
			if (String.IsNullOrWhiteSpace(input))
				return new HashSet<string>();
			string[] result = input.Trim().Split(',');
			for (int i = 0; i < result.Length; i++)
				result[i] = result[i].Trim().ToLower();
			return new HashSet<string>(result);
		}
	}
}
