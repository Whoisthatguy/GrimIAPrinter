// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

namespace IAPrinter.Util
{
	public enum RarityEnum
	{		
		Purple = 0,
		Green = 1,
		Blue = 2,
		White = 3
	}

	public static class RarityEnumHelpers
	{
		public static RarityEnum StringToRarityEnum(string input)
		{
			if (String.IsNullOrWhiteSpace(input))
				return RarityEnum.Purple;
			switch (input.ToLower())
			{
				case "purple":
					return RarityEnum.Purple;
				case "green":
					return RarityEnum.Green;
				case "blue":
					return RarityEnum.Blue;
				case "white":
					return RarityEnum.White;
				default:
					return RarityEnum.Purple;
			}
		}

		public static string RarityToTextColorString (RarityEnum rarity)
		{
			switch (rarity)
			{
				case RarityEnum.Purple:
					return "DarkOrchid";
				case RarityEnum.Green:
					return "Green";
				case RarityEnum.Blue:
					return "RoyalBlue";
				case RarityEnum.White:
					return ""; 
				default:
					return "";
			}
		}

		public static List<string> RarityToFilterList(this PrinterConfig config)
		{
			List<string> rarityFilter;
			switch (config.Rarity)
			{
				case RarityEnum.Purple:
					rarityFilter = new List<string>() { "Epic" };
					break;
				case RarityEnum.Green:
					rarityFilter = new List<string>() { "Epic", "Green" };
					break;
				case RarityEnum.Blue:
					rarityFilter = new List<string>() { "Epic", "Green", "Blue" };
					break;
				case RarityEnum.White:
					rarityFilter = null;
					break;
				default:
					rarityFilter = new List<string>() { "Epic" };
					break;
			}
			return rarityFilter;
		}
	}
}
