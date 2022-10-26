using System;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;


namespace osuAT.Game.Skills
{
    public static class Skill
    {
        public static FlowAimSkill Flowaim = new FlowAimSkill();
        public static CursorControlSkill CursorControl= new CursorControlSkill();
        public static AimSkill Aim = new AimSkill();
        public static NerveControlSkill NerveControl = new NerveControlSkill();

        /// <summary>
        /// A list of every skill currently supported.
        /// </summary>
        public static List<ISkill> SkillList = new List<ISkill> {
            Flowaim,
            CursorControl,
            Aim,
            NerveControl
        };

        public static ISkill GetSkillByID(string skillid) {
            foreach (ISkill skill in SkillList) {
                if (skill.Identifier == skillid) {
                    return skill;
                }
            }
            return null;
        }

        /// <summary>
        /// Calculates the PP of every Skill for one score.
        /// </summary>
        public static Dictionary<string, double> CalcAll(Score score)
        {
            Dictionary<string, double> dict = new Dictionary<string, double>();
            foreach (ISkill skill in SkillList) {
                dict.Add(skill.Identifier, skill.SkillCalc(score));
            }
            return dict;
        }


        /// <summary>
        /// Calculates the Weghted PP of a list of Scores based on a list of Tuple<Score.ID, double pp>.
        /// </summary>
        public static double CalcWeighted(List<Tuple<Guid, double>> scoreList)
        {
            double total = 0;
            int n = 0;
            foreach (var score in scoreList)
            {
                total += score.Item2*Math.Pow(0.95, n);
                n += 1;
            }
            return Math.Truncate(total * 100) / 100;
        }
        /// <summary>
        /// Calculates the Weghted PP of a list of Scores.
        /// </summary>
        public static double CalcWeighted(List<Tuple<Score, double>> scoreList)
        {
            double total = 0;
            int n = 0;
            foreach (var score in scoreList)
            {
                total += score.Item2 * Math.Pow(0.95, n);
                n += 1;
            }

            return Math.Truncate(total * 100) / 100;
        }
        /// <summary>
        /// Calculates the Weghted PP of a list of pp values.
        /// </summary>
        public static double CalcWeighted(List<double> ppList)
        {
            double total = 0;
            int n = 0;
            foreach (var pp in ppList)
            {
                total += pp * Math.Pow(0.95, n);
                n += 1;
            }
            return Math.Truncate(total * 100) / 100;
        }
    }
}
