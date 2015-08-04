using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignRogue.Clustering
{
    using System.Windows;

    using MultiAlignRogue.Alignment;

    using PNNLOmics.Data.Features;

    public class ClusterViewFactory : IClusterViewFactory
    {
        private readonly string layoutFilePath;

        public ClusterViewFactory(string layoutFilePath = null)
        {
            this.layoutFilePath = layoutFilePath ?? "layout.xml";
        }

        public void CreateNewWindow()
        {
            throw new NotImplementedException();
        }

        public ClusterViewModel ClusterViewModel { get; private set; }

        public void CreateNewWindow(List<UMCClusterLight> clusters)
        {
            this.ClusterViewModel = this.ClusterViewModel ?? new ClusterViewModel(clusters, this.layoutFilePath);
            var window = new ClusterView
            {
                DataContext = this.ClusterViewModel
            };

            window.Show();
        }
    }
}
