using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osuTK;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using osuAT.Game.Skills;

namespace SkillAnalyzer
{
    public class LabelledGraph : Container
    {
        public SpacedBarGraph SBarGraph { get; private set; }

        protected Container GraphContainer;

        public LabelledGraph(SpacedBarGraph barGraph = null) {
            SBarGraph = barGraph ?? new SpacedBarGraph();
            SBarGraph.Anchor = Anchor.TopCentre;
            Add(GraphContainer = new Container
            {
                Anchor = Anchor.Centre,

                Size = new Vector2(500, 500),
                Child = new DrawSizePreservingFillContainer
                {
                    Anchor = Anchor.TopLeft,
                    Child = SBarGraph
                }
            }
            );
            GraphContainer.Y = -504;
            GraphContainer.X = -504;
            SBarGraph.Position += new Vector2(0, GraphContainer.Height * 1.5f);

        }

        public void SetValues(SortedList<string,float> values) // List<Colour4> colors
        {
            // colors.Count should be equal to values.Count
            Console.WriteLine(((Container)GraphContainer.Child).Children[0].Size.X);
            SBarGraph.NameValues = values;
            int i = 0;
            int amount = SBarGraph.Children.Count;
            foreach (Bar child in SBarGraph.Children)
            {
                // Console.WriteLine(i.ToString() + values.Keys[i].ToString());
                if (i % 2 == 1) { i++; continue; } // the gaps
                float index = i/2;
                float scale = ((Container)GraphContainer.Child).Children[0].Scale.X*(1/0.2f);
                // [!] Set color based on skill: child.Colour = Colour4.Red;
                Console.WriteLine(scale.ToString() + "hei");
                ((Container)GraphContainer.Child).Add(new SpriteText()
                {
                    Text = values.Keys[(int)index],
                    Anchor = Anchor.BottomLeft,
                    Size = new Vector2(30f / amount *12 * scale, 40),
                    Position = new Vector2((index)/amount*415*2*scale + 510,360),
                    Font = new FontUsage("VarelaRound", size: 18*scale)
            });
                i++;
            }
            foreach (KeyValuePair<string, float> pair in values) {
                
            }
            RelativeSizeAxes = Axes.Both;
        }
    }
}
