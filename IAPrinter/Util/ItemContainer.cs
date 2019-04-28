// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAPrinter.Util
{

	public class ItemContainer
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ItemContainer));

		private Dictionary<ItemsEnum, Dictionary<RarityEnum, List<PlayerItem>>> ItemsDict;
		private HashSet<RarityEnum> containedRarities = new HashSet<RarityEnum>();
		private HashSet<ItemsEnum> containedItemTypes = new HashSet<ItemsEnum>();

		public ItemContainer(IEnumerable<PlayerItem> input)
		{
			ItemsDict = new Dictionary<ItemsEnum, Dictionary<RarityEnum, List<PlayerItem>>>();
			foreach (var item in input)
			{
				AddIntoContainer(item);
			}
		}

		public void AddIntoContainer(PlayerItem item)
		{
			try
			{
				ItemsEnum itemType = AnalyzeBaseRecord(item.baserecord);
				RarityEnum rarity = RarityEnumHelpers.StringToRarityEnum(item.Rarity);
				containedItemTypes.Add(itemType);
				containedRarities.Add(rarity);
				if (!ItemsDict.ContainsKey(itemType))
					ItemsDict.Add(itemType, new Dictionary<RarityEnum, List<PlayerItem>>());
				if (!ItemsDict[itemType].ContainsKey(rarity))
					ItemsDict[itemType].Add(rarity, new List<PlayerItem>() { item });
				else
					ItemsDict[itemType][rarity].Add(item);
			}
			catch (Exception ex)
			{
				log.Error("Error while adding item to dict:", ex);
			}
		}

		public IEnumerable<RarityEnum> GetContainedRarities()
		{
			return containedRarities.ToList();
		}

		public IEnumerable<ItemsEnum> GetContainedItemTypes()
		{
			return containedItemTypes.ToList();
		}

		public IEnumerable<PlayerItem> Get(ItemsEnum? itemType = null, RarityEnum? rarity = null)
		{
			List<PlayerItem> result = new List<PlayerItem>();
			List<Dictionary<RarityEnum, List<PlayerItem>>> rarityDictList = new List<Dictionary<RarityEnum, List<PlayerItem>>>();

			try
			{
				if (itemType == null)
				{
					rarityDictList = ItemsDict.Values.ToList();
				}
				else
				{
					Dictionary<RarityEnum, List<PlayerItem>> temp;
					if (ItemsDict.TryGetValue(itemType.Value, out temp))
						rarityDictList.Add(temp);
				}

				foreach (var rarityDict in rarityDictList)
				{
					if (rarity == null)
					{
						foreach (var itemList in rarityDict.Values)
						{
							result.AddRange(itemList);
						}
					}
					else
					{
						List<PlayerItem> temp;
						if (rarityDict.TryGetValue(rarity.Value, out temp))
							result.AddRange(temp);
					}
				}
			}
			catch (Exception ex)
			{
				log.Error("Error while getting items", ex);
			}

			return result;
		}

		private ItemsEnum AnalyzeBaseRecord(string baserecord)
		{
			if (String.IsNullOrWhiteSpace(baserecord))
				return ItemsEnum.Other;
			string[] splited = baserecord.Split('/');
			string last = splited.Last();
			string group = splited[splited.Length - 2];
			ItemsEnum result = ItemsEnum.Other;
			switch (group)
			{
				case "necklaces":
					result = ItemsEnum.Necklace;
					break;
				case "rings":
					result = ItemsEnum.Ring;
					break;
				case "medals":
					result = ItemsEnum.Medal;	
					break;
				case "waist":
					result = ItemsEnum.Belt;	
					break;
				case "gearrelic":
					result = ItemsEnum.Relic;
					break;
				case "gearhead":
					result = ItemsEnum.Helm;
					break;
				case "gearshoulders":
					result = ItemsEnum.Shoulders;
					break;
				case "gearhands":
					result = ItemsEnum.Gloves;
					break;
				case "geartorso":
					result = ItemsEnum.Chest;
					break;
				case "gearlegs":
					result = ItemsEnum.Pants;
					break;
				case "gearfeet":
					result = ItemsEnum.Boots;
					break;
				case "swords1h":
					result = ItemsEnum.Sword1H;
					break;
				case "axe1h":
					result = ItemsEnum.Axe1H;
					break;
				case "blunt1h":
					result = ItemsEnum.Mace1H;
					break;
				case "guns1h":
					result = ItemsEnum.Ranged1H;
					break;
				case "guns2h":
					result = ItemsEnum.Ranged2H;
					break;
				case "shields":
					result = ItemsEnum.Shield;
					break;
				case "focus":
					result = ItemsEnum.Offhand;
					break;
				case "caster":
					if (last.Contains("dagger"))
						result = ItemsEnum.Dagger;
					else if (last.Contains("scepter"))
						result = ItemsEnum.Scepter;
					else
						result = ItemsEnum.Caster;
					break;
				case "melee2h":
					if (last.Contains("axe2h"))
						result = ItemsEnum.Axe2H;
					else if (last.Contains("sword2h"))
						result = ItemsEnum.Sword2H;
					else if (last.Contains("blunt2h"))
						result = ItemsEnum.Mace2H;
					else
						result = ItemsEnum.Melee2H;
					break;
				default:
					result = ItemsEnum.Other;
					break;
			}
			return result;
		}
	}
}
