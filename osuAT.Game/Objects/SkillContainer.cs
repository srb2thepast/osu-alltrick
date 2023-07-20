using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Protocol.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuAT.Game.Objects;
using osuAT.Game.Screens;
using osuAT.Game.Skills;
using osuAT.Game.Skills.Resources;  
using osuTK;
using osuTK.Input;

namespace osuAT.Game
{
    public partial class SkillContainer : Container
    {
        private Container container;
        public Dictionary<ISkill, SkillBox> SkillDict = new Dictionary<ISkill, SkillBox>();
        public SkillBox FocusedBox;
        public HomeScreen MainScreen;
        protected Container BoxContainer;
        public SkillContainer(HomeScreen mainscreen)
        {
            Size = new Vector2(8000, 5000);
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
            MainScreen = mainscreen;
            foreach (ISkill skill in Skill.SkillList)
            {
                SkillDict.Add(
                    skill,
                    new SkillBox
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Skill = skill,
                    }
                );
            }

        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            lastoffpos = new Vector2(Size.X / 2, 0);
            Child = container = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Scale = lastscale,
                Children = new Drawable[]
                {
                    new Box {

                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0.3f
                    },
                    BoxContainer = new Container {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.Both,
                        Position = -lastoffpos
                    }
                }
            };
            // load each skill
            foreach (var box in SkillDict.Values)
            {
                box.ParentCont = this;
                BoxContainer.Add(box);
            }
            // add arrows
            // main, target, rotation, offset, Segments
            List<ArrowInfo> connectionList = new List<ArrowInfo> {
                // Aim -> Cursor Control
                new ArrowInfo {
                    MainSkill = Skill.Aim,
                    TargetSkill = Skill.CursorControl,
                    Rotation = Direction.Left,
                    Offset = Vector2.Zero,
                    Colour = Skill.Aim.PrimaryColor,
                },
                // Cursor Control -> Precision
                new ArrowInfo {
                    MainSkill = Skill.CursorControl,
                    TargetSkill = Skill.Precision,
                    Rotation = Direction.Down,
                    Colour = Skill.CursorControl.PrimaryColor,
                    Segments = new List<Segment> {
                        new Segment{
                            Start = 50,
                            Length = 700,
                            Rotation = Direction.Down
                        },
                    }
                },

                // Cursor Control -> Slider Aim
                new ArrowInfo
                {
                    MainSkill = Skill.CursorControl,
                    TargetSkill = Skill.SliderAim,
                    Rotation = 0,
                    Offset = new Vector2(0, -110),
                    Colour = Skill.CursorControl.PrimaryColor,
                    Segments = new List<Segment> {
                        new Segment{
                            Start = 50,
                            Length = 737,
                            Rotation = Direction.Left
                        },
                        new Segment{
                            Start = 50,
                            Length = 111,
                            Rotation = Direction.Up
                        },
                    }
                },

                // Aim -> Aim Stamina
                new ArrowInfo {
                    MainSkill = Skill.Aim,
                    TargetSkill = Skill.AimStamina,
                    Rotation = 0,
                    Offset = new Vector2(-100,90),
                    Colour = Skill.Aim.PrimaryColor,
                    Segments = new List<Segment> {
                        new Segment{
                            Start = 50,
                            Length = 1050+455,
                            Rotation = Direction.Down
                        },
                        new Segment{
                            Start = 100,
                            Length = 385,
                            Rotation = Direction.Left
                        }
                    }
                },
            };
            foreach (ArrowInfo connect in connectionList)
            {
                SkillBox mainBox = SkillDict[connect.MainSkill];
                SkillBox targetBox = SkillDict[connect.TargetSkill];
                Arrow ar = AddArrow(mainBox, targetBox, (float)connect.Rotation, connect.Colour, connect.Segments);
                ar.Position += connect.Offset;
            }



