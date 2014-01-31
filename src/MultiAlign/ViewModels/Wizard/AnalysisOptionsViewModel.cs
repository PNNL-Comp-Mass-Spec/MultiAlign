using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureFinding;
using System.Windows.Input;
using System.Collections.ObjectModel;
using MultiAlign.ViewModels.Wizard;
using MultiAlign.Commands;
using System.Windows;
using MultiAlignCore.IO.Parameters;
using MultiAlign.Windows.Wizard;

namespace MultiAlign.ViewModels.Analysis
{
    public class AnalysisOptionsViewModel: ViewModelBase
    {
        private AnalysisOptions m_options;
        private InstrumentPresetViewModel m_instrumentPreset;
        /// <summary>
        /// Open file dialog for opening an existing parameter file.
        /// </summary>
        private System.Windows.Forms.OpenFileDialog m_dialog;
        private System.Windows.Forms.SaveFileDialog m_saveDialog;
        private ExperimentPresetViewModel m_selectedExperimentPreset;

        public AnalysisOptionsViewModel(AnalysisOptions options)
        {
            m_options            = options;
            ClusteringAlgorithms = new ObservableCollection<ClusteringAlgorithmType>();
            AlignmentAlgorithms  = new ObservableCollection<FeatureAlignmentType>();
            FeatureFindingAlgorithms = new ObservableCollection<FeatureFinderType>();

            UpdateOptions(options);

            InstrumentPresets = new ObservableCollection<InstrumentPresetViewModel>();
            ExperimentPresets = new ObservableCollection<ExperimentPresetViewModel>();

            Dictionary<string, bool> presets = new Dictionary<string, bool>();
            foreach (var preset in ExperimentPresetFactory.Create())
            {
                ExperimentPresets.Add(preset);
                InstrumentPresets.Add(preset.InstrumentPreset);

                if (!presets.ContainsKey(preset.InstrumentPreset.Name))
                {
                    presets.Add(preset.InstrumentPreset.Name, false);
                }
            }

            foreach (var preset in InstrumentPresetFactory.Create())
            {
                if (!presets.ContainsKey(preset.Name))
                {
                    InstrumentPresets.Add(preset);
                }
            }

            
            SelectedExperimentPreset = ExperimentPresets[0];

            m_saveDialog                = new System.Windows.Forms.SaveFileDialog();
            m_dialog                    = new System.Windows.Forms.OpenFileDialog();
            m_dialog.Filter             = "MultiAlign Parameters (*.xml)| *.xml|All Files (*.*)|*.*";
            m_saveDialog.Filter         = "MultiAlign Parameters (*.xml)| *.xml|All Files (*.*)|*.*";

            ShowAdvancedWindowCommand   = new BaseCommandBridge(new CommandDelegate(ShowAdvancedWindow));
            SaveOptionsCommand          = new BaseCommandBridge(new CommandDelegate(SaveCurrentParameters));
            LoadExistingCommand         = new BaseCommandBridge(new CommandDelegate(LoadExistingParameters));

            Enum.GetValues(typeof(ClusteringAlgorithmType)).Cast<ClusteringAlgorithmType>().ToList().ForEach(x => ClusteringAlgorithms.Add(x));
            Enum.GetValues(typeof(FeatureAlignmentType)).Cast<FeatureAlignmentType>().ToList().ForEach(x => AlignmentAlgorithms.Add(x));
            Enum.GetValues(typeof(FeatureFinderType)).Cast<FeatureFinderType>().ToList().ForEach(x => FeatureFindingAlgorithms.Add(x));
        }

