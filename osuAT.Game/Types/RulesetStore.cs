using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Objects;
using osuAT.Game.Objects.LazerAssets;
using osuAT.Game.Objects.LazerAssets.Mod;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects.Legacy.Osu;
using osuTK;
namespace osuAT.Game.Types
{

    // [!] do this
    // also add a way to get the main branch of DifficultyHitObject for that class.
    public class RulesetInfo {

        public int OnlineID { get; }
        public string Name { get; }
        public IconUsage Icon { get; }
            
        public RulesetInfo(string name, IconUsage icon, int onlineID)
        {
            Name = name;
            Icon = icon;
            OnlineID = onlineID;
        }

    }

    public class RulesetStore
    {
        #region Mode Difficulty Calculators
        public class OsuDiffCalc : OsuDifficultyCalculator, IExtendedDifficultyCalculator
        {

            public OsuDiffCalc(IRulesetInfo ruleset, IWorkingBeatmap beatmap) : base(ruleset, beatmap)
            {
            }

            public Skill[] GetSkills() => throw new NotImplementedException();

            protected IEnumerable<DifficultyHitObject> CreateDifficultyHitObjectsOld(IBeatmap beatmap, double clockRate) {
                List<DifficultyHitObject> objects = new List<DifficultyHitObject>();

                // The first jump is formed by the first two hitobjects of the map.
                // If the map has less than two OsuHitObjects, the enumerator will not return anything.
                for (int i = 1; i< beatmap.HitObjects.Count; i++)
                {
                    var lastLast = i > 1 ? beatmap.HitObjects[i - 2] : null;
                    var last = beatmap.HitObjects[i - 1];
                    var cur = beatmap.HitObjects[i];

                    if (cur is HitObject)
                    objects.Add(new OsuDifficultyHitObject(cur, last, lastLast, clockRate, objects, objects.Count));
                }

                return objects;
             }

            public IEnumerable<DifficultyHitObject> GetDifficultyHitObjects(IBeatmap beatmap, double clockRate)
            {
                Console.WriteLine($"rate: {clockRate}");
                return CreateDifficultyHitObjects(beatmap, clockRate);
            }

            DifficultyHitObject[] IExtendedDifficultyCalculator.GetDifficultyHitObjects(IBeatmap beatmap, double clockRate)
            {
                return CreateDifficultyHitObjects(beatmap, clockRate).ToArray();
            }
        }
        #endregion

        public static RulesetInfo Osu = new RulesetInfo("osu", OsuIcon.RulesetOsu,0); 
        public static RulesetInfo Mania = new RulesetInfo("mania", OsuIcon.RulesetMania,1);
        public static RulesetInfo Catch = new RulesetInfo("catch", OsuIcon.RulesetCatch,2);
        public static RulesetInfo Taiko = new RulesetInfo("taiko", OsuIcon.RulesetTaiko,3);

        public static RulesetInfo GetByName(string name)
        {
            switch (name.ToLower())
            {
                case "osu": return Osu;
                case "mania": return Mania;
                case "catch": return Catch;
                case "taiko": return Taiko;

                default: return Osu;
            }
        }

        public static RulesetInfo GetByNum(int num)
        {
            switch (num)
            {
                case 0: return Osu;
                case 1: return Taiko;
                case 2: return Catch;
                case 3: return Mania;

                default: return Osu;
            }
        }

        public static Ruleset ConvertToOsuRuleset(RulesetInfo ruleset)
        {
            if (ruleset == Osu) {
                return new OsuRuleset();
            }
            return null;
        }
        public static RulesetInfo GetByIRulesetInfo(IRulesetInfo ruleset)
        {
            Console.WriteLine($"Rulesetname: {ruleset.Name}");
            return GetByName(ruleset.ShortName.ToLower().Split("!")[0]);
        }

        public static DifficultyCalculator GetDiffCalc(RulesetInfo ruleset, IWorkingBeatmap map)
        {
            Console.WriteLine(ruleset);
            if (ruleset == Osu)
            {
                return new OsuDiffCalc(ConvertToOsuRuleset(ruleset).RulesetInfo, map);
            }
            throw new NotImplementedException();
        }

        public static IExtendedDifficultyCalculator GetDiffCalcObj(RulesetInfo ruleset, IWorkingBeatmap map)
        {
            if (ruleset == Osu)
            {
                return new OsuDiffCalc(ConvertToOsuRuleset(ruleset).RulesetInfo, map);
            }
            throw new NotImplementedException();
        }
    }
}
