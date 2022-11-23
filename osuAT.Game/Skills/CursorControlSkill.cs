using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osuAT.Game.Types;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using static osuAT.Game.Skills.AimSkill;
using osu.Framework.Utils;
using osuAT.Game.Skills.Resources;

namespace osuAT.Game.Skills
{
    public class CursorControlSkill : ISkill
    {
        #region
        public string Name => "Cursor Control";

        public string Identifier => "cursorcontrol";

        public string Version => "0.002";

        public string Summary => "Ability to control the movement \n of your aim.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#F7E65D");

        public Colour4 SecondaryColor => Colour4.FromHex("#F0A1B7");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize { get; }

        public float MiniHeight => 224;

        public int BoxNameSize => 63;

        public Vector2 BoxPosition => new Vector2(2000, -1300);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500,3000, 6000, 9000, 10000);
        #endregion


        public class CursorControlCalculator : SkillCalcuator
        {
            public CursorControlCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };


            [HiddenDebugValue]
            private List<double> hitAngleDiffs;
            [HiddenDebugValue]
            private double distTotal;
            private double avgDist;
            private double curAngle;
            private double timeDiffSum;
            private double curBPM;
            private double avgAng;
            [HiddenDebugValue]
            private double angTotal;
            private int angCount;

            public override void Setup()
            {
                hitAngleDiffs = new List<double>() {};
                avgDist = 0;
                distTotal = 0;
                avgAng = 0;
                angTotal = 0;
                angCount = 0;
                curAngle = 0;
                timeDiffSum = 0;
                curBPM = 0;
            }

            public override void CalcNext(OsuDifficultyHitObject diffHit) { 
                var hitObj = (OsuHitObject)diffHit.BaseObject;
                var lastHitObj = (OsuHitObject)diffHit.LastObject;

                var angle = (diffHit.Angle);

                if (angle != default) {
                    hitAngleDiffs.Add((double)angle);
                    curAngle = (180 / Math.PI) * (double)angle;
                    angTotal += (double)angle;
                    angCount++;
                    avgAng   = angTotal / angCount;
                }
                curBPM = 120 / (timeDiffSum / (CurrentIndex + 1));
                timeDiffSum += hitObj.StartTime - lastHitObj.StartTime;
                distTotal += (hitObj.Position - lastHitObj.Position).Length; 
                CurTotalPP = Math.Pow(SharedMethods.StandardDeviation(hitAngleDiffs, avgAng)
                        * (avgDist/2)
                    ,curBPM
                );
                avgDist = distTotal / (CurrentIndex+1);
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new CursorControlCalculator(score);
    }
}
