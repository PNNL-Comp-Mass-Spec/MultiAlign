using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    public class LcmsWarpResults
    {
        /// <summary>
        /// Gets or sets the the error histogram from the alignment
        /// </summary>
        public Dictionary<double, int> ErrorHistogram { get; set; }

        /// <summary>
        /// Gets or sets the alignment score heat map.
        /// </summary>
        public double[,] HeatScores { get; set; }

        /// <summary>
        /// Gets or sets the mean for this dimension.
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        /// Gets or sets the RSquared for the line fit to this dimension.
        /// </summary>
        public double RSquared { get; set; }

        /// <summary>
        /// Gets or sets the slope for the line fit to this dimension.
        /// </summary>
        public double Slope { get; set; }

        /// <summary>
        /// Gets or sets the kurtosis for this dimension.
        /// </summary>
        public double Kurtosis { get; set; }
    }
}
