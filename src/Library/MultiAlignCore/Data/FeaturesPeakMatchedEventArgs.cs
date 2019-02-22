#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Holds match information for others to use.
    /// </summary>
    public class FeaturesPeakMatchedEventArgs : EventArgs
    {
        public FeaturesPeakMatchedEventArgs(List<UMCClusterLight> clusters,
            List<MassTags.FeatureMatchLight<UMCClusterLight, MassTagLight>> matches)
        {
            Matches = matches;
            Clusters = clusters;
        }

        public List<UMCClusterLight> Clusters { get; private set; }

        /// <summary>
        /// Gets or sets the list of matches.
        /// </summary>
        public List<MassTags.FeatureMatchLight<UMCClusterLight, MassTagLight>> Matches { private set; get; }
    }
}