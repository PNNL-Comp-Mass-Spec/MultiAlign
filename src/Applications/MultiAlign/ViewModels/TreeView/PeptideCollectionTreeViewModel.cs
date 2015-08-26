using System.Collections.Generic;
using System.Collections.ObjectModel;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Extensions;

namespace MultiAlign.ViewModels.TreeView
{
    public class PeptideCollectionTreeViewModel : GenericCollectionTreeViewModel
    {
        private readonly UMCClusterLight m_parentCluster;

        protected ObservableCollection<PeptideTreeViewModel> m_features;

        public PeptideCollectionTreeViewModel(UMCClusterLight parentCluster) :
            this(parentCluster, null)
        {
        }

        public PeptideCollectionTreeViewModel(UMCClusterLight parentCluster, TreeItemViewModel parent)
        {
            m_parent = parent;
            m_parentCluster = parentCluster;
        }

        public override void LoadChildren()
        {
            if (m_loaded)
                return;

            var peptides = m_parentCluster.FindPeptides();

            if (peptides.Count < 1)
                return;

            // Tally up the unique peptides
            var uniqueCounts = new Dictionary<string, List<Peptide>>();
            foreach (var peptide in peptides)
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


            foreach (var sequence in uniqueCounts.Keys)
            {
                // Create a collection for each unique sequence
                var model = new GenericCollectionTreeViewModel();
                model.Name = sequence;
                model.AddStatistic("Total Members", uniqueCounts[sequence].Count);

                foreach (var p in uniqueCounts[sequence])
                {
                    var peptideModel = new PeptideTreeViewModel(p);
                    peptideModel.FeatureSelected += peptideModel_FeatureSelected;
                    model.Items.Add(peptideModel);
                }


                m_items.Add(model);
            }


            m_loaded = true;
        }

        private void peptideModel_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            OnFeatureSelected(e.Feature);
        }
    }
}