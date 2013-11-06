using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MultiAlign.IO;
using PNNLOmics.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels.TreeView
{

    /// <summary>
    /// Feature tree view model.
    /// </summary>
    public class MsMsTreeViewModel : GenericCollectionTreeViewModel
    {
        private MSSpectra m_feature;
        

        public MsMsTreeViewModel(MSSpectra feature)
            : this(feature, null)
        {
        }


        public MsMsTreeViewModel(MSSpectra feature, TreeItemViewModel parent)
        {
            m_feature = feature;
            m_parent  = parent;


            DatasetInformation information = SingletonDataProviders.GetDatasetInformation(m_feature.GroupID);


            AddStatistic("Id",              m_feature.ID);
            AddStatistic("Dataset Id",      m_feature.GroupID);
            AddStatistic("Precursor m/z",   m_feature.PrecursorMZ);
            AddStatistic("Charge",          m_feature.PrecursorChargeState);
            AddStatistic("Scan",            m_feature.Scan);


            Peptides = new ObservableCollection<Peptide>();

            Peptide maxPeptide = null;
            foreach (Peptide p in m_feature.Peptides)
            {
                Peptides.Add(p);
                if (maxPeptide == null)
                {
                    maxPeptide = p;
                }
                else if (p.Score < maxPeptide.Score)
                {
                    maxPeptide = p;
                }
            }

            if (maxPeptide != null)
            {
                Name = maxPeptide.Sequence;
            }
            else
            {
                Name = string.Format("Unknown - Scan: {0} m/z: {1:.00} ", m_feature.Scan, m_feature.PrecursorMZ);
            }
        }

        public ObservableCollection<Peptide> Peptides { get; set; }

        private void AddStatistic(string name, double value)
        {
            StatisticTreeViewItem x = new StatisticTreeViewItem(value, name);
            m_items.Add(x);
        }


        
        public override void LoadChildren()
        {
                      
        }
    }
}
