using PNNLControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MultiAlign.Data
{
    public static class DatasetColorSelector
    {
        static clsColorIterator m_colors = new clsColorIterator();

        /// <summary>
        /// Gets a color based on the group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static Color GetColor(int group)
        {            
            System.Drawing.Color color = m_colors.GetColor(group);

            Color newColor  = new Color();
            newColor.A      = color.A;
            newColor.R      = color.R;
            newColor.G      = color.G;
            newColor.B      = color.B;
            return newColor;
        }
    }
}
