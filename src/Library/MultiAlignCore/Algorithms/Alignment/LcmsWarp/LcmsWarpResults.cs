using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    public class LcmsWarpResults
    {
        /// <summary>
        /// Id value for the database, set when persisted
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the alignment function
        /// </summary>
        public IAlignmentFunction AlignmentFunction { get; set; }

        /// <summary>
        /// Gets or sets the separation type for these results
        /// </summary>
        public FeatureLight.SeparationTypes SeparationType { get; set; }

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

        public LcmsWarpResults()
        {
            this.SeparationType = FeatureLight.SeparationTypes.LC;
        }
    }
}
