using MultiAlign.ViewModels.Charting;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using System.Collections.Generic;

namespace MultiAlign.ViewModels.Viewers
{
    public class GlobalStatisticsViewModel : ViewModelBase
    {
        private ChargeHistogramViewModel m_chargeStateHistogramModel;
        private ChargeHistogramViewModel m_filteredChargeStateModel;

        public GlobalStatisticsViewModel(IEnumerable<UMCClusterLightMatched> clusters, IEnumerable<int> charges)
        {
            var histogram = charges.CreateHistogram(1, 10);
            AllChargeHistogramModel = new ChargeHistogramViewModel(histogram, "All Charge States");
            FilteredChargeHistogramModel = new ChargeHistogramViewModel(histogram, "Filtered Charge States");
        }

        

        public ChargeHistogramViewModel AllChargeHistogramModel
        {
            get
            {
                return m_chargeStateHistogramModel;
            }
            set
            {
                m_chargeStateHistogramModel = value;
                OnPropertyChanged("AllChargeHistogramModel");
            }
        }

        public ChargeHistogramViewModel FilteredChargeHistogramModel
        {
            get
            {
                return m_filteredChargeStateModel;
            }
            set
            {
                m_filteredChargeStateModel = value;
                OnPropertyChanged("FilteredChargeHistogramModel");
            }
        }
    }
}
