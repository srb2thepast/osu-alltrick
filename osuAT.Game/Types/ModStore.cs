using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Objects;
using osu.Framework.Graphics.Sprites;
using osuAT.Game.Objects.LazerAssets;
using osuAT.Game.Objects.LazerAssets.Mod;


namespace osuAT.Game.Types
{

    public class ModInfo {
        public string Name { get; }
        public string Acronym { get; }
        public byte Order { get; } // for score syntaxs like HDDTHR rather than DTHRHD
        public ModType Type { get; }
        public IconUsage? Icon { get; }
        public string Description { get; }
        public ModInfo(string name, string acronym, byte order, ModType type, IconUsage? icon = null) {
            Name = name;
            Acronym = acronym;
            Order = order;
            Icon = icon;
            Type = type;
        }
        
    }
    public class ModStore
    {
        #region osu!standard mods
        public static ModInfo Auto => new ModInfo("Auto", "AT", 1, ModType.Automation, OsuIcon.ModAuto); 
        public static ModInfo Relax => new ModInfo("Relax", "RX", 1, ModType.Automation, OsuIcon.ModRelax);
        public static ModInfo Autopilot => new ModInfo("Autopilot", "AP", 1, ModType.Automation, OsuIcon.ModAutopilot);
        public static ModInfo Spunout => new ModInfo("Spunout", "SO", 3, ModType.DifficultyReduction, OsuIcon.ModSpunOut);
        public static ModInfo Easy => new ModInfo("Easy", "EZ", 4, ModType.DifficultyReduction, OsuIcon.ModEasy);
        public static ModInfo Nofail => new ModInfo("Nofail", "NF", 5, ModType.DifficultyReduction, OsuIcon.ModNoFail);
        public static ModInfo Hidden => new ModInfo("Hidden", "HD", 6, ModType.DifficultyIncrease, OsuIcon.ModHidden);
        public static ModInfo Halftime => new ModInfo("Halftime", "HT", 7, ModType.DifficultyReduction, OsuIcon.ModHalftime);
        public static ModInfo Nightcore => new ModInfo("Nightcore", "NC", 7, ModType.DifficultyIncrease, OsuIcon.ModNightcore);
        public static ModInfo Doubletime => new ModInfo("Doubletime", "DT", 8, ModType.DifficultyIncrease, OsuIcon.ModDoubleTime);
        public static ModInfo Hardrock => new ModInfo("Hardrock", "HR", 9, ModType.DifficultyIncrease, OsuIcon.ModHardRock);
        public static ModInfo Suddendeath => new ModInfo("Suddendeath", "SD", 10, ModType.DifficultyIncrease, OsuIcon.ModSuddenDeath);
        public static ModInfo Perfect => new ModInfo("Perfect", "PF", 10, ModType.DifficultyIncrease, OsuIcon.ModPerfect);
        public static ModInfo Flashlight => new ModInfo("Flashlight", "FL", 11, ModType.DifficultyIncrease, OsuIcon.ModFlashlight);
        public static ModInfo Nomod => new ModInfo("Nomod", "NM", 0, ModType.System, OsuIcon.ModBg);
        #endregion

        public ModInfo GetByAcronym(string acronym) {
            switch (acronym) {
                case "AT": return Auto; 
                case "RX": return Relax;
                case "AP": return Autopilot;
                case "SO": return Spunout;
                case "EZ": return Easy;
                case "NF": return Nofail;
                case "HD": return Hidden;
                case "HT": return Halftime;
                case "NC": return Nightcore;
                case "DT": return Doubletime;
            }
            return Nomod;

        }
    }
}
