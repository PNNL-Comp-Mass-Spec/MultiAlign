using MultiAlign.Commands;
using MultiAlign.IO;
using MultiAlign.ViewModels.Charting;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MultiAlign.ViewModels.Spectra
{
    /// <summary>
    /// View Model for a cluster with spectra
    /// </summary>
    public class UmcClusterSpectraViewModel: ViewModelBase
    {
        /// <summary>
        /// Selected spectra
        /// </summary>
        private MsSpectraViewModel m_selectedSpectra;
        /// <summary>
        /// Selected cluster.
        /// </summary>
        private UMCClusterLightMatched m_selectedCluster;
        private SpectraSortOptions m_selectedSortOption;
        private MsMsSpectraViewModel m_selectedSpectrum;
        private double m_maxMz;
        private double m_maxIntensity;

        public UmcClusterSpectraViewModel()
        {
            Spectra   = new ObservableCollection<MsSpectraViewModel>();
            SortTypes = new ObservableCollection<SpectraSortOptions>();

            CreateSortOptions();

            ExpandIdentifications = new BaseCommandBridge(delegate
            {
                    foreach(var spectrum in Spectra)
                    {
                        spectrum.IdentificationsExpanded = true;
                    }
                }
                );
            CollapseIdentifications = new BaseCommandBridge(delegate
            {
                foreach (var spectrum in Spectra)
                {
                    spectrum.IdentificationsExpanded = false;
                }
            }
                );
        }

        public ICommand CollapseIdentifications
        {
            get;
            private set;
        }
        public ICommand ExpandIdentifications
        {
            get;
            private set;
        }
        
        private void LoadCluster(UMCClusterLightMatched cluster)
        {            
            Spectra.Clear();

            if (cluster == null)
                return;

            List<MSSpectra> spectra = cluster.Cluster.GetLoadedSpectra();
            m_maxMz        = 0.0;
            m_maxIntensity = 0.0;
            foreach (var spectrum in spectra)
            {
                spectrum.Peaks  = SpectraLoader.LoadSpectrum(spectrum);
                m_maxMz = Math.Max(m_maxMz, spectrum.Peaks.Max(x => x.X));
                m_maxIntensity = Math.Max(m_maxIntensity, spectrum.Peaks.Max(x => x.Y));

                var msSpectrum = new MsSpectraViewModel(spectrum);                
                Spectra.Add(msSpectrum);
            }

            foreach (var model in Spectra)
            {
                model.SetMax(m_maxMz, m_maxIntensity);
                model.SelectedSpectrumPlotModel.PlotSpectra(model.Spectrum);
            }

            if (spectra.Count > 0)
            {
                SelectedSpectra = Spectra[0];
            }


        }

        private void CreateSortOptions()
        {            
            SortTypes.Add(new SpectraSortOptions("Charge", () =>
            {
                ObservableCollection<MsSpectraViewModel> temp = new ObservableCollection<MsSpectraViewModel>(
                        from x in Spectra orderby x.Spectrum.ParentFeature.ChargeState select x);

                Spectra.Clear();
                foreach (var x in temp)
                    Spectra.Add(x);
            }
            ));


            SortTypes.Add(new SpectraSortOptions("Dataset", () =>
            {
                ObservableCollection<MsSpectraViewModel> temp = new ObservableCollection<MsSpectraViewModel>(
                        from x in Spectra orderby x.Spectrum.GroupID select x);

                Spectra.Clear();
                foreach (var x in temp)
                    Spectra.Add(x);
            }
            ));

            SortTypes.Add(new SpectraSortOptions("m/z", () =>
            {
                ObservableCollection<MsSpectraViewModel> temp = new ObservableCollection<MsSpectraViewModel>(
                        from x in Spectra orderby x.Spectrum.PrecursorMZ select x);

                Spectra.Clear();
                foreach (var x in temp)
                    Spectra.Add(x);
            }
            ));


            SortTypes.Add(new SpectraSortOptions("Scan", () =>
            {
                ObservableCollection<MsSpectraViewModel> temp = new ObservableCollection<MsSpectraViewModel>(
                        from x in Spectra orderby x.Spectrum.RetentionTime select x);

                Spectra.Clear();
                foreach (var x in temp)
                    Spectra.Add(x);
            }
            ));

        }
        /// <summary>
        /// Gets or sets the selected sort option
        /// </summary>
        public SpectraSortOptions SelectedSort
        {
            get
            {
                return m_selectedSortOption;
            }

            set
            {
                if (m_selectedSortOption != value)
                {
                    m_selectedSortOption = value;
                    if (value != null)
                        m_selectedSortOption.Action.Invoke();

                    OnPropertyChanged("SelectedSort");
                }
            }
        }
        public ObservableCollection<SpectraSortOptions> SortTypes { get; private set; }                
        public UMCClusterLightMatched SelectedCluster
        {
            get
            {
                return m_selectedCluster;
            }
            set
            {
                
                if (m_selectedCluster != value)
                {
                    m_selectedCluster = value;
                    LoadCluster(value);
                    OnPropertyChanged("SelectedCluster");
                }
            }
        }

        public MsSpectraViewModel SelectedSpectra
        {
            get
            {
                return m_selectedSpectra;
            }
            set
            {
                if (value != m_selectedSpectra)
                {
                    m_selectedSpectra = value;
                    UpdateSpectra(value);
                    OnPropertyChanged("SelectedSpectra");
                }
            }
        }

        public MsMsSpectraViewModel SelectedSpectrumPlotModel
        {
            get { return m_selectedSpectrum; }
            set
            {
                m_selectedSpectrum = value;
                OnPropertyChanged("SelectedSpectrumPlotModel");
            }
        }

        private void UpdateSpectra(MsSpectraViewModel value)
        {
            if (value != null)
            {
                var spectrum = value.Spectrum;
                var name     = string.Format("scan {0} @ {1} m/z ", spectrum.Scan,
                                                                    spectrum.PrecursorMZ);
                SelectedSpectrumPlotModel = new MsMsSpectraViewModel(value.Spectrum, name);

                this.SelectedSpectrumPlotModel.SetMax(m_maxMz, m_maxIntensity);             
            }
        }

        public ObservableCollection<MsSpectraViewModel> Spectra { get; set; }
    }
}
