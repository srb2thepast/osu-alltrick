
# -s: rate (jump, stream)
# -b: bpm
# -p: repeats
# -r: rhythm
# default: py makefingcontrol.py -s jump -b 180 -p 1 -r "xxxx"

import sys
import math

import getopt, sys
 
argList = sys.argv[1:]
options = "s:r:b:p:"

bpm = 180
rhythm = "x xxx"
rate = 2
repeats = 1

try:
    arguments, values = getopt.getopt(argList, options)
     
    for arg, val in arguments:
        if arg in ("-s"):
            if val.lower() == "jump":
                rate = 2
            elif val.lower() == "stream":
                rate = 4
            elif val.lower() == "thirds" or val.lower() == "3rds":
                rate = 3
            elif val.lower() == "beat":
                rate = 1
            else:
                rate = int(val)
        elif arg in ("-b"):
            bpm = int(val)
        elif arg in ("-r"):
            rhythm = val
        elif arg in ("-p"):
            repeats = int(val)
     
except getopt.error as err:
    # output error, and return with an error code
    print(str(err))

ms = math.trunc(60000/bpm / rate)
# def clamp(n, smallest, largest): return max(smallest, min(n, largest))

# ap = clamp(83/ms * 2,6,11)

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



num = 0
xnum = 0
y = 130
for i in range(repeats):
    for char in rhythm:
        x = str(100+xnum*20)
        if int(x) > 450: # forced clamp
            xnum = 0
        xnum+=1
        if char == " ":
            if int(x) > 384:
                xnum = 0
            num+=1
            
            # flip flop y
            if y == 192:
                y = 130
            else:
                y = 192

            continue # skip adding circle
        t = str(1000+num*ms)

        

        
        result+=x+","+str(y)+","+t+",1,0,0:0:0:0:\n"
        num+= 1
    if y == 192:
        y = 130
    else:
        y = 192
    xnum+=1
    num+=1

f = open("output.osu", "w")
f.write(result)
f.close()

print("Map Generation Complete")