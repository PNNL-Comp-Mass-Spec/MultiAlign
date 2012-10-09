using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Holds a link from the mass tag to the clusters that it matched to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MassTagToCluster: IFeatureMap
    {
        public MassTagToCluster()
        {
            Matches = new List<UMCClusterLightMatched>();
            MassTag = null;
        }

        public MassTagLight MassTag
        {
            get;
            set;
        }

        public List<UMCClusterLightMatched> Matches
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ID of the underlying feature.
        /// </summary>
        public object Id
        {
            get
            {
                return string.Format("{0}-{1}", MassTag.ID, MassTag.ConformationID);
            }
        }
    }
}
