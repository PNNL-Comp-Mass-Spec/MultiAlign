using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using MultiAlign.Drawing;
using MultiAlignEngine.Features;

using PNNLProteomics.IO;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;

using PNNLProteomics.Data;
using PNNLProteomics.EventModel;
using PNNLProteomics.MultiAlign;
using PNNLProteomics.Data.Analysis;

using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;

namespace MultiAlignConsole
{
    class Program
    {
        #region Constants 
        private const string THUMBNAIL_PATH = "Plots";
        #endregion

        #region Members
        private static ManualResetEvent     m_triggerEvent;
        private static string               m_logPath;
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
        #endregion

        #region Methods
        /// <summary>
        /// Logs the data to the stored text file path.
        /// </summary>
        /// <param name="message"></param>
        static void Log(string message)
        {
            string newMessage = DateTime.Now.ToString() + " - " + GetMemory().ToString() + " MB - " + message;
            File.AppendAllText(m_logPath, newMessage + Environment.NewLine);
            Console.WriteLine(newMessage);
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        static void PrintMessage(string message)
        {
            File.AppendAllText(m_logPath, message + Environment.NewLine);
            Console.WriteLine(message);
        }
        /// <summary>
        /// Prints the help message.
        /// </summary>
        static void PrintHelp()
        {
            PrintMessage("For baseline datasets - ");
            PrintMessage("MultiAlignConsole fileInputList paramterFile.ini analysisPath analysisName");
            PrintMessage("   fileInputList     = ASCII Text file with input file names.");
            PrintMessage("      In list of files use asterik to indicate the baseline choice, e.g. 'dataset *'");                
            PrintMessage("   parameterFile.xml = XML file defining MultiAlign parameters.");
            PrintMessage("   analysisPath      = file directory of where to put MultiAlign output.");
            PrintMessage("For Mass Tag Databases - ");
            PrintMessage("MultiAlignConsole paramterFile.xml analysisPath");                
            PrintMessage("   parameterFile.xml = XML file defining MultiAlign parameters.");
            PrintMessage("   analysisPath      = file directory of where to put MultiAlign output.");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Displays an update on the percentage complete.
        /// </summary>
        /// <param name="complete"></param>
        static void DisplayPercentComplete(int complete)
        {
            
        }
        /// <summary>
        /// Displays the status message when the analysis completes.
        /// </summary>
        /// <param name="status"></param>
        static void DisplayStatus(int level, string status)
        {
            Log(status);            
        }
        /// <summary>
        /// Displays the title.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="title"></param>
        static void DisplayTitle(int data, string title)
        {
            Log(title);
        }
        /// <summary>
        /// Terminates the application when the analysis is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_AnalysisComplete(object sender, EventArgs e)
        {
            // Create the heatmap
            UmcDAOHibernate cache           = new UmcDAOHibernate();
            List<clsUMC> umcs               = cache.FindAll();
            ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true, 
                                                1, 100, 
                                                "Charge State Histogram", "Charge State", "Count", m_width, m_height);

            Image image = RenderDatasetInfo.ChargeStateHistogram_Thumbnail(umcs, m_width, m_height, options);
            string name = "chargeStates.png";
            string path = Path.Combine(m_plotSavePath, name);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png); 
 
            UmcClusterDAOHibernate clusterCache = new UmcClusterDAOHibernate();
            List<clsCluster> clusters = clusterCache.FindAll();

            // Then render the cluster size image.
            image = RenderDatasetInfo.ClusterSizeHistogram_Thumbnail(clusters, m_width, m_height, options);
            name  = "clusterSizes.png";
            path  = Path.Combine(m_plotSavePath, name);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

            m_triggerEvent.Set();
        }
        /// <summary>
        /// Logs when features are aligned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesAligned(object sender, FeaturesAlignedEventArgs e)
        {
            string name = e.AligneeDatasetInformation.DatasetName;

            Log("Features Aligned - " + name);

            ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);

            options.MarginMin   = 1;
            options.MarginMax   = 100;
            options.Title       = "NET Error Histogram " + name;
            options.XAxisLabel  = "NET Error (%)";
            options.YAxisLabel  = "Count";
            options.Width       = m_width;
            options.Height      = m_height;

            // Create the heatmap
            Image image      = RenderDatasetInfo.AlignmentHeatmap_Thumbnail(e.AlignmentData, m_width, m_height);            
            string labelName = Path.GetFileNameWithoutExtension(name) + "_heatmap.png";
            string path      = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

            image       = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.netErrorHistogram, options);
            labelName   = Path.GetFileNameWithoutExtension(name) + "_netErrorHistogram.png";
            path        = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

