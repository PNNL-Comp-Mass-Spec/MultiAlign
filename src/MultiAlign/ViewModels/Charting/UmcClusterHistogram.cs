
using PNNLOmics.Data.Features;
using MultiAlignCore.Extensions;

namespace MultiAlign.ViewModels.Charting
{
    public class UmcClusterHistogram: HistogramViewModel
    {
        public UmcClusterHistogram(UMCClusterLight cluster, string name)
            : base(cluster.BuildChargeStateHistogram(), name)
        {
            m_xAxis.Maximum = 10;            
        }
    }
}
