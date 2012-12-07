using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MultiAlignCore.Data;
using Manassa.Data;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for PerformAnalysisWindow.xaml
    /// </summary>
    public partial class PerformAnalysisControl : UserControl
    {
        public event EventHandler AnalysisQuit;
        public event EventHandler AnalysisStart;

        public PerformAnalysisControl()
        {
            InitializeComponent();

            DataContext         = this;
            DatasetCount        = 0;
            ParameterFileName   = "No Parameter File Chosen";
            CurrentStep         = AnalysisSetupStep.DatasetSelection;                        
        }

        public AnalysisConfig AnalysisConfiguration
        {
            get { return (AnalysisConfig)GetValue(AnalysisConfigurationProperty); }
            set { SetValue(AnalysisConfigurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnalysisConfiguration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalysisConfigurationProperty =
            DependencyProperty.Register("AnalysisConfiguration", typeof(AnalysisConfig), typeof(PerformAnalysisControl));
                

        public AnalysisSetupStep CurrentStep
        {
            get { return (AnalysisSetupStep)GetValue(CurrentStepProperty); }
            set { SetValue(CurrentStepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentStep.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentStepProperty =
            DependencyProperty.Register("CurrentStep", typeof(AnalysisSetupStep), typeof(PerformAnalysisControl));


        public int DatasetCount
        {
            get { return (int)GetValue(DatasetCountProperty); }
            set { SetValue(DatasetCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DatasetCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatasetCountProperty =
            DependencyProperty.Register("DatasetCount", typeof(int), typeof(PerformAnalysisControl), new UIPropertyMetadata(0));

        public string ParameterFileName
        {
            get { return (string)GetValue(ParameterFileNameProperty); }
            set { SetValue(ParameterFileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParameterFileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParameterFileNameProperty =
            DependencyProperty.Register("ParameterFileName", typeof(string), typeof(PerformAnalysisControl));


        
        #region Event Handlers
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.AnalysisQuit != null)
            {
                AnalysisQuit(this, null);
            }
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LoadFromPreviousButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            MoveNext();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        #region Analysis Advancement         
        private void MoveBack()
        {
            switch (CurrentStep)
            {
                case AnalysisSetupStep.DatasetSelection:
                    break;
                case AnalysisSetupStep.BaselineSelection:
                    CurrentStep = AnalysisSetupStep.DatasetSelection;
                    break;
                case AnalysisSetupStep.OptionsSelection:
                    CurrentStep = AnalysisSetupStep.BaselineSelection;
                    break;
                case AnalysisSetupStep.Naming:
                    CurrentStep = AnalysisSetupStep.OptionsSelection;
                    break;
                case AnalysisSetupStep.Started:
                    break;
                default:
                    break;
            }
        }
        private void MoveNext()
        {
            // Validate the move
            string errorMessage = "";
            bool isValid = MultiAlignAnalysisValidator.IsStepValid(AnalysisConfiguration, CurrentStep, ref errorMessage);

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
                    CurrentStep = AnalysisSetupStep.BaselineSelection;
                    break;
                case AnalysisSetupStep.BaselineSelection:
                    CurrentStep = AnalysisSetupStep.OptionsSelection;
                    break;
                case AnalysisSetupStep.OptionsSelection:
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
        #endregion
    }

}
