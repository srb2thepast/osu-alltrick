﻿using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;
using static osuAT.Game.Skills.AimSkill;

namespace osuAT.Game.Skills
{
    [Obsolete(
        "In osu!phd Nerve Control inherits properties from focus, " +
        "which inherits from pattern processing/reading. " +
        "It's better to work down to nerve control from reading so that a better understanding" +
        "of reading difficulty in maps can be achieved that can then be used while developing Nerve Control.")]
    public class NerveControlSkill : ISkill
    {
        #region Info

        public string Name => "Nerve Control";

        public string Identifier => "nervecontrol";

        public string Version => "0.002";

        public string Summary => "Ability to maintain composure \n and steadiness under pressure.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#99FF69");

        public Colour4 SecondaryColor => Colour4.FromHex("#00FFF0");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize { get; }

        public float MiniHeight => 224;

        public int BoxNameSize => 65;

        public Vector2 BoxPosition => new Vector2(953, 855);

        #endregion Info

        public class NerveControlCalculator : SkillCalcuator
        {
            public NerveControlCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private float totaldist = 0;

            public override void CalcNext(OsuDifficultyHitObject diffHitObj)
            {
                var DiffHitObj = diffHitObj;
                var HitObj = (OsuHitObject)DiffHitObj.BaseObject;
                var LastHitObj = (OsuHitObject)DiffHitObj.LastObject;
                totaldist += Math.Abs(HitObj.Position.Length - LastHitObj.Position.Length);
                CurTotalPP = (totaldist / (FocusedScore.BeatmapInfo.Contents.DiffHitObjects.Count + 1));
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new NerveControlCalculator(score);
    }
}
