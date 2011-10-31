using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using MultiAlignCore;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.MTDB;
using MultiAlignCore.IO.Parameters;
using MultiAlignCore.IO.Reports;
using MultiAlignCustomControls.Charting;
using MultiAlignCustomControls.Drawing;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;
using MultiAlignConsole.IO;

using MultiAlignCore.Data.Factors;

namespace MultiAlignConsole
{
    /// <summary>
    /// Analysis Steps
    /// </summary>
    internal enum AnalysisType
    {
        Full,
        Alignment,
        Clustering,
        PeakMatching,
        ExportDataOnly,
        InvalidParameters
    }
    

    /// <summary>
    /// Main application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="dwMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        #region Constants
        private const uint ENABLE_EXTENDED_FLAGS    = 0x0080;
		private const int LC_DATA                   = 0;
		private const int IMS_DATA                  = 1;
        /// <summary>
        /// Default path for plots.
        /// </summary>
        private const string THUMBNAIL_PATH         = "Plots";        
        private const int PLOT_WIDTH                = 800; 
        private const int PLOT_HEIGHT               = 800;
        private const int PLOT_WIDTH_HTML           = 256; 
        private const int PLOT_HEIGHT_HTML          = 256;            
        #endregion

        #region Members
        /// <summary>
        /// Path to the file that specifies input data paths.
        /// </summary>
        private static string m_inputPaths;
        /// <summary>
        /// Path to parameter file.
        /// </summary>
        private static string m_parameterFile;
        /// <summary>
        /// Name of the HTML webpage.
        /// </summary>
        private static string m_htmlPathName;
        /// <summary>
        /// Holds a list of HTML tags for displaying plot results.
        /// </summary>
        private static List<string> m_htmlPage;
        /// <summary>
        /// Event that is triggered when an analysis is completed.
        /// </summary>
        private static ManualResetEvent     m_triggerEvent;
        /// <summary>
        /// Event that is triggered when an analysis is completed.
        /// </summary>
        private static ManualResetEvent     m_errorEvent;
        /// <summary>
        /// Exception thrown by the analysis engine.
        /// </summary>
        private static Exception m_errorException;
        /// <summary>
        /// Path of log file.
        /// </summary>
        private static string               m_logPath;
        /// <summary>
        /// ID of the job to use.
        /// </summary>
        private static int                  m_jobID;
        /// <summary>
        /// Reference to the analysis that is running.
        /// </summary>
        private static MultiAlignAnalysis   m_analysis;
        /// <summary>
        /// Where the data and plots needs to go.
        /// </summary>
        private static string m_analysisPath;
        /// <summary>
        /// The name of the analysis.
        /// </summary>
        private static string m_analysisName;
        /// <summary>
        /// Path to save the plots.
        /// </summary>
        private static string m_plotSavePath;
        /// <summary>
        /// Height of the thumbnail plots.
        /// </summary>
        private static int m_height;
        /// <summary>
        /// Width of the thumbnail plots.
        /// </summary>
        private static int m_width;
        /// <summary>
        /// Flag to show help or not.
        /// </summary>
        private static bool m_showHelp;
        /// <summary>
        /// Options from command line argument
        /// </summary>
        private static IDictionary<string, List<string>> m_options;
        /// <summary>
        /// Creates plots on an existing database.
        /// </summary>
        private static bool m_createPlots;
        /// <summary>
        /// Path to database to create plots from.
        /// </summary>
        private static string   m_databaseName;
        /// <summary>
        /// Objects that access data from the databases.
        /// </summary>
        private static FeatureDataAccessProviders m_dataProviders;
        /// <summary>
        /// Flag that indicates whether a plot for the baseline features has been made.
        /// </summary>
        static bool m_createdBaselinePlots;
        /// <summary>
        /// Object that can generate an HTML report.
        /// </summary>
        private static AnalysisHTMLReport m_report;
        /// <summary>
        /// Cluster Exporters for writing cluster data.
        /// </summary>
        private static List<IFeatureClusterWriter> m_clusterExporters;
        /// <summary>
        /// Tracks the names of the files to export.
        /// </summary>
        private static ClusterExporterComposite m_exporterNames;
        #endregion
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        static Program()
        {            
            m_width                 = PLOT_WIDTH;
            m_height                = PLOT_HEIGHT;
            m_logPath               = null;
            m_inputPaths            = null;
            m_parameterFile         = null;
            m_htmlPathName          = "index.html";
            m_showHelp              = false;
            m_createdBaselinePlots  = false;
            m_htmlPage              = new List<string>();
            m_analysisPath          = null;
            m_analysisName          = null;
            m_databaseName          = null;
            m_createPlots           = false;
            m_report                = new AnalysisHTMLReport();
            m_report.ImageWidth     = PLOT_WIDTH_HTML;
            m_report.ImageHeight    = PLOT_HEIGHT_HTML;            
            m_exporterNames         = new ClusterExporterComposite();            
            m_clusterExporters      = new List<IFeatureClusterWriter>();
        }

