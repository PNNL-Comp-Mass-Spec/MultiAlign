using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignRogue.Clustering
{
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.IO.RawData;

    using MultiAlignRogue.Utils;

    public interface IClusterViewFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(List<ClusterMatch> clusters, ScanSummaryProviderCache provider);
        void CreateChargeStateDistributionWindow(IEnumerable<UMCClusterLight> clusters, string title);
        void CreateDatasetHistogramWindow(IEnumerable<UMCClusterLight> clusters, string title);
        void CreateSettingsWindow(ClusterViewerSettings clusterViewerSettings);
    }
}
