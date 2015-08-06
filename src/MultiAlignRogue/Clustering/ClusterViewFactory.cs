using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlign.ViewModels.Charting;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;
using OxyPlot.Wpf;

namespace MultiAlignRogue.Clustering
{
    using System.Windows;

    using MultiAlignRogue.Alignment;

    using PNNLOmics.Data.Features;

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

        public ClusterViewModel ClusterViewModel { get; private set; }

        public void CreateNewWindow(List<UMCClusterLight> clusters)
        {
            this.ClusterViewModel = this.ClusterViewModel ?? new ClusterViewModel(this, clusters, providers, this.layoutFilePath);
            var window = new ClusterView
            {
                DataContext = this.ClusterViewModel
            };

            window.Show();
        }

        public void CreateChargeStateDistributionWindow(IEnumerable<UMCClusterLight> clusters, string title)
        {
            var viewModel = new UmcClusterChargeHistogram(clusters, title);
            var window = new Window
            {
                Content = new PlotView
                {
                    Model = viewModel.Model
                }
            };

            window.Show();
        }
    }
}
