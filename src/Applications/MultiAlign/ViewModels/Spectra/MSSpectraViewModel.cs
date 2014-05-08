using System.Collections.Generic;
using System.Collections.ObjectModel;
using MultiAlign.IO;
using MultiAlign.ViewModels.Charting;
using MultiAlign.ViewModels.Datasets;
using MultiAlign.ViewModels.Proteins;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace MultiAlign.ViewModels.Spectra
{
    public class MsSpectraViewModel : ViewModelBase
    {
        private bool m_isExpanded;
        private MsMsSpectraViewModel m_selectedSpectrum;
        private XicViewModel m_xicModel;

        public MsSpectraViewModel(MSSpectra spectrum)
        {
            Peptides = new ObservableCollection<PeptideViewModel>();
            spectrum.Peptides.ForEach(x => Peptides.Add(new PeptideViewModel(x)));

            DatasetInformation info = SingletonDataProviders.GetDatasetInformation(spectrum.GroupId);
            if (info != null)
            {
                Dataset = new DatasetInformationViewModel(info);
            }

            Spectrum = spectrum;
            SelectedSpectrumPlotModel = new MsMsSpectraViewModel(spectrum, "");

            if (spectrum.ParentFeature != null)
            {
                MSFeatureLight msFeature = spectrum.ParentFeature;
                UMCLight umc = msFeature.ParentFeature;
                if (umc != null)
                {
                    var newList = new List<UMCLight> {umc};

                    var xic = new XicViewModel(newList, "xic")
                    {
                        Model =
                        {
                            IsLegendVisible = false,
                            TitleFontSize = 0
                        }
                    };

                    SelectedSpectrumXic = xic;
                }
            }
        }

        public ObservableCollection<PeptideViewModel> Peptides { get; set; }

        public MSSpectra Spectrum { get; private set; }

        public MsMsSpectraViewModel SelectedSpectrumPlotModel
        {
            get { return m_selectedSpectrum; }
            set
            {
                m_selectedSpectrum = value;
                OnPropertyChanged("SelectedSpectrumPlotModel");
            }
        }

        public XicViewModel SelectedSpectrumXic
        {
            get { return m_xicModel; }
            set
            {
                m_xicModel = value;
                OnPropertyChanged("SelectedSpectrumXic");
            }
        }

        public DatasetInformationViewModel Dataset { get; set; }

        public bool IdentificationsExpanded
        {
            get { return m_isExpanded; }
            set
            {
                if (m_isExpanded != value)
                {
                    m_isExpanded = value;
                    OnPropertyChanged("IdentificationsExpanded");
                }
            }
        }

        /// <summary>
        ///     Updates the view models with a maximum value for m/z and intensity.
        /// </summary>
        /// <param name="maxMz"></param>
        /// <param name="maxIntensity"></param>
        public void SetMax(double maxMz, double maxIntensity)
        {
            SelectedSpectrumPlotModel.SetMax(maxMz, maxIntensity);
        }
    }
}