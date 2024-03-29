![oatlogo](https://github.com/srb2thepast/osu-alltrick/assets/19241426/1ff236de-e085-47c8-af5a-b1d4430c8bae)

**osu! alltrick is an app aimed to give every single listed in osu!phd it's own pp system.** It's inspried by [DigitalHypno's osu!phd video.](https://www.youtube.com/watch?v=uc99yWeP1h4) 

https://user-images.githubusercontent.com/19241426/200956928-d7465949-5004-44c3-b7f7-724dc69ffc09.mp4

<sub> - An older FlowAim pp calculator from 9/11/2022, made in 30 minutes, wonderfully being broken by the power of Guess Who is Back. </sub>

**!!!!! osu! alltrick is still in beta, and thus the calculations implemented are guaranteed to change drastically and be broken in many ways. Please report all bugs found onto the github page, and do not take the calculations shown as an objective measure of your skill!!! !!!!!** 
# How do I use this!?!?!?!?!?!?!?

It's simple, really!!!

<sub> Currently, only windows is supported... Mac and Linux are incoming as soon as I can find a way to test and develop for the two.<sub>
1. - **Download the latest release** over [here.](https://github.com/srb2thepast/osu-alltrick/releases/latest)
2. - **Open the settings menu**, It'll be the gray cog to the right of the top bar.
3. - **Copy your API key** from https://osu.ppy.sh/p/api/ and paste it into the API Key box by clicking to the right of the text. 
4. - - **MAKE SURE YOU DO NOT PUBLICLY SHARE YOUR KEY OR YOUR OSU!AT SAVEFILE. READ ALL OF THE WARNINGS ON THE OSU!API PAGE.** osu! alltrick only uses your API key in order to get details on the scores you set so it can be displayed in each skill. That api key is automatically saved to your savefile, but that means if you give your savefile to someone else they will also have your api key. 
- - - **If you need to share your savefile, you can remove your apikey from it by entering "RESET" into the API Key box.** If your key was removed successfully, the box should flash <span style="color:yellow">yellow</span> rather than green or red.
5. - **Type your username into the Username box.** If you change your username, you might have to input it again.
6. - **Set your osu! path location.** Click on "Select" and navigate! The button to select that path will not show up until you are in your osu! install folder. 
7. - - Usually, the path is "AppData/Local/osu!".
8. - **If your settings look something like this, you're be done!** Now play a map on osu, set that 223000pp Aim Skill top play, and make the world proud!!!
9. - - Also, scores you set are only able to get submitted to osu! alltrick if it's submitted to osu! as well. That means if you set a play with lower score than another play with the same mods, it wont submit to osu! alltrick since osu! doesn't record those scores online.

![aimage-removebg-preview (6)](https://user-images.githubusercontent.com/19241426/200733022-36f6aba6-dc27-43a1-940c-75493831dea7.png)

# Controls
- Drag to look around!
- Scroll to zoom!
- Double click the boxes to open them! wait, these are more like bentos than boxes...

## Importing a score set on a specific map
If you want to import a score you've set in the past, just click on the download icon to the left of the top bar and input in the beatmap ID the score was set on. Currently, osu! alltrick is only able to import the play with the best score, regardless of what mods are applied.

The beatmap ID is the very last set of the 2 IDs in the link, for example, "osu.ppy.sh/beatmapsets/1229091#osu/**2555568**".

## Updating
If there's ever a new update, the releases page will automatically open in your default browser when you open osu! alltrick. Just install the new release and delete the old release. Your scores will be transferred over easily as your savedata is stored in a stabler location.

# Reporting Bugs
Simply make an issue with:
- The problem (preferably as your issue's title)
- Directions to reproduce 
- The contents of your errlog.txt file
- - (which is located in the same place as your exe.)
- - ![image](https://user-images.githubusercontent.com/19241426/200729079-14b2911b-9cca-46af-bba4-86d9488c93dd.png)

# Wiki & Community
[![image](https://user-images.githubusercontent.com/19241426/202961627-bdb72750-281a-4ddb-91c4-60f299d9554b.png)](https://srb2thepast.notion.site/o-at-Wiki-66777027010443759a7f0274fcae4322)
The wiki contains information on how every skill works and even on how to make your own skill! (Maybe one day there will be community guides on how to improve your skills as well!) Click on the logo to the left to check it out!

[![image](https://user-images.githubusercontent.com/19241426/203195729-773a4514-bd70-44cd-bba2-1131e0ebf0cd.png)](https://discord.gg/C7TGBUzndN)
The discord server is currently a work in progress, but it's still open to the public. Click on the icon to the left to join! 

# Contributing
If you want to contribute to the development of a skill, [check out the guide in the wiki!](https://github.com/srb2thepast/osu-alltrick/wiki) It (will be) crafted so that even the beginners to programming and the greatest haters of math will enjoy it. 

There's also an Analyzer that lets you visualize the changes you make to your calculator and test them instantly:

https://user-images.githubusercontent.com/19241426/200947927-0d00020e-1d20-41d3-a26b-f056b11b59bf.mp4

If you want to contribute to the base, just follow the directions given below. 
**Make sure to create an issue before starting to make something if you're planning make a pull request!!**

# Compiling

You will need three things:
1. An IDE to code.
* * **If you don't have an IDE installed** you can start out by downloading the [.NET Coding Pack bundle of Visual Studio Code](https://code.visualstudio.com/learn/educators/installers#_net-coding-pack). The pack already contains .NET as well, so you can just skip step 2.
2. .NET Framework
* * Download [.NET6.0](https://dotnet.microsoft.com/en-us/download). (If you downloaded the .NET Coding Pack bundle from above, you already have it installed into VS Code.)
3. Git
* * Download it [here](https://git-scm.com/downloads).


After all of that, simply open a terminal (ex. Command Prompt) in the folder you want to install osuAT to and run these three commands one by one to set it up:
```
git clone https://github.com/srb2thepast/osu-alltrick/main
cd osuAT
dotnet build
```

Then, if you're using VS Code, right click the folder you downloaded osu!AT into and click "Open With Code", then press Ctrl+Shift+` to open a terminal there for ease of access. 
* * If not, on windows you can navigate to the directory you want osuAT to be installed to then type "cmd" into the path bar to open Command Prompt quickly.
    
    
    https://user-images.githubusercontent.com/19241426/201435627-7dc87b73-7a31-4862-8544-9c4690daa810.mp4




Now, if you want to create or edit a Skill, navigate to the SkillAnalyzer directory via the terminal with this command: 
```
cd SkillAnalyzer
```

If you instead want to edit osuAT itself, navigate to the test browser with this command:
```
cd osuAT.Game.Tests
```

And finally, **run osuAT!**
```
dotnet run
```
Also, the savefile is stored in a seperate folder while testing, usually "osuAT.Game.Tests/bin/Debug/net6.0/dev_savedata", so that the main save file in AppData doesn't get overriden while testing.

    
# Credits
```csharp

# ppy Pty Ltd | osu-framework [MIT License]
    - Assets used: "osu!framework"

# ppy Pty Ltd | osu! [MIT License]
    - Assets used: "StarRatingDisplay.cs" [Modified]
      |            "osu.Game" [Namespace]
      |             |   |
      |            "ColourUtils.cs" [SampleFromLinearGradient() method]
      |             |   |
      |            "TestSceneStarRatingDisplay.cs" [Modified]
      |             |   |
      |            "StarDifficulty.cs" [Modified] 
      |             |   with dependencies from : "DifficultyRating.cs"
      |             |   |
      |            "ModIcon.cs" [Modified]
      |             |   with dependencies from : "OsuColour.cs" 
      |             |   |   
      |            "OsuIcon.cs"
      |             |   |
      |            "ModDisplay.cs" [Modified]
      |             |   with dependencies from : "ReverseChildIDFillFlowContainer.cs"
      |             |   |
      |            "TestSceneModDisplay.cs" [Modified]
      |             |   |
      |            "DrawableRank.cs" [Modified]
      |             |   with dependencies from : "OsuColour.cs"

    
# Piotrekol | OsuMemoryDataProvider [GNU General Public License v3.0]
    - Assets used: "OsuMemoryDataProvider"

# ppy Pty Ltd | osu-resources [Creative Commons Attribution-NonCommercial 4.0 International]
    - Assets used: "osuFont.bin"
      |             |
      |            "osuFont_0.png"

# ppy Pty Ltd | osu-tools [MIT License]
    - Assets used: "ProcessorWorkingBeatmap"
      |             |   with dependencies from : "LegacyHelper.cs"
      |            "StrainVisualizer.cs"
```
```csharp
# [- Fonts - ] #

# Dan Sayers | Averia Sans Libre [Open Font License]
    - Assets used: "Averia Sans Libre Font"

# Joe Prince | Varela Round [Open Font License]
    - Assets used: "Varela Round Font"

# Joe Prince | Venera [Personal Use License]
    - Assets used: "Venera Font" 

# Haley Fiege | Sniglet [Open Font License]
    - Assets used: "Sniglet Font"
```

# Special Thanks
### **DigitalHypno -** For creating osu!phd and everything you've done for the community

