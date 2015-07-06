#region

using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;

#endregion

namespace MultiAlignCore.Drawing
{
    public class ColorTypeIterator
    {
        private readonly IList<OxyColor> m_colors;

        public ColorTypeIterator()
        {
            m_colors = new List<OxyColor>
            {
                OxyColors.Red,
                OxyColors.Black,
                OxyColors.Green,
                OxyColors.Blue,
                OxyColors.Orange,
                OxyColors.Purple,
                OxyColors.Crimson,
                OxyColors.Gold,
                OxyColors.DarkGray
            };
        }

        public OxyColor GetColor(int i)
        {
            i = Math.Max(0, i - 1);
            return m_colors[i%m_colors.Count()];
        }
    }
}