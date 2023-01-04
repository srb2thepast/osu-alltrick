# ex:  py makestream.py 180 jump
import sys
import math
bpm = int(sys.argv[1])

rate = 4

if len(sys.argv)-1 >= 2:
    if sys.argv[2].lower() == "jump":
        rate = 2
    if sys.argv[2].lower() == "stream":
        rate = 4
    if sys.argv[2].lower() == "thirds" or sys.argv[2].lower() == "3rds":
        rate = 3 
    if sys.argv[2].lower() == "beat":
        rate = 1

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

ms = math.trunc(60000/bpm / rate)

for num in range(30):
    x = str(100+num*10)
    t = str(1000+num*ms)
    result+=x+",192,"+t+",1,0,0:0:0:0:\n"

f = open("output.osu", "w")
f.write(result)
f.close()
