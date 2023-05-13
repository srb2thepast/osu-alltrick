using System;
using System.Reflection;
using FFmpeg.AutoGen;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Mods;
using osuAT.Game.Objects;
using osuAT.Game.Objects.LazerAssets;
using osuAT.Game.Objects.LazerAssets.Mod;
using Sentry.Infrastructure;

namespace osuAT.Game.Types
{
    public class ModInfo
    {
        public string Name { get; }
        public string Acronym { get; }
        public byte Order { get; } // for score syntaxs like HDDTHR rather than DTHRHD
        public ModType Type { get; }
        public IconUsage? Icon { get; }
        public string Description { get; }

        /// <summary>
        /// Creates a new ModInfo class.
        /// </summary>
        /// <param name="name">The mod's name</param>
        /// <param name="acronym">The mod's acronym</param>
        /// <param name="order">The order for the mod to be displayed in when placed with others</param>
        /// <param name="type">The osu!lazer type of the mod</param>
        /// <param name="icon">The osu! Icon for the mod</param>
        public ModInfo(string name, string acronym, byte order, ModType type, IconUsage? icon = null)
        {
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

        public static ModInfo Auto => new("Auto", "AT", 1, ModType.Automation, OsuIcon.ModAuto);
        public static ModInfo Relax => new("Relax", "RX", 1, ModType.Automation, OsuIcon.ModRelax);
        public static ModInfo Autopilot => new("Autopilot", "AP", 1, ModType.Automation, OsuIcon.ModAutopilot);
        public static ModInfo Spunout => new("Spunout", "SO", 2, ModType.DifficultyReduction, OsuIcon.ModSpunOut);
        public static ModInfo Touchdevice => new("Touchdevice", "TD", 3, ModType.System, null);
        public static ModInfo Easy => new("Easy", "EZ", 4, ModType.DifficultyReduction, OsuIcon.ModEasy);
        public static ModInfo Nofail => new("Nofail", "NF", 5, ModType.DifficultyReduction, OsuIcon.ModNoFail);
        public static ModInfo Hidden => new("Hidden", "HD", 6, ModType.DifficultyIncrease, OsuIcon.ModHidden);
        public static ModInfo Halftime => new("Halftime", "HT", 7, ModType.DifficultyReduction, OsuIcon.ModHalftime);
        public static ModInfo Nightcore => new("Nightcore", "NC", 7, ModType.DifficultyIncrease, OsuIcon.ModNightcore);
        public static ModInfo Doubletime => new("Doubletime", "DT", 8, ModType.DifficultyIncrease, OsuIcon.ModDoubleTime);
        public static ModInfo Hardrock => new("Hardrock", "HR", 9, ModType.DifficultyIncrease, OsuIcon.ModHardRock);
        public static ModInfo Suddendeath => new("Suddendeath", "SD", 10, ModType.DifficultyIncrease, OsuIcon.ModSuddenDeath);
        public static ModInfo Perfect => new("Perfect", "PF", 10, ModType.DifficultyIncrease, OsuIcon.ModPerfect);
        public static ModInfo Flashlight => new("Flashlight", "FL", 11, ModType.DifficultyIncrease, OsuIcon.ModFlashlight);
        public static ModInfo Nomod => new("Nomod", "NM", 0, ModType.System, OsuIcon.ModBg);

        #endregion osu!standard mods

        public static Mod ConvertToOsuMod(ModInfo mod)
        {
            Console.WriteLine(mod.Name, mod.Acronym, mod.Description);
            string name = mod.Name;
            switch (name.ToUpper()[0] + name.ToLower()[1..])
            {
                case "Auto": return new OsuModAutoplay();
                case "Relax": return new OsuModRelax();
                case "Autopilot": return new OsuModAutopilot();
                case "Spunout": return new OsuModSpunOut();
                case "Easy": return new OsuModEasy();
                case "Nofail": return new OsuModNoFail();
                case "Hidden": return new OsuModHidden();
                case "Halftime": return new OsuModHalfTime();
                case "Nightcore": return new OsuModNightcore();
                case "Doubletime": return new OsuModDoubleTime();
                case "Hardrock": return new OsuModHardRock();
                case "Suddendeath": return new OsuModSuddenDeath();
                case "Perfect": return new OsuModPerfect();
                case "Flashlight": return new OsuModFlashlight();
                case "Touchdevice": return new OsuModTouchDevice();
                default: throw new NullReferenceException("Could not find a lazer-mod equivalent for  mod " + name);
            }
        }

        public static ModInfo GetModInfoByName(string name)
        {
            switch (name.ToUpper()[0] + name.ToLower()[1..])
            {
                case "Auto": return Auto;
                case "Relax": return Relax;
                case "Autopilot": return Autopilot;
                case "Spunout": return Spunout;
                case "Easy": return Easy;
                case "Nofail": return Nofail;
                case "Hidden": return Hidden;
                case "Halftime": return Halftime;
                case "Nightcore": return Nightcore;
                case "Doubletime": return Doubletime;
                case "Hardrock": return Hardrock;
                case "Suddendeath": return Suddendeath;
                case "Perfect": return Perfect;
                case "Flashlight": return Flashlight;
                case "Touchdevice": return Touchdevice;
                default: throw new NullReferenceException("Could not find any ModInfo for mod " + name);
            }
        }

        public ModInfo GetByAcronym(string acronym)
        {
            switch (acronym)
            {
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
                case "HR": return Hardrock;
                case "SD": return Suddendeath;
                case "PF": return Perfect;
                case "FL": return Flashlight;
                case "TD": return Touchdevice;
                default: throw new NullReferenceException("Could not find any ModInfo for mod acronym" + acronym);
            }
        }
    }

    [Flags]
    public enum OsuMod
    {
        None = 0,
        NoFail = 1,
        Easy = 2,
        TouchDevice = 4,
        Hidden = 8,
        HardRock = 16,
        SuddenDeath = 32,
        DoubleTime = 64,
        Relax = 128,
        HalfTime = 256,
        Nightcore = 512, // Only set along with DoubleTime. i.e: NC only gives 576
        Flashlight = 1024,
        Autoplay = 2048,
        SpunOut = 4096,
        Relax2 = 8192,    // Autopilot
        Perfect = 16384, // Only set along with SuddenDeath. i.e: PF only gives 16416
        Key4 = 32768,
        Key5 = 65536,
        Key6 = 131072,
        Key7 = 262144,
        Key8 = 524288,
        FadeIn = 1048576,
        Random = 2097152,
        Cinema = 4194304,
        Target = 8388608,
        Key9 = 16777216,
        KeyCoop = 33554432,
        Key1 = 67108864,
        Key3 = 134217728,
        Key2 = 268435456,
        ScoreV2 = 536870912,
        Mirror = 1073741824,
        KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,
        FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | KeyMod,
        ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
    }
}
