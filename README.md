
![oatlogo](https://user-images.githubusercontent.com/19241426/200944429-65748f64-e1e8-415d-b5a6-1291331385b3.png)
**osu!alltrick is an app aimed to give every single skill from osu!phd it's own pp system.** 
It's inspried by [DigitalHypno's osu!phd video.](https://www.youtube.com/watch?v=uc99yWeP1h4) 

https://user-images.githubusercontent.com/19241426/200956928-d7465949-5004-44c3-b7f7-724dc69ffc09.mp4

<sub> - The current FlowAim pp calculator as of 9/11/2022, made in 30 minutes, wonderfully being broken by the power of Guess Who is Back. </sub>

# How do I use this!?!?!?!?!?!?!?
It's simple, really!!!
1. - **Open the settings menu**, It'll be the gray cog to the right of the top bar.
2. - **Copy your API key** from https://osu.ppy.sh/p/api and paste it into the API Key box by clicking to the right of the text. 
2. - - **MAKE SURE YOU DO NOT PUBLICLY SHARE YOUR KEY OR YOUR OSU!AT SAVEFILE. READ ALL OF THE WARNINGS ON THE PAGE.** osu!alltrick only uses your API key in order to get details on the scores you set so it can be displayed in each skill. That api key is also saved to your savefile so you don't have to type it in every time, but that means if you give your savefile to someone else they will also have your api key.
3. - **Type your username into the Username box.** If you change your username, you might have to input it again.
4. - **Set your osu! path location.** Click on "Select" and navigate! The button to select that path will not show up until you are in your osu! install folder. 
4. - - Usually, the path is "C:/Users/AppData/Local/osu!".
5. - **If your settings look something like this, you're be done!** Now play a map on osu, set that 223000pp Aim Skill top play, and make the world proud!!!
![aimage-removebg-preview (6)](https://user-images.githubusercontent.com/19241426/200733022-36f6aba6-dc27-43a1-940c-75493831dea7.png)

Make sure you have one window of osu!at open at a time. Opening multiple may end up in the osu! website invalidating your API key.
Currently, only windows is supported... Mac and Linux are incoming as soon as I can find a way to test and develop for the two.
# Controls!
- Drag to look around!
- Scroll to zoom!
- Double click the boxes to open them!... wait, these are more like bentos than boxes...

## Importing a score set on a specific map
If you want to import a score you set, just click on the download icon to the left of the top bar and input in the beatmap ID the score was set on. Currently, osu!alltrick is only able to import the play with the best score, regardless of what mods are applied.

The beatmap ID is the very last set of the 2 IDs in the link, for example, "osu.ppy.sh/beatmapsets/1229091#osu/**2555568**". If you cant find the second set, reselect the difficulty on the website.

## Updating
If there's ever a new update, the releases page will automatically open in your default browser when you open osu!alltrick. Just install the new release then drag and drop the savedata folder from your current install into your new install so you can keep your scores.
![image](https://user-images.githubusercontent.com/19241426/200728966-db8ed2fc-f62a-4271-8046-3dcb47c0f8c4.png)

# Reporting Bugs
Simply make an issue with:
- The problem (preferably as your issue's title)
- Directions to reproduce 
- The contents of your errlog.txt file
- - (which is located in the same place as your exe.)
![image](https://user-images.githubusercontent.com/19241426/200729079-14b2911b-9cca-46af-bba4-86d9488c93dd.png)

# Contributing
If you want to contribute to the development of a skill, [check out the wiki!](https://github.com/srb2thepast/osu-alltrick/wiki) It's been crafted so that even the beginners to programming and the greatest haters of math will enjoy it. 

There's also an Analyzer that lets you visualize the changes you make to your calculator and test them instantly:

https://user-images.githubusercontent.com/19241426/200947927-0d00020e-1d20-41d3-a26b-f056b11b59bf.mp4


If you want to contribute to making the wiki better, just go ahead! DM or ping me on discord (srb2thepast#7380) when you want your edits to become offical.

If you want to contribute to the base, just git clone the repository and install the necessary nuget packages. There should be a list in the credits down below, so you can manually install them if you're using something that doesn't install them automatically. 
Make sure to create an issue before starting to make something if you want your edits to be merged!!
