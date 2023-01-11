using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Objects;
using osuAT.Game.Objects.Displays;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game.Skills.Resources
{
    // When a skill is ready to be used, dont forget to add it to Skill.cs   
    internal partial class DefaultContributorPage : Page
    {
        public ISkill Skill;
        public Contributor[] Contribs;
        public DefaultContributorPage()
        {
            Index = 2;
            Name = "Page2";
            Size = new Vector2(130, 180);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            var flow = new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Y = 20,
                Size = new Vector2(63, 180),
                Spacing = new Vector2(54),
                Direction = FillDirection.Full,
            };
            Children = new Drawable[] {    
                // Big Thank You
                new SpriteText
                {
                    Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Y = -45,
                        Text = "Thank you so much",
                        Font = new FontUsage("VarelaRound", size:15),
                        Shadow = true,
                        ShadowOffset = new Vector2(0, 0.05f),
                        Spacing = new Vector2(0.1f, 0),

                    }.WithEffect(new OutlineEffect
                    {
                        BlurSigma = new Vector2(0.5f),
                        Strength = 5,
                        Colour = Colour4.White,
                        PadExtent = true,
                    }).WithEffect(new GlowEffect
                    {
                        BlurSigma = new Vector2(0.5f),
                        Strength = 5,
                        Colour = ColourInfo.GradientHorizontal(Skill.PrimaryColor, Skill.SecondaryColor),
                        PadExtent = true,

                    }),
                // Big Thank You (2)
                new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = -34,
                    Text = "for contributing code to this skill.",
                    Font = new FontUsage("VarelaRound", size:10),
                    Shadow = true,
                    ShadowOffset = new Vector2(0, 0.05f),
                    Spacing = new Vector2(0.1f, 0),

                }.WithEffect(new OutlineEffect
                {
                    BlurSigma = new Vector2(0.5f),
                    Strength = 5,
                    Colour = Colour4.White,
                    PadExtent = true,
                }).WithEffect(new GlowEffect
                {
                    BlurSigma = new Vector2(0.5f),
                    Strength = 5,
                    Colour = ColourInfo.GradientHorizontal(Skill.PrimaryColor, Skill.SecondaryColor),
                    PadExtent = true,

                }),
                flow
                };

            var position = new Vector2(-35, 10);
            if (Contribs.Length == 0)
            {
                Add(new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = 0,
                    Text = "oops, looks like there's no one",
                    Font = new FontUsage("VarelaRound", size: 10),
                    Shadow = true,
                    ShadowOffset = new Vector2(0, 0.05f),
                    Spacing = new Vector2(0.1f, 0),

                });
                Add(new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = 9,
                    Text = "here. Go to srb2thepast/osu-a",
                    Font = new FontUsage("VarelaRound", size: 10),
                    Shadow = true,
                    ShadowOffset = new Vector2(0, 0.05f),
                    Spacing = new Vector2(0.1f, 0),

                });
                Add(new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = 18,
                    Text = "lltrick on github and help out",
                    Font = new FontUsage("VarelaRound", size: 10),
                    Shadow = true,
                    ShadowOffset = new Vector2(0, 0.05f),
                    Spacing = new Vector2(0.1f, 0),

                });
                Add(new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = 27,
                    Text = "to be the first!",
                    Font = new FontUsage("VarelaRound", size: 10),
                    Shadow = true,
                    ShadowOffset = new Vector2(0, 0.05f),
                    Spacing = new Vector2(0.1f, 0),

                });
                return;
            }
            var i = 0;
            foreach (var Contrib in Contribs)
            {
                i++;
                flow.Add(new ContributorDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,

                    Skill = Skill,
                    Position = position,
                    CornerRadius = 10,
                    Cont = Contrib,
                    ProfileSize = new Vector2(34),
                });
                position = (i - 1) % 2 == 0 ? new Vector2(position.X + i, position.Y) : new Vector2(position.X, position.Y + i);

            }

        }
    }
}
