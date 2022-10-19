using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Objects;
using osuAT.Game.Objects.LazerAssets;
using osuAT.Game.Objects.LazerAssets.Mod;
using osuAT.Game.Types.BeatmapParsers;
using osuTK;


namespace osuAT.Game.Types
{


    public class RulesetInfo {

        public string Name { get; }
        public IconUsage Icon { get; }
        public IParser? MapParser { get; }

        public RulesetInfo(string name, IconUsage icon, IParser? parser)
        {
            Name = name;
            Icon = icon;
            MapParser = parser;
        }

    }
    public class RulesetStore
    {
        public static RulesetInfo Osu = new RulesetInfo("osu", OsuIcon.RulesetOsu, new OsuParser()); 
        public static RulesetInfo Mania = new RulesetInfo("mania", OsuIcon.RulesetMania, null);
        public static RulesetInfo Catch = new RulesetInfo("catch", OsuIcon.RulesetCatch,null);
        public static RulesetInfo Taiko = new RulesetInfo("taiko", OsuIcon.RulesetTaiko, null);

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
    }
}
