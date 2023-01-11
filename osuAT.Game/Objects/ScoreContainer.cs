using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Objects.Displays;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game
{
    public partial class ScoreContainer : CompositeDrawable
    {
        protected List<Score> ScoreList = new List<Score>();
        public ISkill Skill { get; set; }
        public ScoreContainer()
        {
            Size = new osuTK.Vector2(100, 100);
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
        }
        private ScoreScrollContainer<Drawable> scrollbox;

        public partial class ScoreScrollContainer<T> : ScrollContainer<T> where T : Drawable
        {
            protected partial class ScoreScrollBar : ScrollbarContainer
            {
                public ScoreScrollBar(Direction direction) : base(direction)
                {
                    Child = new Circle { RelativeSizeAxes = Axes.Both };
                }

                public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None) // from osu!lazer
                {
                    Vector2 size = new Vector2(5)
                    {
                        [(int)ScrollDirection] = val
                    };
                    this.ResizeTo(size, duration, easing);
                }
            };
            protected override ScrollbarContainer CreateScrollbar(Direction direction) => new ScoreScrollBar(direction);
        }


        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            ScoreList = SaveStorage.GetTrickTopScoreList(Skill);

            AddInternal(new Circle
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = -43,
                X = -62.5f,
                Size = new Vector2(10, 186),
                Colour = Colour4.PeachPuff
            });
            AddInternal(new Circle
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = -41.5f,
                X = -62.5f,
                Size = new Vector2(7, 183),
                Colour = Colour4.FromHex("465550")
            });

            AddInternal(scrollbox = new ScoreScrollContainer<Drawable>
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = -40,
                X = 10,
                Size = new Vector2(150, 180),
                ScrollContent =
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Children = new ScoreDisplay[] { }
                },

                ScrollbarAnchor = Anchor.TopLeft
            });

            int index = 0;
            foreach (Score score in ScoreList)
            {
                ScoreDisplay display = new ScoreDisplay
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.Centre,
                    Y = 15 + 40 * index,
                    X = -10,
                    Current = score,
                    Skill = Skill,
                    IndexPos = index,
                    Scale = new Vector2(0.17f)
                };
                scrollbox.Add(display);
                index += 1;
            }
        }

        public void ReloadScores()
        {
            ScoreList = SaveStorage.GetTrickTopScoreList(Skill);

            scrollbox.Clear();
            int index = 0;
            foreach (Score score in ScoreList)
            {
                ScoreDisplay display = new ScoreDisplay
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.Centre,
                    Y = 15 + 40 * index,
                    X = -10,
                    Current = score,
                    Skill = Skill,
                    IndexPos = index,
                    Scale = new Vector2(0.17f),
                };
                scrollbox.Add(display);
                index += 1;
            }

        }

        public void Appear(float delay = 0, float offset = 30)
        {
            ReloadScores();
            int index = 0;
            foreach (ScoreDisplay display in scrollbox.ScrollContent.Children)
            {
                display.Appear(delay + index * offset);
                index += 1;
            }
        }

        public void HideScores(float delay)
        {
            foreach (ScoreDisplay display in scrollbox.ScrollContent.Children)
            {
                display.Disappear(delay);
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
        }
    }
}
