using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Objects;

namespace osuAT.Game
{
    public class SkillContainer : Container
    {

        public SkillContainer()
        {
            Size = new osuTK.Vector2(3000,2000);
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
            
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
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
                // Flow Aim
                new SkillBox {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    SkillName = "Flow Aim",
                    SkillPrimaryColor = Colour4.FromHex("#99FF69"),
                    SkillSecondaryColor = Colour4.FromHex("#00FFF0"),
                    MiniHeight = 100,
                    TextSize = 83,
                    X=700,
                    Y=400,
                    
                },

            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
        }
    }
}
