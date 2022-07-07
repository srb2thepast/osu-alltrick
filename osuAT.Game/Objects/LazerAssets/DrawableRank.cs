// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace osuAT.Game.Objects.LazerAssets
{
    public class DrawableRank : CompositeDrawable
    {
        private readonly string rank;

        public static Color4 ForRank(string rank)
        {
            switch (rank)
            {
                case "SSH":
                case "SS":
                    return Color4Extensions.FromHex(@"de31ae");

                case "SH":
                case "S":
                    return Color4Extensions.FromHex(@"02b5c3");

                case "A":
                    return Color4Extensions.FromHex(@"88da20");

                case "B":
                    return Color4Extensions.FromHex(@"e3b130");

                case "C":
                    return Color4Extensions.FromHex(@"ff8e5d");

                default:
                    return Color4Extensions.FromHex(@"ff5a5a");
            }
        }

        public DrawableRank(string rank)
        {
            this.rank = rank;

            RelativeSizeAxes = Axes.Both;
            FillMode = FillMode.Fit;
            FillAspectRatio = 2;

            var rankColour = ForRank(rank);
            InternalChild = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(64, 32),
                Strategy = DrawSizePreservationStrategy.Minimum,
                Child = new CircularContainer
                {
                    Masking = true,
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = rankColour,
                        },
                        new SpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Spacing = new Vector2(-3, 0),
                            Padding = new MarginPadding { Top = 5 },
                            Colour = getRankNameColour(),
                            Font = new FontUsage("Venera",size: 25),
                            Text = GetRankName(rank),
                            ShadowColour = Color4.Black.Opacity(0.3f),
                            ShadowOffset = new Vector2(0, 0.08f),
                            Shadow = true,
                        },
                    }
                }
            };
        }

        public static string GetRankName(string rank) => rank.GetDescription().TrimEnd('H');

        /// <summary>
        ///  Retrieves the grade text colour.
        /// </summary>
        private ColourInfo getRankNameColour()
        {
            switch (rank)
            {
                case "SSH":
                case "SH":
                    return ColourInfo.GradientVertical(Color4.White, Color4Extensions.FromHex("afdff0"));

                case "SS":
                case "S":
                    return ColourInfo.GradientVertical(Color4Extensions.FromHex(@"ffe7a8"), Color4Extensions.FromHex(@"ffb800"));

                case "A":
                    return Color4Extensions.FromHex(@"275227");

                case "B":
                    return Color4Extensions.FromHex(@"553a2b");

                case "C":
                    return Color4Extensions.FromHex(@"473625");

                default:
                    return Color4Extensions.FromHex(@"512525");
            }
        }
    }
}
