// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
/* License text:
Copyright (c) 2022 ppy Pty Ltd <contact@ppy.sh>.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


using System;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;
namespace osuAT.Game.Objects.LazerAssets.StarRating
{
    /// <summary>
    /// A pill that displays the star rating of a beatmap.
    /// </summary>
    public class StarRatingDisplay : CompositeDrawable
    {
        public static Color4 SampleFromLinearGradient((float position, Color4 colour)[] gradient, float point)
        {
            if (point < gradient[0].position)
                return gradient[0].colour;

            for (var i = 0; i < 11; i++) // 11 because there are 12 elements being passed through ForStarDifficutly
            {
                var (position, colour) = gradient[i];
                var endStop = gradient[i + 1];

                if (point >= endStop.position)
                    continue;

                return Interpolation.ValueAt(point, colour, endStop.colour, position, endStop.position);
            }

            return gradient[^1].colour;
        }
        public static Color4 ForStarDifficulty(double starDifficulty) => SampleFromLinearGradient(
            new[]{
            (0.1f, Color4Extensions.FromHex("aaaaaa")),
            (0.1f, Color4Extensions.FromHex("4290fb")),
            (1.25f, Color4Extensions.FromHex("4fc0ff")),
            (2.0f, Color4Extensions.FromHex("4fffd5")),
            (2.5f, Color4Extensions.FromHex("7cff4f")),
            (3.3f, Color4Extensions.FromHex("f6f05c")),
            (4.2f, Color4Extensions.FromHex("ff8068")),
            (4.9f, Color4Extensions.FromHex("ff4e6f")),
            (5.8f, Color4Extensions.FromHex("c645b8")),
            (6.7f, Color4Extensions.FromHex("6563de")),
            (7.7f, Color4Extensions.FromHex("18158e")),
            (9.0f, Color4.Black),
        }, (float)Math.Round(starDifficulty, 2, MidpointRounding.AwayFromZero));

        private readonly bool animated;
        private readonly Box background;
        private readonly SpriteIcon starIcon;
        private readonly SpriteText starsText;

        private readonly BindableWithCurrent<StarDifficulty> current = new BindableWithCurrent<StarDifficulty>();

        public Bindable<StarDifficulty> Current
        {
            get => current.Current;
            set => current.Current = value;
        }

        private readonly Bindable<double> displayedStars = new BindableDouble();

        /// <summary>
        /// The currently displayed stars of this display wrapped in a bindable.
        /// This bindable gets transformed on change rather than instantaneous, if animation is enabled.
        /// </summary>
        public IBindable<double> DisplayedStars => displayedStars;



        /// <summary>
        /// Creates a new <see cref="StarRatingDisplay"/> using an already computed <see cref="StarDifficulty"/>.
        /// </summary>
        /// <param name="starDifficulty">The already computed <see cref="StarDifficulty"/> to display.</param>
        /// <param name="size">The size of the star rating display.</param>
        /// <param name="animated">Whether the star rating display will perform transforms on change rather than updating instantaneously.</param>
        public StarRatingDisplay(StarDifficulty starDifficulty, StarRatingDisplaySize size = StarRatingDisplaySize.Regular, bool animated = false)
        {
            this.animated = animated;

            Current.Value = starDifficulty;

            AutoSizeAxes = Axes.Both;

            MarginPadding margin = default;

            switch (size)
            {
                case StarRatingDisplaySize.Small:
                    margin = new MarginPadding { Horizontal = 7f };
                    break;

                case StarRatingDisplaySize.Range:
                    margin = new MarginPadding { Horizontal = 8f };
                    break;

                case StarRatingDisplaySize.Regular:
                    margin = new MarginPadding { Horizontal = 8f, Vertical = 2f };
                    break;
            }

            InternalChild = new CircularContainer
            {
                Masking = true,
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    new GridContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        AutoSizeAxes = Axes.Both,
                        Margin = margin,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize),
                            new Dimension(GridSizeMode.Absolute, 3f),
                            new Dimension(GridSizeMode.AutoSize, minSize: 25f),
                        },
                        RowDimensions = new[] { new Dimension(GridSizeMode.AutoSize) },
                        Content = new[]
                        {
                            new[]
                            {
                                starIcon = new SpriteIcon
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Icon = FontAwesome.Solid.Star,
                                    Size = new Vector2(8f),
                                },
                                Empty(),
                                starsText = new SpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Margin = new MarginPadding { Bottom = 1.5f },
                                    // todo: this should be size: 12f, but to match up with the design, it needs to be 14.4f
                                    // see https://github.com/ppy/osu-framework/issues/3271.
                                    Font = new FontUsage("ChivoBold", size: 14.4f),
                                    Shadow = false,
                                },
                            }
                        }
                    },
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Current.BindValueChanged(c =>
            {
                if (animated)
                    this.TransformBindableTo(displayedStars, c.NewValue.Stars, 750, Easing.OutQuint);
                else
                    displayedStars.Value = c.NewValue.Stars;
            });

            displayedStars.Value = Current.Value.Stars;

            displayedStars.BindValueChanged(s =>
            {
                starsText.Text = s.NewValue.ToLocalisableString("0.00");

                background.Colour = ForStarDifficulty(s.NewValue);

                starIcon.Colour = s.NewValue >= 6.5 ? Color4Extensions.FromHex(@"ffd966") : Color4Extensions.FromHex("303d47");
                starsText.Colour = s.NewValue >= 6.5 ? Color4Extensions.FromHex(@"ffd966") : Color4.Black.Opacity(0.75f);
            }, true);
        }
    }

    public enum StarRatingDisplaySize
    {
        Small,
        Range,
        Regular,
    }
}
