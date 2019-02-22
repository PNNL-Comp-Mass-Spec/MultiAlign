#region

using System.Collections.Generic;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Holds a link from the mass tag to the clusters that it matched to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MassTagToCluster : IFeatureMap
    {
        public MassTagToCluster()
        {
            Matches = new List<UMCClusterLightMatched>();
            MatchingProteins = new List<ProteinToMassTags>();
            MassTag = null;
        }

        public MassTagLight MassTag { get; set; }

        public List<UMCClusterLightMatched> Matches { get; set; }
        public List<ProteinToMassTags> MatchingProteins { get; set; }

        /// <summary>
        /// Gets the ID of the underlying feature.
        /// </summary>
        public object Id
        {
            get { return string.Format("{0}-{1}", MassTag.Id, MassTag.ConformationId); }
        }
    }
}