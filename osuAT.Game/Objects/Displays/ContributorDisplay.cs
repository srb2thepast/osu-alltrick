using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Graphics.Colour;
using osuAT.Game.Types;
using osuAT.Game.Skills;
using osu.Framework.Input.Events;
using osuTK;



namespace osuAT.Game.Objects.Displays
{
    public class ContributorDisplay : CompositeDrawable
    {

        public ISkill Skill;
        public Contributor Cont;
        public Vector2 ProfileSize;
        public new float CornerRadius;

        public ContributorDisplay()
        {

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {

            InternalChild = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[] {
                            new Container{
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Masking = true,
                                CornerRadius = CornerRadius,
                                Child = new Sprite {
                                    Size = ProfileSize,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Texture = textures.Get(Cont.ProfilePicture),
                                },
                            },
                            new SpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Y = 22,
                                Text = Cont.DisplayName,
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
                            new SpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Y = 30.5f,
                                Text = Cont.Hours.ToString()+" Hours",
                                Font = new FontUsage("VarelaRound", size:8),
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
                        }
            };
        }
    }

}
