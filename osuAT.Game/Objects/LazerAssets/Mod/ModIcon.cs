// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
/* License text:
Copyright (c) 2022 ppy Pty Ltd <contact@ppy.sh>.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osuAT.Game.Types;
using osuTK;
using osuTK.Graphics;

namespace osuAT.Game.Objects.LazerAssets.Mod
{
    /// <summary>
    /// Display the specified mod at a fixed size.
    /// </summary>
    public partial class ModIcon : Container, IHasTooltip
    {
        public readonly BindableBool Selected = new BindableBool();

        private readonly SpriteIcon modIcon;
        private readonly SpriteText modAcronym;
        private readonly SpriteIcon background;

        private const float size = 80;

        public virtual LocalisableString TooltipText => showTooltip ? mod.Name : null;

        private ModInfo mod;
        private readonly bool showTooltip;

        public ModInfo Mod
        {
            get => mod;
            set
            {
                mod = value;

                if (IsLoaded)
                    updateMod(value);
            }
        }

        private Color4 backgroundColour;
        private Color4 highlightedColour;

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="mod">The mod to be displayed</param>
        /// <param name="showTooltip">Whether a tooltip describing the mod should display on hover.</param>
        public ModIcon(ModInfo mod, bool showTooltip = true)
        {
            this.mod = mod ?? throw new ArgumentNullException(nameof(mod));
            this.showTooltip = showTooltip;

            Size = new Vector2(size);

            Children = new Drawable[]
            {
                background = new SpriteIcon
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Size = new Vector2(size),
                    Icon = OsuIcon.ModBg,
                    Shadow = true,
                },
                modAcronym = new SpriteText
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Colour = new Color4(84, 84, 84, 1f),
                    Alpha = 0,
                    Font = new FontUsage("VarelaRound", size: 22f,weight: "Bold"), // Font Should be Venera
                    UseFullGlyphHeight = false,
                    Text = mod.Acronym
                },
                modIcon = new SpriteIcon
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Colour = new Color4(84, 84, 84, 255),
                    Size = new Vector2(45),
                    Icon = FontAwesome.Solid.Question
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Selected.BindValueChanged(_ => updateColour());

            updateMod(mod);
        }

        private void updateMod(ModInfo value)
        {
            modAcronym.Text = value.Acronym;
            modIcon.Icon = value.Icon ?? FontAwesome.Solid.Question;

            if (value.Icon is null)
            {
                modIcon.FadeOut();
                modAcronym.FadeIn();
            }
            else
            {
                modIcon.FadeIn();
                modAcronym.FadeOut();
            }

            switch (value.Type)
            {
                default:
                case ModType.DifficultyIncrease:
                    backgroundColour = Color4Extensions.FromHex(@"ffcc22");
                    highlightedColour = Color4Extensions.FromHex(@"ffdd55");
                    break;

                case ModType.DifficultyReduction:
                    backgroundColour = Color4Extensions.FromHex(@"88b300");
                    highlightedColour = Color4Extensions.FromHex(@"b3d944");
                    break;

                case ModType.Automation:
                    backgroundColour = Color4Extensions.FromHex(@"66ccff");
                    highlightedColour = Color4Extensions.FromHex(@"99eeff");
                    break;

                case ModType.Conversion:
                    backgroundColour = Color4Extensions.FromHex(@"8866ee");
                    highlightedColour = Color4Extensions.FromHex(@"aa88ff");
                    break;

                case ModType.Fun:
                    backgroundColour = Color4Extensions.FromHex(@"ff66aa");
                    highlightedColour = Color4Extensions.FromHex(@"ff99cc");
                    break;

                case ModType.System:
                    backgroundColour = Color4Extensions.FromHex(@"666");
                    highlightedColour = Color4Extensions.FromHex(@"777");
                    modIcon.Colour = Color4Extensions.FromHex(@"ffcc22");
                    break;
            }

            updateColour();
        }

        private void updateColour()
        {
            background.Colour = Selected.Value ? highlightedColour : backgroundColour;
        }
    }
}