        #region Plot Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="name"></param>
        static void SaveImage(Image image, string name)
        {
            if (image != null)
            {                
                string path = Path.Combine(m_plotSavePath, name);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);                
            }
            else
            {
                PrintMessage(string.Format("Could not create {0} plot.", name));
            }
        }
        /// <summary>
        /// Creates the final analysis plots
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="clusterCache"></param>
        static void CreateFinalAnalysisPlots()
        {
            UmcDAOHibernate cache               = new UmcDAOHibernate();
            UmcClusterDAOHibernate clusterCache = new UmcClusterDAOHibernate();

            CreateFinalAnalysisPlots(cache, clusterCache);
        }
        /// <summary>
        /// Creates the final analysis plots.
        /// </summary>
        static void CreateFinalAnalysisPlots(IUmcDAO cache, IUmcClusterDAO clusterCache)
        {
            PrintMessage("Creating Final Plots");
            m_report.PushTextHeader("Analysis Info ");
            m_report.PushStartTable();
            m_report.PushStartTableRow();

            // Create the heatmap
            List<clsUMC> umcs           = cache.FindAll();
            ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true,
                                                1, 100,
                                                "Charge State Histogram", "Charge State", "Count", m_width, m_height);
            options.DisplayLegend   = false;
            Image image             = RenderDatasetInfo.ChargeStateHistogram_Thumbnail(umcs, m_width, m_height, options);
            SaveImage(image, "ChargeStates.png");
            m_report.PushImageColumn(Path.Combine("Plots", "ChargeStates.png"));

            m_report.PushEndTableRow();
            m_report.PushEndTable();

            m_report.PushTextHeader("Cluster Data");
            m_report.PushStartTable();
            m_report.PushStartTableRow();

            List<clsCluster> clusters   = clusterCache.FindAll();            
            options.Title               = "Cluster Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            options.DisplayLegend       = false;
            image                       = RenderDatasetInfo.ClusterSizeHistogram_Thumbnail(clusters, m_width, m_height, options);
            SaveImage(image, "ClusterMemberSizes.png");
            m_report.PushImageColumn(Path.Combine("Plots", "ClusterMemberSizes.png"));
            
            options.Title               = "Cluster Dataset Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            image                       = RenderDatasetInfo.ClusterDatasetMemberSizeHistogram_Thumbnail(clusters, m_width, m_height, options);
            SaveImage(image, "ClusterDatasetMemberSizes.png");
            m_report.PushImageColumn(Path.Combine("Plots", "ClusterDatasetMemberSizes.png"));

            // Mass vs. Cluster score 
            options.Title               = "Clusters";
            options.YAxisLabel          = "Cluster Monoisotopic Mass";
            options.XAxisLabel          = "Cluster NET";
            image                       = RenderDatasetInfo.ClusterScatterPlot_Thumbnail(clusters, options);
            SaveImage(image, "ClusterScatterPlot.png");
            m_report.PushImageColumn(Path.Combine(THUMBNAIL_PATH, "ClusterScatterPlot.png"));
            m_report.PushEndTableRow();
            m_report.PushStartTableRow();
            m_report.PushEndTable();


            m_report.PushStartTable(true);
            m_report.PushStartTableRow();
            m_report.PushStartTableColumn();
            m_report.PushData("Dataset Members");
            m_report.PushEndTableColumn();

            m_report.PushStartTableColumn();
            m_report.PushData("Count");
            m_report.PushEndTableColumn();
            m_report.PushEndTableRow();

            float[] histogram = RenderDatasetInfo.GetClusterMemberSizes(clusters);
            if (histogram != null)
            {
                for (int i = 0; i < histogram.Length; i++)
                {
                    m_report.PushStartTableRow();
                    m_report.PushStartTableColumn();
                    m_report.PushData(i.ToString());
                    m_report.PushEndTableColumn();

                    m_report.PushStartTableColumn();
                    m_report.PushData(histogram[i].ToString());
                    m_report.PushEndTableColumn();
                    m_report.PushEndTableRow();
                }
            }
            m_report.PushEndTable();
        }
        /// <summary>
        /// Creates the plots post analysis.
        /// </summary>
        /// <param name="providers"></param>
        static void CreatePlotsOffline(FeatureDataAccessProviders providers)
        {
            PrintMessage("Connecting to existing database: " + m_databaseName);
            try
            {
                bool exists = File.Exists(m_databaseName);
                if (!exists)
                {
                    PrintMessage("The database you specified does not exist.");
                    return;
                }

                providers = SetupDataProviders(m_databaseName, false);
            }
            catch (System.IO.IOException ex)
            {
                PrintMessage("Could not connect to database: " + ex.Message);
                return;
            }

            PrintMessage("Creating Plot Thumbnail Path");
            // set the plot save path.
            string directoryName = Path.GetDirectoryName(m_databaseName);

            m_htmlPathName = Path.Combine(directoryName, m_htmlPathName);
            m_plotSavePath = Path.Combine(directoryName, THUMBNAIL_PATH);

            // Find out where it's located.
            if (!Directory.Exists(m_plotSavePath))
            {
                Directory.CreateDirectory(m_plotSavePath);
            }

            // We have to create a header for the HTML file.
            m_report.PushHeader();
            CreateFinalAnalysisPlots(providers.FeatureCache, providers.ClusterCache);
            m_report.PushEndHeader();
            CreatePlotReport();
            return;
        }
        /// <summary>
        /// Creates alignment plots.
        /// </summary>
        static void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            string name = e.AligneeDatasetInformation.DatasetName;
            PrintMessage("Features Aligned - " + name);
                
            // Hack so that the baseline plot is made first.
            if (!m_createdBaselinePlots)
            {                
                m_createdBaselinePlots  = true;
                if (m_analysis.MetaData.BaselineDataset != null)
                {
                    m_report.PushTextHeader("Baseline Dataset for " + m_analysis.MetaData.BaselineDataset.DatasetName);
                    m_report.PushStartTable();
                    m_report.PushStartTableRow();

                    DatasetInformation baselineInfo     = m_analysis.MetaData.BaselineDataset;
                    ChartDisplayOptions baselineOptions = new ChartDisplayOptions(false, true, true, true);
                    baselineOptions.MarginMin           = 1;
                    baselineOptions.MarginMax           = 100;
                    baselineOptions.Title               = "Feature Plot " + baselineInfo.DatasetName;
                    baselineOptions.XAxisLabel          = "Scan";
                    baselineOptions.YAxisLabel          = "Monoisotopic Mass";
                    baselineOptions.Width               = m_width;
                    baselineOptions.Height              = m_height;
                    baselineOptions.DisplayLegend       = true;
                    List<clsUMC> baselineUmcs           = m_dataProviders.FeatureCache.FindByDatasetId(Convert.ToInt32(baselineInfo.DatasetId));
                    Image baselineImage                 = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(baselineUmcs, baselineOptions);
                    string baselineLabelName            = Path.GetFileNameWithoutExtension(baselineInfo.DatasetName) + "_featurePlot.png";
                    string baselinePath                 = Path.Combine(m_plotSavePath, baselineLabelName);
                    baselineImage.Save(baselinePath, System.Drawing.Imaging.ImageFormat.Png);
                    m_report.PushImageColumn(Path.Combine("Plots", baselineLabelName));
                    m_report.PushEndTableRow();
                    m_report.PushEndTable();
                }
            }

            m_report.PushTextHeader("Alignment Plots for " + e.AligneeDatasetInformation.DatasetName);
            m_report.PushStartTable();
            m_report.PushStartTableRow();
            ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);

            options.MarginMin       = 1;
            options.MarginMax       = 100;
            options.Title           = "Feature Plot " + name;
            options.XAxisLabel      = "Scan";
            options.YAxisLabel      = "Monoisotopic Mass";
            options.Width           = m_width;
            options.Height          = m_height;
            options.DisplayLegend   = true;
            
            List<clsUMC> umcs       = m_dataProviders.FeatureCache.FindByDatasetId(Convert.ToInt32(e.AligneeDatasetInformation.DatasetId));
            Image image             = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(umcs, options);
            string labelName        = Path.GetFileNameWithoutExtension(name) + "_featurePlot.png";
            string path             = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            options.MarginMin       = 1;
            options.MarginMax       = 100;
            options.Title           = "Alignment Heatmap " + name;
            options.XAxisLabel      = "Baseline";
            options.YAxisLabel      = "Alignee";
            options.Width           = m_width;
            options.Height          = m_height;
            
            image                   = RenderDatasetInfo.AlignmentHeatmap_Thumbnail(e.AlignmentData, m_width, m_height);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_heatmap.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayLegend   = false;
            options.Title           = "NET Error Histogram " + name;
            options.XAxisLabel      = "NET Error (%)";
            options.YAxisLabel      = "Count";
            image                   = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.netErrorHistogram, options);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_netErrorHistogram.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayGridLines = true;
            options.DisplayLegend = false;
            options.Title = "Net vs. Scan Residuals" + name;
            image = RenderDatasetInfo.NETResiduals_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_netResiduals.png";
            path = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));
            
            options.Title           = "Mass Error Histogram " + name;
            options.XAxisLabel      = "Mass Error (PPM)";
            image                   = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.massErrorHistogram, options);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_massErrorHistogram.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            
            
            if (e.AlignmentData.driftErrorHistogram != null)
            {
                options.Title           = "Drift Time Error Histogram " + name;
                options.XAxisLabel      = "Drift Time Error (ms)";
                image                   = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.driftErrorHistogram, options);
                if (image != null)
                {
                    labelName   = Path.GetFileNameWithoutExtension(name) + "_driftTimeErrorHistogram.png";
                    path        = Path.Combine(m_plotSavePath, labelName);
                    image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    m_report.PushImageColumn(Path.Combine("Plots", labelName));
                }
            }

            options.DisplayLegend   = true;
            options.Title           = "Mass vs. Scan Residuals" + name;
            image                   = RenderDatasetInfo.MassVsScanResiduals_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_massScanResiduals.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayLegend   = true;
            options.Title           = "Mass vs. m/z Residuals" + name;
            image                   = RenderDatasetInfo.ClusterMassVsMZResidual_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_massMZResiduals.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));
            m_report.PushEndTableRow();

            if (e.DriftTimeAlignmentData != null)
            {
                
                options.DisplayLegend   = false;
                options.Title           = "Drift Time Plot";
                options.XAxisLabel      = "Baseline Drift Times (ms)";
                options.YAxisLabel      = "Alignee Drift Times (ms)";

                List<FeatureMatch<UMC, UMC>> matches = e.DriftTimeAlignmentData.Matches;
                int totalMatches                     = matches.Count;
                float[] x                            = new float[totalMatches];
                float[] yC                           = new float[totalMatches];
                float[] y                            = new float[totalMatches];

                int i = 0;
                foreach (FeatureMatch<UMC, UMC> match in matches)
                {
                    y[i]    = Convert.ToSingle(match.ObservedFeature.DriftTime);
                    yC[i]   = Convert.ToSingle(match.ObservedFeature.DriftTimeAligned);
                    x[i]    = Convert.ToSingle(match.TargetFeature.DriftTime);
                    i++;
                }
                labelName   = Path.GetFileNameWithoutExtension(name) + "_driftTimes.png";
                path        = Path.Combine(m_plotSavePath, labelName);
                image       = GenericPlotFactory.ScatterPlot_Thumbnail(x, y, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                m_report.PushImageColumn(Path.Combine("Plots", labelName));
                
                options.Title   = "Aligned Drift Time Plot";
                labelName       = Path.GetFileNameWithoutExtension(name) + "_driftTimesAligned.png";
                path            = Path.Combine(m_plotSavePath, labelName);
                image           = GenericPlotFactory.ScatterPlot_Thumbnail(x, yC, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                m_report.PushImageColumn(Path.Combine("Plots", labelName));
                
                options.Title   = "Drift Time Error Distributions";
                labelName       = Path.GetFileNameWithoutExtension(name) + "_driftTimesErrorHistogram.png";
                path            = Path.Combine(m_plotSavePath, labelName);
                image           = GenericPlotFactory.ResidualHistogram_Thumbnail(x, y, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                m_report.PushImageColumn(Path.Combine("Plots", labelName));

                options.Title   = "Aligned Drift Time Error Distributions";
                labelName       = Path.GetFileNameWithoutExtension(name) + "_driftTimesErrorHistogramAligned.png";
                path            = Path.Combine(m_plotSavePath, labelName);
                image           = GenericPlotFactory.ResidualHistogram_Thumbnail(x, yC, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                m_report.PushImageColumn(Path.Combine("Plots", labelName));
                m_report.PushEndTableRow();
            }

            m_report.PushEndTable();
        }
        #endregion

        #region Command Line Methods
        /// <summary>
        /// Validates the input options to make sure everything is set.
        /// </summary>
        /// <returns></returns>
        private static AnalysisType ValidateSetup()
        {
            AnalysisType analysisType   = AnalysisType.Full;
 
            bool isExporting            = (m_exporterNames.CrossTabPath != null);
            isExporting                 = (isExporting || m_exporterNames.ClusterScanPath != null);
            isExporting                 = (isExporting || m_exporterNames.ClusterMSMSPath != null);
            isExporting                 = (isExporting || m_exporterNames.CrossTabAbundance != null);

            if (m_inputPaths == null)
            {
                PrintMessage("No input file provided.");
                analysisType = AnalysisType.ExportDataOnly;
            }

            if (m_parameterFile == null)
            {
                PrintMessage("No parameter file specified.");
                analysisType = AnalysisType.ExportDataOnly;
            }

            if (m_analysisName == null)
            {
                PrintMessage("No analysis database name provided.");
                analysisType = AnalysisType.InvalidParameters;
            }

            if (m_analysisPath == null)
            {
                PrintMessage("No analysis path provided.");
                analysisType = AnalysisType.InvalidParameters;
            }

            if (!isExporting && analysisType == AnalysisType.ExportDataOnly)
            {
                PrintMessage("No export file names provided.");
                analysisType = AnalysisType.InvalidParameters;
            }

            return analysisType;
        }       
        /// <summary>
        /// Processes the command line arguments.
        /// </summary>
        /// <param name="args"></param>
        static void ProcessCommandLineArguments(string[] args)
        {
            int jobID   = -1;
            bool worked = false;
            m_options   = CommandLineParser.ProcessArgs(args, 0);
            foreach (string option in m_options.Keys)
            {                
                try
                {
                    List<string> values = m_options[option];
                    switch (option)
                    {
                        case "-job":
                            worked = int.TryParse(values[0], out jobID);
                            if (worked)
                            {
                                m_jobID = jobID;
                            }
                            else
                            {
                                PrintMessage(string.Format("The Job ID {0} specified could not be understood.",
                                                            values[0]),
                                                            true);
                            }
                            break;
                        case "-path":
                            m_analysisPath  = values[0];
                            break;
                        case "-files":
                            m_inputPaths    = values[0];
                            break;
                        case "-params":
                            m_parameterFile = values[0];
                            break;
                        case "-name":
                            m_analysisName  = values[0];                            
                            break;
                        case "-log":
                            m_logPath       = values[0];
                            break;
                        case "-html":
                            m_htmlPathName  = values[0];
                            break;
                        case "-h":
                            m_showHelp      = true;
                            break;
                        case "-help":
                            m_showHelp      = true;
                            break;
                        case "-extractmsms":
                            m_exporterNames.ClusterMSMSPath = values[0];
                            break;
                        case "-exportclusters":
                            m_exporterNames.ClusterScanPath = values[0];                            
                            break;
                        case "-exportcrosstab":
                            m_exporterNames.CrossTabPath      = values[0];
                            break;
                        case "-exportabundances":
                            m_exporterNames.CrossTabAbundance = values[0];
                            break;
                        case "-plots":
                            m_createPlots   = true;
                            if (values.Count > 0)
                            {
                                m_databaseName = values[0];
                            }
                            else
                            {
                                m_databaseName = null;
                            }
                            break;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    PrintMessage(string.Format("You did not provide enough information for the option {0}", option));
                    return;
                }                
            }
        }
        #endregion

        #region Processor Events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_AnalysisError(object sender, AnalysisErrorEventArgs e)
        {
            PrintMessage(string.Format("There was an error while performing the analysis.  {0} : {1}", e.ErrorMessage, e.Exception.Message));
            if (e.Exception.StackTrace != null)
            {
                PrintMessage(string.Format("\n{0}", e.Exception.StackTrace));
            }
            m_errorException = e.Exception;
            m_errorEvent.Set();            
        }
        #endregion

        #region Printing
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        static void PrintMessage(string message)
        {
            PrintMessage(message, true);
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        static void PrintSpacer()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();            
            for(int i = 0; i < 80; i++)
            {
                builder.Append("-");
            }
            PrintMessage(builder.ToString(), false);
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        static void PrintMessage(string message, bool useMemory)
        {
            string newMessage = message;
            if (useMemory)
            {
                newMessage = DateTime.Now.ToString() + " - " + ApplicationUtility.GetMemory().ToString() + " MB - " + newMessage;
            }
            if (m_logPath != null)
            {
                File.AppendAllText(m_logPath, newMessage + Environment.NewLine);
            }
            Console.WriteLine(newMessage);
        }
        /// <summary>
        /// Prints the help message.
        /// </summary>
        static void PrintHelp()
        {
            PrintMessage(" ", false);
            PrintMessage("usage: MultiAlignConsole [options]", false);
            PrintMessage(" ", false);
            PrintMessage(" Input File Format Notes: ", false);
            PrintMessage("    MS Feature Input Files -  MultiAlign needs a separate file to determine what deisotoped MS features or LCMS Features", false);
            PrintMessage("                              to load.", false);
            PrintMessage("                              You can use these file types:", false);
            PrintMessage("                                  *LCMSFeatures.txt", false);
            PrintMessage("                                  *_isos.csv", false);
            PrintMessage("                      Single Dataset - (You must specify a database to align to and peak match with.)", false);
            PrintMessage("                              [Files]", false);
            PrintMessage("                              pathOfFile_isos.csv", false);
            PrintMessage("                      Multiple Dataset - (If you don't specify a baseline database, you must specify a database to align to.)", false);
            PrintMessage("                              [Files]", false);
            PrintMessage("                              pathOfFile1_isos.csv", false);
            PrintMessage("                              pathOfFile2_isos.csv", false);
            PrintMessage("                      Specifying a baseline - This is done by placing an asterisk after one of the dataset names.", false);
            PrintMessage("                      (NOTE: If you do not specify a baseline, a database must be used for alignment)", false);
            PrintMessage("                              [Files]", false);
            PrintMessage("                              pathOfFile1_isos.csv", false);
            PrintMessage("                              pathOfFile2_isos.csv*", false);
            PrintMessage("                      Specifying a baseline - This is done by placing an asterisk after one of the dataset names.", false);
            PrintMessage("                              [Files]", false);
            PrintMessage("                              pathOfFile1_isos.csv", false);
            PrintMessage("                              pathOfFile2_isos.csv*", false);
            PrintMessage("    MS/MS Data Linking - linking MS Features to MS/MS spectra can be done by specifying the RAW dataset files.", false);
            PrintMessage("                         This currently only works for 32-bit (x86) versions and with Thermo Finnigan data files.", false);
            PrintMessage("                         To use this feature, you must have matching dataset (MS Feature Input Files) file ", false);
            PrintMessage("                         names (extension excluded).", false);
            PrintMessage("                              [Raw]", false);
            PrintMessage("                              pathOfFile1.Raw", false);
            PrintMessage("    Peak Matching -   To perform peak matching to an Accurate Mass and Time Tag Database (AMT DB) you need to specify", false);
            PrintMessage("                      the name of the database in the input file. ", false);
            PrintMessage("                      To do this with a local Microsoft Access database use: ", false);
            PrintMessage("                              [Database]", false);
            PrintMessage("                              accessPath = pathOfDatabase.mdb", false);
            PrintMessage("                      To use one of the Mass Tag System's (MTS) databases use: ", false);
            PrintMessage("                              [Database]", false);
            PrintMessage("                              database = nameOfDatabase", false);
            PrintMessage("                              server   = serverDatabaseLivesOn", false);
            PrintMessage("[Options]", false);
            PrintMessage(" ", false);
            PrintMessage("   -files  inputFile.txt ", false);
            PrintMessage("          ASCII Text file with input file names.", false);
            PrintMessage("          In list of files use asterik to indicate the baseline choice, e.g. 'dataset *'", false);            
            PrintMessage("   -name analysisName  ", false);
            PrintMessage("          Name to give analysis.", false);
            PrintMessage("   -log logPath.txt", false);
            PrintMessage("          Path to provide for log files.", false);
            PrintMessage("   -h", false);
            PrintMessage("          Prints this help message.", false);
            PrintMessage("   -help", false);
            PrintMessage("          Prints this help message.", false);
            PrintMessage("   -html htmlPathName.html", false);
            PrintMessage("          Name to give output HTML plot file.", false);
            PrintMessage("   -params parameterFile.xml  ", false);
            PrintMessage("          XML file defining MultiAlign parameters.", false);
            PrintMessage("   -path  AnalysisPath      ", false);
            PrintMessage("          File directory of where to put MultiAlign output.  Can be relative or absolute.", false);
            PrintMessage("   -centroid      ", false);
            PrintMessage("          To use centroid distance as clustering algorithm.", false);
            PrintMessage("   -factors factorFilePath", false);
            PrintMessage("          Path to factor definition file to be loaded.", false);
            PrintMessage("   -exportClusters clusterFileName     ", false);
            PrintMessage("          Exports clusters and their LC-MS features to the file name specified.  This file will be sent to the analysis path folder you specified.", false);
            PrintMessage("   -exportCrossTab  crossTabFileName     ", false);
            PrintMessage("          Exports clusters and their LC-MS features in cross tab fashion.  Each row is a cluster.  No mass tags are exported.  This file will be sent to the analysis path folder you specified.", false);
            PrintMessage("   -exportAbundances  crossTabFileName     ", false);
            PrintMessage("          Exports cluster ids and the abundances of their LC-MS features in cross tab fashion.  Each row is a cluster.  No mass tags are exported.  This file will be sent to the analysis path folder you specified.", false);
            PrintMessage("   -extractMSMS      ", false);
            PrintMessage("          Exctracts information about clusters that have tandem mass spectra.  Does not execute any further analysis.", false);
            PrintMessage("   -plots   [databaseName]  ", false);
            PrintMessage("          Creates plots for final analysis.  If [databaseName] specified when not running analysis, this will create plots post-analysis.", false);            
        }
        /// <summary>
        /// Prints the version of MA to the log file.
        /// </summary>
        public static void PrintVersion()
        {
            PrintMessage("[Version Info]");
            string assemblyData = ApplicationUtility.GetAssemblyData();
            PrintMessage("\t" + assemblyData);

            AppDomain MyDomain = AppDomain.CurrentDomain;
            Assembly[] AssembliesLoaded = MyDomain.GetAssemblies();

            PrintMessage("\tLoaded Assemblies");
            foreach (Assembly subAssembly in AssembliesLoaded)
            {
                AssemblyName subName = subAssembly.GetName();                
                PrintMessage(string.Format("\t\t{0} - version {1}",
                                                                subName,
                                                                subName.Version));                
            }

            PrintMessage("[System Information]");
            string systemData = ApplicationUtility.GetSystemData();
            PrintMessage("\t" + systemData);            
            PrintMessage("[LogStart]");
        }
        /// <summary>
        /// Writes the parameters to the log file and database.
        /// </summary>
        /// <param name="analysis"></param>
        static void PrintParameters(MultiAlignAnalysis analysis)
        {
            PrintMessage("Parameters Loaded");
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("MS Linker Options", analysis.Options.MSLinkerOptions);
            options.Add("UMC Finding Options", analysis.Options.UMCFindingOptions);
            options.Add("Feature Filtering Options", analysis.Options.FeatureFilterOptions);
            options.Add("Mass Tag Database Options", analysis.Options.MassTagDatabaseOptions);
            options.Add("Alignment Options", analysis.Options.DefaultAlignmentOptions);
            options.Add("Drift Time Alignment Options", analysis.Options.DriftTimeAlignmentOptions);
            options.Add("Cluster Options", analysis.Options.ClusterOptions);
            options.Add("Peak Matching Options", analysis.Options.PeakMatchingOptions);
            options.Add("STAC Options", analysis.Options.STACAdapterOptions);

            List<ParameterHibernateMapping> allmappings = new List<MultiAlignCore.IO.Parameters.ParameterHibernateMapping>();
            foreach (string key in options.Keys)
            {
                object o = options[key];
                PrintMessage(key, true);
                List<string> parameters = ParameterUtility.ConvertParameterObjectToStrings(o);
                foreach (string parameter in parameters)
                {
                    PrintMessage("\t" + parameter, true);
                }

                List<ParameterHibernateMapping> mappings = ParameterUtility.ExtractParameterMapObjects(o, key);
                allmappings.AddRange(mappings);
            }


            ParameterHibernateMapping assemblyMap   = new ParameterHibernateMapping();
            string assemblyData                     = ApplicationUtility.GetAssemblyData();
            assemblyMap.OptionGroup                 = "Assembly Info";
            assemblyMap.Parameter                   = "Version";
            assemblyMap.Value                       = assemblyData;
            allmappings.Add(assemblyMap);
          
            ParameterHibernateMapping systemMap = new ParameterHibernateMapping();
            string systemData                   = ApplicationUtility.GetSystemData();
            systemMap.OptionGroup               = "Assembly Info";
            systemMap.Parameter                 = "System Info";
            systemMap.Value                     = systemData;
            allmappings.Add(systemMap);

            PrintMessage("Writing parameters to the analysis database.");
            GenericDAOHibernate<ParameterHibernateMapping> parameterCache = new GenericDAOHibernate<MultiAlignCore.IO.Parameters.ParameterHibernateMapping>();
            parameterCache.AddAll(allmappings);
        }
        /// <summary>
        /// Creates the HTML output file.
        /// </summary>
        static void CreatePlotReport()
        {
            PrintMessage("Creating Report.");
            string htmlPath = m_htmlPathName;
            if (m_analysisPath != null)
            {
                htmlPath = Path.Combine(m_analysisPath, m_htmlPathName);
            }
            m_report.AnalysisName = m_analysisName;
            m_report.CreateReport(htmlPath);
        } 
        #endregion 

        #region Event Handlers
        /// <summary>
        /// Displays the status message when the analysis completes.
        /// </summary>
        /// <param name="status"></param>
        static void DisplayStatus(int level, string status)
        {
            PrintMessage(status);            
        }
        /// <summary>
        /// Displays the title.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="title"></param>
        static void DisplayTitle(int data, string title)
        {
            PrintMessage(title);
        }
        /// <summary>
        /// Terminates the application when the analysis is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_AnalysisComplete(object sender, AnalysisCompleteEventArgs e)
        {            
            if (m_analysis.MassTagDatabase != null)
            {
                m_report.PushLargeText("Mass Tag Database Stats");                
                m_report.PushStartTable();
                m_report.PushStartTableRow();
                string databaseTags = string.Format("Number Of Mass Tags Loaded {0}", m_analysis.MassTagDatabase.MassTags.Count);
                m_report.PushData(databaseTags);
                m_report.PushEndTableRow();
                m_report.PushEndTable();
            }

            m_report.PushEndHeader();
            m_triggerEvent.Set();
        }
        /// <summary>
        /// Logs when features are aligned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesAligned(object sender, FeaturesAlignedEventArgs e)
        {
            CreateAlignmentPlots(e);
        }
        /// <summary>
        /// Logs when features are clustered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesClustered(object sender, FeaturesClusteredEventArgs e)
        {
            List<DatasetInformation> information = m_dataProviders.DatasetCache.FindAll();
            foreach (IFeatureClusterWriter writer in m_clusterExporters)
            {
                writer.WriteClusters(e.Clusters, information);
            }
            PrintMessage("Features Clustered.");            
        }
        /// <summary>
        /// Logs when features are peak matched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesPeakMatched(object sender, FeaturesPeakMatchedEventArgs e)
        {
            PrintMessage("Features are peak matched.");
        }
        /// <summary>
        /// Logs when features are loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesLoaded(object sender, FeaturesLoadedEventArgs e)
        {
            PrintMessage(string.Format("Loaded {0} features from {1}", e.Features.Count, e.DatasetInformation.DatasetName));
        }
        /// <summary>
        /// Logs status messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_Status(object sender, AnalysisStatusEventArgs e)
        {
            PrintMessage(e.StatusMessage);
        }        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesExtracted(object sender, MultiAlignCore.Algorithms.MSLinker.FeaturesExtractedEventArgs e)
        {
            string extractionPath = Path.Combine(m_analysisPath, m_exporterNames.ClusterMSMSPath);

            FeatureExtractionTableWriter writer = new MultiAlignCore.IO.Features.FeatureExtractionTableWriter();
            writer.WriteData(extractionPath, e);
        }
        #endregion
               
        #region Data Provider Setup
        /// <summary>
        /// Sets up the NHibernate caches for storing and retrieving data.
        /// </summary>
        /// <param name="analysisPath"></param>
        /// <returns></returns>
        private static FeatureDataAccessProviders SetupDataProviders(string path, bool createNew)
        {
            try
            {                
                return DataAccessFactory.CreateDataAccessProviders(path, createNew);
            }
            catch (System.IO.IOException ex)
            {
                PrintMessage("Could not access the database.  Is it opened somewhere else?");
                throw ex;
            }
        }
        /// <summary>
        /// Creates data providers to the database of the analysis name and path provided.
        /// </summary>
        /// <returns></returns>
        private static FeatureDataAccessProviders SetupDataProviders(bool createNewDatabase)
        {
            FeatureDataAccessProviders providers = null;
            PrintMessage("Setting up data providers for caching and storage.");
            try
            {
                string path = AnalysisPathUtils.BuildAnalysisName(m_analysisPath, m_analysisName);
                providers   = SetupDataProviders(path, createNewDatabase);                   
            }
            catch (System.IO.IOException ex)
            {
                PrintMessage(ex.Message);
                PrintMessage(ex.StackTrace);
            }
            return providers;
        }
        /// <summary>
        /// Cleans up the old database providers.
        /// </summary>
        private static void CleanupDataProviders()
        {            
            NHibernateUtil.Dispose();
        }
        #endregion
        
        #region Construction
        private static void ConstructPlotPath()
        {
            PrintMessage("Creating Plot Thumbnail Path");
            // set the plot save path.
            m_plotSavePath = Path.Combine(m_analysisPath, THUMBNAIL_PATH);

            // Find out where it's located.
            if (!Directory.Exists(m_plotSavePath))
            {
                Directory.CreateDirectory(m_plotSavePath);
            }
        }        
        /// <summary>
        /// Creates the analysis processor and synchronizs the events.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        private static MultiAlignAnalysisProcessor ConstructAnalysisProcessor(AlgorithmBuilder builder, FeatureDataAccessProviders providers)
        {
            MultiAlignAnalysisProcessor processor = new MultiAlignAnalysisProcessor();
            processor.AnalysisError         += new EventHandler<AnalysisErrorEventArgs>(processor_AnalysisError);
            processor.FeaturesAligned       += new EventHandler<FeaturesAlignedEventArgs>(processor_FeaturesAligned);
            processor.FeaturesLoaded        += new EventHandler<FeaturesLoadedEventArgs>(processor_FeaturesLoaded);
            processor.FeaturesClustered     += new EventHandler<FeaturesClusteredEventArgs>(processor_FeaturesClustered);
            processor.FeaturesPeakMatched   += new EventHandler<FeaturesPeakMatchedEventArgs>(processor_FeaturesPeakMatched);
            processor.AnalysisComplete      += new EventHandler<AnalysisCompleteEventArgs>(processor_AnalysisComplete);
            processor.Status                += new EventHandler<AnalysisStatusEventArgs>(processor_Status);
            processor.FeaturesExtracted     += new EventHandler<MultiAlignCore.Algorithms.MSLinker.FeaturesExtractedEventArgs>(processor_FeaturesExtracted);
            m_dataProviders                 = providers;

            processor.AlgorithmProvders     = builder.GetAlgorithmProvider(m_analysis.Options);

            return processor;
        }        
        /// <summary>
        /// Sets up the analysis essentials including analysis path, log path, and prints the version and parameter information
        /// to the log.
        /// </summary>
        private static void SetupAnalysisEssentials()
        {
            // Create the analysis path and log file paths.
            ConstructAnalysisPath();
            string dateSuffix = ConstructLogPath();

            // Log the version information to the log.
            PrintVersion();
            PrintSpacer();

            // Build Plot Path
            m_plotSavePath = AnalysisPathUtils.BuildPlotPath(m_analysisPath);

            // Build analysis name                  
            bool containsExtensionDB3 = m_analysisName.EndsWith(".db3");
            if (!containsExtensionDB3)
            {
                m_analysisName += ".db3";
            }

            // create application and analysis.
            PrintMessage("Starting MultiAlign Console Application.");
            PrintMessage("Creating analysis: ");
            PrintMessage("\t" + m_analysisName);
            PrintMessage("Storing analysis: ");
            PrintMessage("\t" + Path.GetFullPath(m_analysisPath));
            if (m_inputPaths != null && m_inputPaths.Length > 0)
            {
                PrintMessage("Using Files:  ");
                PrintMessage("\tFull Path: " + Path.GetFullPath(m_inputPaths));
                PrintMessage("\tFile Name: " + Path.GetFileName(m_inputPaths));
            }
            else
            {
                PrintMessage("No input files specified.");
            }

            if (m_parameterFile != null)
            {
                PrintMessage("Using Parameters: ");
                PrintMessage("\tFull Path: " + Path.GetFullPath(m_parameterFile));
                PrintMessage("\tFile Name: " + Path.GetFileName(m_parameterFile));
            }
            else
            {
                PrintMessage("No parameter file specified.");
            }
        }
        private static void ReadParameterFile()
        {
            // Setup the parameters.
            PrintMessage("Loading parameters.");
            // Make sure we have parameters!
            if (!File.Exists(m_parameterFile))
            {
                PrintMessage("The parameter file does not exist.");
                //return 1;
            }
            XMLParamterFileReader reader = new XMLParamterFileReader();
            reader.ReadParameterFile(m_parameterFile, ref m_analysis);
        }
        private static string ConstructLogPath()
        {
            // Create the LOG FILE.
            string dateSuffix = AnalysisPathUtils.BuildDateSuffix();
            if (m_logPath == null)
            {
                m_logPath = AnalysisPathUtils.BuildLogPath(m_analysisPath,
                                                            m_analysisName,
                                                            dateSuffix);
            }
            else
            {
                m_logPath = Path.Combine(m_analysisPath,
                                         m_logPath,
                                         dateSuffix);
            }
            return dateSuffix;
        }
        private static void ConstructAnalysisPath()
        {
            //Create the analysis directory.
            if (!Directory.Exists(m_analysisPath))
            {
                PrintMessage("Creating analysis path " + m_analysisPath);
                Directory.CreateDirectory(m_analysisPath);
            }
            else
            {
                PrintMessage("Analysis path " + m_analysisPath + " already exists.");
            }
        }
        private static MultiAlignAnalysis ConstructAnalysisObject(InputAnalysisInfo analysisSetupInformation)
        {
            PrintMessage("Creating Analysis Objects.");
            MultiAlignAnalysis analysis             = new MultiAlignAnalysis();
            analysis.MetaData.AnalysisPath          = m_analysisPath;
            analysis.MetaData.AnalysisName          = m_analysisName;
            analysis.Options.UseMassTagDBAsBaseline = true;

            analysis.MetaData.ParameterFile         = m_parameterFile;
            analysis.MetaData.InputFileDefinition   = m_inputPaths;
            analysis.MetaData.AnalysisSetupInfo     = analysisSetupInformation;
            return analysis;
        }
        private static bool ReadInputDefinitionFile(out InputAnalysisInfo analysisSetupInformation, out bool useMTDB)
        {
            // Read the input datasets.
            if (!File.Exists(m_inputPaths))
            {
                PrintMessage(string.Format("The input file {0} does not exist.", m_inputPaths));
                //return 1;
            }
            else
            {
                PrintMessage("Copying input file to output directory.");
                try
                {
                    string dateSuffix   = AnalysisPathUtils.BuildDateSuffix();
                    string newPath      = Path.GetFileNameWithoutExtension(m_inputPaths);
                    newPath             = newPath + "_" + dateSuffix + ".txt";
                    File.Copy(m_inputPaths, Path.Combine(m_analysisPath, newPath));
                }
                catch (Exception ex)
                {
                    PrintMessage("Could not copy the input file to the output directory.  " + ex.Message);
                }
            }

            PrintMessage("Parsing Input Filenames and Databases.");
            useMTDB = false;
            analysisSetupInformation = null;
            try
            {
                analysisSetupInformation = MultiAlignFileInputReader.ReadInputFile(m_inputPaths);
            }
            catch (Exception ex)
            {
                PrintMessage("The input file had some bad lines in it.  " + ex.Message);
                return false;
            }
            PrintMessage("Found " + analysisSetupInformation.Files.Count.ToString() + " files.");

            // Validate the mass tag database settings.
            try
            {
                useMTDB = analysisSetupInformation.Database.ValidateDatabaseType();
            }
            catch (AnalysisMTDBSetupException ex)
            {
                PrintMessage("There was a problem with the mass tag database specification.  " + ex.Message);
                return false;
            }
            return true;
        }
        private static void ExportParameterFile()
        {
            // Output the settings to INI for viewing.
            string outParamName = Path.GetFileNameWithoutExtension(m_parameterFile);
            string outParamPath = Path.Combine(m_analysisPath, outParamName);
            XMLParameterFileWriter xmlWriter = new XMLParameterFileWriter();
            xmlWriter.WriteParameterFile(outParamPath + ".xml", m_analysis);
        }
        /// <summary>
        /// Constructs the baseline databases.
        /// </summary>
        /// <param name="analysisSetupInformation"></param>
        /// <param name="useMTDB"></param>
        private static bool ConstructBaselines(InputAnalysisInfo analysisSetupInformation, AnalysisMetaData analysisMetaData,  bool useMTDB)
        {
            PrintMessage("Confirming baseline selections.");
            if (useMTDB)
            {
                switch (analysisSetupInformation.Database.DatabaseFormat)
                {
                    case MassTagDatabaseFormat.Access:
                        PrintMessage("Using local Mass Tag Database at location: ");
                        PrintMessage(string.Format("\tFull Path: {0}", analysisSetupInformation.Database.LocalPath));
                        PrintMessage(string.Format("\tDatabase Name: {0}", Path.GetFileName(analysisSetupInformation.Database.LocalPath)));

                        m_analysis.Options.MassTagDatabaseOptions.mstr_databaseFilePath = analysisSetupInformation.Database.LocalPath;
                        m_analysis.Options.MassTagDatabaseOptions.mstrServer = analysisSetupInformation.Database.DatabaseServer;
                        m_analysis.Options.MassTagDatabaseOptions.menm_databaseType = MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS;
                        break;

                    case MassTagDatabaseFormat.SQL:
                        PrintMessage("Using Mass Tag Database:");
                        PrintMessage(string.Format("\tServer:        {0}", analysisSetupInformation.Database.DatabaseServer));
                        PrintMessage(string.Format("\tDatabase Name: {0}", analysisSetupInformation.Database.DatabaseName));
                        m_analysis.Options.MassTagDatabaseOptions.mstrDatabase = analysisSetupInformation.Database.DatabaseName;
                        m_analysis.Options.MassTagDatabaseOptions.mstrServer = analysisSetupInformation.Database.DatabaseServer;
                        m_analysis.Options.MassTagDatabaseOptions.menm_databaseType = MultiAlignEngine.MassTags.MassTagDatabaseType.SQL;
                        break;
                }

                // Validate the baseline
                if (analysisSetupInformation.BaselineFile == null)
                {
                    m_analysis.Options.UseMassTagDBAsBaseline = true;
                    PrintMessage(string.Format("Using mass tag database {0} as the alignment baseline.", analysisSetupInformation.Database.DatabaseName));
                }
                else
                {
                    m_analysis.Options.UseMassTagDBAsBaseline   = false;                                     
                    m_analysis.MetaData.BaselineDataset         = null;
                    foreach (DatasetInformation info in analysisMetaData.Datasets)
                    {
                        if (info.Path == analysisSetupInformation.BaselineFile.Path)
                        {                            
                            m_analysis.MetaData.BaselineDataset = info;
                        }
                    }                    
                    PrintMessage(string.Format("Using dataset {0} as the alignment baseline.", m_analysis.MetaData.BaselineDataset));
                }
            }
            else
            {
                m_analysis.Options.MassTagDatabaseOptions.menm_databaseType = MultiAlignEngine.MassTags.MassTagDatabaseType.None;
                m_analysis.Options.UseMassTagDBAsBaseline = false;
                // Validate the baseline
                if (analysisSetupInformation.BaselineFile == null)
                {
                    PrintMessage("No baseline dataset or database was selected.");
                    return false;
                }
                else
                {                    
                    m_analysis.MetaData.BaselineDataset         = null;
                    foreach (DatasetInformation info in analysisMetaData.Datasets)
                    {
                        if (info.Path == analysisSetupInformation.BaselineFile.Path)
                        {                            
                            m_analysis.MetaData.BaselineDataset = info;
                        }
                    }                    
                    PrintMessage(string.Format("Using dataset {0} as the alignment baseline.", m_analysis.MetaData.BaselineDataset));
                }
            }
            return true;
        }
        /// <summary>
        /// Loads factors from file or other.
        /// </summary>
        /// <param name="analysisSetupInformation"></param>
        /// <param name="datasets"></param>
        private static void ConstructFactorInformation(InputAnalysisInfo analysisSetupInformation, List<DatasetInformation> datasets)
        {
            MultiAlignCore.IO.Factors.MAGEFactorAdapter mage = new MultiAlignCore.IO.Factors.MAGEFactorAdapter();

            if (analysisSetupInformation.FactorFile == null)
            {
                mage.LoadFactorsFromDMS(datasets, m_analysis.DataProviders);
            }
            else
            {
                mage.LoadFactorsFromFile(analysisSetupInformation.FactorFile, datasets, m_analysis.DataProviders);
            }
        }
        /// <summary>
        /// Constructs dataset infromation from the input analysis information.
        /// </summary>
        /// <param name="analysisSetupInformation"></param>
        /// <param name="analysis"></param>
        private static void ConstructDatasetInformation(InputAnalysisInfo analysisSetupInformation, MultiAlignAnalysis analysis)
        {
            // Create dataset information.
            int i = 0;
            PrintMessage("Creating dataset and other input information.");
            foreach (InputFile file in analysisSetupInformation.Files)
            {
                switch (file.FileType)
                {
                    case InputFileType.Features:
                        DatasetInformation datasetInfo  = new DatasetInformation();
                        datasetInfo.Path                = file.Path;
                        datasetInfo.DatasetId           = i++;
                        datasetInfo.DatasetName         = Path.GetFileName(file.Path);                        
                        datasetInfo.DatasetName         = datasetInfo.DatasetName.Replace("_isos.csv", "");
                        datasetInfo.DatasetName         = datasetInfo.DatasetName.Replace(".pek", "");
                        datasetInfo.DatasetName         = datasetInfo.DatasetName.Replace("_lcmsfeatures.txt", "");                       
                        datasetInfo.JobId               = "";
                        datasetInfo.mstrResultsFolder   = Path.GetDirectoryName(file.Path);
                        datasetInfo.ParameterFileName   = "";
                        datasetInfo.Selected            = true;
                        PrintMessage("\tDataset Information:   " + file.Path);
                        analysis.MetaData.Datasets.Add(datasetInfo);
                        break;
                    case InputFileType.Scans:
                        analysis.MetaData.OtherFiles.Add(file);
                        PrintMessage("\tScan File Information: " + file.Path);
                        break;
                    case InputFileType.Raw:
                        analysis.MetaData.OtherFiles.Add(file);
                        PrintMessage("\tRaw Data Information:  " + file.Path);
                        break;
                }
            }

            m_analysis.DataProviders.DatasetCache.AddAll(analysis.MetaData.Datasets);
        }
        /// <summary>
        /// Determine what exporting features need to be had.
        /// </summary>
        private static void ConstructExporting()
        {
            if (m_exporterNames.ClusterScanPath != null)
            {
                m_clusterExporters.Add(new UMCClusterScanWriter(Path.Combine(m_analysisPath, m_exporterNames.ClusterScanPath)));
            }

            if (m_exporterNames.CrossTabPath != null)
            {
                UMCClusterCrossTabWriter writer = new UMCClusterCrossTabWriter(Path.Combine(m_analysisPath, m_exporterNames.CrossTabPath));
                writer.Consolidator = new MultiAlignCore.Algorithms.Features.UMCAbundanceConsolidator();
                m_clusterExporters.Add(writer);
            }

            if (m_exporterNames.CrossTabAbundance != null)
            {
                UMCClusterAbundanceCrossTabWriter writer = new UMCClusterAbundanceCrossTabWriter(Path.Combine(m_analysisPath,
                                                                m_exporterNames.CrossTabAbundance));
                writer.Consolidator = new MultiAlignCore.Algorithms.Features.UMCAbundanceConsolidator();
                m_clusterExporters.Add(writer);
            }
        }
        /// <summary>
        /// Create the clustering algorithms.
        /// </summary>
        /// <param name="builder"></param>
        private static void ConstructClustering(AlgorithmBuilder builder)
        {
            // Setup algorithm providers.
            if (m_options.ContainsKey("-centroid"))
            {
                PrintMessage("Building centroid clusterer");
                builder.BuildClusterer(ClusteringAlgorithmType.Centroid);
            }
            else if (m_options.ContainsKey("-singlelinkage"))
            {

                PrintMessage("Building single linkage clusterer");
                builder.BuildClusterer(ClusteringAlgorithmType.SingleLinkage);
            }
            else
            {
                PrintMessage("Built average linkage clusterer.");
            }
        }
        #endregion

        /// <summary>
        /// Processes the MA analysis data.
        /// </summary>
        /// <param name="args"></param>
        /// 
        static int StartMultiAlign()
        {
            /// /////////////////////////////////////////////////////////////
            /// Setup essential objects
            /// /////////////////////////////////////////////////////////////
            // Builds the list of algorithm providers.
            AlgorithmBuilder builder                = new AlgorithmBuilder();
            FeatureDataAccessProviders providers    = null;
            MultiAlignAnalysisProcessor processor   = null;            

            // Use this to signal when the analysis is done.              
            m_triggerEvent                          = new ManualResetEvent(false);
            m_errorEvent                            = new ManualResetEvent(false);
            m_errorException                        = null;

            /// /////////////////////////////////////////////////////////////
            /// Print Help
            /// /////////////////////////////////////////////////////////////
            // See if the user wants help
            if (m_showHelp)
            {
                PrintHelp();
                m_errorEvent.Dispose();
                m_triggerEvent.Dispose();
                return 0;
            }

            /// /////////////////////////////////////////////////////////////
            /// Validate the command line
            /// /////////////////////////////////////////////////////////////            
            AnalysisType validated  = ValidateSetup();
            if (validated == AnalysisType.InvalidParameters)
            {
                if (m_createPlots && m_databaseName != null)
                {
                    PrintVersion();
                    PrintSpacer();
                    CreatePlotsOffline(providers);
                }
                else
                {
                    PrintHelp();
                }
                return 0;
            }
            
            /// /////////////////////////////////////////////////////////////
            /// Setup log path, analysis path, and print version to log file.            
            /// /////////////////////////////////////////////////////////////                        
            SetupAnalysisEssentials();                                           
            InputAnalysisInfo analysisSetupInformation  = null;
                                    
            /// /////////////////////////////////////////////////////////////
            /// Determine if we have specified a valid database to extract
            /// data from or to re-start an analysis.
            /// /////////////////////////////////////////////////////////////    
            string  databasePath    = Path.Combine(m_analysisPath, m_analysisName); 
            bool    databaseExists  = File.Exists(databasePath);
                        
            switch (validated)
            {
                case  AnalysisType.ExportDataOnly:
                    PrintMessage("Exporting data only selected.");
                    if (!databaseExists)
                    {
                        PrintMessage("The database you specified to extract data from does not exist.");
                        return 1;
                    }

                    // Create access to data.
                    providers = SetupDataProviders(false);
                    if (providers == null)
                    {
                        PrintMessage("Could not create connection to database.");
                        return 1;
                    }

                    // Find all the datasets 
                    List<DatasetInformation> datasets = providers.DatasetCache.FindAll();
                    if (datasets == null || datasets.Count == 0)
                    {
                        PrintMessage("There are no datasets present in the current database.");
                        CleanupDataProviders();
                        return 1;
                    }

                    if (m_exporterNames.ClusterMSMSPath != null)
                    {
                        processor = ConstructAnalysisProcessor(builder, providers);
                        processor.ExtractMSMS(providers, datasets);             
                    }

                    ConstructExporting();
                    if (m_clusterExporters.Count > 0)
                    {
                        List<UMCClusterLight> clusters = null;
                        using (MultiAlignCore.IO.Mammoth.MammothDatabase database = new MultiAlignCore.IO.Mammoth.MammothDatabase(databasePath))
                        {
                            database.Connect();
                            clusters = database.GetClusters(
                                        new MultiAlignCore.IO.Mammoth.MammothDatabaseRange(-1, 10000000, -1, 2, -1, 200));
                            database.Close();
                        }
                        if (clusters.Count < 1)
                        {
                            PrintMessage("No clusters present in the database.");
                            return 1;
                        }


                        foreach (IFeatureClusterWriter writer in m_clusterExporters)
                        {
                            writer.WriteClusters(clusters, datasets);
                        }
                    }
                    CleanupDataProviders();                    
                    break;
                case AnalysisType.Full:
                    PrintMessage("Performing full analysis.");

                    /// /////////////////////////////////////////////////////////////            
                    /// Read the input files.
                    /// /////////////////////////////////////////////////////////////                                    
                    bool useMTDB                                = false;            
                    bool isInputFileOk = ReadInputDefinitionFile(out analysisSetupInformation, out useMTDB);
                    if (!isInputFileOk)
                        return 1;

                    /// /////////////////////////////////////////////////////////////            
                    /// Figure out if the factors are defined.
                    /// /////////////////////////////////////////////////////////////                                    
                    if (m_options.ContainsKey("-factors"))
                    {
                        PrintMessage("Factor file specified.");
                        string factorFile = m_options["-factors"][0];
                        analysisSetupInformation.FactorFile = factorFile;
                    }

                    /// /////////////////////////////////////////////////////////////            
                    /// Creates or connects to the underlying analysis database.
                    /// ///////////////////////////////////////////////////////////// 
                    providers                = SetupDataProviders(true);
                    /// /////////////////////////////////////////////////////////////
                    /// Create the clustering, analysis, and plotting paths.
                    /// /////////////////////////////////////////////////////////////                                    
                    ConstructClustering(builder);
                    ConstructExporting();

                    m_analysis               = ConstructAnalysisObject(analysisSetupInformation);
                    m_analysis.DataProviders = providers;

                    ConstructPlotPath();

                    /// /////////////////////////////////////////////////////////////
                    /// Read the parameter files.
                    /// /////////////////////////////////////////////////////////////            
                    ReadParameterFile();

                    /// /////////////////////////////////////////////////////////////
                    /// Construct Dataset information
                    /// /////////////////////////////////////////////////////////////            
                    // Construct the dataset information for export.
                    ConstructDatasetInformation(analysisSetupInformation, m_analysis);
                    ConstructFactorInformation(analysisSetupInformation,  m_analysis.MetaData.Datasets);

                    bool isBaselineSpecified = ConstructBaselines(analysisSetupInformation, m_analysis.MetaData, useMTDB);
                    if (!isBaselineSpecified)
                    {
                        return 1;
                    }
                    
                    ExportParameterFile();
                    PrintSpacer();
                    PrintParameters(m_analysis);
                    PrintSpacer();

                    /// /////////////////////////////////////////////////////////////
                    /// Setup the processor.
                    /// /////////////////////////////////////////////////////////////            
                    processor                = ConstructAnalysisProcessor(builder, providers);

                    /// /////////////////////////////////////////////////////////////
                    /// Start the analysis
                    /// /////////////////////////////////////////////////////////////            
                    PrintMessage("Analysis Started.");    
                    processor.StartAnalysis(m_analysis);            
                    int handleID = WaitHandle.WaitAny(new WaitHandle[] { m_triggerEvent , m_errorEvent});

                    if (handleID == 1)
                    {
                        PrintMessage("There was an error during processing.");
                        return 1;
                    }

                    /// /////////////////////////////////////////////////////////////
                    /// Finalize the analysis plots etc.
                    /// /////////////////////////////////////////////////////////////
                    try
                    {
                        CreateFinalAnalysisPlots(providers.FeatureCache, providers.ClusterCache);
                        CreatePlotReport();
                    }
                    catch(Exception ex)
                    {
                        PrintMessage("There was an error when trying to create the final analysis plots, however, the data analysis is complete.");
                        PrintMessage(ex.Message);
                        PrintMessage(ex.StackTrace);
                    }

                    m_analysis.Dispose();
                    m_triggerEvent.Dispose();
                    m_errorEvent.Dispose();
                    processor.Dispose();
                    CleanupDataProviders();
                    PrintMessage("Analysis Complete.");
                    break;
            }
            return 0;  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
		static int Main(string[] args)
		{
			IntPtr handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
			SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);

            try
            {
                ProcessCommandLineArguments(args);
                return StartMultiAlign();
            }
            catch(Exception ex)
            {
                PrintMessage("Unhandled Error: " + ex.Message);
                return 1;
            }
        }
    }       
}
