using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Effects;
using osuAT.Game.Objects.Displays;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game
{
    public class ScoreContainer : CompositeDrawable
    {

        public ScoreContainer()
        {
            Size = new osuTK.Vector2(100, 100);
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

        }
        private ScoreScrollContainer<Drawable> scrollbox;

        public class ScoreScrollContainer<T> : ScrollContainer<T> where T : Drawable
        {
            protected class ScoreScrollBar : ScrollbarContainer
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
            List<ModInfo> ModList = new List<ModInfo>
            {
                ModStore.Hidden,
                ModStore.Doubletime,
                ModStore.Hardrock,
                ModStore.Flashlight
            };
            Score dummyscore = new Score()
            {
                ScoreRuleset = RulesetStore.Osu,
                IsLazer = false,
                OsuID = 3152295822,
                BeatmapInfo = new Beatmap
                {
                    MapID = 651507,
                    MapsetID = 1380717,
                    SongArtist = "a_hisa",
                    SongName = "Logical Stimulus",
                    DifficultyName = "owo",
                    MapsetCreator = "Naidaaka",
                    StarRating = 7.93,
                    MaxCombo = 2336
                },
                Grade = "SH",
                Accuracy = 99.51,
                AccuracyStats = new AccStat(2020, 15, 0, 0),
                Combo = 2333,
                TotalScore = 116276034,
                Mods = ModList,
                DateCreated = System.DateTime.Today
            };
            dummyscore.Register();

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

            AddInternal(scrollbox = new ScoreScrollContainer<Drawable> {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = -40,
                X = 10,
                Size = new Vector2(150,180),
                ScrollContent =
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                },
                
                ScrollbarAnchor = Anchor.TopLeft
            });
            
            int index = 0;
            foreach (Score score in SaveStorage.SaveData.Scores)
            {
                ScoreDisplay display = new ScoreDisplay
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.Centre,
                    Y = 15 + 40 * index,
                    X = -10,
                    Current = score,
                    PrimaryColor = Colour4.FromHex("#99FF69"),
                    SecondaryColor = Colour4.FromHex("#00FFF0"),
                    Scale = new Vector2(0.17f)
                };
                scrollbox.Add(display);
                display.Appear(index*30);
                index += 1;
            }
        }


        protected override void LoadComplete()
        {
            base.LoadComplete();
        }
    }
}
