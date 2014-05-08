using System.Collections.Generic;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmicsViz.Drawing;

namespace MultiAlign.ViewModels.Viewers
{
    public class GlobalStatisticsViewModel : ViewModelBase
    {
        private ChargeHistogramPlot m_chargeStateHistogramModel;
        private ChargeHistogramPlot m_filteredChargeStateModel;

        public GlobalStatisticsViewModel(IEnumerable<UMCClusterLightMatched> clusters, IEnumerable<int> charges)
        {
            Dictionary<int, int> histogram = charges.CreateHistogram(1, 10);
            AllChargeHistogramModel = new ChargeHistogramPlot(histogram, "All Charge States");
            FilteredChargeHistogramModel = new ChargeHistogramPlot(histogram, "Filtered Charge States");
        }


        public ChargeHistogramPlot AllChargeHistogramModel
        {
            get { return m_chargeStateHistogramModel; }
            set
            {
                m_chargeStateHistogramModel = value;
                OnPropertyChanged("AllChargeHistogramModel");
            }
        }

        public ChargeHistogramPlot FilteredChargeHistogramModel
        {
            get { return m_filteredChargeStateModel; }
            set
            {
                m_filteredChargeStateModel = value;
                OnPropertyChanged("FilteredChargeHistogramModel");
            }
        }
    }
}