using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Objects;
using osu.Framework.Graphics.Sprites;
using osuAT.Game.Objects.LazerAssets;
using osuAT.Game.Objects.LazerAssets.Mod;


namespace osuAT.Game.Types
{

    public class RulesetInfo {
        public string Name { get; }
        public IconUsage Icon { get; }
        public RulesetInfo(string name, IconUsage icon) {
            Name = name;
            Icon = icon;
        }
        
    }
    public class RulesetStore
    {
        public static RulesetInfo Osu => new RulesetInfo("Osu", OsuIcon.RulesetOsu); 
        public static RulesetInfo Mania => new RulesetInfo("Mania", OsuIcon.RulesetMania);
        public static RulesetInfo Catch => new RulesetInfo("Catch", OsuIcon.RulesetCatch);
        public static RulesetInfo Taiko => new RulesetInfo("Taiko", OsuIcon.RulesetTaiko);


    }
}
