using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Extensions;

namespace MultiAlign.ViewModels.TreeView
{
    public class MsMsCollectionTreeViewModel : TreeItemViewModel
    {
        private readonly UMCClusterLight m_cluster;
        protected ObservableCollection<MsMsTreeViewModel> m_spectra;

        public MsMsCollectionTreeViewModel(UMCClusterLight cluster) :
            this(cluster, null)
        {
        }

        public MsMsCollectionTreeViewModel(UMCClusterLight cluster, UMCClusterTreeViewModel parent)
        {
            m_cluster = cluster;
            m_parent = parent;
            m_spectra = new ObservableCollection<MsMsTreeViewModel>();
        }

        public ObservableCollection<MsMsTreeViewModel> Spectra
        {
            get { return m_spectra; }
        }

        public event EventHandler<IdentificationFeatureSelectedEventArgs> SpectrumSelected;


        public override void LoadChildren()
        {
            if (m_loaded)
                return;

            var spectra = m_cluster.FindSpectra();

            var peptideModels =
                (from spectrum in spectra
                    select new MsMsTreeViewModel(spectrum)).ToList();

            foreach (var model in peptideModels)
            {
                m_spectra.Add(model);
                model.FeatureSelected += model_FeatureSelected;
                model.SpectrumSelected += model_SpectrumSelected;
            }

            m_loaded = true;
        }

        private void model_SpectrumSelected(object sender, IdentificationFeatureSelectedEventArgs e)
        {
            if (SpectrumSelected != null)
            {
                SpectrumSelected(sender, e);
            }
        }

        private void model_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            OnFeatureSelected(e.Feature);
        }
    }
}