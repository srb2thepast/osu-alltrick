using System;
using osu.Framework.Graphics.Containers;

namespace osuAT.Game.Objects
{
    public partial class Page : Container
    {
        // public Action UpdateContent;
        public int Index;
        // Page0 only for here
        public Action Appear = new Action(() => { });
        // Page0 only for here
        public Action Reset = new Action(() => { });

        /// <summary>
        /// Instantly shows this page.
        /// </summary>
        public new Action Show = new Action(() => { });

        /// <summary>
        /// Instantly hides this page.
        /// </summary>
        public new Action Hide = new Action(() => { });
    }

}
