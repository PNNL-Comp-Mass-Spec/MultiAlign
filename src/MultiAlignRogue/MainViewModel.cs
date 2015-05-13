using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs;
using Ookii;
using MultiAlign;
using MultiAlign.Commands;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.ViewModels.Datasets;
using MultiAlign.ViewModels.Wizard;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.InputFiles;
using OxyPlot;

namespace MultiAlignRogue
{
    public class MainViewModel : INotifyPropertyChanged
    {


        public AnalysisDatasetSelectionViewModel DataSelectionViewModel;
        public MultiAlignAnalysis m_analysis { get; set; }
        VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog();
        private AnalysisConfig m_config;
        private MultiAlignAnalysisOptions m_options;
        private string[] files; 
        public List<DatasetInformation> selectedFiles; 
        public RelayCommand SelectFilesCommand { get; private set; }
        public RelayCommand FindMSFeaturesCommand { get; private set; }
        public RelayCommand PlotMSFeaturesCommand { get; private set; }
        public ICommand AddFolderCommand { get; private set; }
        public string inputFilePath { get; set; }
        public DataTable datasetInfo { get; set; }
        public MSFeatureFinding featureFinder { get; set; }
 

        public MainViewModel()
        {
            m_config = new AnalysisConfig();
            m_analysis = new MultiAlignAnalysis();
            m_options = m_analysis.Options;
            m_config.Analysis = m_analysis;
            SelectFilesCommand = new RelayCommand(SelectFiles);
            FindMSFeaturesCommand = new RelayCommand(LoadMSFeatures);
            PlotMSFeaturesCommand = new RelayCommand(PlotMSFeatures);

            DataSelectionViewModel = new AnalysisDatasetSelectionViewModel(m_config.Analysis);
            AddFolderCommand = new BaseCommand(AddFolderDelegate, BaseCommand.AlwaysPass);
            featureFinder = new MSFeatureFinding{Providers = m_config.Analysis.DataProviders};
            this.selectedFiles = new List<DatasetInformation>();
        }


        #region Import Files
        public void SelectFiles()
        {
            var result = folderBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                inputFilePath = folderBrowser.SelectedPath;
                OnPropertyChanged("inputFilePath");
                files = Directory.GetFiles(inputFilePath, "*isos.csv");

            }

        }

        public void AddFolderDelegate()
        {
            var supportedTypes = DatasetInformation.SupportedFileTypes;
            var extensions = new List<string>();

            supportedTypes.ForEach(x => extensions.Add("*" + x.Extension));

            if (inputFilePath == null)
            {
                ApplicationStatusMediator.SetStatus("Select a folder path first. File -> Select Files");
                System.Windows.MessageBox.Show("Select a folder path first. File -> Select Files");
                return;
            }

            if (!Directory.Exists(inputFilePath))
            {
                ApplicationStatusMediator.SetStatus("The directory specified does not exist.");
                System.Windows.MessageBox.Show("The directory specified does not exist.");
                return;
            }

            var files = DatasetSearcher.FindDatasets(inputFilePath,
                extensions,
                SearchOption.TopDirectoryOnly);
            DataSelectionViewModel.AddDatasets(files);
            OnPropertyChanged("m_analysis");
            //DisplayWorkingDataset();
        }
        #endregion


        public void LoadMSFeatures()
        {
            foreach (var file in selectedFiles)
            {
                featureFinder.LoadDataset(file, m_options.MsFilteringOptions, m_options.LcmsFindingOptions, m_options.LcmsFilteringOptions);
            }
            
        }

        public void PlotMSFeatures()
        {
            
        }



        


        #region MS Feature Settings
        public double MassResolution
        {
            get { return m_options.InstrumentTolerances.Mass; }
            set
            {
                m_options.AlignmentOptions.MassTolerance = value;
                m_options.InstrumentTolerances.Mass = value;
                OnPropertyChanged("MassResolution");
            }
        }

        public double MinimumFeatureLength
        {
            get { return m_options.LcmsFilteringOptions.FeatureLengthRange.Minimum; }
            set
            {
                m_options.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                m_options.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                OnPropertyChanged("MinimumFeatureLength");
            }
        }

        public double MaximumFeatureLength
        {
            get { return m_options.LcmsFilteringOptions.FeatureLengthRange.Maximum; }
            set
            {
                m_options.LcmsFilteringOptions.FeatureLengthRange.Maximum = value;
                m_options.LcmsFilteringOptions.FeatureLengthRange.Maximum = value;
                OnPropertyChanged("MaximumFeatureLength");
            }
        }

        public double MinimumIntensity
        {
            get { return m_options.MsFilteringOptions.MinimumIntensity; }
            set
            {
                m_options.MsFilteringOptions.MinimumIntensity = value;
                OnPropertyChanged("MinimumIntensity");
            }
        }

        public double FragmentationTolerance
        {
            get { return m_options.InstrumentTolerances.FragmentationWindowSize; }
            set
            {
                m_options.InstrumentTolerances.FragmentationWindowSize = value;
                OnPropertyChanged("FragmentationTolerance");
            }
        }

        public double MaximumMz
        {
            get { return m_options.MsFilteringOptions.MzRange.Maximum; }
            set
            {
                m_options.MsFilteringOptions.MzRange.Maximum = value;
                OnPropertyChanged("MaximumMz");
            }
        }

        public double MinimumMz
        {
            get { return m_options.MsFilteringOptions.MzRange.Minimum; }
            set
            {
                m_options.MsFilteringOptions.MzRange.Minimum = value;
                OnPropertyChanged("MinimumMz");
            }
        }


        public double MaximumCharge
        {
            get { return m_options.MsFilteringOptions.ChargeRange.Maximum; }
            set
            {
                m_options.MsFilteringOptions.ChargeRange.Maximum = value;
                OnPropertyChanged("MaximumCharge");
            }
        }

        public double MinimumCharge
        {
            get { return m_options.MsFilteringOptions.ChargeRange.Minimum; }
            set
            {
                m_options.MsFilteringOptions.ChargeRange.Minimum = value;
                OnPropertyChanged("MinimumCharge");
            }
        }

        public double MinimumDeisotopingScore
        {
            get { return m_options.MsFilteringOptions.MinimumDeisotopingScore; }
            set
            {
                m_options.MsFilteringOptions.MinimumDeisotopingScore = value;
                OnPropertyChanged("MinimumDeisotopingScore");
            }
        }

        public bool ShouldUseChargeStateFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseChargeFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseChargeFilter = value;
                OnPropertyChanged("ShouldUseChargeStateFilter");
            }
        }

        public bool ShouldUseMzFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseMzFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseMzFilter = value;
                OnPropertyChanged("ShouldUseMzFilter");
            }
        }

        public bool ShouldUseIntensityFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseIntensityFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseIntensityFilter = value;
                OnPropertyChanged("ShouldUseIntensityFilter");
            }
        }

        public bool ShouldUseDeisotopingFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseDeisotopingFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseDeisotopingFilter = value;
                OnPropertyChanged("ShouldUseDeisotopingFilter");
            }
        }
        #endregion

        

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
