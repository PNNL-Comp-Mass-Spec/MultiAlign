using System;
using System.Windows.Input;
using MultiAlign.Commands.Wizard;
using MultiAlign.Data;
using MultiAlign.Data.States;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels.Wizard
{
    public class AnalysisSetupViewModel : ViewModelBase
    {
        private ICommand m_backCommand;
        private AnalysisBaselineSelectionViewModel m_baselineViewModel;
        private ICommand m_cancelCommand;
        private AnalysisConfig m_config;
        private AnalysisSetupStep m_currentStep;
        private AnalysisDatasetSelectionViewModel m_datasetViewModel;
        private AnalysisNamingViewModel m_namingViewModel;
        private ICommand m_nextCommand;

        public AnalysisSetupViewModel(AnalysisConfig configuration)
        {
            SetupViewModels(configuration);
        }

        public AnalysisConfig AnalysisConfiguration
        {
            get { return m_config; }
            set
            {
                if (m_config != value)
                {
                    SetupViewModels(value);
                    OnPropertyChanged("");
                }
            }
        }

        public AnalysisSetupStep CurrentStep
        {
            get { return m_currentStep; }
            set
            {
                if (m_currentStep != value)
                {
                    m_currentStep = value;
                    OnPropertyChanged("CurrentStep");
                }
            }
        }

        public int DatasetCount
        {
            get { return m_config.Analysis.MetaData.Datasets.Count; }
        }

        public string ParameterFileName
        {
            get { return m_config.ParameterFile; }
            set
            {
                if (m_config.ParameterFile != value)
                {
                    m_config.ParameterFile = value;
                    OnPropertyChanged("ParameterFileName");
                }
            }
        }

        #region Commands

        /// <summary>
        /// Gets the command for cancelling an analysis setup.
        /// </summary>
        public ICommand CancelCommand
        {
            get { return m_cancelCommand; }
            set
            {
                if (m_cancelCommand != value)
                {
                    m_cancelCommand = value;
                    OnPropertyChanged("CancelCommand");
                }
            }
        }

        /// <summary>
        /// Gets the command used to move the analysis setup back one step
        /// </summary>
        public ICommand BackCommand
        {
            get { return m_backCommand; }
            private set
            {
                if (m_backCommand != value)
                {
                    m_backCommand = value;
                    OnPropertyChanged("BackCommand");
                }
            }
        }

        /// <summary>
        /// Gets the command for progressing the analysis setup one step
        /// </summary>
        public ICommand NextCommand
        {
            get { return m_nextCommand; }
            private set
            {
                if (m_nextCommand != value)
                {
                    m_nextCommand = value;
                    OnPropertyChanged("NextCommand");
                }
            }
        }

        #endregion

        #region Analysis Setup Control

        public void MoveNext()
        {
            // Validate the move
            var errorMessage = "";
            var isValid = MultiAlignAnalysisValidator.IsStepValid(AnalysisConfiguration, CurrentStep, ref errorMessage);

            // Then display the error if exists...
            if (!isValid)
            {
                ApplicationStatusMediator.SetStatus(errorMessage);
                return;
            }
            ApplicationStatusMediator.SetStatus(errorMessage);

            // Then move the UI.
            switch (CurrentStep)
            {
                case AnalysisSetupStep.DatasetSelection:
                    CurrentStep = AnalysisSetupStep.OptionsSelection;
                    break;
                case AnalysisSetupStep.OptionsSelection:
                    BaselineSelectionViewModel.UpdateDatasets();
                    CurrentStep = AnalysisSetupStep.BaselineSelection;
                    break;
                case AnalysisSetupStep.BaselineSelection:
                    CurrentStep = AnalysisSetupStep.Naming;
                    break;
                case AnalysisSetupStep.Naming:
                    CurrentStep = AnalysisSetupStep.Started;
                    if (AnalysisStart != null)
                    {
                        AnalysisConfiguration.ParameterFile = AnalysisConfiguration.AnalysisName + ".xml";
                        AnalysisStart(this, null);
                    }
                    break;
                case AnalysisSetupStep.Started:
                    break;
                default:
                    break;
            }
        }

        public void MoveBack()
        {
            switch (CurrentStep)
            {
                case AnalysisSetupStep.DatasetSelection:
                    break;
                case AnalysisSetupStep.OptionsSelection:
                    CurrentStep = AnalysisSetupStep.DatasetSelection;
                    break;
                case AnalysisSetupStep.BaselineSelection:
                    CurrentStep = AnalysisSetupStep.OptionsSelection;
                    break;
                case AnalysisSetupStep.Naming:
                    BaselineSelectionViewModel.UpdateDatasets();
                    CurrentStep = AnalysisSetupStep.BaselineSelection;
                    break;
                case AnalysisSetupStep.Started:
                    break;
                default:
                    break;
            }
        }

        public void Cancel()
        {
            if (AnalysisQuit != null)
            {
                AnalysisQuit(this, null);
            }
        }

        #endregion

        public event EventHandler AnalysisQuit;
        public event EventHandler AnalysisStart;

        private void SetupViewModels(AnalysisConfig configuration)
        {
            m_config = configuration;
            CurrentStep = AnalysisSetupStep.DatasetSelection;

            CancelCommand = new AnalysisCancelCommand(this);
            BackCommand = new AnalysisBackCommand(this);
            NextCommand = new AnalysisNextCommand(this);

            DatasetSelectionViewModel = new AnalysisDatasetSelectionViewModel(configuration.Analysis);
            BaselineSelectionViewModel = new AnalysisBaselineSelectionViewModel(configuration.Analysis);
            AnalysisNamingViewModel = new AnalysisNamingViewModel(configuration);
            AnalysisOptionsViewModel = new AnalysisOptionsViewModel(configuration.Analysis.Options);
        }

        #region ViewModels for Sub Controls

        /// <summary>
        /// Gets the dataset selection view model
        /// </summary>
        public AnalysisDatasetSelectionViewModel DatasetSelectionViewModel
        {
            get { return m_datasetViewModel; }
            private set
            {
                if (m_datasetViewModel != value)
                {
                    m_datasetViewModel = value;
                    OnPropertyChanged("DatasetSelectionViewModel");
                }
            }
        }

        /// <summary>
        /// Gets the baseline view model
        /// </summary>
        public AnalysisBaselineSelectionViewModel BaselineSelectionViewModel
        {
            get { return m_baselineViewModel; }
            private set
            {
                if (m_baselineViewModel != value)
                {
                    m_baselineViewModel = value;
                    OnPropertyChanged("BaselineSelectionViewModel");
                }
            }
        }

        /// <summary>
        /// Gets the analysis options view model
        /// </summary>
        public AnalysisOptionsViewModel AnalysisOptionsViewModel { get; private set; }

        /// <summary>
        /// Gets the view model for naming the analysis
        /// </summary>
        public AnalysisNamingViewModel AnalysisNamingViewModel
        {
            get { return m_namingViewModel; }
            set
            {
                if (m_namingViewModel != value)
                {
                    m_namingViewModel = value;
                    OnPropertyChanged("AnalysisNamingViewModel");
                }
            }
        }

        #endregion
    }
}