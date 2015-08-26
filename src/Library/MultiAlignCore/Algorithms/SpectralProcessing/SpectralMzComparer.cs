using System;
using System.Collections.Generic;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    /// <summary>
    /// Compares two spectra based on their 
    /// </summary>
    public class SpectralMzComparer: ISpectralComparer
    {
        /// <summary>
        /// Gets or sets the mass tolerance in m/z
        /// </summary>
        public double MassTolerance
        {
            get;
            set;
        }

        #region ISpectralComparer Members
        /// <summary>
        /// Compares two spectra finding similar peaks in two masses.
        /// </summary>
        /// <param name="spectraX"></param>
        /// <param name="spectraY"></param>
        /// <returns></returns>
        public double CompareSpectra(MSSpectra xSpectrum, MSSpectra ySpectrum)
        {
            var x = xSpectrum.Peaks;
            var y = ySpectrum.Peaks;
            
            var i       = 0;
            var j       = 0;
            var eps  = .0000001;
            var Nx      = x.Count;
            var Ny      = y.Count;

            var distanceMap = new Dictionary<int,int>();

            while(i < Nx)
            {
                var massX    = x[i].X;
                double massMax  = 0;
                double massMin  = 0;

                if (i + 1 >= Nx)
                {
                    massMax = MassTolerance;
                }
                else
                {
                    massMax = x[i + 1].X - massX - eps;
                }

                if (i == 0)
                {
                    massMin = MassTolerance;
                }
                else
                {
                    massMin = massX - x[i - 1].X - eps;
                }

                var mzTol     = Math.Min(Math.Min(MassTolerance, massMax), massMin);
                var bestDelta = mzTol; 
                var bestMatch    = -1;

                while (j < Ny)
                {
                    var massY        = y[j].X;
                    var deltaMass    = massX - massY;
                    var absDelta     = Math.Abs(deltaMass);

                    if (absDelta >= bestDelta && deltaMass < 0)
                    {
                        break;
                    }
                    bestMatch = j;
                    j = j + 1;
                }   

                if (bestMatch >= 0)
                {
                    distanceMap.Add(i, j);                       
                }
                i = i + 1;
            }

            // Score
            var nx      = x.Count;
            var ny      = y.Count;
            var matches = distanceMap.Keys.Count;

            return Convert.ToDouble(matches) / Convert.ToDouble(Math.Max(nx, ny));            
        }
        #endregion
    }
}
