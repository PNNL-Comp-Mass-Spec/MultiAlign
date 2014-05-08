using System.Collections.Generic;
using PNNLOmics.Data.Features;
using PNNLOmics.Extensions;
using PNNLOmicsViz.Drawing;

namespace MultiAlign.ViewModels.Charting
{
    public class UmcClusterChargeHistogram : ChargeHistogramPlot
    {
        public UmcClusterChargeHistogram(UMCClusterLight cluster, string name)
            : base(cluster.BuildChargeStateHistogram(), name)
        {
            m_xAxis.Maximum = 10;
        }

        public UmcClusterChargeHistogram(IEnumerable<UMCClusterLight> clusters, string name)
            : base(clusters.BuildChargeStateHistogram(), name)
        {
            m_xAxis.Maximum = 10;
        }
    }
}