            BoxContainer.Add(
            new Sprite
            {

                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0.3f,
                // Texture = textures.Get(@"Contributors/chart")
            });
        }
        protected enum Direction
        {
            Up = 180,
            Down = 0,
            Left = 90
        }

        protected struct ArrowInfo
        {
            public ISkill MainSkill;
            public ISkill TargetSkill;
            public Direction Rotation;
            public Vector2 Offset;
            public List<Segment> Segments;
            public Colour4 Colour;
        }

        protected struct Segment
        {
            /// <summary>
            /// % start compared to the previous Segment.
            /// </summary>
            public float Start;
            public float Length;
            public Direction Rotation;
        }

        protected Arrow AddArrow(SkillBox mainBox, SkillBox targetBox, float rotation, Colour4 color, List<Segment> Segments = null)
        {
            Arrow newArrow = new Arrow
            {
                Length = (mainBox.X - targetBox.X) - mainBox.MiniBox.Width * 2,
                Anchor = Anchor.Centre,
                Rotation = rotation,
                Segments = Segments,
                Colour = color,
                Position = new Vector2(mainBox.X - 8, mainBox.Y - 14),
                Depth = 100
            };
            BoxContainer.Add(newArrow);
            return newArrow;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
        }

        public void FocusOnBox(SkillBox box)
        {
            Vector2 BoxScreenPos = new Vector2(
                (Size.X / 2 + box.Position.X),
                (box.Position.Y));
            BoxContainer.MoveTo(-BoxScreenPos, 400, Easing.OutCubic);
            FocusedBox?.MiniBox.OnDefocus();
            Task.Run(async () =>
            {
                await Task.Delay(200);
                Schedule(() => box.MiniBox.OnFocus());
            });
            FocusedBox = box;
            lastoffpos = BoxScreenPos;
        }

        public void FocusToClosest(Vector2 bias)
        {
            FocusedBox ??= SkillDict[Skill.Flowaim];
            SkillBox closest = FocusedBox;
            float closestDist = 100000;
            foreach (SkillBox box in SkillDict.Values)
            {
                if (box == FocusedBox) continue;
                float curBoxDist = (bias * (box.Position - FocusedBox.Position)).Length;
                if (closest == null || curBoxDist < closestDist)
                {
                    if (Math.Abs(bias.Y) > Math.Abs(bias.X))
                    {
                        if (Math.Sign((box.Position - FocusedBox.Position).Y) == -Math.Sign(bias.Y)) // up/down
                        {
                            closest = box;
                            closestDist = (bias * (closest.Position - FocusedBox.Position)).Length;
                        }
                    }
                    else
                    {
                        if (Math.Sign((box.Position - FocusedBox.Position).X) == -Math.Sign(bias.X)) // left/right
                        {
                            closest = box;
                            closestDist = (bias * (closest.Position - FocusedBox.Position)).Length;
                        }
                    }
                }
                Console.WriteLine(box.Skill.Identifier + ".Y <" + FocusedBox.Skill.Identifier + ".Y");
                Console.WriteLine(curBoxDist + "<" + closestDist);
            }
            FocusOnBox(closest);
            return;
        }

        public void Defocus()
        {
            FocusedBox = null;
        }

        protected partial class Arrow : CompositeDrawable
        {

            public float Length = 0;

            // <Segment Start (% of Segment length), Segment length, Segment Rotation>
            public List<Segment> Segments;

            public Arrow()
            {
                Origin = Anchor.TopCentre;
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                Length += 20;
                RelativeSizeAxes = Axes.Both;
                // straight ahead (no Segments)
                if (Segments == null)// arrowhead
                {
                    var ar = new Sprite
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.Centre,
                        Y = -2,
                        Scale = new Vector2(1.2f),
                        Rotation = 180,
                        Colour = Colour,
                        Texture = textures.Get("FigmaVectors/ArrowHead.png")
                    }.WithEffect(new GlowEffect { BlurSigma = new Vector2(2f), Strength = 2, PadExtent = true, Colour = Colour });
                    ar.Padding = new MarginPadding { Bottom = 40 };
                    InternalChildren = new Drawable[] {
                        new Container {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Size = new Vector2(16, Length),
                            Colour = Colour,
                            Children = new Drawable[] {
                                new Box {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Size = new Vector2(16, Length),
                                    Colour = Colour,
                                }.WithEffect(new GlowEffect { BlurSigma = new Vector2(2f), Strength = 2, PadExtent = true, Colour = Colour }),
                                ar
                            }
                        },
                    };
                }
                else
                {
                    _ = new Circle()
                    {

                    };
                    // Vector2 lastrotdir = new Vector2(1);
                    Vector2 lastEndPosition = Vector2.Zero;
                    int i = 0;
                    foreach (Segment Segment in Segments)
                    {
                        Circle curSegmentBox = new Circle
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Height = Segment.Length,
                            Rotation = (float)Segment.Rotation,
                            Width = 16,
                            Position = lastEndPosition,
                            Colour = Colour
                        };
                        var lastSegmentBox = curSegmentBox;
                        Vector2 curEndPosition = curSegmentBox.Position +
                            (new Vector2(
                                (float)Math.Sin((-(double)Segment.Rotation) * Math.PI / 180),
                                (float)Math.Cos((-(double)Segment.Rotation) * Math.PI / 180)) * (Segment.Length - 4f)
                             );
                        AddInternal(curSegmentBox.WithEffect(new GlowEffect { BlurSigma = new Vector2(2f), Strength = 10, PadExtent = true, Colour = Colour }));
                        if (i < Segments.Count - 1)
                        {
                            /*AddInternal(new Circle
                            {
                                Colour = Colour,
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(18),
                                Position = curEndPosition,
                            }.WithEffect(new GlowEffect { Strength = 2, PadExtent = true, Colour = Colour }));*/
                        }
                        else
                        {
                            var ar = new Sprite
                            {
                                Anchor = Anchor.BottomCentre,
                                Origin = Anchor.Centre,
                                Y = -2,
                                Scale = new Vector2(1.2f),
                                Rotation = 180,
                                Colour = Colour,
                                Texture = textures.Get("FigmaVectors/ArrowHead.png")
                            }.WithEffect(new GlowEffect { BlurSigma = new Vector2(2f), Strength = 2, PadExtent = true, Colour = Colour });
                            ar.Padding = new MarginPadding { Bottom = 40 };
                            AddInternal(new Container
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Size = new Vector2(16, Length),
                                Height = Segment.Length,
                                Rotation = (float)Segment.Rotation,
                                Position = lastEndPosition,
                                Children = new Drawable[] {
                                    new Box
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Height = Segment.Length,
                                        Width = 16,
                                        Colour = Colour
                                    }.WithEffect(new GlowEffect { BlurSigma = new Vector2(2f), Strength = 2, PadExtent = true, Colour = Colour }),
                                    ar
                                }
                            });
                        }
                        Console.WriteLine(curEndPosition);
                        lastEndPosition = curEndPosition;
                        i++;
                    }
                };
            }

        }

        #region Input Handlers
        private Vector2 lastoffpos = Vector2.Zero;
        private Vector2 lastscale = new Vector2(0.2f);
        protected override bool OnDragStart(DragStartEvent e)
        {
            if ((!(FocusedBox?.State == SkillBoxState.FullBox) || (FocusedBox == null)) && MainScreen.CurrentlyFocused == true)
            {
                FocusedBox = null; // if you start dragging, the box will defocus.
                return true;
            }
            return false;
        }
        protected override void OnDrag(DragEvent e)
        {
            Vector2 newPos = ((-e.MousePosition + e.MouseDownPosition) * (1 / Child.Scale.X) + lastoffpos);
            newPos = new Vector2(Math.Clamp(newPos.X, 0, 8000), Math.Clamp(newPos.Y, -2500, 2500));
            BoxContainer.MoveTo(-newPos);

        }
        protected override void OnDragEnd(DragEndEvent e)
        {
            base.OnDragEnd(e);
            lastoffpos = -BoxContainer.Position;

        }
        protected override bool OnScroll(ScrollEvent e)
        { // one day... scrolling will be good... right...!??!?!??!?!?
            // we did it
            // scrolling...
            // is finally...
            // GOOD!!!!!!!!!!!!!!!!!!!!!!!!!
            if (MainScreen.CurrentlyFocused == false) return false;
            if (FocusedBox?.State == SkillBoxState.FullBox) { return false; }
            Vector2 newScale = lastscale + new Vector2(e.ScrollDelta.Y / 10, e.ScrollDelta.Y / 10);

            lastscale = ((newScale.X < 0.2 && newScale.Y < 0.2) || (newScale.X > 1.5 && newScale.Y > 1.5)) ? lastscale : newScale;


            //Child.MoveTo(BoxScreenPos, 300, Easing.OutExpo);
            //lastoffpos = BoxScreenPos;
            Child.ScaleTo(lastscale, 300, Easing.OutExpo);
            return true;
        }
        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (MainScreen.CurrentlyFocused == false) return false;
            if (FocusedBox?.State == SkillBoxState.FullBox) { return false; }
            if (e.Key == Key.W || e.Key == Key.Up)
            {
                FocusToClosest(new Vector2(0.3f, 1.5f));
                return true;
            }
            if (e.Key == Key.A || e.Key == Key.Left)
            {
                FocusToClosest(new Vector2(1.5f, 0.3f));
                return true;
            }
            if (e.Key == Key.S || e.Key == Key.Down)
            {
                FocusToClosest(new Vector2(0.3f, -1.5f));
                return true;
            }
            if (e.Key == Key.D || e.Key == Key.Right)
            {
                FocusToClosest(new Vector2(-1.5f, 0.3f));
                return true;
            }
            if (e.Key == Key.Enter)
            {
                FocusedBox?.MiniBox.TryTransition();
            }
            return false;

        }
        #endregion
    }
}
