// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using IAPrinter.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace IAPrinter
{
	public class Printer
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Printer));
		private static readonly string connectionStringName = "IADbContext";

		public Printer() { }

		public Printer(string dbPath)
		{
			if (File.Exists(dbPath) && dbPath.EndsWith(".db"))
			{
				string newConnectionString = ConnectionTools.ModifyDatabaseConnectionString(connectionStringName, dataSource: dbPath);
				string oldConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
				if (oldConnectionString == newConnectionString)
					return;
				ConnectionTools.ChangeConnectionString(newConnectionString, connectionStringName);
				using (IADbContext dbContext = new IADbContext())
				{
					if (!dbContext.Database.Exists())
					{
						ConnectionTools.ChangeConnectionString(oldConnectionString, connectionStringName);
						throw new FileNotFoundException();
					}
				}
			}
			else
			{
				throw new FileNotFoundException();
			}
		}

		public string Print(PrinterConfig config = null)
		{
			if (config == null)
				config = new PrinterConfig();
			using (IADbContext dbContext = new IADbContext())
			{
				if (!dbContext.Database.Exists())
				{
					log.Warn("Db not found.");
					return "";
				}
				List<PlayerItem> itemResult = new List<PlayerItem>();
				try
				{
					IQueryable<PlayerItem> items = dbContext.PlayerItem;
					items = FilterItems(items, config, dbContext);
					itemResult = items.ToList();
					log.Info("LoadedPlayerItems");
				}
				catch (Exception ex)
				{
					log.Error("Error fetching playeritems. Most likely path to db is incorrect.", ex);
					return "";
				}
				log.Info("Formatting");
				ItemContainer container = new ItemContainer(itemResult);
				string stringResult = ResultFormatter.FormatResult(container, config);
				log.Info("Writing output");
				CreateOutputFile(stringResult, config);
				log.Info("Printer Finished");
				return stringResult;
			}
		}

		private void CreateOutputFile(string stringResult, PrinterConfig config)
		{
			string outputPath = "";
			if (!String.IsNullOrWhiteSpace(config.OutputPath))
				outputPath = config.OutputPath;
			else
				outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GrimIAPrinterOutput");
			string fileExtension = config.Format == FormatEnum.Html ? ".html" : ".txt";
			string fileName = $"GrimIAPrinterResult_{DateTime.Now.ToString("dd-MM-yy_HH_mm_ss")}{fileExtension}";
			string fullPath;
			try
			{
				fullPath = Path.Combine(outputPath, fileName);
			}
			catch (Exception ex)
			{
				log.Error($"Couldn't create outputpath, check if your -out: path contains any chars that are not allowed.{System.Environment.NewLine}Outpath was: {outputPath}", ex);
				return;
			}
			try
			{
				if (!Directory.Exists(outputPath))
					Directory.CreateDirectory(outputPath);
			}
			catch (Exception ex)
			{
				log.Error("Couldn't create output directory. This is most likely due to missing admin rights.", ex);
				return;
			}
			try
			{
				File.WriteAllText(fullPath, stringResult);
			}
			catch (Exception ex)
			{
				log.Error("Couldn't write output file. This is most likely because the selected path needs admin rights.", ex);
				return;
			}
			log.Info($"Output Dir: {outputPath}");
			log.Info($"FileName: {fileName}");
		}

		private IQueryable<PlayerItem> FilterItems(IQueryable<PlayerItem> items, PrinterConfig config, IADbContext dbContext)
		{
			items = items.Where(e => e.LevelRequirement >= config.ItemLevel);
			List<string> rarityFilter = config.RarityToFilterList();
			if (rarityFilter != null)
				items = items.Where(e => rarityFilter.Contains(e.Rarity) && e.Rarity != "Unknown");
			if (config.Rarity != RarityEnum.Purple && config.ExcludedGreens != null && config.ExcludedGreens.Count > 0)
			{
				var excludedItemIds = dbContext.PlayerItem.Where(e => config.ExcludedGreens.Any(x => e.namelowercase.Contains(x)) && e.Rarity == "Green").Select(e => e.Id);
				items = items.Where(e => !excludedItemIds.Contains(e.Id));
			}				
			return items;
		}
	}
}
