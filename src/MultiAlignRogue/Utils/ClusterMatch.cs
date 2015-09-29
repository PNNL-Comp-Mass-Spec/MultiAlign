using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignRogue.Utils
{
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MassTags;

    public class ClusterMatch
    {
        public UMCClusterLight Cluster { get; set; }
        public MassTagLight MassTag { get; set; }
    }
}
