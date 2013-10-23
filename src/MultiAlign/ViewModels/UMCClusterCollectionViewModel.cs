
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MultiAlignCore.Data.Features;

namespace MultiAlign.ViewModels
{
    /// <summary>
    /// Root tree object.
    /// </summary>
    public class UMCClusterCollectionViewModel : TreeItemViewModel
    {
        readonly ReadOnlyCollection<UMCClusterViewModel> m_clusters;

        public UMCClusterCollectionViewModel(List<UMCClusterLightMatched> clusters)
            : this(clusters, null)
        {
        }

        public UMCClusterCollectionViewModel(List<UMCClusterLightMatched> clusters, UMCClusterViewModel parent)
        {
            m_clusters = new ReadOnlyCollection<UMCClusterViewModel>(
                (from cluster in clusters
                 select new UMCClusterViewModel(cluster)).ToList());
        }

        public ReadOnlyCollection<UMCClusterViewModel> Clusters
        {
            get
            {
                return m_clusters;
            }
        }

        public override void LoadChildren()
        {

        }
    }
}
