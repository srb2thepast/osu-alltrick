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
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game.Objects.LazerAssets.Mod
{
    /// <summary>
    /// Displays a single-line horizontal auto-sized flow of mods. For cases where wrapping is required, use <see cref="ModFlowDisplay"/> instead.
    /// </summary>
    public class ModDisplay : CompositeDrawable, IHasCurrentValue<IReadOnlyList<ModInfo>>
    {

        public class ReverseChildIDFillFlowContainer<T> : FillFlowContainer<T> where T : Drawable
        {
            protected override int Compare(Drawable x, Drawable y) => CompareReverseChildID(x, y);
        }

        private const int fade_duration = 1000;

        public ExpansionMode ExpansionMode = ExpansionMode.ExpandOnHover;

        private readonly BindableWithCurrent<IReadOnlyList<ModInfo>> current = new BindableWithCurrent<IReadOnlyList<ModInfo>>();

        public Bindable<IReadOnlyList<ModInfo>> Current
        {
            get => current.Current;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                current.Current = value;
            }
        }

        private readonly FillFlowContainer<ModIcon> iconsContainer;

        public ModDisplay()
        {
            AutoSizeAxes = Axes.Both;

            InternalChild = iconsContainer = new ReverseChildIDFillFlowContainer<ModIcon>
            {
                AutoSizeAxes = Axes.Both,
                Y = -3,
                Direction = FillDirection.Horizontal,
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Current.BindValueChanged(updateDisplay, true);

            iconsContainer.FadeInFromZero(fade_duration, Easing.OutQuint);
        }

        private void updateDisplay(ValueChangedEvent<IReadOnlyList<ModInfo>> mods)
        {
            iconsContainer.Clear();

            if (mods.NewValue == null) return;

            foreach (ModInfo mod in mods.NewValue)
                iconsContainer.Add(new ModIcon(mod) { Scale = new Vector2(0.6f) });

            appearTransform();
        }

        private void appearTransform()
        {
            expand();

            using (iconsContainer.BeginDelayedSequence(1200))
                contract();
        }

        private void expand()
        {
            if (ExpansionMode != ExpansionMode.AlwaysContracted)
                iconsContainer.TransformSpacingTo(new Vector2(5, 0), 500, Easing.OutQuint);
        }

        private void contract()
        {
            if (ExpansionMode != ExpansionMode.AlwaysExpanded)
                iconsContainer.TransformSpacingTo(new Vector2(-25, 0), 500, Easing.OutQuint);
        }

        protected override bool OnHover(HoverEvent e)
        {
            expand();
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            contract();
            base.OnHoverLost(e);
        }
    }

    public enum ExpansionMode
    {
        /// <summary>
        /// The <see cref="ModDisplay"/> will expand only when hovered.
        /// </summary>
        ExpandOnHover,

        /// <summary>
        /// The <see cref="ModDisplay"/> will always be expanded.
        /// </summary>
        AlwaysExpanded,

        /// <summary>
        /// The <see cref="ModDisplay"/> will always be contracted.
        /// </summary>
        AlwaysContracted
    }
}
