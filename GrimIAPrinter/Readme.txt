GrimIAPrinter (because typing googlesheets sucks)

This console app prints items that are saved into the sqlite database of the GD Item Assistant.
This means the app will NOT print items that are in your normal Grim Dawn stash.

Usage:
Either use the comandline interface or set your desired settings as start parameters or write your settings into GrimIAPrinter.exe.config (recommended because its faster and your settings will probably stay the same).
The commandline interface will only show up if you start the app with 0 startparameters and if it is enabled in the config (enabled by default). 
The GrimIAPrinter.exe.config will be in the installation directory. Default should be: C:\Program Files (x86)\GrimIAPrinter

In the following paragraphs all setting options will be explained the format is as follows.
-Name(startparameter)
-Info about this setting in GrimIAPrinter.exe.config
General information about this setting

The available settings are:

-GD ItemAssistant Database Path(-db:"pathWithDbName"):
-In GrimIAPrinter.exe.config at the very bottom under connectionStrings -> IADbContext -> connectionString -> data source -> replace userdata.db with PATHTOUSERDATADB\userdata.db
This has to be set the first time and will be saved.
The example path should work if you exchange YOURWINUSER with your windows username.
If it doesn't you can find the userdata.db if you open GD ItemAssistant -> go to settings -> click view backups -> go back one folder (IAGD) -> go into the data folder
startparameter example:
-db:"C:\Users\YOURWINUSER\AppData\Local\EvilSoft\IAGD\data\userdata.db" 

-ItemLevel(-ilvl:"value"):
-In GrimIAPrinter.exe.config under appsettings -> ItemLevel
This sets the minimum itemlevel that will be printed, all items below will be ignored. (default = 84)
Only integer values (whole numbers) are accepted
startparameter example:
-ilvl:"84" 

-Rarity(-r:"value"):
-In GrimIAPrinter.exe.config under appsettings -> Rarity
This sets the minimum rarity that will be printed, all rarities below will be ignored. (default = purple)
Rarity order (and valid rarity inputs) is in this case white < blue < green < purple (Reason for this is because I value MIs higher than blues, deal with it.)
startparameter example:
-r:"green"

-GreenExclusion(-ex:"value, value, value,...")
-In GrimIAPrinter.exe.config under appsettings -> GreenExclusion
This excludes certain Greens. This can be used to exlude green items that are only collected for crafting purposes,
which would be unecessary to print. (default = none)
Green items will get excluded as soon as the contain any of the given values.
You can be more specific than the example by adding Affixes to the item name or less specific, 
for example by just writing "Sword, Axe" which will exclude all green items with sword or axe in their name.
GreenExclusion in GrimIAPrinter.exe.config will be combined/unified with GreenExclusion from parameters or commandLineInput
startparameter example:
-ex:"Spectral Longsword, Troll Bonecrusher" 

-OutputPath(-out:"pathWithoutFileName")
-In GrimIAPrinter.exe.config under appsettings -> OutputPath
This is the folderpath were the outputfile will be saved. (default = Desktop\GrimIAPrinterOutput\)
startparameter example:
-out:"C:\Users\Test\Documents\GrimIAPrinterOutput" 

-OutputFormat(-f:"value")
-In GrimIAPrinter.exe.config under appsettings -> OutputFormat
This defines what output should be generated.
Valid formats are: plaintext, googlesheets, forum, html
startparameter example:
-f:"googlesheets"


I highly recommend to either put your settings into the config (prefered) or write a batch file with startup parameters.
Both options will save you lots of time by not typing your desired configuration everytime. 
The commandline interface is nice for testing stuff or for trying a specific new configuration.

Setting up GrimIAPrinter.exe.config:
To do that fill out at least the GD ItemAssistant Database Path in the config.
Also in GrimIAPrinter.exe.config -> CmdLineInputsEnabled write "false" instead of "true".
Lasty edit the desired filter options in GrimIAPrinter.exe.config -> appsettings
Then you re done save the config and you can just doubleclick the GrimIAPrinter.exe. 

The second best option would be to setup your startparameters once.
How to do that in 4 simple steps:
1. Create a new txt file and use any way to edit it.
2. write: start "" PATHTOGRIMIAPRINTEREXE PARAMS
   Example: start "" "C:\Program Files (x86)\GrimIAPrinter\GrimIAPrinter.exe" -db:"C:\Users\YOURWINUSER\AppData\Local\EvilSoft\IAGD\data\userdata.db" -ilvl:"84" -r:"green" -ex:"Spectral Longsword, Troll Bonecrusher" -out:"C:\Users\YOURWINUSER\Desktop\GrimIAPrinterOutput" -f:"googlesheets"
3. save the file as Batch file or change the file extension to .bat after saving. 
4. Doublclick the Batch file.

Enjoy.

Missing things (this is mostly a reminder for myself):
Maybe implement github integration so that the program can push/commit new index.html versions
Maybe build a gui.
