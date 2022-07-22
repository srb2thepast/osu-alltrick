using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuAT.Game.Screens;
using osuAT.Game.Objects;
using osuAT.Game.Skills;
using osuTK;

namespace osuAT.Game
{
    public class SkillContainer : Container
    {
        private Container container;
        public Dictionary<ISkill, SkillBox> SkillDict = new Dictionary<ISkill, SkillBox>();
        public SkillBox FocusedBox;
        public HomeScreen MainScreen;
            
        public SkillContainer(HomeScreen mainscreen)
        {
            Size = new Vector2(8000,6000);
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
        private void load(TextureStore textures)
        {
            Child = container = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box {

                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0.3f
                    },
                    /* Nerve Control
                    new SkillBox {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        SkillName = "Nerve Control",
                        SkillPrimaryColor = Colour4.FromHex("#99FF69"),
                        SkillSecondaryColor = Colour4.FromHex("#00FFF0"),
                        MiniHeight = 100,
                        TextSize = 63,
                    }, */

                }
            };
            // load each skill
            foreach (var box in SkillDict.Values)
            {
                box.ParentCont = this;
                container.Add(box);
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
        }

        public void FocusOnBox(SkillBox box) {
            Vector2 BoxScreenPos = new Vector2(
                -(box.Position.X * Child.Scale.X) ,
                -(box.Position.Y * Child.Scale.Y)
                );
            Child.MoveTo(BoxScreenPos, 400,Easing.OutCubic);

            FocusedBox = box;
            lastoffpos = BoxScreenPos;
        }
        public void Defocus() {
            FocusedBox = null;
        }

        #region Input Handlers
        private Vector2 lastoffpos = Vector2.Zero;
        private Vector2 lastscale = Vector2.One;
        protected override bool OnDragStart(DragStartEvent e)
        {
            if ((!(FocusedBox?.State == SkillBoxState.FullBox) || (FocusedBox == null)) && MainScreen.CurrentlyFocused == true )
            {
                FocusedBox = null; // if you start dragging, the box will defocus.
                return true;
            }
            return false;
        }
        protected override void OnDrag(DragEvent e)
        {
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
        { // one day... scrolling will be good... right...!??!?!??!?!?
            if (MainScreen.CurrentlyFocused == false) return false;
                if (FocusedBox?.State == SkillBoxState.FullBox) { return false; }
                Vector2 newScale = lastscale + new Vector2(e.ScrollDelta.Y / 10, e.ScrollDelta.Y / 10);

            lastscale = ((newScale.X < 0.5 && newScale.Y < 0.5) || (newScale.X > 1.5 && newScale.Y > 1.5)) ? lastscale : newScale;

            Vector2 MouseWorldPos = new Vector2(
                (e.MousePosition.X * Child.Scale.X) + Child.Position.X,
                (e.MousePosition.Y * Child.Scale.Y) + Child.Position.Y);


            Vector2 BoxScreenPos = new Vector2(
                -(e.MousePosition.X * Child.Scale.X) -MouseWorldPos.X,
                -(e.MousePosition.Y * Child.Scale.Y) - MouseWorldPos.Y
                );

            //Child.MoveTo(BoxScreenPos, 300, Easing.OutExpo);
            //lastoffpos = BoxScreenPos;
            Child.ScaleTo(lastscale, 300, Easing.OutExpo);

            System.Console.Write("World: ");
            System.Console.WriteLine(BoxScreenPos);
            System.Console.Write("Mouse World: ");
            System.Console.WriteLine(MouseWorldPos);
            System.Console.WriteLine(e.MousePosition);
            System.Console.WriteLine(Child.Position);
            

            return true;
        }
        #endregion
    }
}
