using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MultiAlignEngine.Features;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.MSLinker
{
    public class MSnFeaturesExtractedEventArgs: EventArgs
    {
        /// <summary>
        /// Gets the set of features that are mapped to MSn fragmentation spectra.
        /// </summary>
        public Dictionary<int, List<FeatureExtractionMap>> MappedFeatures
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the list of clusters that are mapped to fragmentation spectra.
        /// </summary>
        public Dictionary<int, clsCluster> ClusterMap
        {
            get;
            private set;
        }

    }
}
