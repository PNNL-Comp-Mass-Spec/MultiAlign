using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void CreateNewWindow(IEnumerable<UMCClusterLight> clusters)
        {
            var window = new Window();

            ClusterControl clusterControl = new ClusterControl()
            {
                DataContext = new ClusterDetailViewModel()
            };

            window.Content = clusterControl;
            window.Show();
        }
    }
}