        private void ShowAdvancedWindow(object parameter)
        {
            AdvancedOptionsViewModel viewModel  = new AdvancedOptionsViewModel(m_options);            
            AnalysisOptionsView view            = new AnalysisOptionsView();
            view.DataContext                    = viewModel;            
            view.MinWidth       = 800;
            view.MinHeight      = 600;
            view.MaxWidth       = 1200;
            view.MaxHeight      = 1024;
            view.Width          = 800;
            view.Height         = 600;
            view.ShowInTaskbar  = true;            
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.ShowDialog();

            UpdateOptions(m_options);
        }
        private void LoadExistingParameters(object parameter)
        {
            try
            {
                System.Windows.Forms.DialogResult result = m_dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    XMLParamterFileReader reader = new XMLParamterFileReader();
                    MultiAlignAnalysis analysis  = new MultiAlignAnalysis();
                    reader.ReadParameterFile(m_dialog.FileName, ref m_options);

                    UpdateOptions(m_options);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void SaveCurrentParameters(object parameter)
        {
            try
            {
                System.Windows.Forms.DialogResult result = m_saveDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    XMLParameterFileWriter writer = new XMLParameterFileWriter();
                    MultiAlignAnalysis analysis   = new MultiAlignAnalysis();
                    writer.WriteParameterFile(m_saveDialog.FileName, m_options);
                }
            }
            catch (Exception ex)
            {
            }
        }


        #region Updating 
        private void UpdateOptions(AnalysisOptions options)
        {
            m_options = options;

            OnPropertyChanged("FeatureFindingAlgorithms");
            OnPropertyChanged("IsIonMobility");
            OnPropertyChanged("HasMsMsFragmentation");
            OnPropertyChanged("MinimumScans");
            OnPropertyChanged("IsotopicFitScore");
            OnPropertyChanged("MinimumAbundance");
            OnPropertyChanged("LowMassRange");
            OnPropertyChanged("HighMassRange");            
            OnPropertyChanged("MassResolution");
            OnPropertyChanged("FragmentationTolerance");
            OnPropertyChanged("NETTolerance");
            OnPropertyChanged("DriftTimeTolerance");
            OnPropertyChanged("IsIonMobility");                               
        }
        #endregion

        #region Instrument Presets
        public ObservableCollection<InstrumentPresetViewModel> InstrumentPresets { get; private set; }
        public ObservableCollection<ExperimentPresetViewModel> ExperimentPresets { get; private set; }
        public InstrumentPresetViewModel SelectedPreset 
        {
            get
            {
                return m_instrumentPreset;
            }
            set
            {
                if (m_instrumentPreset != value)
                {
                    m_instrumentPreset = value;
                    UpdatePreset(value);
                    OnPropertyChanged("SelectedPreset");
                }
            }
        }
        public ExperimentPresetViewModel SelectedExperimentPreset 
        {
            get
            {
                return m_selectedExperimentPreset;
            }
            set
            {
                if (m_selectedExperimentPreset != value)
                {
                    m_selectedExperimentPreset = value;
                    
                    m_options.FeatureFilterOptions.MinimumMonoIsotopicMass = value.MassRangeLow;
                    m_options.FeatureFilterOptions.MaximumMonoIsotopicMass = value.MassRangeHigh;
                    HasMsMsFragmentation = value.HasMsMs;

                    SelectedPreset = value.InstrumentPreset;
                    OnPropertyChanged("SelectedExperimentPreset");
                }
            }
        }


        private void UpdatePreset(InstrumentPresetViewModel preset)
        {
            MassResolution         = preset.Mass;
            FragmentationTolerance = preset.FragmentWindowSize;
            NETTolerance           = preset.NETTolerance;
            DriftTimeTolerance     = preset.DriftTimeTolerance;
            IsIonMobility          = preset.IsIonMobility;
        }
        #endregion

        #region Experiment Type
        public bool IsIonMobility
        {
            get
            {
                return m_options.IsImsExperiment;
            }
            set
            {
                if (m_options.IsImsExperiment != value)
                {
                    m_options.IsImsExperiment               = value;                    
                    m_options.ClusterOptions.IgnoreCharge   = (value == false);

                    OnPropertyChanged("IsIonMobility");
                }
            }
        }
        public bool HasMsMsFragmentation
        {
            get
            {
                return m_options.HasMsMsData;
            }
            set
            {
                if (m_options.HasMsMsData != value)
                {
                    m_options.HasMsMsData = value;                    
                    OnPropertyChanged("HasMsMsFragmentation");
                }
            }
        }
        #endregion

        #region Instrument Tolerances
        public double MassResolution 
        {
            get
            {
                return m_options.ClusterOptions.MassTolerance;
            }
            set
            {
                if (m_options.ClusterOptions.MassTolerance != value)
                {
                    m_options.AlignmentOptions.MassTolerance = value;
                    m_options.ClusterOptions.MassTolerance   = value;
                    OnPropertyChanged("MassResolution");
                }
            }
        }
        public double NETTolerance
        {
            get
            {
                return m_options.ClusterOptions.NETTolerance;
            }
            set
            {
                if (m_options.ClusterOptions.NETTolerance != value)
                {
                    m_options.AlignmentOptions.NETTolerance = value;
                    m_options.ClusterOptions.NETTolerance   = value;
                    OnPropertyChanged("NETTolerance");
                }
            }
        }
        public double DriftTimeTolerance
        {
            get
            {
                return m_options.ClusterOptions.DriftTimeTolerance;
            }
            set
            {
                if (m_options.ClusterOptions.DriftTimeTolerance != value)
                {
                    m_options.ClusterOptions.DriftTimeTolerance = value;
                    OnPropertyChanged("DriftTimeTolerance");
                }
            }
        }
        public double FragmentationTolerance
        {
            get
            {
                return m_options.MSLinkerOptions.MzTolerance;
            }
            set
            {
                if (m_options.MSLinkerOptions.MzTolerance != value)
                {
                    m_options.MSLinkerOptions.MzTolerance = value;                    
                    OnPropertyChanged("FragmentationTolerance");
                }
            }
        }
        public double HighMassRange
        {
            get
            {
                return m_options.FeatureFilterOptions.MaximumMonoIsotopicMass;
            }
            set
            {
                if (m_options.FeatureFilterOptions.MaximumMonoIsotopicMass != value)
                {
                    m_options.FeatureFilterOptions.MaximumMonoIsotopicMass = value;
                    OnPropertyChanged("HighMassRange");
                }
            }
        }
        public double LowMassRange
        {
            get
            {
                return m_options.FeatureFilterOptions.MinimumMonoIsotopicMass;
            }
            set
            {
                if (m_options.FeatureFilterOptions.MinimumMonoIsotopicMass != value)
                {
                    m_options.FeatureFilterOptions.MinimumMonoIsotopicMass = value;
                    OnPropertyChanged("LowMassRange");
                }
            }
        }
        #endregion

        #region Algorithms
        public ObservableCollection<ClusteringAlgorithmType> ClusteringAlgorithms { get; set; }
        public ObservableCollection<FeatureAlignmentType>    AlignmentAlgorithms { get; set; }
        public ObservableCollection<FeatureFinderType>       FeatureFindingAlgorithms { get; set; }

        public ClusteringAlgorithmType SelectedClusteringAlgorithm
        {
            get
            {
                return m_options.ClusterOptions.ClusteringAlgorithm;
            }
            set
            {
                if (m_options.ClusterOptions.ClusteringAlgorithm != value)
                {
                    m_options.ClusterOptions.ClusteringAlgorithm = value;
                    OnPropertyChanged("SelectedClusteringAlgorithm");
                }
            }
        }
        public FeatureAlignmentType SelectedAlignmentAlgorithm
        {
            get
            {
                return m_options.AlignmentOptions.AlignmentAlgorithm;
            }
            set
            {
                if (m_options.AlignmentOptions.AlignmentAlgorithm != value)
                {
                    m_options.AlignmentOptions.AlignmentAlgorithm = value;
                    OnPropertyChanged("SelectedAlignmentAlgorithm");
                }
            }
        }
        public FeatureFinderType SelectedFeatureFindingAlgorithm
        {
            get
            {
                return m_options.FeatureFindingOptions.FeatureFinderAlgorithm;
            }
            set
            {
                if (m_options.FeatureFindingOptions.FeatureFinderAlgorithm != value)
                {
                    m_options.FeatureFindingOptions.FeatureFinderAlgorithm = value;                    
                    OnPropertyChanged("SelectedFeatureFindingAlgorithm");
                }
            }
        }

        public double ClusteringDriftTime
        {
            get
            {
                return m_options.ClusterOptions.DriftTimeTolerance;
            }
            set
            {
                if (m_options.ClusterOptions.DriftTimeTolerance != value)
                {
                    m_options.ClusterOptions.DriftTimeTolerance = value;
                    OnPropertyChanged("ClusteringDriftTime");
                }
            }
        }
        public double ClusteringMassTolerance
        {
            get
            {
                return m_options.ClusterOptions.MassTolerance;
            }
            set
            {
                if (m_options.ClusterOptions.MassTolerance != value)
                {
                    m_options.ClusterOptions.MassTolerance = value;
                    OnPropertyChanged("ClusteringMassTolerance");
                }
            }
        }
        public double ClusteringNetTolerance
        {
            get
            {
                return m_options.ClusterOptions.NETTolerance;
            }
            set
            {
                if (m_options.ClusterOptions.NETTolerance != value)
                {
                    m_options.ClusterOptions.NETTolerance = value;
                    OnPropertyChanged("ClusteringNetTolerance");
                }
            }
        }        
        #endregion

        #region Feature Definition Algorithm Parameters
        public int MinimumFeatureLength 
        {
            get
            {
                return m_options.FeatureFilterOptions.MinimumScanLength;
            }
            set
            {
                if (m_options.FeatureFilterOptions.MinimumScanLength != value)
                {
                    m_options.FeatureFilterOptions.MinimumScanLength = value;
                    m_options.FeatureFindingOptions.MinUMCLength     = value;
                    OnPropertyChanged("MinimumFeatureLength");
                }
            }
        }
        public double DeisotopingFitScore 
        {
            get
            {
                return m_options.FeatureFindingOptions.IsotopicFitFilter;
            }
            set
            {
                if (m_options.FeatureFindingOptions.IsotopicFitFilter != value)
                {
                    m_options.FeatureFindingOptions.IsotopicFitFilter = value;
                    OnPropertyChanged("DeisotopingFitScore");
                }
            }
        }
        public double MinimumAbundance
        {
            get
            {
                return m_options.FeatureFilterOptions.MinimumAbundance;
            }
            set
            {
                if (m_options.FeatureFilterOptions.MinimumAbundance != value)
                {
                    m_options.FeatureFilterOptions.MinimumAbundance = value;
                    OnPropertyChanged("MinimumAbundance");
                }
            }
        }
        #endregion

        #region Commands 
        public ICommand ShowAdvancedWindowCommand 
        { 
            get;
            set; 
        }
        public ICommand LoadExistingCommand
        {
            get;
            set;
        }
        public ICommand SaveOptionsCommand { get; set; }
        #endregion
    }

}
