using System;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Testing;
using osuAT.Game.Types;

#nullable enable

namespace osuAT.Game.Types
{
    public enum SkillLevel
    {
        None = 0,
        Learner = 1,
        Experienced = 2,
        Confident = 3,
        Proficient = 4,
        Mastery = 5,
        Chosen = 6,
    }
}
