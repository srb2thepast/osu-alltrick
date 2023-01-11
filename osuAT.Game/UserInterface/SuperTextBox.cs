using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace osuAT.Game.UserInterface
{

    public partial class SuperTextBox : BasicTextBox
    {
        public FontUsage TextFont;

        public Colour4 BGFocusedColor;
        public Colour4 BGUnfocusedColor;
        public Colour4 BGCommitColor;

        public Action OnDefocus = new Action(() => { });
        public bool Shadow = false;
        public Vector2 ShadowOffset = new Vector2(0, 0.05f);


        public SuperTextBox()
        {

            BackgroundFocused = BGFocusedColor;
            BackgroundUnfocused = BGUnfocusedColor;
            BackgroundCommit = BGCommitColor;
            TextContainer.Height = 0.75f;
        }

        protected override void OnFocusLost(FocusLostEvent e)
        {
            base.OnFocusLost(e);
            OnDefocus();
        }


        protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
        {
            AutoSizeAxes = Axes.Both,
            Child = new SpriteText { Text = c.ToString(), Font = TextFont, Shadow = Shadow, ShadowOffset = ShadowOffset }
        };


    }
}
