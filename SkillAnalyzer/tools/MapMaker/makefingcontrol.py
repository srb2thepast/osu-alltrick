# ex:  py makestream.py 180 "x xx x xxx" 5 jump
import sys
import math
bpm = int(sys.argv[1])

rate = 4
rhythm = sys.argv[2]
repeats = 1


if len(sys.argv)-1 >= 3:
    repeats = int(sys.argv[3])
    if repeats == 0: 
        repeats = 1 

if len(sys.argv)-1 >= 4:
    if sys.argv[4].lower() == "jump":
        rate = 2
    if sys.argv[4].lower() == "stream":
        rate = 4
    if sys.argv[4].lower() == "thirds" or sys.argv[2].lower() == "3rds":
        rate = 3 
    if sys.argv[4].lower() == "beat":
        rate = 1

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
StackLeniency: 0.7
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


rhythm = rhythm.replace("[","")
rhythm = rhythm.replace("]","")

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
