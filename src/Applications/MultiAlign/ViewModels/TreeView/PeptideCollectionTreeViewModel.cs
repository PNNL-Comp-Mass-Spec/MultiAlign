using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PNNLOmics.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data;
using MultiAlign.IO;

namespace MultiAlign.ViewModels.TreeView
{
    public class PeptideCollectionTreeViewModel: GenericCollectionTreeViewModel
    {
        private UMCClusterLight m_parentCluster;

        protected ObservableCollection<PeptideTreeViewModel> m_features;

        public PeptideCollectionTreeViewModel(UMCClusterLight parentCluster):
            this(parentCluster, null)
        {
        }

        public PeptideCollectionTreeViewModel(UMCClusterLight parentCluster, TreeItemViewModel parent)
        {
            m_parent        = parent;
            m_parentCluster = parentCluster;
        }

        public override void LoadChildren()
        {
            if (m_loaded)
                return;

            List<Peptide> peptides = m_parentCluster.FindPeptides();

            if (peptides.Count < 1)
                return;

            // Tally up the unique peptides
            Dictionary<string, List<Peptide>> uniqueCounts = new Dictionary<string,List<Peptide>>();
            foreach(Peptide peptide in peptides)
            {
                if (!uniqueCounts.ContainsKey(peptide.Sequence))
                {
                    uniqueCounts.Add(peptide.Sequence, new List<Peptide>());
                }
                uniqueCounts[peptide.Sequence].Add(peptide);
            }
            // Show that to the user
            AddStatistic("Unique Identifications", uniqueCounts.Count);
            AddStatistic("Total Identifications", peptides.Count);
            

            foreach(string sequence in uniqueCounts.Keys)
            {
                // Create a collection for each unique sequence
                GenericCollectionTreeViewModel model = new GenericCollectionTreeViewModel();
                model.Name = sequence;
                model.AddStatistic("Total Members", uniqueCounts[sequence].Count);

                foreach(Peptide p in uniqueCounts[sequence])
                {
                    PeptideTreeViewModel peptideModel = new PeptideTreeViewModel(p);
                    peptideModel.FeatureSelected += new EventHandler<FeatureSelectedEventArgs>(peptideModel_FeatureSelected);
                    model.Items.Add(peptideModel);
                }


                m_items.Add(model);                                
            }
            

            m_loaded = true;
        }

        void peptideModel_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            OnFeatureSelected(e.Feature);
        }


    }
}
