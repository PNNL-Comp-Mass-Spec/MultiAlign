using System.Collections.Generic;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Cluster of MS/MS spectra using matches through features.
    /// </summary>
    public sealed class MsmsCluster
    {
        public MsmsCluster()
        {
            Features = new List<MSFeatureLight>();
            MeanScore = double.NaN;
        }

        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the list of available features.
        /// </summary>
        public List<MSFeatureLight> Features
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the list of spectra associated with the
        /// </summary>
        public double MeanScore
        {
            get;
            set;
        }

    }
}
