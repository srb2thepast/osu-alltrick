using System.Collections.Generic;
using osuAT.Game.Types;

namespace osuAT.Game.Skill
{
    public class SkillManager
    {

        /// <summary>
        /// The skill's PP Calculator System.
        /// </summary>
        public static SkillPPTotals CalcAll(RulesetInfo ruleset, Score score, List<ModInfo> mods) {
            // todo: this function should return skillPPCalcs for every skill that exists.
            return new SkillPPTotals { FlowAim = 600 };
        }

    }
}
