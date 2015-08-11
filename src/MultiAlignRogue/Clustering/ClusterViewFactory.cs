using System;
using System.Collections.Generic;
using MultiAlign.ViewModels.Charting;
using MultiAlignCore.Drawing;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue.Clustering
{

    public class ClusterViewFactory : IClusterViewFactory
    {
        private readonly string layoutFilePath;

        private readonly FeatureDataAccessProviders providers;

        public ClusterViewFactory(FeatureDataAccessProviders providers, string layoutFilePath = null)
        {
            this.providers = providers;
            this.layoutFilePath = layoutFilePath ?? "layout.xml";
        }

        public void CreateNewWindow()
        {
            throw new NotImplementedException();
        }

        public void CreateNewWindow(List<UMCClusterLight> clusters)
        {
            var clusterViewModel = new ClusterViewModel(this, clusters, providers, this.layoutFilePath);
            var window = new ClusterView
            {
                DataContext = clusterViewModel
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

            window.ShowDialog();
        }
    }
}
