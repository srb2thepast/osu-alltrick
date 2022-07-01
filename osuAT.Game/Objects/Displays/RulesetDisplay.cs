using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuAT.Game.Objects.LazerAssets;
using osuAT.Game.Types;
using osuTK;
namespace osuAT.Game.Objects.Displays
{
    public class RulesetDisplay : CompositeDrawable
    {


        public RulesetInfo[] RulesetList;
        private Container rulesetContainer;
        private Circle innerCircle;
        private Circle outerCircle;
        public RulesetDisplay()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                Children = new Drawable[]
                {
                    
                    // Outer Circle
                     outerCircle = new Circle
                     {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Colour = Colour4.FromHex("#F0A1B7"),
                            Size = new Vector2(78,67),
                            Alpha = 0
                     },
                    // Inner Circle
                    innerCircle = new Circle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Colour4.FromHex("#F7E65D"),
                        Size = new Vector2(69,58),
                        Alpha = 0
                    },
                    // Container
                    rulesetContainer = new Container{
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    }
                    
                }
            };
            outerCircle.FadeIn(200, Easing.InOutCubic);
            innerCircle.FadeIn(200, Easing.InOutCubic);

            using (outerCircle.BeginDelayedSequence(50))
                outerCircle.ResizeTo(new Vector2(78 + RulesetList.Length * 60, 67), 500, Easing.InOutCubic);
            using (innerCircle.BeginDelayedSequence(50))
                innerCircle.ResizeTo(new Vector2(69 + RulesetList.Length * 60, 58),500,Easing.InOutCubic);

            for (var i = 0; i < RulesetList.Length; i++)
            {
                RulesetInfo ruleset = RulesetList[i];
                SpriteIcon newIcon = new SpriteIcon
                {
                    X = RulesetList.Length % 2 == 1 ? i * 60 + (-30 * (RulesetList.Length - 1)) : i * 60 + (-60 * (RulesetList.Length - 2) / 2 - 30),
                    Y = 25, // starts at 25, moves to 0 in the animation
                    Icon = ruleset.Icon,

                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Colour4.White,
                    Alpha = 0,
                    Size = new Vector2(48)

                };
                rulesetContainer.Add(
                    newIcon
                );

                using (newIcon.BeginDelayedSequence(450))
                    newIcon.FadeIn(250, Easing.InOutSine);
                    newIcon.MoveToY(0, 1000,Easing.Out);
            };


        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

        }

    }
}
