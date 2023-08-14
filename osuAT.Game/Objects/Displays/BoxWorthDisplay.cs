using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuAT.Game.Objects;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game.Objects
{
    public partial class BoxWorthDisplay : CompositeDrawable
    {
        private int scorePP;
        private int scorePlacement;

        public BoxWorthDisplay(int scorepp, int scoreplace)
        {
            scorePP = scorepp;
            scorePlacement = scoreplace;
            if (scoreplace <= 0)
            {
                scorePlacement = -1;
            }

            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
            Size = new Vector2(280, scorePlacement == -1? 100 : 180);
            Alpha = 0;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            Colour4 dropShadCol = Colour4.White;
            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Children = new Drawable[] {
                    // box drop shadow
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Y = 4,
                        CornerRadius = 30,
                        Masking = true,
                        BorderThickness = 6,
                        BorderColour = dropShadCol,
                        Children = new Drawable[] {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = dropShadCol
                            },
                        }
                    },
                    // square drop shadow
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.BottomCentre,
                        Scale = new Vector2(1.1f),
                        Y = 8,
                        Children = new Drawable[] {
                            new Container
                                {
                                Size = new Vector2(40),
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                CornerRadius = 7,
                                Masking = true,
                                Rotation= 45,
                                Y = 5,
                                BorderThickness = 6,
                                BorderColour = dropShadCol,
                                Child = new Box {
                                    Origin = Anchor.Centre,
                                    Anchor = Anchor.Centre,
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = dropShadCol
                                },
                            },
                        }
                    },
                    // square arrow
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.BottomCentre,
                        Scale = new Vector2(1.1f),
                        Y = 2,
                        Children = new Drawable[] {
                            new Container
                                {
                                Size = new Vector2(40),
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                CornerRadius = 7,
                                Masking = true,
                                Rotation= 45,
                                Y = 5,
                                BorderThickness = 6,
                                BorderColour = dropShadCol,
                                Child = new Box {
                                    Origin = Anchor.Centre,
                                    Anchor = Anchor.Centre,
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Colour4.FromHex("FF96C5"),
                                },
                            },
                        }
                    },
                    // insides
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        CornerRadius = 40,
                        Masking = true,
                        BorderThickness = 8,
                        BorderColour = dropShadCol,
                        Children = new Drawable[] {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Colour4.FromHex("FF96C5"),
                            },
                            new Circle
                            {
                                Alpha = (scorePlacement== -1)? 0: 1,
                                Size = new Vector2(200,6),
                                Y = 3,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Colour4.Black.MultiplyAlpha(0.25f),
                            },
                            new Circle
                            {
                                Alpha = (scorePlacement== -1)? 0: 1,
                                Size = new Vector2(200,6),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Colour4.White,
                            },
                            new Container {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Alpha = (scorePlacement <= 10 && scorePlacement != -1)? 1: 0,
                                Children = new Drawable[] {
                                    new Sprite {
                                        Size = new Vector2(38),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        X = -100,
                                        Y = 0,
                                        Texture = textures.Get("FigmaVectors/StarFull"),
                                        Colour = Colour4.Black.MultiplyAlpha(0.25f)
                                    },
                                    new Sprite {
                                        Size = new Vector2(38),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        X = -100,
                                        Y = -3,
                                        Texture = textures.Get("FigmaVectors/StarFull"),
                                        Colour = Colour4.White
                                    },
                                    new Sprite {
                                        Size = new Vector2(38),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        X = 100,
                                        Y = 0,
                                        Texture = textures.Get("FigmaVectors/StarFull"),
                                        Colour = Colour4.Black.MultiplyAlpha(0.25f)
                                    },
                                    new Sprite {
                                        Size = new Vector2(38),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        X = 100,
                                        Y = -3,
                                        Texture = textures.Get("FigmaVectors/StarFull"),
                                        Colour = Colour4.White
                                    },
                                    new Circle
                                    {
                                        Size = new Vector2(200,6),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Colour = Colour4.White,
                                    },
                                }
                            },

                            new SpriteText
                            {
                                Y = 40,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Alpha = (scorePlacement== -1)? 0: 1,
                                Text= "#" + scorePlacement.ToString(),
                                Font = new FontUsage("VarelaRound", size: 60),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowColour = Colour4.Black.MultiplyAlpha(0.25f)
                            },
                            new SpriteText
                            {
                                Y = (scorePlacement== -1)? 0: -40,
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                Text= scorePP.ToString() + "pp",
                                Font = new FontUsage("VarelaRound", size: 60),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowColour = Colour4.Black.MultiplyAlpha(0.25f)
                            },
                        }
                    },
                    // square arrow masking
                    new Container {
                        RelativeSizeAxes = Axes.Both,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.BottomCentre,
                        Scale = new Vector2(1.1f),
                        Y = 2,
                        Children = new Drawable[] {
                            new Circle {
                                Size = new Vector2(20,8),
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                Y = -2,
                                Colour = Colour4.FromHex("FF96C5"),
                            },
                            new Box {
                                Size = new Vector2(20),
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                Y = -12,
                                Colour = Colour4.FromHex("FF96C5"),
                            },
                            new Circle {
                                Size = new Vector2(35,8),
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                Y = -2,
                                X = -7,
                                Rotation = 60,
                                Colour = Colour4.FromHex("FF96C5"),
                            },
                            new Circle {
                                Size = new Vector2(35,8),
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                Y = -2,
                                X = 7,
                                Rotation = -60,
                                Colour = Colour4.FromHex("FF96C5"),
                                //Colour = Colour4.FromHex("000000"),
                            },
                        }
                    },
                }
            };
        }

        public void Appear()
        {
            this.Y = 30;
            this.FadeIn(00, Easing.OutSine);
            this.MoveToY(0, 500, Easing.OutCubic);
        }
    }
}
