using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osuTK;

namespace SkillAnalyzer
{

    /// <summary>w
    /// A spaced out bar graph.
    /// </summary>
    public class SpacedBarGraph : BarGraph
    {
        private SortedList<string, float> nameValues;

        public SortedList<string, float> NameValues
        {
            get => nameValues;
            set
            {
                nameValues = value;
                Values = nameValues.Values;
            }
        }

        public SpacedBarGraph()
        {

        }

        public float BarSpacing = 1;

        public new IEnumerable<float> Values
        {
            set
            {
                RemoveAll(d => { return true; },true) ;
                List<Bar> bars = Children.ToList();
                var selectedItems = value.Select((float length, int index) => new
                {
                    Value = length + 1 - 1,
                    Bar = ((bars.Count > index) ? bars[index] : null)
                });

                foreach (var item in selectedItems) {
                    float num = MaxValue ?? value.Max();
                    if (num != 0f)
                    {
                        num = item.Value / num;
                    }

                    float num2 = value.Count();
                    num2 *= 2;
                    if (num2 != 0f)
                    {
                        num2 = 1f / num2;
                    }

                    if (item.Bar != null)
                    {
                        item.Bar.Length = num;
                        item.Bar.Size = new Vector2(num2, 1f);
                        continue;
                    }

                    Add(new Bar
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(num2, 1f),
                        Length = num,
                        Direction = Direction
                    });

                    Add(new Bar
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(num2* BarSpacing, 1f),
                        Length = num,
                        Direction = Direction,
                        Colour = Colour4.Red.MultiplyAlpha(0)
                    });

                }
                Size = new Vector2(2, 3);
                RemoveRange(base.Children.Where((Bar _, int index) => index >= value.Count()*2).ToList(), disposeImmediately: true);
            }
        }


    }
}
