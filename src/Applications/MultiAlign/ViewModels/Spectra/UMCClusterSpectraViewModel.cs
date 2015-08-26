using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.IO;
using MultiAlign.ViewModels.Charting;
using MultiAlign.ViewModels.Features;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;

namespace MultiAlign.ViewModels.Spectra
{
    /// <summary>
    ///     View Model for a cluster with spectra
    /// </summary>
    public class UmcClusterSpectraViewModel : ViewModelBase
    {
        private double m_maxIntensity;
        private double m_maxMz;

        /// <summary>
        ///     Selected cluster.
        /// </summary>
        private UMCClusterLightMatched m_selectedCluster;

        private SpectraSortOptions m_selectedSortOption;

        /// <summary>
        ///     Selected spectra
        /// </summary>
        private MsSpectraViewModel m_selectedSpectra;

        private MsMsSpectraViewModel m_selectedSpectrum;

        public UmcClusterSpectraViewModel()
        {
            Spectra = new ObservableCollection<MsSpectraViewModel>();
            SortTypes = new ObservableCollection<SpectraSortOptions>();

            CreateSortOptions();

            ExpandIdentifications = new BaseCommand(delegate
            {
                foreach (var spectrum in Spectra)
                {
                    spectrum.IdentificationsExpanded = true;
                }
            }
                );


            CollapseIdentifications = new BaseCommand(delegate
            {
                foreach (var spectrum in Spectra)
                {
                    spectrum.IdentificationsExpanded = false;
                }
            }
                );
        }

        public ICommand CollapseIdentifications { get; private set; }
        public ICommand ExpandIdentifications { get; private set; }

        /// <summary>
        ///     Gets or sets the selected sort option
        /// </summary>
        public SpectraSortOptions SelectedSort
        {
            get { return m_selectedSortOption; }

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
            get { return m_selectedCluster; }
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
            get { return m_selectedSpectra; }
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

        public ObservableCollection<MsSpectraViewModel> Spectra { get; set; }

        private void LoadCluster(UMCClusterLightMatched cluster)
        {
            Spectra.Clear();

            if (cluster == null)
                return;

            //TODO: Make this a task!!!


            var spectra = cluster.Cluster.GetLoadedSpectra();
            m_maxMz = 0.0;
            m_maxIntensity = 0.0;
            foreach (var spectrum in spectra)
            {
                spectrum.Peaks = SpectraLoader.LoadSpectrum(spectrum);
                if (spectrum.Peaks.Count > 0)
                {
                    m_maxMz = Math.Max(m_maxMz, spectrum.Peaks.Max(x => x.X));
                    m_maxIntensity = Math.Max(m_maxIntensity, spectrum.Peaks.Max(x => x.Y));
                }

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
                var temp = new ObservableCollection<MsSpectraViewModel>(
                    from x in Spectra orderby x.Spectrum.ParentFeature.ChargeState select x);

                Spectra.Clear();
                foreach (var x in temp)
                {
                    var newmodel = new MsSpectraViewModel(x.Spectrum);
                    Spectra.Add(newmodel);
                }
            }
                ));


            SortTypes.Add(new SpectraSortOptions("Dataset", () =>
            {
                var temp = new ObservableCollection<MsSpectraViewModel>(
                    from x in Spectra orderby x.Spectrum.GroupId select x);

                Spectra.Clear();
                foreach (var x in temp)
                {
                    var newmodel = new MsSpectraViewModel(x.Spectrum);
                    Spectra.Add(newmodel);
                }
            }
                ));

            SortTypes.Add(new SpectraSortOptions("m/z", () =>
            {
                var temp = new ObservableCollection<MsSpectraViewModel>(
                    from x in Spectra orderby x.Spectrum.PrecursorMz select x);

                Spectra.Clear();
                foreach (var x in temp)
                {
                    var newmodel = new MsSpectraViewModel(x.Spectrum);
                    Spectra.Add(newmodel);
                }
            }
                ));


            SortTypes.Add(new SpectraSortOptions("Scan", () =>
            {
                var temp = new ObservableCollection<MsSpectraViewModel>(
                    from x in Spectra orderby x.Spectrum.RetentionTime select x);


                Spectra.Clear();
                foreach (var x in temp)
                {
                    var newmodel = new MsSpectraViewModel(x.Spectrum);
                    Spectra.Add(newmodel);
                }
            }
                ));
        }

        private void UpdateSpectra(MsSpectraViewModel value)
        {
            if (value != null)
            {
                var spectrum = value.Spectrum;
                var name = string.Format("scan {0} @ {1} m/z ", spectrum.Scan,
                    spectrum.PrecursorMz);
                SelectedSpectrumPlotModel = new MsMsSpectraViewModel(value.Spectrum, name);

                SelectedSpectrumPlotModel.SetMax(m_maxMz, m_maxIntensity);
            }
        }
    }
}