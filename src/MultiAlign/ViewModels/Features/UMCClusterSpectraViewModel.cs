using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PNNLOmics.Data;
using MultiAlignCore.Extensions;
using MultiAlignCore.Data.Features;
using MultiAlignCustomControls.Charting;
using System.Windows.Forms.Integration;
using MultiAlign.IO;
using System.Drawing;
using System.Windows.Input;
using MultiAlign.Commands;

namespace MultiAlign.ViewModels
{
    /// <summary>
    /// View Model for a cluster with spectra
    /// </summary>
    public class UMCClusterSpectraViewModel: ViewModelBase
    {
        /// <summary>
        /// Selected spectra
        /// </summary>
        private MSSpectraViewModel m_selectedSpectra;
        /// <summary>
        /// Selected cluster.
        /// </summary>
        private UMCClusterLightMatched m_selectedCluster;
        /// <summary>
        /// 
        /// </summary>
        WindowsFormsHost m_host;
        /// <summary>
        /// 
        /// </summary>
        SpectraChart m_chart;
        private SpectraSortOptions m_selectedSortOption;

        public UMCClusterSpectraViewModel()
        {
            Spectra = new ObservableCollection<MSSpectraViewModel>();
            SortTypes = new ObservableCollection<SpectraSortOptions>();

            CreateSortOptions();

            ExpandIdentifications = new BaseCommandBridge(delegate(object parameter)
                {
                    foreach(var spectrum in Spectra)
                    {
                        spectrum.IdentificationsExpanded = true;
                    }
                }
                );
            CollapseIdentifications = new BaseCommandBridge(delegate(object parameter)
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
            spectra.ForEach(x => Spectra.Add(new MSSpectraViewModel(x)));

            m_chart = new SpectraChart();
            m_chart.Title       = "MS/MS";
            m_chart.XAxisLabel  = "m/z";
            m_chart.YAxisLabel  = "Intensity";
            m_chart.DrawSticks  = true;            
            SpectraChart = new WindowsFormsHost() { Child = m_chart };

            if (spectra.Count > 0)
            {
                SelectedSpectra = Spectra[0];
            }

        }

        private void CreateSortOptions()
        {            
            SortTypes.Add(new SpectraSortOptions("Charge", () =>
            {
                ObservableCollection<MSSpectraViewModel> temp = new ObservableCollection<MSSpectraViewModel>(
                        from x in Spectra orderby x.Spectrum.ParentFeature.ChargeState select x);

                Spectra.Clear();
                foreach (var x in temp)
                    Spectra.Add(x);
            }
            ));


            SortTypes.Add(new SpectraSortOptions("Dataset", () =>
            {
                ObservableCollection<MSSpectraViewModel> temp = new ObservableCollection<MSSpectraViewModel>(
                        from x in Spectra orderby x.Spectrum.GroupID select x);

                Spectra.Clear();
                foreach (var x in temp)
                    Spectra.Add(x);
            }
            ));

            SortTypes.Add(new SpectraSortOptions("m/z", () =>
            {
                ObservableCollection<MSSpectraViewModel> temp = new ObservableCollection<MSSpectraViewModel>(
                        from x in Spectra orderby x.Spectrum.PrecursorMZ select x);

                Spectra.Clear();
                foreach (var x in temp)
                    Spectra.Add(x);
            }
            ));


            SortTypes.Add(new SpectraSortOptions("Scan", () =>
            {
                ObservableCollection<MSSpectraViewModel> temp = new ObservableCollection<MSSpectraViewModel>(
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

        public WindowsFormsHost SpectraChart 
        { 
            get
            {
                return m_host;
            }
            set
            {
                if (value != null && m_host != value)
                {
                    m_host = value;                    
                    OnPropertyChanged("SpectraChart");
                }
            }
        
        }
        
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

        public MSSpectraViewModel SelectedSpectra
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

        private void UpdateSpectra(MSSpectraViewModel value)
        {
            if (m_chart != null && value != null)
            {
                MSSpectra spectrum = value.Spectrum;
                if (value.Spectrum.Peaks.Count < 1)
                {
                    value.Spectrum.Peaks = SpectraLoader.LoadSpectrum(spectrum);
                }
            
                m_chart.SetSpectra(value.Spectrum.Peaks);
                m_chart.Title = string.Format("scan {0} @ {1} m/z ", spectrum.Scan,
                                                                        spectrum.PrecursorMZ);
                m_chart.AutoViewPort();
                RectangleF viewport = m_chart.ViewPort;
                m_chart.ViewPort = new System.Drawing.RectangleF(0, viewport.Y, 2000, viewport.Height);                
            }
        }

        public ObservableCollection<MSSpectraViewModel> Spectra { get; set; }
    }
}
