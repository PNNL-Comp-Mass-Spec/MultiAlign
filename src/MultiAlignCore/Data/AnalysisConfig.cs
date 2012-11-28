using System;
using System.Collections.Generic;
using System.Threading;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Reports;
using MultiAlignCore.IO;
using MultiAlignCore.Algorithms;
using System.Windows;
using System.ComponentModel;

namespace MultiAlignCore.Data
{

    /// <summary>
    /// Class that holds all information for setting up a job.
    /// </summary>
    public class AnalysisConfig : INotifyPropertyChanged
    {
        #region Constants
        private const int LC_DATA = 0;
        private const int IMS_DATA = 1;
        private const int PLOT_WIDTH = 800;
        private const int PLOT_HEIGHT = 800;
        private const int PLOT_WIDTH_HTML = 256;
        private const int PLOT_HEIGHT_HTML = 256;
        #endregion

        public AnalysisConfig()
        {            
            ParameterFile           = null;
            HtmlPathName            = "index.html";
            width                   = PLOT_WIDTH;
            height                  = PLOT_HEIGHT;
            logPath                 = null;
            InputPaths              = null;
            showHelp                = false;            
            HtmlPage                = new List<string>();
            AnalysisPath            = null;
            AnalysisName            = null;
            DatabaseName            = null;

            Report                  = new AnalysisHTMLReport();
            Report.ImageWidth       = PLOT_WIDTH_HTML;
            Report.ImageHeight      = PLOT_HEIGHT_HTML;

            ExporterNames           = new ClusterExporterComposite();
            ClusterExporters        = new List<IFeatureClusterWriter>();

            // Processing
            InitialStep                     = AnalysisStep.FindFeatures;
            ShouldLoadMTDB                  = true;
            ShouldCreateFeatureDatabaseOnly = false;
            ShouldUseFactors                = false;
            ShouldExportSICs                = false;
            ShouldExportMSMS                = false;
            ShouldUseExistingDatabase       = false;
        }
        #region Properties 
        public AnalysisStep InitialStep { get; set; }
        /// <summary>
        /// Gets or sets the analysis graph used by the processor to execute an analysis.
        /// </summary>
        public AnalysisGraph AnalysisGraph { get; set; }
        /// <summary>
        /// Gets or sets whether to load a mass tag database.
        /// </summary>
        public bool ShouldLoadMTDB { get; set; }
        /// <summary>
        /// Path to the file that specifies input data paths.
        /// </summary>
        public string InputPaths { get; set; }
        /// <summary>
        /// Path to parameter file.
        /// </summary>
        public string ParameterFile { get; set; }
        /// <summary>
        /// Name of the HTML webpage.
        /// </summary>
        public string HtmlPathName { get; set; }
        /// <summary>
        /// Holds a list of HTML tags for displaying plot results.
        /// </summary>
        public List<string> HtmlPage { get; set; }
        /// <summary>
        /// Event that is triggered when an analysis is completed.
        /// </summary>
        public ManualResetEvent triggerEvent { get; set; }
        /// <summary>
        /// Event that is triggered when an analysis is completed.
        /// </summary>
        public ManualResetEvent errorEvent { get; set; }
        /// <summary>
        /// Exception thrown by the analysis engine.
        /// </summary>
        public Exception errorException { get; set; }
        /// <summary>
        /// Path of log file.
        /// </summary>
        public string logPath { get; set; }
        /// <summary>
        /// ID of the job to use.
        /// </summary>
        public int JobID { get; set; }

        private void OnNotify(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private MultiAlignAnalysis m_analysis;
        public MultiAlignAnalysis Analysis
        {
            get { return m_analysis; }
            set 
            {
                if (value != m_analysis)
                {
                    m_analysis = value;
                    OnNotify("Analysis");
                }
            }
        }


        private string m_analysisPath;
        public string AnalysisPath
        {
            get { return m_analysisPath; }
            set
            {
                if (value != m_analysisPath)
                {
                    m_analysisPath = value;
                    OnNotify("AnalysisPath");
                }
            }
        }

        private string m_analysisName;
        public string AnalysisName
        {
            get { return m_analysisName; }
            set
            {
                if (value != m_analysisName)
                {
                    m_analysisName = value;
                    OnNotify("AnalysisName");
                }
            }
        }

        /// <summary>
        /// Path to save the plots.
        /// </summary>
        public string plotSavePath { get; set; }
        /// <summary>
        /// Height of the thumbnail plots.
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// Width of the thumbnail plots.
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// Flag to show help or not.
        /// </summary>
        public bool showHelp { get; set; }
        /// <summary>
        /// Options from command line argument
        /// </summary>
        public IDictionary<string, List<string>> options { get; set; }
        /// <summary>
        /// Path to database to create plots from.
        /// </summary>
        public string DatabaseName { get; set; }     
        /// <summary>
        /// Object that can generate an HTML report.
        /// </summary>
        public AnalysisHTMLReport Report { get; set; }
        /// <summary>
        /// Cluster Exporters for writing cluster data.
        /// </summary>
        public List<IFeatureClusterWriter> ClusterExporters { get; set; }
        /// <summary>
        /// Tracks the names of the files to export.
        /// </summary>
        public ClusterExporterComposite ExporterNames { get; set; }
        /// <summary>
        /// Determines if we need to use factors.
        /// </summary>
        public bool ShouldUseFactors { get; set; }
        /// <summary>
        /// Gets or sets whether to cluster spectra to find links between datasets.
        /// </summary>
        public bool ShouldClusterSpectra {get; set;}
        /// <summary>
        /// Extract the SIC's for each umc.
        /// </summary>
        public bool ShouldExportSICs { get; set; }        
        /// <summary>
        /// Flag indicating whether export the MS/MS spectra or not.
        /// </summary>
        public bool ShouldExportMSMS { get; set; }
        /// <summary>
        /// Gets or sets whetehr to use an existing database.
        /// </summary>
        public bool ShouldUseExistingDatabase { get; set; }
        /// <summary>
        /// Flag indicating whether to create the database only.
        /// </summary>
        public bool ShouldCreateFeatureDatabaseOnly { get; set; }
        /// <summary>
        /// Gets or sets the flag on whether to perform traceback.
        /// </summary>
        public bool ShouldTraceback { get; set; }       
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }    
}

