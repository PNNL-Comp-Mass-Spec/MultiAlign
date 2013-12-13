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

        public event EventHandler<IdentificationFeatureSelectedEventArgs> SpectrumSelected;

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
            if (feature.ParentFeature != null)
            {
                AddStatistic("Charge", m_feature.ParentFeature.ChargeState);
            }
            else
            {
                AddStatistic("Charge", m_feature.PrecursorChargeState);
            }
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
                AddStatistic("Score", maxPeptide.Score);
                AddStatistic("Scan",  maxPeptide.Scan);
            }
            else
            {
                Name = string.Format("Unknown - Scan: {0} m/z: {1:.00} ", m_feature.Scan, m_feature.PrecursorMZ);
            }
        }

        public ObservableCollection<Peptide> Peptides { get; set; }


        private void OnSpectrumSelected(UMCLight feature)
        {
            if (SpectrumSelected != null)
            {
                Peptide peptide = null;
                if (m_feature.Peptides != null && m_feature.Peptides.Count  > 0)
                {
                    peptide = m_feature.Peptides[0];
                }

                IdentificationFeatureSelectedEventArgs args = new IdentificationFeatureSelectedEventArgs(
                                                                    m_feature,
                                                                    peptide,
                                                                    feature);


                SpectrumSelected(this, args);
            }
        }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                base.IsSelected = value;
                if (m_feature != null)
                {
                    UMCLight feature =  m_feature.GetParentUmc();
                    OnFeatureSelected(feature);                    
                    OnSpectrumSelected(feature);
                }
            }
        }
                
        public override void LoadChildren()
        {
                      
        }
    }
}
