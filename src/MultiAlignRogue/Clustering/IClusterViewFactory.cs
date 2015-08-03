using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignRogue.Clustering
{
    using MultiAlignCore.Data.Alignment;

    using PNNLOmics.Data.Features;

    public interface IClusterViewFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(List<UMCClusterLight> clusters);
    }
}
