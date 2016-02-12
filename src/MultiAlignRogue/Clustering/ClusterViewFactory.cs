using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Drawing;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;

namespace MultiAlignRogue.Clustering
{
    using MultiAlignCore.IO.RawData;

    using MultiAlignRogue.Utils;

    public class ClusterViewFactory : IClusterViewFactory
    {
        private readonly FeatureDataAccessProviders providers;

        public ClusterViewFactory(FeatureDataAccessProviders providers)
        {
            this.providers = providers;
        }

        public void CreateNewWindow()
        {
            throw new NotImplementedException();
        }

        public ClusterViewModel ClusterViewModel { get; private set; }

        public void CreateNewWindow(List<ClusterMatch> matches, ScanSummaryProviderCache provider)
        {
            this.ClusterViewModel = new ClusterViewModel(this, matches, providers, provider);

            var window = new ClusterView
            {
                DataContext = this.ClusterViewModel
            };

            window.Show();
        }

        public void CreateChargeStateDistributionWindow(IEnumerable<UMCClusterLight> clusters, string title)
        {
            var charges = this.providers.FeatureCache.RetrieveChargeStates();
            var viewModel = new ChargeHistogramPlot(charges.CreateHistogram(1, 10), "Charge Distribution");
            var window = new ChargeStateDistributionWindow
            {
                DataContext = viewModel
            };

            window.Show();
        }

        public void CreateSettingsWindow(ClusterViewerSettings clusterViewerSettings)
        {
            var viewModel = new ClusterViewerSettingsViewModel(clusterViewerSettings);
            var window = new ClusterViewerSettingsWindow
            {
                DataContext = viewModel,
            };

            viewModel.ReadyToClose += (o, e) => window.Close();

            window.ShowDialog();

            if (this.ClusterViewModel != null && viewModel.Status)
            {
                this.ClusterViewModel.ClusterPlotViewModel.ClusterViewerSettings = viewModel.ClusterViewerSettings;
            }
        }

        public void CreateDatasetHistogramWindow(IEnumerable<UMCClusterLight> clusters, string title)
        {
            var histogram = new Dictionary<int, int>();
            foreach (var cluster in clusters)
            {
                foreach (var umc in cluster.UmcList)
                {
                    var key = umc.GroupId + 1;
                    if (!histogram.ContainsKey(key))
                    {
                        histogram.Add(key, 0);
                    }
                    histogram[key] = histogram[key] + 1;
                }
            }
            
            var viewModel = new ChargeHistogramPlot(histogram, "Dataset Histogram");
            var window = new UmcClusterDatasetHistogram
            {
                DataContext = viewModel
            };

            window.Show();
        }
    }
}
