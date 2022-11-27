using System;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;
using System.IO;
using osuAT.Game.Skills.Resources;

namespace osuAT.Game.Skills
{
    public static class Skill
    {
        public static FlowAimSkill Flowaim = new FlowAimSkill();
        public static CursorControlSkill CursorControl= new CursorControlSkill();
        public static AimSkill Aim = new AimSkill();
        public static SliderAimSkill SliderAim = new SliderAimSkill();
        public static TappingStaminaSkill TappingStamina = new TappingStaminaSkill();

        /// <summary>
        /// A list of every skill currently supported.
        /// </summary>
        public static List<ISkill> SkillList = new List<ISkill> {
            Flowaim,
            CursorControl,
            Aim,
            SliderAim,
            TappingStamina
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
            if (!File.Exists(SaveStorage.ConcateOsuPath(score.BeatmapInfo.FolderLocation)))
            {
                Console.WriteLine($"Could not calculate {score.ID} (osu: {score.OsuID}). Resetting values.");
                return GetEmptyAlltrickPP();
            }
            Dictionary<string, double> dict = new Dictionary<string, double>();
            foreach (ISkill skill in SkillList)
            {
                SkillCalcuator calculator = skill.GetSkillCalc(score);
                dict.Add(skill.Identifier,  calculator.SkillCalc());
            }
            return dict;
        }

        public static Dictionary<string, double> GetEmptyAlltrickPP()
        {
            Dictionary<string, double> dict = new Dictionary<string, double>();
            foreach (ISkill skill in SkillList)
            {
                dict.Add(skill.Identifier, 0);
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
