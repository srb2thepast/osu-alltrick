using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuAT.Game.Objects;
using osuTK;

namespace osuAT.Game.Screens
{
    public abstract partial class ButtonScreen : CompositeDrawable
    {
        protected abstract IconUsage ButtonIcon { get; }
        protected abstract Drawable DisplayBox { get; }

        protected Drawable DisplayButton;

        public bool CanOpen = true;
        public bool BoxOpened = false;

        [BackgroundDependencyLoader]
        private void load()
        {
            Drawable iconBut;
            InternalChildren = new Drawable[]
            {
                new ClickableContainer{
                    AutoSizeAxes = Axes.Both,
                    Action = ButtonClicked,
                    Children = new Drawable[] {
                         iconBut = new SpriteIcon {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,

                            Size = new Vector2(38,38),
                            Colour = Colour4.LightSlateGray,
                            Shadow = true,
                            ShadowOffset = new Vector2(0,4f),
                            ShadowColour = Colour4.Black,
                            Icon = ButtonIcon,
                        },
                    },
                },
                DisplayBox
            };
            Size = InternalChildren[0].Size;

            DisplayButton = iconBut;
        }

        protected void ButtonClicked()
        {
            if (!CanOpen) return;
            if (DisplayBox.Alpha == 0)
            {
                ShowBox(); return;
            }
            HideBox();
        }

        public void HideBox()
        {
            BoxOpened = false;
            DisplayBox.FadeOut(200, Easing.OutCubic);
        }

        public void ShowBox()
        {
            BoxOpened = true;
            DisplayBox.Show();
        }

        protected override bool OnHover(HoverEvent e)
        {
            if (CanOpen)
                DisplayButton.ScaleTo(1.07f, 100, Easing.Out);
            return CanOpen;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            DisplayButton.ScaleTo(1f, 100, Easing.Out);
        }
    }
}
