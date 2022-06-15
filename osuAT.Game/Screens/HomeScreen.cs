using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
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
                background = new DragContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[] { 
                        new Box {
                            Colour = Color4.Violet,
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },
                new SpriteText
                {
                    Y = 20,
                    Text = "Welcome to osu!alltrick. [Version 0.0.1]",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Font = FontUsage.Default.With(size: 40)
                },
                new SpinningBox {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.Centre,
                },
                new SimpleSkillBox {
                       Anchor = Anchor.Centre,
                       Origin = Anchor.Centre,
                       SkillName = "Flow Aim",
                       SkillPrimaryColor = Colour4.FromHex("#76FF00"),
                       SkillSecondaryColor = Colour4.FromHex("#00FFF0"),
                       HScale= 100,
                       TextSize = 83
                       
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
        private class DragContainer : Container {
            private Vector2 lastoffpos = Vector2.Zero;
            protected override bool OnDragStart(DragStartEvent e) {
                
                return true;
            }
            protected override void OnDrag(DragEvent e) {
                Vector2 DragChange = (e.MousePosition - e.MouseDownPosition);
                Parent.Parent.MoveTo(DragChange + lastoffpos);

            }
            protected override void OnDragEnd(DragEndEvent e)
            {
                base.OnDragEnd(e);
                lastoffpos = Parent.Parent.Position;
                System.Console.WriteLine(Parent.Parent.Position);
                
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);
                System.Console.WriteLine("Dragging");
            }
        }

        protected override void OnMouseUp(MouseUpEvent e)
            {
            base.OnMouseUp(e);
            System.Console.WriteLine("Clicked");
        }
        
    }
}
