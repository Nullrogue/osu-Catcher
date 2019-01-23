# osu!Catcher
## Overview
osu!Catcher is a program that allows users to automatically delete the background of any
<a href="https://osu.ppy.sh/beatmapsets" target="_blank">Osu! beatmap</a> as it is being added to Osu! Inspired by [Recursion's program](https://osu.ppy.sh/community/forums/topics/365196) and my unwillingness to open it and click one button every time I download beatmaps.
## Usage
Once the program has been [installed](#installation) and run, you should be presented with a screen similar to the one below:
![ ](https://i.imgur.com/0M12Wbo.png)
</br>
Right off the bat you can see the log box in the middle of the window says there is a warning mentioning the settings.json file. This is a normal occurrence the first time you start the program up but should you need to change them you can change them by going `File > Settings` (This is where you would change the directory the program uses to check for songs if you get an error related to that)
</br>
 ![](https://i.imgur.com/ynwYjXH.png)
 </br>
 After that you can see a couple buttons along the bottom for things like a manual scan, one for copying the contents of the log box to your clipboard (Use this to report any bugs or errors in [Issues](https://github.com/NullvaIue/osu-Catcher/issues)), and one to start and stop the program. The program should be functional right out of the box as long as your `Osu installation directory` setting is configured properly. At this point you can load up Osu! and start to added beatmaps and have their backgrounds deleted with no issues. (Hopefully)
## Requirements
* [.NET Framework 4.6 or higher](https://www.microsoft.com/en-us/download/details.aspx?id=53344) (You probably already have a compatible version installed)
## Installation
* Go to [releases](https://github.com/NullvaIue/BeatmapCatcher/releases) and download the latest version of either the installer (recommended) or the zip file.
	* For the installer:
		* Beware: The installer might raise a false positive with your anti-virus and Windows Defender will tell you that it is an untrusted executable, this is common for [NSIS](https://nsis.sourceforge.io/Main_Page) based installers. Click `More Info > Run Anyway`
		* Run the installer exe and follow the basic steps.
	* For the zip file:
		* Extract the folder to a place that you are familiar with and can access easily/create a shortcut to.
		* The drawback of using the zip is that it doesn't automatically create start menu shortcuts and if you move the folder while using the `Run on startup` setting it might break that functionality.

## Issues
Please see the [Issues](https://github.com/NullvaIue/osu-Catcher/releases) to see if anyone has had a similar issue or to create a new issue if it isn't already documented(recommended) or add me on discord: `Nullvalue#8123`. Thanks
