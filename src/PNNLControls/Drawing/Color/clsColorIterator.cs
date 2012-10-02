using System;
using System.Drawing ;
using System.Collections.Generic;

namespace PNNLControls
{
	/// <summary>
	/// clsColorIterator can be used to provide a sequence of colors based on indices. This can be used by 
	/// forms that want to give random starting colors to series that are plotted
	/// </summary>
	public class clsColorIterator
	{
		Color [] marr_clr ;
		public clsColorIterator()
		{
            marr_clr = new Color[] { Color.Blue, Color.Red, Color.Black, Color.Green, Color.Salmon, Color.Pink, Color.Navy, Color.DarkSlateBlue };
		}
		public Color GetColor(int index)
		{
			return marr_clr[index % marr_clr.Length] ; 
		}

        public void CreateColorGradient(Color baseColor, int numberOfLevels)
        {
            numberOfLevels = Math.Max(1, Math.Min(255, numberOfLevels));

            List<Color> colors = new List<Color>();

            int stepR = baseColor.R / numberOfLevels;
            stepR     = Math.Max(1, stepR);
            
            int stepG = baseColor.G / numberOfLevels;
            stepG     = Math.Max(1, stepG);
            
            int stepB = baseColor.B / numberOfLevels;
            stepB     = Math.Max(1, stepB);

            for (int i = 0; i < numberOfLevels; i++)
            {
                Color color = Color.FromArgb(255,
                                             stepR * i,
                                             stepG * i,
                                             stepB * i);

                colors.Add(color);
            }

            marr_clr = new Color[colors.Count];
            colors.CopyTo(marr_clr, 0);
        }
    }

}
