﻿using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osuTK;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using osuAT.Game.Skills;
using osu.Framework.Extensions.IEnumerableExtensions;
using static System.Net.Mime.MediaTypeNames;
using osu.Framework.Graphics.Colour;

namespace SkillAnalyzer
{
    public class LabelledBarGraph : Container
    {
        public SpacedBarGraph SBarGraph { get; private set; }

        protected Container GraphContainer;

        protected Container PresContainer;

        protected Container BackerPresContainer;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This class is not intended for other works (it was designed very quickly)</remarks>
        /// <param name="barGraph"></param>
        public LabelledBarGraph(SpacedBarGraph barGraph = null) {
            SBarGraph = barGraph ?? new SpacedBarGraph();
            SBarGraph.Anchor = Anchor.TopCentre;
            Add(GraphContainer = new Container
            {
                Anchor = Anchor.Centre,

                Size = new Vector2(500, 500),
                Children = new Drawable[]{
                    BackerPresContainer = new DrawSizePreservingFillContainer
                    {
                        Anchor = Anchor.TopLeft,
                    },
                    PresContainer = new DrawSizePreservingFillContainer
                    {
                        Anchor = Anchor.TopLeft,
                        Child = SBarGraph
                    }
                }
            }
            );
            GraphContainer.Y = -504;
            GraphContainer.X = -504;
            SBarGraph.Position += new Vector2(0, GraphContainer.Height * 1.5f);

        }

        public void SetValues(SortedList<string,float> values, List<ColourInfo> colors = null, List<float> Backers = null)
        {
            colors ??= new List<ColourInfo> { Colour4.White };
            // colors.Count should be equal to values.Count
            // Console.WriteLine(((Container)GraphContainer.Child).Children[0].Size.X);
            List<Drawable> removeList = new List<Drawable>();
            PresContainer.RemoveAll(
                (draw) => { return (draw is TextFlowContainer || draw is Bar); }, false
                );
            BackerPresContainer.RemoveAll(
                (draw) => { return (draw is TextFlowContainer || draw is Bar); }, false
                );

            SBarGraph.NameValues = values;
            int i = 0;
            int amount = SBarGraph.Children.Count;
            List<Bar> barsToAdd = new List<Bar>();

            foreach (Bar child in SBarGraph.Children)
            {
                // Console.WriteLine(i.ToString() + values.Keys[i].ToString());
                if (i % 2 == 1) { i++; continue; } // the gaps
                float index = i / 2;
                float scale = SBarGraph.Scale.X * (1 / 0.2f);
                // [!] Set color based on skill: child.Colour = Colour4.Red;
                // Console.WriteLine(scale.ToString() + "hei");
                child.Colour = colors[(int)index % colors.Count];
                TextFlowContainer textflow;
                PresContainer.Add(textflow = new TextFlowContainer()
                {
                    Anchor = Anchor.BottomLeft,
                    Size = new Vector2(30f / amount * 12 * scale, 40),
                    Position = new Vector2(((index) / amount * 410 * 2 * scale) * (SBarGraph.BarSpacing * 1 / 2f + 0.5f) + 510, 360),
                });
                textflow.AddText(
                    $"{values.Keys[(int)index]} \n ({(Math.Truncate((float)values.Values[(int)index] * 100)) / 100})",
                    t => t.Font = new FontUsage("VarelaRound", size: 18 * scale));
                i++;
                if (Backers != default)
                {
                    Bar newBar = new Bar()
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(child.Size.X, 1f),
                        Length = child.Length/1.65f,
                        Direction = SBarGraph.Direction,
                        Position = new Vector2(((index) / amount * 410 * 2 * scale) * (SBarGraph.BarSpacing * 1 / 2f + 0.5f) + 412  , 342),
                    };
                    newBar.X += 100;
                    newBar.X += 200;
                    newBar.Colour = Colour4.DimGray.MultiplyAlpha(0.5f);
                    barsToAdd.Add(newBar);
                }
            }
            foreach (Bar newBar in barsToAdd)
            {
                BackerPresContainer.Add(newBar);
            }
            RelativeSizeAxes = Axes.Both;
        }
    }
}
