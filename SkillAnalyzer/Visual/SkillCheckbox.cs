using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
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

namespace SkillAnalyzer.Visual
{
    public class SkillCheckbox : OsuCheckbox
    {

        public ISkill Skill;
        public float TextSize = 18;
        private Colour4 nubColor = Colour4.Cyan;
        private Colour4 textColor = Colour4.White;

        public SkillCheckbox(ISkill skill,float textSize=18) {
            Skill = skill;
            TextSize = textSize;
            textColor = skill.PrimaryColor;
            nubColor = skill.PrimaryColor;
            LabelText = skill.Name;

        }

        protected override void ApplyLabelParameters(SpriteText text)
        {
            text.Colour = nubColor;
            text.Shadow = true;
            text.ShadowColour = Colour4.Gray;
            text.ShadowOffset = new Vector2(0,0.1f);
            text.Font = new FontUsage("VarelaRound", size: TextSize);
        }

        [BackgroundDependencyLoader]
        private void load() {
            X = 10;
            Nub.X -= 10;
            Nub.Y += 2;
            Nub.AccentColour = nubColor;
            Nub.GlowColour = nubColor.Lighten(0.5f);
            Nub.GlowingAccentColour = nubColor.Lighten(0.5f);
        }

    }
}
