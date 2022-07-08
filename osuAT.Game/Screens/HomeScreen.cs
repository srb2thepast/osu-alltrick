using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Effects;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osu.Framework.Graphics.Containers;
using osuTK;
using osuTK.Graphics;
using osuAT.Game.Objects;

namespace osuAT.Game
{
    public class HomeScreen : Screen
    {
        private Drawable background;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {

            InternalChildren = new Drawable[]
            {
                background = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[] {
                        new Box {
                            Colour = Color4.Violet,
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },
                new DraggableContainer {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.Centre,
                    Children = new Drawable[] {
                        new SkillContainer{}
                    }
                },
                new Container {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.Centre,
                    Y = 75,

                    Children = new Drawable[] {

                        // Username Section Background
                        new Circle {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(601,63),
                            Y = 60
                        },
                        


                        // Title Background 
                        new Circle {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(1110,93),
                        },
                        new Circle {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(1100,83),
                            Colour = Colour4.FromHex("#FF59A4")
                        },
                        /////////////

                        // PFP
                        new Circle {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(100,100)

                        },
                        new Circle {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(90,90),
                            Colour = Colour4.FromHex("#FF66AB")

                        },
                        new CircularContainer {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(85,85),
                            Masking = true,
                            Child = new Sprite{
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Texture = textures.Get("TestPFP"),
                                Size = new Vector2(87,87)
                            }
                        },
                        /////////////
                        
                        // Title text
                        new SpriteText {
                            Text = "osu!alltrick",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.CentreLeft,
                            Spacing = new Vector2(16),
                            Y = -3,
                            X = -500,
                            Font = FontUsage.Default.With(size: 60),

                        },
                        new SpriteText {
                            Text = "version 0.15.5",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.CentreLeft,
                            Spacing = new Vector2(10),
                            Y = -3,
                            X = 70,
                            Font = FontUsage.Default.With(size: 60),

                        },

                        // Username text
                        new SpriteText {
                            Text = "srb2thepast",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 65,
                            Font = FontUsage.Default.With(size: 40),
                            Colour = Colour4.FromHex("#FF66AB"),
                        },
                        new Sprite{
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 80,
                            X = -350,
                            Size = new Vector2(50),
                            Texture = textures.Get("FigmaVectors/StarFull")
                        },
                        new Sprite{
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 80,
                            X = 350,
                            Size = new Vector2(50),
                            Texture = textures.Get("FigmaVectors/StarFull")
                        }

                    }
                },
                
                
            };
            
        }

        private class HomeBackground : Container {
            private Container box;

            public HomeBackground()
            {
                AutoSizeAxes = Axes.Both;
                Origin = Anchor.Centre;
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                InternalChild = box = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Texture = textures.Get("logo")
                    },
                    }
                };
            }   

            protected override void LoadComplete()
            {
                base.LoadComplete();
                box.Loop(b => b.RotateTo(0).RotateTo(360, 2500));
            }
        }
        private class DraggableContainer : Container {
            private Vector2 lastoffpos = Vector2.Zero;
            private Vector2 lastscale = Vector2.One;
            protected override bool OnDragStart(DragStartEvent e) {
                
                return true;
            }
            protected override void OnDrag(DragEvent e) {
                Vector2 newPos = (e.MousePosition - e.MouseDownPosition) + lastoffpos;
                Child.MoveTo(newPos);
                
            }
            protected override void OnDragEnd(DragEndEvent e)
            {
                base.OnDragEnd(e);
                lastoffpos = Child.Position;
                
                System.Console.WriteLine(Parent.Parent.Position);
                System.Console.WriteLine(lastoffpos);

            }
            protected override bool OnScroll(ScrollEvent e)
            {
                Vector2 newScale = lastscale + new Vector2(e.ScrollDelta.Y / 10, e.ScrollDelta.Y / 10);
                
                lastscale = ( (newScale.X < 0.5 && newScale.Y < 0.5) || (newScale.X > 3 && newScale.Y > 3   )) ? lastscale : newScale;
                Vector2 BoxScreenPos = new Vector2(
                    (Child.Position.X * Child.Scale.X) + Child.Position.X,
                    (Child.Position.X * Child.Scale.X) + Child.Position.X
                    );


                Child.ScaleTo(lastscale,300,Easing.OutExpo);

                System.Console.Write("World: ");
                System.Console.WriteLine(BoxScreenPos);
                System.Console.Write("Mouse World: ");
                System.Console.WriteLine(new Vector2(
                    (e.MousePosition.X - Child.Position.X) / Child.Scale.X,
                    (e.MousePosition.Y - Child.Position.Y) / Child.Scale.Y

                    ));
                
                return true;
                
            }
            
        }

        
        
    }
}
