using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;

namespace MultiAlignRogue.Utils
{
    public class ClusterMatch
    {
        public UMCClusterLight Cluster { get; set; }
        public MassTagLight MassTag { get; set; }
    }
}
