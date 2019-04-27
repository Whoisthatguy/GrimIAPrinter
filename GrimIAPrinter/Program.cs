using IAPrinter;
using IAPrinter.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrimIAPrinter
{
	class Program
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
		private static readonly List<string> allowedRarities = new List<string>() { "white", "blue", "green", "purple" };

		static void Main(string[] args)
		{
			bool cmdLineInputsEnabled = true;
			if (!Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["CmdLineInputsEnabled"], out cmdLineInputsEnabled))
				log.Warn("Configuration cmdLineInputsEnabled ignored because it was not a valid boolean value. Has value to be either true or false");

			string dbPath = "";
			string dbPathPrefix = "-db:";		
			if(args != null && args.Length > 0 && args.Any(e => e.StartsWith(dbPathPrefix)))
			{
				
				var temp = args.FirstOrDefault(e => e.StartsWith(dbPathPrefix));
				if (!String.IsNullOrWhiteSpace(temp))
					dbPath = temp.Remove(0, dbPathPrefix.Length);				
			}
			else if (cmdLineInputsEnabled)
			{
				Console.WriteLine("Insert Item Assistant Db Path (enter if connection string hasn't changed):");
				Console.WriteLine("Example: C:\\Users\\Test\\AppData\\Local\\EvilSoft\\IAGD\\data\\userdata.db");
				dbPath = Console.ReadLine();
			}

			//init printer
			Printer printer = null;
			try
			{
				if (!String.IsNullOrWhiteSpace(dbPath))
					printer = new Printer(dbPath);
				else
					printer = new Printer();
			}
			catch (Exception ex)
			{
				
				log.Error("Error while initializing Printer. This is mostlikely due to an incorrect dbPath.", ex);
				Console.ReadLine();
				return;
			}
			PrinterConfig config;

			//fill config
			if (args != null && args.Length > 0)
				config = ConfigFromArgs(args);
			else if (cmdLineInputsEnabled)
				config = ConfigFromConsoleInput();
			else
				config = new PrinterConfig();
			
			if(config == null) //if ConfigFromArgs or Config FromConsoleInput returns null stop to let the user read the error
			{
				Console.ReadLine();
				return;
			}

			//print
			printer.Print(config);
			Console.ReadLine();
		}

		private static PrinterConfig ConfigFromArgs(string[] args)
		{
			/*
			-db:"C:\Users\Test\AppData\Local\EvilSoft\IAGD\data\userdataTest.db" -ilvl:"84" -r:"green" -ex:"Spectral Longsword, Troll Bonecrusher" -out:"C:\Users\Test\Desktop\GrimIAPrinterOutputa" -f:"googlesheets"  
			*/
			PrinterConfig config = new PrinterConfig();

			string ilvlPrefix = "-ilvl:";
			string ilvlInput = args.FirstOrDefault(e => e.StartsWith(ilvlPrefix));
			if (!String.IsNullOrWhiteSpace(ilvlInput))
			{
				string ilvlValue = ilvlInput.Remove(0, ilvlPrefix.Length);
				if (!String.IsNullOrWhiteSpace(ilvlValue))
				{
					int temp;
					if (Int32.TryParse(ilvlValue, out temp))
						config.ItemLevel = temp;
					else
						Console.WriteLine("Itemlevel input has to be an integer value. Input ignored.");
				}
			}

			string rarityPrefix = "-r:";
			string rarityInput = args.FirstOrDefault(e => e.StartsWith(rarityPrefix));
			if (!String.IsNullOrWhiteSpace(rarityInput))
			{
				string rarityValue = rarityInput.Remove(0, rarityPrefix.Length);
				if (!String.IsNullOrWhiteSpace(rarityValue) && allowedRarities.Contains(rarityValue))
					config.Rarity = RarityEnumHelpers.StringToRarityEnum(rarityValue);
				else
				{
					Console.WriteLine("Rarity input ignored. Value wasnt a valid rarity.");
					Console.Write("Valid rarities are: ");
					foreach (var rarity in allowedRarities)
						Console.Write($"{rarity} ");
					Console.WriteLine();
				}
			}

			string exclusionPrefix = "-ex:";
			string exclusionInput = args.FirstOrDefault(e => e.StartsWith(exclusionPrefix));
			if (!String.IsNullOrWhiteSpace(exclusionInput) && config.Rarity != RarityEnum.Purple)
			{
				string exclusionValue = exclusionInput.Remove(0, exclusionPrefix.Length);
				if (!String.IsNullOrWhiteSpace(exclusionValue))
					config.ExcludedGreens.UnionWith(PrinterConfigHelpers.ProcessExcludeGreensInput(exclusionValue));
			}
				

			string outDirPrefix = "-out:";
			string outDirInput = args.FirstOrDefault(e => e.StartsWith(outDirPrefix));
			if (!String.IsNullOrWhiteSpace(outDirInput))
			{
				string outDirValue = outDirInput.Remove(0, outDirPrefix.Length);
				if (!String.IsNullOrWhiteSpace(outDirValue))
					config.OutputPath = outDirValue;
			}

			string formatPrefix = "-f:";
			string formatInput = args.FirstOrDefault(e => e.StartsWith(formatPrefix));
			if (!String.IsNullOrWhiteSpace(formatInput))
			{
				string formatValue = formatInput.Remove(0, formatPrefix.Length);
				if (!String.IsNullOrWhiteSpace(formatValue))
					config.Format = FormatEnumHelpers.StringToFormatEnum(formatValue);
			}

			return config;
		}

		private static PrinterConfig ConfigFromConsoleInput()
		{
			PrinterConfig config = new PrinterConfig();
			Console.WriteLine("Set ItemLevel (enter for defaultvalue 84):");
			string itemLevelString = Console.ReadLine();
			int itemLevel = -1;
			if (!String.IsNullOrWhiteSpace(itemLevelString) && !Int32.TryParse(itemLevelString, out itemLevel))
			{
				Console.WriteLine("ItemLevel has to be an integer.");
				return null;
			}
			if (itemLevel >= 0)
				config.ItemLevel = itemLevel;

			Console.WriteLine("Set rarity all rarities below selected rarity will not be shown.");
			Console.WriteLine("Rarityorder: white < blue < green < purple (enter for defaultvalue purple)");
			string rarity = Console.ReadLine();
			if (!String.IsNullOrWhiteSpace(rarity) && !allowedRarities.Contains(rarity.ToLower()))
			{
				Console.WriteLine("Entered Rarity not valid.");
				return null;
			}
			RarityEnum rarityEnum = RarityEnumHelpers.StringToRarityEnum(rarity);
			if (!String.IsNullOrWhiteSpace(rarity))
				config.Rarity = rarityEnum;

			HashSet<string> excludedGreens = new HashSet<string>();
			if (rarityEnum != RarityEnum.Purple)
			{
				Console.WriteLine("Exclude specific items (case insensitive) (multiple items seperated by , ) (enter for no filtering):");
				Console.WriteLine("Example: Spectral Longsword, Troll Bonecrusher");
				var temp = Console.ReadLine();
				if (!String.IsNullOrWhiteSpace(temp))				
					excludedGreens = PrinterConfigHelpers.ProcessExcludeGreensInput(temp);
				config.ExcludedGreens.UnionWith(excludedGreens);
			}			

			Console.WriteLine("Output Path without filename (defaultpath = Desktop\\GrimIAPrinterOutput):");
			Console.WriteLine("Example: C:\\Users\\Test\\Desktop\\");
			string outputPath = Console.ReadLine();
			if (!String.IsNullOrWhiteSpace(outputPath))
				config.OutputPath = outputPath;

			Console.WriteLine("Enter Outputformat (enter for default = plaintext)");
			Console.Write("Options: ");
			bool firstOption = true;
			foreach (var option in Enum.GetNames(typeof(FormatEnum)))
			{
				if (!firstOption)
					Console.Write(", ");
				else
					firstOption = false;
				Console.Write(option);
			}
			Console.WriteLine();
			string formatOption = Console.ReadLine();
			if (!String.IsNullOrWhiteSpace(formatOption))
				config.Format = FormatEnumHelpers.StringToFormatEnum(formatOption);

			return config;
		}

	}
}
