using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignRogue.Clustering
{
    using MultiAlignCore.Data.Alignment;

    
    public interface IClusterViewFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(List<UMCClusterLight> clusters);
        void CreateChargeStateDistributionWindow(IEnumerable<UMCClusterLight> clusters, string title);
    }
}
