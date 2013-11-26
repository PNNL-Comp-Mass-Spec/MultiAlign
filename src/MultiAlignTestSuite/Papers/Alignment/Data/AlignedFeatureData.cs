using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;

namespace MultiAlignTestSuite.Papers.Alignment.Data
{
    /// <summary>
    /// Holds data for features that were aligned.
    /// </summary>
    public class AlignedFeatureData
    {
        /// <summary>
        /// Gets or sets the baseline features
        /// </summary>
        public Tuple<List<UMCLight>, List<MSFeatureLight>> Baseline { get; set; }
        /// <summary>
        /// Gets or sets the alignee features
        /// </summary>
        public Tuple<List<UMCLight>, List<MSFeatureLight>> Alignee { get; set; }
    }
}
