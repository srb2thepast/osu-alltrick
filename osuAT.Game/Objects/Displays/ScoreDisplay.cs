using System;
using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuAT.Game.Objects.LazerAssets.StarRating;
using osuAT.Game.Objects.LazerAssets.Mod;
using osuAT.Game.Objects.LazerAssets;
using osuAT.Game.Types;
using osuAT.Game.Skills;
using osuTK;

namespace osuAT.Game.Objects.Displays
{
    public class ScoreDisplay : CompositeDrawable
    {

        public Score Current { get; set; }
        public ISkill Skill { get; set; }
        public int IndexPos { get; set; } = 0;

        public int TextSize; // Text Size

        public Texture Background; // Background
        public ScoreDisplay()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
        }

        private Container starDisplayContainer;
        private Container scoreInfo;
        private Container diffInfo;
        private Container display;
        private Circle comboBar;

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            InternalChild = display = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,

                Children = new Drawable[] {
                    // Beatmap Difficulty Info
                    diffInfo = new Container{
                        Size = new Vector2(210,25),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Masking = true,
                        CornerRadius = 10,
                        Position = new Vector2(0,25),
                        Children = new Drawable[] {
                            new Box {
                                Size = new Vector2(210,25),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,

                                Colour = Skill.PrimaryColor,
                            },

                            new FillFlowContainer {
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(3, 0),
                                Position = new Vector2(15,11),
                                AutoSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Spacing = new Vector2(-0.3f,0),
                                        Text = Current.BeatmapInfo.DifficultyName,
                                        Shadow = true,
                                        ShadowOffset = new Vector2(0,0.1f),
                                        Truncate = true,
                                        Width = Math.Clamp((Current.BeatmapInfo.DifficultyName.Length)*2+16,40,100),
                                        Font = new FontUsage("ChivoBold",size: 13),
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft
                                    },
                                    new SpriteText
                                    {
                                        Spacing = new Vector2(-0.3f,0),
                                        Text = "mapped by "+Current.BeatmapInfo.MapsetCreator,
                                        Shadow = true,
                                        ShadowOffset = new Vector2(0,0.1f),
                                        Truncate = true,
                                        
                                        Colour = Colour4.FromHex("#DDFFF4"),
                                        Font = new FontUsage("ChivoBold",size: 11),
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft
                                    },
                                }
                            }
                        }
                    },

                    // Score Info
                    scoreInfo = new Container{
                        Size = new Vector2(252,50),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Masking = true,
                        X = 20,
                        CornerRadius = 10,
                        Children = new Drawable[] {

                            // Background
                            new BufferedContainer {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Children = new Drawable[] {
                                    // Gray Box
                                    new Box
                                    {
                                        Size = new Vector2(345,220),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,

                                    },
                                    
                                    // Image
                                    new Sprite {
                                        Size = new Vector2(352, 224),
                                        X = 20,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Texture = textures.Get("TestBG"),
                                    },

                                    // Gradient
                                    new Box
                                    {
                                        Size = new Vector2(400,240),
                                        X = 0,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,

                                        Colour = new osu.Framework.Graphics.Colour.ColourInfo{
                                            BottomLeft = Colour4.FromHex("#45544F"),
                                            TopLeft = Colour4.FromHex("#45544F"),
                                            BottomRight = Colour4.FromHex("#45544F").MultiplyAlpha(0.5f),
                                            TopRight = Colour4.FromHex("#45544F").MultiplyAlpha(0.5f),
                                        }
                                    },
                                }
                            },

                            // Ruleset Icon
                            new Container{
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre, 
                                Origin = Anchor.Centre,
                                Children = new Drawable[] {
                                    // Background
                                    new BufferedContainer {
                                        Size = new Vector2(45,45),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Masking = true,
                                        CornerRadius = 22.5f,
                                        X = -100,
                                        Children = new Drawable[] {
                                    

                                    
                                            // Image
                                            new Sprite {
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Size = new Vector2(70,45),
                                                Texture = textures.Get("TestBG2"),
                                                Alpha = 1
                                            },

                                            // Gradient Cover
                                            new Box
                                            {
                                                Size = new Vector2(45,80),
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,

                                                Colour = new osu.Framework.Graphics.Colour.ColourInfo{
                                                    BottomLeft = Colour4.FromHex("#222A27").MultiplyAlpha(1),
                                                    BottomRight = Colour4.FromHex("#222A27").MultiplyAlpha(1),
                                                    TopLeft = Colour4.FromHex("#222A27").MultiplyAlpha(0),
                                                    TopRight = Colour4.FromHex("#222A27").MultiplyAlpha(0),
                                                }
                                            },
                                        }
                                    },
                                    (Current.BeatmapInfo.FolderLocation != default && File.Exists(SaveStorage.ConcateOsuPath(Current.BeatmapInfo.FolderLocation)) )? 
                                    // Icon
                                    new Container {
                                        AutoSizeAxes = Axes.Both,
                                        Anchor = Anchor.Centre, 
                                        Origin = Anchor.Centre,
                                        Child = new SpriteIcon {

                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Size = new Vector2(38,38),
                                            Colour = new osu.Framework.Graphics.Colour.ColourInfo{
                                                BottomLeft = Colour4.FromHex("#F0A8AE"),
                                                TopLeft = Colour4.FromHex("#F0A8AE"),
                                                BottomRight = Colour4.FromHex("#FBE7E9"),
                                                TopRight = Colour4.FromHex("#FBE7E9")
                                            },
                                            Icon = Current.ScoreRuleset.Icon,
                                            X = -100
                                        }
                                    } :
                                    new Container {
                                        AutoSizeAxes = Axes.Both,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Child = new SpriteIcon {

                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Size = new Vector2(38,38),
                                            Colour = new osu.Framework.Graphics.Colour.ColourInfo{
                                                BottomLeft = Colour4.FromHex("#F0A8AE"),
                                                TopLeft = Colour4.FromHex("#F0A8AE"),
                                                BottomRight = Colour4.FromHex("#FBE7E9"),
                                                TopRight = Colour4.FromHex("#FBE7E9")
                                            },
                                            Icon = OsuIcon.CrossCircle,
                                            X = -100
                                        }
                                    }
                                    ,
                                }
                            },

                            // Song and Artist Name
                            new Container{

                                // 1st layer
                                new FillFlowContainer {
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(3, 0),
                                    Position = new Vector2(50,3f),
                                    Width = 180,
                                    Masking = true,
                                    AutoSizeAxes = Axes.Y,
                                    Children = new Drawable[]
                                    {
                                        new SpriteText
                                        {
                                            Spacing = new Vector2(-0.3f,0),
                                            Text = Current.BeatmapInfo.SongName,
                                            Truncate = true,
                                            MaxWidth = 105,
                                            //Width = 95,
                                            Font = new FontUsage("VarelaRound",size: 13),
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft
                                        },
                                        new SpriteText
                                        {
                                            Spacing = new Vector2(-0.3f,0),
                                            Text = "by "+ Current.BeatmapInfo.SongArtist,
                                            Truncate = true,
                                            MaxWidth = 69,
                                            Font = new FontUsage("VarelaRound",size: 11),
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft
                                        },
                                    }
                                },

                                // 2nd layer
                                new FillFlowContainer {
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(3, 0),
                                    Position = new Vector2(50,3.1f),
                                    Width = 180,
                                    Masking = true,
                                    AutoSizeAxes = Axes.Y,
                                    Children = new Drawable[]
                                    {
                                        new SpriteText
                                        {
                                            Spacing = new Vector2(-0.3f,0),
                                            Text = Current.BeatmapInfo.SongName,
                                            Truncate = true,
                                            MaxWidth = 105,
                                            //Width = 95,
                                            Font = new FontUsage("VarelaRound",size: 13),
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft
                                        },
                                        new SpriteText
                                        {
                                            Spacing = new Vector2(-0.3f,0),
                                            Text = "by "+ Current.BeatmapInfo.SongArtist,
                                            Truncate = true,
                                            MaxWidth = 69,
                                            Font = new FontUsage("VarelaRound",size: 11),
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft
                                        },
                                    }
                                },
                            },

                            // Grade Display
                            new DrawableRank(Current.Grade) {
                                Scale = new Vector2(0.3f),
                                Position = new Vector2(215,3)
                            },

                            // Performance, Star Rating, and Mod Displays
                            new FillFlowContainer {
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(3, 0),
                                Position = new Vector2(50,18),
                                AutoSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                      
                                    // Performance Display
                                    new Container{
                                        AutoSizeAxes = Axes.Both,
                                        Children = new Drawable[] {
                                            // Shadow
                                            new Circle {
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = Colour4.Black,
                                                Y = 1.9f,
                                                Alpha = 0.3f

                                            },
                                            new PerformanceDisplay(Current.AlltrickPP[Skill.Identifier]) { Scale = new Vector2(0.75f)},
                                        }
                                    },

                                    // Star Rating Display
                                    starDisplayContainer = new Container{
                                        AutoSizeAxes = Axes.Both,
                                        Children = new Drawable[] {
                                            // Shadow
                                            new Circle {
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = Colour4.Black,
                                                Y = 1.5f,
                                                Alpha = 0.4f

                                            },
                                    
                                            // Display
                                            new StarRatingDisplay(new StarDifficulty(Current.BeatmapInfo.StarRating), StarRatingDisplaySize.Regular) {
                                                Scale = new Vector2(0.75f),
                                            },
                                            
                                        }
                                    },

                                    // Mod Display
                                    new ModDisplay {
                                        Scale = new Vector2(0.4f),
                                        Current = { Value = Current.Mods},
                                    },
                                }
                            },

                            new SpriteText {
                                Origin = Anchor.Centre,
                                Position = new Vector2(229,27),
                                Spacing = new Vector2(-0.3f,0),
                                Text = "#"+(IndexPos+1).ToString(),
                                Font = new FontUsage("ChivoBold",size: 13),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                            },

                            // Combo Bar
                            new Container{
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(50,17),
                                Height = 8,
                                Children = new Drawable[] {
                                    // Background
                                    new Circle {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        RelativeSizeAxes = Axes.Y,
                                        Width = 175,
                                        Colour = Colour4.Black,
                                        Alpha = 0.5f

                                    },

                                    // Combo Bar
                                    new Container{
                                        AutoSizeAxes = Axes.X,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Height = 8,
                                        Alpha = 1,
                                        Children = new Drawable[] {

                                            // Bar
                                            comboBar = new Circle {
                                                RelativeSizeAxes = Axes.Y,
                                                Width = 0,
                                                Colour = Skill.PrimaryColor
                                            },

                                            // Combo Number
                                            new CircularContainer
                                            {
                                                Masking = true,
                                                AutoSizeAxes = Axes.Both,
                                                Anchor = Anchor.CentreRight,
                                                Origin = Anchor.CentreRight,
                                                Children = new Drawable[]
                                                {
                                                    new Box
                                                    {
                                                        RelativeSizeAxes = Axes.Both,
                                                        Colour = Skill.PrimaryColor + new Colour4(30,30,30,0)
                                                    },
                                                    new FillFlowContainer
                                                    {
                                                        AutoSizeAxes = Axes.Both,
                                                        Anchor = Anchor.CentreRight,
                                                        Origin = Anchor.CentreRight,
                                                        Direction = FillDirection.Horizontal,
                                                        Spacing = new Vector2(0.5f, 0),
                                                        Margin = new MarginPadding { Horizontal = 4f, Vertical = 0.1f },
                                                        Children = new Drawable[]
                                                        {

                                                            new SpriteText
                                                            {
                                                                Anchor = Anchor.CentreRight,
                                                                Origin = Anchor.CentreRight,
                                                                Spacing = new Vector2(-0.3f,0),
                                                                Text = "x",
                                                                Font = new FontUsage("ChivoBold",size: 10),
                                                                Colour = (Current.Combo == Current.BeatmapInfo.MaxCombo)? Colour4.FromHex("#FFD966") : Colour4.HotPink,
                                                                Shadow = true,
                                                                ShadowOffset = new Vector2(0,0.1f),
                                                            },
                                                            new SpriteText
                                                            {
                                                                Anchor = Anchor.CentreRight,
                                                                Origin = Anchor.CentreRight,
                                                                Spacing = new Vector2(-0.3f,0),
                                                                Text = Current.Combo.ToString(),
                                                                Font = new FontUsage("ChivoBold",size: 10),
                                                                Colour = (Current.Combo == Current.BeatmapInfo.MaxCombo)? Colour4.FromHex("#FFD966") : Colour4.HotPink,
                                                                Shadow = true,
                                                                ShadowOffset = new Vector2(0,0.1f),
                                                            },
                                                        }
                                                    },
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            };
            InternalChild.ScaleTo(3);

        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            

        }
        protected override bool OnHover(HoverEvent e)
        {
            return base.OnHover(e);
        }
        public void Appear(float delay = 0) {
            scoreInfo.Alpha = 0;
            diffInfo.Alpha = 0;
            display.Y = 13;

            scoreInfo.Delay(delay).FadeIn(450, Easing.InOutSine);
            diffInfo.Delay(delay).FadeInFromZero(550, Easing.InOutSine);
            display.Delay(delay).MoveToY(0, 450, Easing.InOutSine);
            comboBar.Delay(delay).ResizeTo(new Vector2(((float)Current.Combo / Current.BeatmapInfo.MaxCombo) * 175, comboBar.Height), 500, Easing.InOutQuad);
        }

        public void Disappear(float delay) {
            scoreInfo.Delay(delay).FadeOut();
            diffInfo.Delay(delay).FadeOut();
            display.Delay(delay).MoveToY(13);
            comboBar.Delay(delay).ResizeTo(new Vector2(0, comboBar.Height));
        }

    }
}
