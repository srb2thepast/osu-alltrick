using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Sprites;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Colour;
using osu.Game.Rulesets.Difficulty.Preprocessing;

namespace osuAT.Game.Skills
{
    // [!] add a way to use a stream-like loop to calculate pp per hitobject (like curPP = SkillCalcStream.LoadNextHit())
    // When a skill is ready to be used, dont forget to add it to Skill.cs   
    public interface ISkillCalcuator
    {
        private Beatmap focusedMap;

        /// <summary>
        /// The rulesets this skill can support.
        /// </summary>
        public RulesetInfo[] SupportedRulesets { get; }

        public ISkillCalcuator(Beatmap map) {
            focusedMap = map
        }

        /// <summary>
        /// This skill's PP Calculator System.
        /// </summary>
        public double SkillCalc(Score score) { return -1; }

        // What to do for one object in a  
        public void CalcObj(DifficultyHitObject) {
            
        }

        /// <summary>
        /// Returns the most pp possible in a map for this skill.
        /// </summary>
        public double GetCalcStream(Beatmap map, RulesetInfo ruleset, List<ModInfo> mods = null) 
        {


            mods ??= new List<ModInfo>();
            return SkillCalc(new Score
            {
                RulesetName = ruleset.Name,
                ScoreRuleset = ruleset,
                Combo = map.MaxCombo,
                BeatmapInfo = map,
                Mods = mods,
                AccuracyStats = new AccStat(map.Contents.HitObjects.Count,0,0,0),
            });

        }

        /// <summary>
        /// The people who contributed to this skill's development.
        /// </summary>
        public Contributor[] Contributors => new Contributor[] {};

        /// <summary>
        /// Returns a list of how the Skill's PP changes over the course of a list of hit
        /// </summary>
        public List<double> SkillCalcHitlist(Beatmap map, RulesetInfo ruleset, List<ModInfo> mods) {
            throw new NotImplementedException("not done");
        }

    }

    public class SkillCalcStream
}
