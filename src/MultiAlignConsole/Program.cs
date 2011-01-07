using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

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
        #region Members
        private static ManualResetEvent     m_triggerEvent;
        private static string               m_logPath;
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
        /// Prints the help message.
        /// </summary>
        static void PrintHelp()
        {
            Log("For baseline datasets - ");
            Log("MultiAlignConsole fileInputList paramterFile.xml analysisPath");
            Log("   fileInputList     = ASCII Text file with input file names.");
            Log("      In list of files use asterik to indicate the baseline choice, e.g. 'dataset *'");                
            Log("   parameterFile.xml = XML file defining MultiAlign parameters.");
            Log("   analysisPath      = file directory of where to put MultiAlign output.");
            Log("For Mass Tag Databases - ");
            Log("MultiAlignConsole paramterFile.xml analysisPath");                
            Log("   parameterFile.xml = XML file defining MultiAlign parameters.");
            Log("   analysisPath      = file directory of where to put MultiAlign output.");
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
            m_triggerEvent.Set();
        }
        /// <summary>
        /// Logs when features are aligned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesAligned(object sender, FeaturesAlignedEventArgs e)
        {
            Log("Features Aligned - " + e.AligneeDatasetInformation.DatasetName);            
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
        static void analysis_AnalysisComplete(object sender)
        {
            m_triggerEvent.Set();
        }
        static void Fail(int id)
        {
            Log("Comparison Failed @ feature : " + id.ToString());
        }
        static void Compare(string cachePath, MultiAlignAnalysis analysis)
        {
            // Compare features 
            NHibernateUtil.SetDbLocationForRead(cachePath);
            UmcDAOHibernate featureCache        = new UmcDAOHibernate();
            UmcClusterDAOHibernate clusterCache = new UmcClusterDAOHibernate();
            
            // Dataset information.
            foreach (DatasetInformation information in analysis.Datasets)
            {
                int datasetID         = Convert.ToInt32(information.DatasetId);
                clsUMC[] umcs         = analysis.UMCData.GetUMCS(datasetID);

                List<clsUMC> features = featureCache.FindByDatasetId(datasetID);

                Log("Comparing features");
                for (int i = 0; i < features.Count; i++)
                {
                    clsUMC ma = umcs[i];
                    clsUMC map = features[i];
                    
                    if (Math.Abs(ma.mfloat_drift_time - map.mfloat_drift_time) > .00000001)
                        Fail(i);
                    if (ma.mint_scan != map.mint_scan)
                        Fail(i);
                    if (Math.Abs(ma.mdouble_mono_mass - map.mdouble_mono_mass) > .00000001)
                        Fail(i);
                    if (ma.mint_umc_id != map.mint_umc_id)
                        Fail(i);
                    if (Math.Abs(ma.MZForCharge - map.MZForCharge) > .0000000001)
                        Fail(i);
                }

                Log("Comparing aligned versions of features");
                for (int i = 0; i < features.Count; i++)
                {
                    clsUMC ma  = umcs[i];
                    clsUMC map = features[i];
                    if (Math.Abs(ma.mdouble_net - map.mdouble_net) > .000000001)
                        Fail(i);
                    if (Math.Abs(ma.mdouble_mono_mass_calibrated - map.mdouble_mono_mass_calibrated) > .000000001)
                        Fail(i);
                    if (ma.mint_scan_aligned != map.mint_scan_aligned)
                        Fail(i);
                    if (Math.Abs(ma.MZForCharge - map.MZForCharge) > .000000001)
                        Fail(i);
                }
            }
            List<clsCluster> clusters = clusterCache.FindAll();
            Log("Comparing cluster values");

            for (int i = 0; i < clusters.Count; i++)
            {
                clsCluster ma  = analysis.UMCData.mobjClusterData.GetCluster(i);
                clsCluster map = clusters[i];

                if (Math.Abs(ma.Mass - map.Mass) > .00000001)
                    Fail(i);
                if (ma.Charge != map.Charge)
                    Fail(i);
                if (Math.Abs(ma.Net - map.Net) > .00000001)
                    Fail(i);
                if (Math.Abs(ma.DriftTime - map.DriftTime) > .0000000001)
                    Fail(i);                
            }
            Log("Comparison successful.");
        }
        /// <summary>
        /// Calculates the current usage of current processes memory.
        /// </summary>
        /// <returns>Memory usage of current process.</returns>
        static long GetMemory()
        {
            Process process = Process.GetCurrentProcess();
            long memory  = process.WorkingSet64;
            memory      /= 1024;
            memory      /= 1024;
            process.Dispose();

            return memory;
        }
        #endregion

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

            Log("Staring MultiAlign Console."); 

            if (args[0] == "-h" || args[0] == "-help")
            {
                PrintHelp();
                return;
            }

            // Determine options.
            string fileInputList = null;
            string parameterFile = null;
            string analysisPath  = null;
            string analysisName  = null;
            if (args.Length == 4)
            {
                fileInputList   = args[0];
                parameterFile   = args[1];
                analysisPath    = args[2];
                analysisName    = args[3];
            }
            else
            {
                Log("The argument list was not long enough.");
                PrintHelp();
                return;
            }
          

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
            analysis.PathName                       = analysisPath;
            analysis.AnalysisName                   = analysisName;
            analysis.BaselineDataset                = baseline;
            MultiAlignAnalysisProcessor processor   = new MultiAlignAnalysisProcessor();

            m_triggerEvent = new ManualResetEvent(false);

            // Setup the parameters.
            Log("Loading parameters.");
            analysis.LoadParametersFromFile(parameterFile);

            MultiAlignParameterIniFileWriter writer = new MultiAlignParameterIniFileWriter();            
            writer.WriteParametersToFile("options.ini", analysis);
            

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
    }
}
