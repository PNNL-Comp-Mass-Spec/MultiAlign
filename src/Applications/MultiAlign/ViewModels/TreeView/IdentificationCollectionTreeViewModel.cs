using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlign.ViewModels.TreeView;
using PNNLOmics.Data.Features;
using System.Collections.ObjectModel;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data;

namespace MultiAlign.ViewModels
{
    public class IdentificationCollectionTreeViewModel : TreeItemViewModel
    {
        ObservableCollection<TreeItemViewModel> m_items;

        public IdentificationCollectionTreeViewModel(IEnumerable<UMCClusterLightMatched> tags,
                                                     IEnumerable<Peptide> peptides) :
            this(tags, peptides, null)
        {

        }

        public IdentificationCollectionTreeViewModel(IEnumerable<UMCClusterLightMatched> clusters,
                                                     IEnumerable<Peptide> peptides, 
                                                     TreeItemViewModel parent)
        {

            Dictionary<int, MassTagToCluster> massTagMap = new Dictionary<int,MassTagToCluster>();

            GenericCollectionTreeViewModel tags = new GenericCollectionTreeViewModel();
            tags.Name = "AMT Tags";

            foreach (UMCClusterLightMatched cluster in clusters)
            {
                foreach (ClusterToMassTagMap match in cluster.ClusterMatches)
                {
                    tags.Items.Add(new MassTagToClusterMatch(match.MassTag));
                }                
            }

            GenericCollectionTreeViewModel peptideItems = new GenericCollectionTreeViewModel();
            peptideItems.Name                           = "Database Searches";
            List<PeptideTreeViewModel> peptideModels    = (from peptide in peptides
                                            select new PeptideTreeViewModel(peptide)).ToList();
            foreach(PeptideTreeViewModel item in peptideModels)
            {
                peptideItems.Items.Add(item);
            }
            m_items.Add(tags);
            m_items.Add(peptideItems);
        }

        public ObservableCollection<TreeItemViewModel> Items
        {
            get
            {
                return m_items;
            }
        }
                 
        /// <summary>
        /// Loads the children 
        /// </summary>
        public override void  LoadChildren()
        {
            if (m_loaded)
                return;            
            m_loaded = true;
        }        
    }
}
