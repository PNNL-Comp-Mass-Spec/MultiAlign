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
        public void CreateNewWindow()
        {
            throw new NotImplementedException();
        }

        public void CreateNewWindow(List<UMCClusterLight> clusters)
        {
            var clusterViewModel = new ClusterViewModel(clusters);
            var window = new ClusterView
            {
                DataContext = clusterViewModel
            };

            window.Show();
        }
    }
}
