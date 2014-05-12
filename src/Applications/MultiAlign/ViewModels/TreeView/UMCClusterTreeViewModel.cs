using System;
using System.Collections.ObjectModel;
using System.Globalization;
using MultiAlign.IO;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.Features;

namespace MultiAlign.ViewModels.TreeView
{
    /// <summary>
    ///     Cluster Tree View Model
    /// </summary>
    public class UMCClusterTreeViewModel : TreeItemViewModel
    {
        private readonly GenericCollectionTreeViewModel m_allIdentifications;

        /// <summary>
        ///     The cluster in question
        /// </summary>
        private readonly UMCClusterLightMatched m_cluster;

        private readonly ObservableCollection<TreeItemViewModel> m_items;
        private MsMsCollectionTreeViewModel m_spectra;

        public UMCClusterTreeViewModel(UMCClusterLightMatched cluster) :
            this(cluster, null)
        {
        }

        public UMCClusterTreeViewModel(UMCClusterLightMatched matchedCluster, TreeItemViewModel parent)
        {
            m_items = new ObservableCollection<TreeItemViewModel>();
            m_parent = parent;
            m_cluster = matchedCluster;
            var features = new UMCCollectionTreeViewModel(matchedCluster.Cluster);
            features.FeatureSelected += feature_FeatureSelected;
            features.Name = "Features";

            // Cluster level statistics
            var cluster = matchedCluster.Cluster;
            AddStatistics("Mass", cluster.MassMonoisotopic);


            var item = new StringTreeViewItem(
                cluster.RetentionTime.ToString("F3",
                    CultureInfo.InvariantCulture),
                "NET");

            m_items.Add(item);

            if (cluster.DriftTime > 0)
            {
                AddStatistics("Drift Time", cluster.DriftTime);
            }
            AddStatistics("Datasets", cluster.DatasetMemberCount);
            AddStatistics("Total", cluster.MemberCount);

            AddStatistics("Total MS/MS", cluster.MsMsCount);
            AddStatistics("Total Identifications", cluster.IdentifiedSpectraCount);

            var allIdentifications = new GenericCollectionTreeViewModel();
            allIdentifications.Name = "Identifications";

            // Items to display the base childen.
            var identifications = new PeptideCollectionTreeViewModel(cluster);
            identifications.Name = "Search Results";
            identifications.FeatureSelected += feature_FeatureSelected;


            var spectra = new MsMsCollectionTreeViewModel(cluster);
            spectra.Name = "MS/MS";
            m_spectra = spectra;
            spectra.SpectrumSelected += spectra_SpectrumSelected;
            spectra.FeatureSelected += feature_FeatureSelected;

            var matches = new MassTagCollectionMatchTreeViewModel(matchedCluster.ClusterMatches);
            matches.Name = "AMT Tags";

            allIdentifications.Items.Add(identifications);
            allIdentifications.Items.Add(spectra);
            allIdentifications.Items.Add(matches);
            m_allIdentifications = allIdentifications;

            m_items.Add(features);
            m_items.Add(allIdentifications);
        }


        public int DatasetMemberCount
        {
            get { return m_cluster.Cluster.DatasetMemberCount; }
        }

        public ObservableCollection<TreeItemViewModel> Items
        {
            get { return m_items; }
        }

        public UMCClusterLightMatched Cluster
        {
            get { return m_cluster; }
        }


        /// <summary>
        ///     Gets the ID of the cluster.
        /// </summary>
        public string ClusterId
        {
            get { return m_cluster.Cluster.Id.ToString(); }
        }

        public event EventHandler<IdentificationFeatureSelectedEventArgs> SpectrumSelected;

        private void spectra_SpectrumSelected(object sender, IdentificationFeatureSelectedEventArgs e)
        {
            if (SpectrumSelected != null)
            {
                SpectrumSelected(sender, e);
            }
        }

        private void feature_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            OnFeatureSelected(e.Feature);
        }

        private void AddStatistics(string name, double value)
        {
            var item = new StatisticTreeViewItem(value, name);
            m_items.Add(item);
        }

        public override void LoadChildren()
        {
            if (m_loaded)
                return;

            try
            {
                m_cluster.Cluster.ReconstructUMCCluster(SingletonDataProviders.Providers, true, true);

                foreach (var treeModel in m_items)
                {
                    treeModel.LoadChildren();
                }


                foreach (var treeModel in m_allIdentifications.Items)
                {
                    treeModel.LoadChildren();
                }
            }
            catch (Exception ex)
            {
                var x = 0;
                x++;
                if (x > 5)
                {
                    throw ex;
                }
                //TODO: Handle error?!
            }
        }
    }
}