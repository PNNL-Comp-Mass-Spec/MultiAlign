using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.Algorithms.MSLinker
{
    /// <summary>
    /// Features extracted event arguments.
    /// </summary>
    public class FeaturesExtractedEventArgs: EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="MappedFeatures"></param>
        /// <param name="Clusters"></param>
        /// <param name="MassTags"></param>
        /// <param name="MassTagMatches"></param>
        public FeaturesExtractedEventArgs(  Dictionary<int, List<FeatureExtractionMap>> mappedFeatures,
                                            Dictionary<int, UMCClusterLight> clusters,
                                            Dictionary<int, MassTagLight> massTags,
                                            Dictionary<int, List<ClusterToMassTagMap>> massTagMatches)
        {
            MappedFeatures  = mappedFeatures;
            Clusters        = clusters;
            MassTags        = massTags;
            MassTagMatches  = massTagMatches;
        }
        /// <summary>
        /// Gets data flattened as a LC-MS, MS and MSn feature in one keyed by cluster id.
        /// </summary>
        public Dictionary<int, List<FeatureExtractionMap>> MappedFeatures
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the clusters from the database.
        /// </summary>
        public Dictionary<int, UMCClusterLight> Clusters
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the list of mass tags loaded in the database.
        /// </summary>
        public Dictionary<int, MassTagLight> MassTags
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the set of mass tag matches.
        /// </summary>
        public Dictionary<int, List<ClusterToMassTagMap>> MassTagMatches
        {
            get;
            private set;
        }
    }
}
