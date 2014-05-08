using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data;

namespace MultiAlign.ViewModels.TreeView
{
    public class IdentificationCollectionTreeViewModel : TreeItemViewModel
    {
        private ObservableCollection<TreeItemViewModel> m_items;

        public IdentificationCollectionTreeViewModel(IEnumerable<UMCClusterLightMatched> tags,
            IEnumerable<Peptide> peptides) :
                this(tags, peptides, null)
        {
        }

        public IdentificationCollectionTreeViewModel(IEnumerable<UMCClusterLightMatched> clusters,
            IEnumerable<Peptide> peptides,
            TreeItemViewModel parent)
        {
            var tags = new GenericCollectionTreeViewModel();
            tags.Name = "AMT Tags";

            foreach (UMCClusterLightMatched cluster in clusters)
            {
                foreach (ClusterToMassTagMap match in cluster.ClusterMatches)
                {
                    tags.Items.Add(new MassTagToClusterMatch(match.MassTag));
                }
            }

            var peptideItems = new GenericCollectionTreeViewModel();
            peptideItems.Name = "Database Searches";
            List<PeptideTreeViewModel> peptideModels = (from peptide in peptides
                select new PeptideTreeViewModel(peptide)).ToList();
            foreach (PeptideTreeViewModel item in peptideModels)
            {
                peptideItems.Items.Add(item);
            }
            m_items = new ObservableCollection<TreeItemViewModel> {tags, peptideItems};
        }

        /// <summary>
        ///     Loads the children
        /// </summary>
        public override void LoadChildren()
        {
            if (m_loaded)
                return;
            m_loaded = true;
        }
    }
}