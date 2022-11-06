using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Colour;
using osu.Framework.Input.Events;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;
using osuAT.Game.Types;
using osuAT.Game.Skills;
using osuTK;


namespace osuAT.Game.Objects
{

    public enum SkillBoxState
    {
        MiniBox = 1,
        FullBox = 2
    }

    public class SkillBox : Container
    {

        public ISkill Skill;
        public SkillContainer ParentCont;

        public SkillBoxState State = SkillBoxState.MiniBox;

        public MiniSkillBox MiniBox;
        public FullSkillBox FullBox;

        public SkillBox()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Position = Skill.BoxPosition;
            MiniBox = new MiniSkillBox(Skill,this);
            FullBox = new FullSkillBox(Skill,this);

            InternalChild = new Container
            {
                FullBox,
                MiniBox
            };  
            MiniBox.ScaleTo(3.3f);
            FullBox.ScaleTo(1.7f);
            FullBox.InnerBox.Width = 1;
            SaveStorage.OnScoreAdded += new SaveStorage.ScoreAddedHandler(score =>
            {
                if (State == SkillBoxState.FullBox)
                {

                    Schedule(() => FullBox.Appear());
                };
                if (State == SkillBoxState.MiniBox)
                {
                    // Check if the SkillLevel has increased and play the star animation.
                };
            });
        }

        public void TransitionToFull()
        {
            ParentCont.MainScreen.AllowButtons = false;
            State = SkillBoxState.FullBox;
            FullBox.InnerBox.Height = 217;
            FullBox.InnerBox.Width = 320;

            // Move top bar
            ParentCont.MainScreen.TopBar.ScaleTo(0.5f, 500, Easing.InOutCubic);
            ParentCont.MainScreen.TopBar.MoveToY(50, 500, Easing.InOutCubic);
            // Refocus and zoom in
            ParentCont.FocusOnBox(this);
            ParentCont.ScaleTo(1.9f/ParentCont.Child.Scale.Y, 400, Easing.InOutExpo);

            // Boxes
            MiniBox.Slideout();
            FullBox.Appear(200);    
        }

        public void TransitionToMini()
        {
            ParentCont.MainScreen.AllowButtons = true;
            State = SkillBoxState.MiniBox;
            ParentCont.MainScreen.TopBar.ScaleTo(0.8f, 400, Easing.InOutCubic);
            ParentCont.MainScreen.TopBar.MoveToY(60, 400, Easing.InOutCubic);
            ParentCont.Delay(100).ScaleTo(1f,600,Easing.InOutExpo);
            
            MiniBox.Slidein();
            FullBox.Disappear(800);
            
        }

    }
}
