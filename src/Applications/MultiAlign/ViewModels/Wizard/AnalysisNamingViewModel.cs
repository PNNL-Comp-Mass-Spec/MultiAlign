using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels.Wizard
{
    /// <summary>
    /// Handles interaction logic for naming the analysis.
    /// </summary>
    public class AnalysisNamingViewModel : ViewModelBase
    {
        /// <summary>
        /// Reference to the analysis configuration.
        /// </summary>
        private readonly AnalysisConfig m_configuration;

        /// <summary>
        /// Creates an analysis naming view model
        /// </summary>
        /// <param name="configuration"></param>
        public AnalysisNamingViewModel(AnalysisConfig configuration)
        {
            m_configuration = configuration;

            var command = new BrowseFolderCommand(command_FolderSelected);
            BrowseCommand = command;
        }


        /// <summary>
        /// Gets or sets the path of the analysis
        /// </summary>
        public string Path
        {
            get { return m_configuration.AnalysisPath; }
            set
            {
                if (m_configuration.AnalysisPath != value)
                {
                    m_configuration.Analysis.MetaData.AnalysisPath = value;
                    m_configuration.AnalysisPath = value;
                    OnPropertyChanged("Path");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the analysis.
        /// </summary>
        public string Name
        {
            get { return m_configuration.AnalysisName; }
            set
            {
                if (m_configuration.AnalysisName != value)
                {
                    m_configuration.Analysis.MetaData.AnalysisName = value;
                    m_configuration.AnalysisName = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets the command for browsing to a folder for the analysis path.
        /// </summary>
        public ICommand BrowseCommand { get; private set; }

        /// <summary>
        /// Updates the path to the analysis.
        /// </summary>
        private void command_FolderSelected(string path)
        {
            Path = path;
        }
    }
}