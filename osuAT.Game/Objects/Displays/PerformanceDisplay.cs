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
namespace osuAT.Game.Objects.Displays
{
    /// <summary>
    /// A pill that displays the star rating of a beatmap.
    /// </summary>
    public class PerformanceDisplay : CompositeDrawable
    {
        public static Color4 SampleFromLinearGradient((float position, Color4 colour)[] gradient, float point)
        {
            if (point < gradient[0].position)
                return gradient[0].colour;

            for (var i = 0; i < 7; i++)
            {
                var (position, colour) = gradient[i];
                var endStop = gradient[i + 1];

                if (point >= endStop.position)
                    continue;

                return Interpolation.ValueAt(point, colour, endStop.colour, position, endStop.position);
            }

            return gradient[^1].colour;
        }
        public static Color4 ForPerformance(double starDifficulty) => SampleFromLinearGradient(
            new[]{
            (0f, Color4Extensions.FromHex("#FFBBC9")),
            (300f, Color4Extensions.FromHex("#FFC537")),
            (400f, Color4Extensions.FromHex("#F58263")),
            (500f, Color4Extensions.FromHex("#FC575A")),
            (600f, Color4Extensions.FromHex("#9D41A6")),
            (750, Color4Extensions.FromHex("#2E29FF")),
            (950f, Color4Extensions.FromHex("#AE6CF2")),
            (1100f, Color4Extensions.FromHex("#000000")),
        }, (float)Math.Round(starDifficulty, 2, MidpointRounding.AwayFromZero));

        private readonly Bindable<double> displayedStars = new BindableDouble();

        public double Current;

        public PerformanceDisplay(double performance)
        {
            Current = performance;

            AutoSizeAxes = Axes.Both;
            Colour4 textColor = (Current > 750) ? Colour4.FromHex("#FFD966") : Colour4.Black.Opacity(0.75f);

            InternalChild = new CircularContainer
            {
                Masking = true,
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = ForPerformance(Current)
                    },
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(0.5f, 0),
                        Margin = new MarginPadding { Horizontal = 8f, Vertical = 2.3f },
                        AutoSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Spacing = new Vector2(-0.3f,0),
                                Text = Current.ToString(),
                                Font = new FontUsage("ChivoBold",size: 15),
                                Colour = textColor
                            },
                            new SpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Spacing = new Vector2(-0.3f,0),
                                Text = "pp",
                                Font = new FontUsage("ChivoBold",size: 13),
                                Colour = textColor
                            },
                        }
                    },
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
        }
    }
}
