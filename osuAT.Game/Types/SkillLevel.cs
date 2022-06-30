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
        Learner,
        Confident,
        Experienced,
        Proficient,
        Mastery,
        Chosen
    }
}