            options.Title       = "Mass Error Histogram " + name;
            options.XAxisLabel  = "Mass Error (PPM)";
            image       = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.massErrorHistogram, options);
            labelName   = Path.GetFileNameWithoutExtension(name) + "_massErrorHistogram.png";
            path        = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

        }
        /// <summary>
        /// Logs when features are clustered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesClustered(object sender, FeaturesClusteredEventArgs e)
        {
            Log("Features Clustered.");            
        }
        /// <summary>
        /// Logs when features are peak matched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesPeakMatched(object sender, FeaturesPeakMatchedEventArgs e)
        {
            Log("Features are peak matched.");
        }
        /// <summary>
        /// Logs when features are loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesLoaded(object sender, FeaturesLoadedEventArgs e)
        {
            Log("Features are loaded - " + e.DatasetInformation.DatasetName);
        }
        /// <summary>
        /// Logs status messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_Status(object sender, AnalysisStatusEventArgs e)
        {
            Log(e.StatusMessage);
        }
        /// <summary>
        /// Calculates the current usage of current processes memory.
        /// </summary>
        /// <returns>Memory usage of current process.</returns>
        static long GetMemory()
        {
            Process process = Process.GetCurrentProcess();
            long memory = process.WorkingSet64;
            memory /= 1024;
            memory /= 1024;
            process.Dispose();

            return memory;
        }
        #endregion

        /// <summary>
        /// Processes the MA analysis data.
        /// </summary>
        /// <param name="args"></param>
        static void StartMultiAlign(string [] args)
        {

            // Determine options.
            string fileInputList = null;
            string parameterFile = null;            
            if (args.Length == 4)
            {
                fileInputList   = args[0];
                parameterFile   = args[1];
                m_analysisPath    = args[2];
                m_analysisName    = args[3];
            }
            else
            {
                Log("The argument list was not long enough.");
                PrintHelp();
                return;
            }

            //TODO: Make this a default value constant 
            m_width  = 800;
            m_height = 800; 

            // Read the input datasets.
            if (!File.Exists(fileInputList))
            {
                Log(string.Format("The input file {0} does not exist.", fileInputList));
                return;
            }

            Log("Parsing Filenames.");
            string [] files         = File.ReadAllLines(fileInputList);
            List<string> filenames  = new List<string>();
            string baseline         = null;
            foreach(string filename in files)
            {
                string [] baselineCheck = filename.Split('*');

                if (baselineCheck.Length == 2 && baselineCheck[1] == "")
                {
                    baseline = baselineCheck[0];
                    filenames.Add(baseline);
                    Log(string.Format("Baseline dataset selected: {0}", baseline));
                }
                else
                {                    
                    filenames.Add(filename);
                }
                
            }

            // Construct an analysis object.
            DelegateSetPercentComplete complete     = new DelegateSetPercentComplete(DisplayPercentComplete);
            DelegateSetStatusMessage status         = new DelegateSetStatusMessage(DisplayStatus);
            DelegateSetStatusMessage title          = new DelegateSetStatusMessage(DisplayTitle);

            Log("Creating Analysis.");
            MultiAlignAnalysis analysis             = new MultiAlignAnalysis(complete, status, title);
            analysis.PathName                       = m_analysisPath;
            analysis.AnalysisName                   = m_analysisName;
            analysis.BaselineDataset                = baseline;
            MultiAlignAnalysisProcessor processor   = new MultiAlignAnalysisProcessor();

            Log("Creating Plot Thumbnail Path");
            // set the plot save path.
            m_plotSavePath = Path.Combine(m_analysisPath, THUMBNAIL_PATH);

            // Find out where it's located.
            if (!Directory.Exists(m_plotSavePath))
            {
                Directory.CreateDirectory(m_plotSavePath);
            }

            // Use this to signal when the analysis is done.  We are using a asnychronous call here.
            m_triggerEvent = new ManualResetEvent(false);

            // Setup the parameters.
            Log("Loading parameters.");
            analysis.LoadParametersFromFile(parameterFile);

            MultiAlignParameterIniFileWriter writer = new MultiAlignParameterIniFileWriter();            
            writer.WriteParametersToFile("options.ini", analysis);
            processor.ClusterAlgorithmType = ClusteringAlgorithmType.AverageLinkage;

            // Create dataset information.
            int i = 0;
            Log("Creating dataset information.");

            foreach (string filename in filenames)
            {
                DatasetInformation info = new DatasetInformation();
                info.ArchivePath = filename;
                info.DatasetId = (i.ToString());
                i++;

                info.DatasetName = Path.GetFileName(filename);
                info.JobId = "";
                info.mstrLocalPath = filename;
                info.mstrResultsFolder = Path.GetDirectoryName(filename);
                info.ParameterFileName = "";
                info.Selected = true;

                Log("\tCreated dataset information for " + filename);
                analysis.Datasets.Add(info);
            }

            // Load alignment data.
            if (baseline != null)
            {
                for (i = 0; i < analysis.Datasets.Count; i++)
                {
                    analysis.AlignmentOptions.Add(analysis.DefaultAlignmentOptions);
                }
                analysis.BaselineDataset = baseline;
            }

            // Start Analysis.
            Log("Analysis Started.");
            processor.FeaturesAligned       += new EventHandler<FeaturesAlignedEventArgs>(processor_FeaturesAligned);
            processor.FeaturesLoaded        += new EventHandler<FeaturesLoadedEventArgs>(processor_FeaturesLoaded);
            processor.FeaturesClustered     += new EventHandler<FeaturesClusteredEventArgs>(processor_FeaturesClustered);
            processor.FeaturesPeakMatched   += new EventHandler<FeaturesPeakMatchedEventArgs>(processor_FeaturesPeakMatched);
            processor.AnalysisComplete      += new EventHandler(processor_AnalysisComplete);
            processor.Status                += new EventHandler<AnalysisStatusEventArgs>(processor_Status);
            processor.StartAnalysis(analysis);

            // Wait for the analysis to complete.
            WaitHandle.WaitAll(new WaitHandle[] { m_triggerEvent });

            analysis.Dispose();
            processor.Dispose();
            Log("Analysis Complete.");                        
        }


        /// <summary>
        /// Main entry point of application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Set the log path.
            DateTime now = DateTime.Now;
            m_logPath = string.Format("log_{1}-{0}-{2}-{3}-{4}-{5}.txt",
                                        now.Day,
                                        now.Month,
                                        now.Year,
                                        now.Hour,
                                        now.Minute,
                                        now.Second);

            Log("Starting MultiAlign Console Application.");
            if (args.Length < 1 || args[0] == "-h" || args[0] == "-help")
            {
                PrintHelp();
                return;
            }

            StartMultiAlign(args);
        }
    }
}
