using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osuAT.Game.Objects
{
    public partial class Book : Container {
        public Page[] Pages = new Page[] { };
        public Bindable<int> CurrentPage = new Bindable<int>(defaultValue: 0);

        public Vector2 PageOffset;
        public float SlideSpeed = 500;
        public Easing SlideEasing = Easing.InOutCubic;
        public BufferedContainer PageContainer { get; private set; }

        public Book()
        {
            Masking = true;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Colour = Colour4.White;
            CornerRadius = 20;
        }

        [BackgroundDependencyLoader]
        private void load() {
            PageContainer = new BufferedContainer {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both

            };
            Add(PageContainer);
            for (int i = 0; i < Pages.Length; i++)
            {
                Pages[i].Index = i;
                Pages[i].Position = PageOffset * i;
                PageContainer.Add(Pages[i]);
            }
            CurrentPage.ValueChanged += SlideToPage;
        }

        /// <summary>
        /// Slides to the chosen page.
        /// </summary
        public void SlideToPage(ValueChangedEvent<int> curindex) {
            Page curpage = Pages[curindex.NewValue];
            PageContainer.MoveToX(-curpage.X,SlideSpeed, SlideEasing);
        }
           
    }
}
