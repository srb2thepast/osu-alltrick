using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using osuAT.Game.Skills;
using osuTK;
using osuTK.Graphics;

namespace SkillAnalyzer.Visual
{
    public class SkillCheckbox : OsuCheckbox
    {

        public ISkill Skill;
        public float TextSize = 18;
        private Colour4 nubColor = Colour4.Cyan;
        private Colour4 textColor = Colour4.White;

        public SkillCheckbox(ISkill skill,float textSize=18)
        {
            Skill = skill;
            TextSize = textSize;
            textColor = skill.PrimaryColor;
            nubColor = skill.PrimaryColor;
            LineCount -= skill.Name.Split(' ').Length - 1;
            Console.WriteLine(LineCount);
            LabelText = skill.Name;
        }
        public int LineCount = 1;
        protected override void ApplyLabelParameters(SpriteText text)
        {
            LineCount++;
            Console.WriteLine(LineCount + "|" + Skill.Identifier);
            text.Colour = nubColor;
            text.Shadow = true;
            text.ShadowColour = Colour4.Gray;
            text.ShadowOffset = new Vector2(0,0.1f);
            text.Font = new FontUsage("VarelaRound", size: TextSize);
            text.Truncate = true;
        }

        [BackgroundDependencyLoader]
        private void load() {
            X = 10;
            Nub.X -= -5;
            Nub.Y = 2*(LineCount-1);
            Nub.AccentColour = nubColor;
            Nub.GlowColour = nubColor.Lighten(0.5f);
            Nub.GlowingAccentColour = nubColor.Lighten(0.5f); 
        }

    }
}
