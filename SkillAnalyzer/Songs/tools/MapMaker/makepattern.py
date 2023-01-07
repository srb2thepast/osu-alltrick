# py makepattern.py -a angle -b bpm -c circles -s spacing -x xpositiont -y yposition
# ex: py makepattern.py -a 90 -b 250 -s 50 -x 200-y 10
import sys
import math

#######

import getopt, sys
 
argList = sys.argv[1:]
options = "a:b:c:s:p:x:y:"

angle = 36
bpm = 180
circles = 30
rate = 2
spacing = 150
curpos = [512/2,384/2]
ms = math.trunc(60000/bpm / rate)

try:
    arguments, values = getopt.getopt(argList, options)
     
    for arg, val in arguments:
        if arg in ("-a"):
            angle = int(val)
        elif arg in ("-b"):
            bpm = int(val)
        elif arg in ("-c"):
            circles = int(val)
        elif arg in ("-s"):
            spacing = int(val)
        elif arg in ("-x"):
            curpos[0] = int(val)
        elif arg in ("-y"):
            curpos[1] = int(val)
     
except getopt.error as err:
    # output error, and return with an error code
    print(str(err))


#######


print("Angle:",angle)
angle = math.radians(180-angle)
print("BPM:",bpm) 
print("Circles:",circles)
print("Spacing:",spacing)
print("Position: "+str(curpos[0])+","+str(curpos[1]))

result = """osu file format v14

[General]
AudioFilename: song.mp3
AudioLeadIn: 0
PreviewTime: 60000
Countdown: 0
SampleSet: Soft
StackLeniency: 0
Mode: 0
LetterboxInBreaks: 0
WidescreenStoryboard: 0

[Editor]
DistanceSpacing: 1.4
BeatDivisor: 4
GridSize: 4
TimelineZoom: 1.5

[Metadata]
Title: Song Title
TitleUnicode: Song Title (Unicode)
Artist: Artist
ArtistUnicode: Artist (Unicode)
Creator: Creator
Version: Normal
Source:
Tags:
BeatmapID: 0
BeatmapSetID: 0

[Difficulty]
HPDrainRate: 5
CircleSize: 4
OverallDifficulty: 6
ApproachRate: 6
SliderMultiplier: 1.4
SliderTickRate: 1

[Events]
0,0,"bg.jpg",0,0

[TimingPoints]
0,250,4,1,0,100,0
60000,125,4,2,0,100,0
120000,63,4,4,0,100,0

[Colours]
2,0,0,0,0

[HitObjects]
"""


for num in range(circles):
    print(num)
    x = str(math.sin(num*angle) * spacing + curpos[0]) 
    y = str(math.cos(num*angle) * spacing + curpos[1]) 
    t = str(1000+num*ms)
    result+=x+","+y+","+t+",1,0,0:0:0:0:\n"

f = open("output.osu", "w")
f.write(result)
f.close()
