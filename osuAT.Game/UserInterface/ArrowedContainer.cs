using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Effects;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Colour;
using osuTK.Graphics;
using osuAT.Game.Objects;
using osuAT.Game.Types;
using osuAT.Game.Skills;
using osuAT.Game.Objects.Displays;
using osuTK;


namespace osuAT.Game.UserInterface
{
    public class ArrowedContainer : FillFlowContainer
    {
        public Drawable[] Objects = { Drawable.Empty() };
        public int StartingIndex;
        public Drawable FocusedObject { get; private set; }
        private int focusIndex = 0;

        public bool Shadow = false;
        public Vector2 ShadowOffset = new Vector2(0, 0.05f);
        public ObjectSwitched OnObjectSwitched = delegate(Drawable focusedObject) { };
        public delegate void ObjectSwitched(Drawable focusedObject);
        public new int Spacing = 3;

        private Container objcontainer;

        public class ArrowButton : SpriteText {
            public Action ClickAction = new Action(() => { });
            protected override bool OnClick(ClickEvent e)
            {
                ClickAction();
                return true;
            }
        }
        public ArrowedContainer()
        {
            Direction = FillDirection.Horizontal; // only horizontal is supported as that's the only one needed
            AutoSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
        }

        protected void LeftArrowClick()
        {
            if (focusIndex == 0) return;
            objcontainer[focusIndex].Hide();
            focusIndex--;
            FocusedObject = Objects[focusIndex];
            objcontainer[focusIndex].Show();
            OnObjectSwitched(FocusedObject);
        }

        protected void RightArrowClick()
        {
            if (focusIndex == Objects.Length-1) return;
            objcontainer[focusIndex].Hide();
            focusIndex++;
            FocusedObject = Objects[focusIndex];
            objcontainer[focusIndex].Show();
            OnObjectSwitched(FocusedObject);

        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            FocusedObject = Objects[StartingIndex];
            Console.WriteLine(Objects[0].ToString());

            Children = new Drawable[] {
                new ArrowButton() {
                    Text = "<",
                    Shadow = Shadow,
                    ShadowOffset = ShadowOffset,
                    Font = new FontUsage("ChivoBold",size: 50),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    ClickAction = LeftArrowClick
                },
                objcontainer = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Padding = new MarginPadding{ Horizontal = 10}
                },
                new ArrowButton() {
                    Text = ">",
                    Shadow = Shadow,
                    ShadowOffset = ShadowOffset,
                    Font = new FontUsage("ChivoBold",size: 50),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    ClickAction = RightArrowClick
                },
            };
            foreach (Drawable draw in Objects)
            {
                objcontainer.Add(draw);
                draw.Hide();
            }
            objcontainer[focusIndex].Show();
        }
    }
}
