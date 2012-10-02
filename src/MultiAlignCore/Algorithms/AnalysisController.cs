using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MultiAlignCore;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.MTDB;
using System.Threading;
using MultiAlignCore.IO.Parameters;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    /// Builds an analysis object and the required dependencies for processing.
    /// </summary>
    public class AnalysisController
    {
        #region Analysis Config and Reporting     
        private IAnalysisReportGenerator        m_reportCreator;
        private AnalysisConfig                  m_config;        
        #endregion

        public AnalysisController()
        {
            m_config        = null;
            m_reportCreator = null;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the processor associated with this analysis.
        /// </summary>
        public MultiAlignAnalysisProcessor Processor
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the 
        /// </summary>
        public AnalysisConfig Config
        {
            get
            {
                return m_config;
            }
            set
            {
                m_config = value;
            }
        }
        #endregion

        #region Data Provider Setup
        public void LoadExistingAnalysis(string path, IAnalysisReportGenerator reporter)
        {
            // Create the access to the database.
            FeatureDataAccessProviders providers = SetupDataProviders(path, false);

            // Create an analysis configuration
            m_config        = new AnalysisConfig();
            m_reportCreator = reporter;

            // Create an analysis processor.            
            m_config.Analysis   = ConstructAnalysisObject(providers);            
        }
        /// <summary>
        /// Sets up the NHibernate caches for storing and retrieving data.
        /// </summary>
        /// <param name="analysisPath"></param>
        /// <returns></returns>
        private  FeatureDataAccessProviders SetupDataProviders(string path, bool createNew)
        {
            try
            {
                return DataAccessFactory.CreateDataAccessProviders(path, createNew);
            }
            catch (System.IO.IOException ex)
            {
                Logger.PrintMessage("Could not access the database.  Is it opened somewhere else?");
                throw ex;
            }
        }
        /// <summary>
        /// Creates data providers to the database of the analysis name and path provided.
        /// </summary>
        /// <returns></returns>
        private  FeatureDataAccessProviders SetupDataProviders(bool createNewDatabase)
        {
            FeatureDataAccessProviders providers = null;
            Logger.PrintMessage("Setting up data providers for caching and storage.");
            try
            {
                string path = AnalysisPathUtils.BuildAnalysisName(m_config.AnalysisPath, m_config.AnalysisName);
                providers = SetupDataProviders(path, createNewDatabase);
            }
            catch (System.IO.IOException ex)
            {
                Logger.PrintMessage(ex.Message);
                Logger.PrintMessage(ex.StackTrace);
                throw ex;
            }
            return providers;
        }
        /// <summary>
        /// Cleans up the old database providers.
        /// </summary>
        private  void CleanupDataProviders()
        {
            NHibernateUtil.Dispose();
        }
        #endregion

        #region Processor Event Handlers
        /// <summary>
        /// Terminates the application when the analysis is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void processor_AnalysisComplete(object sender, AnalysisCompleteEventArgs e)
        {
            if (e.Analysis.MassTagDatabase != null)
            {
                
            }

            m_config.Report.PushEndHeader();

            ExportData(m_config.Analysis.DataProviders, AnalysisPathUtils.BuildAnalysisName(m_config.AnalysisPath, m_config.AnalysisName), e.Analysis.MetaData.Datasets);
            
            m_config.triggerEvent.Set();
        }
        /// <summary>
        /// Extractor progress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void extractor_Progress(object sender, PNNLOmics.Algorithms.ProgressNotifierArgs e)
        {
            Logger.PrintMessage(e.Message, true);
        }
        /// <summary>
        /// Logs when features are aligned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processor_FeaturesAligned(object sender, FeaturesAlignedEventArgs e)
        {
            Logger.PrintMessage("Creating feature alignment plots.");
            m_reportCreator.CreateAlignmentPlots(e);
        }
        /// <summary>
        /// Logs when features are clustered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processor_FeaturesClustered(object sender, FeaturesClusteredEventArgs e)
        {            
        }
        /// <summary>
        /// Logs when features are peak matched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processor_FeaturesPeakMatched(object sender, FeaturesPeakMatchedEventArgs e)
        {
            Logger.PrintMessage("Creating peak match plots");
            m_reportCreator.CreatePeakMatchedPlots(e);
        }
        /// <summary>
        /// Logs when features are loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processor_FeaturesLoaded(object sender, FeaturesLoadedEventArgs e)
        {
            Logger.PrintMessage(string.Format("Loaded {0} features from {1}", e.Features.Count, e.DatasetInformation.DatasetName));           
        }
        void processor_MassTagsLoaded(object sender, MassTagsLoadedEventArgs e)
        {
            m_reportCreator.CreateMassTagPlot(e);            
        }
        /// <summary>
        /// Logs status messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processor_Status(object sender, AnalysisStatusEventArgs e)
        {
            Logger.PrintMessage(e.StatusMessage);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processor_FeaturesExtracted(object sender, MultiAlignCore.Algorithms.MSLinker.FeaturesExtractedEventArgs e)
        {
            string extractionPath = Path.Combine(m_config.AnalysisPath, m_config.ExporterNames.ClusterMSMSPath);

            FeatureExtractionTableWriter writer = new MultiAlignCore.IO.Features.FeatureExtractionTableWriter();
            writer.WriteData(extractionPath, e);
        }
        #endregion

        #region Processor Events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processor_AnalysisError(object sender, AnalysisErrorEventArgs e)
        {
            Logger.PrintMessage(string.Format("There was an error while performing the analysis.  {0} : {1}", e.ErrorMessage, e.Exception.Message));
            if (e.Exception.StackTrace != null)
            {
                Logger.PrintMessage(string.Format("\n{0}", e.Exception.StackTrace));
            }
            m_config.errorException = e.Exception;
            m_config.errorEvent.Set();
        }
        #endregion

        #region Help
        /// <summary>
        /// Prints the help message.
        /// </summary>
        private void PrintHelp()
        {
            Logger.PrintMessage(" ", false);
            Logger.PrintMessage("usage: MultiAlignConsole [options]", false);
            Logger.PrintMessage(" ", false);
            Logger.PrintMessage(" Input File Format Notes: ", false);
            Logger.PrintMessage("    MS Feature Input Files -  MultiAlign needs a separate file to determine what deisotoped MS features or LCMS Features", false);
            Logger.PrintMessage("                              to load.", false);
            Logger.PrintMessage("                              You can use these file types:", false);
            Logger.PrintMessage("                                  *LCMSFeatures.txt", false);
            Logger.PrintMessage("                                  *_isos.csv", false);
            Logger.PrintMessage("                      Single Dataset - (You must specify a database to align to and peak match with.)", false);
            Logger.PrintMessage("                              [Files]", false);
            Logger.PrintMessage("                              pathOfFile_isos.csv", false);
            Logger.PrintMessage("                      Multiple Dataset - (If you don't specify a baseline database, you must specify a database to align to.)", false);
            Logger.PrintMessage("                              [Files]", false);
            Logger.PrintMessage("                              pathOfFile1_isos.csv", false);
            Logger.PrintMessage("                              pathOfFile2_isos.csv", false);
            Logger.PrintMessage("                      Specifying a baseline - This is done by placing an asterisk after one of the dataset names.", false);
            Logger.PrintMessage("                      (NOTE: If you do not specify a baseline, a database must be used for alignment)", false);
            Logger.PrintMessage("                              [Files]", false);
            Logger.PrintMessage("                              pathOfFile1_isos.csv", false);
            Logger.PrintMessage("                              pathOfFile2_isos.csv*", false);
            Logger.PrintMessage("                      Specifying a baseline - This is done by placing an asterisk after one of the dataset names.", false);
            Logger.PrintMessage("                              [Files]", false);
            Logger.PrintMessage("                              pathOfFile1_isos.csv", false);
            Logger.PrintMessage("                              pathOfFile2_isos.csv*", false);
            Logger.PrintMessage("    MS/MS Data Linking - linking MS Features to MS/MS spectra can be done by specifying the RAW dataset files.", false);
            Logger.PrintMessage("                         This currently only works for 32-bit (x86) versions and with Thermo Finnigan data files.", false);
            Logger.PrintMessage("                         To use this feature, you must have matching dataset (MS Feature Input Files) file ", false);
            Logger.PrintMessage("                         names (extension excluded).", false);
            Logger.PrintMessage("                              [Raw]", false);
            Logger.PrintMessage("                              pathOfFile1.Raw", false);
            Logger.PrintMessage("    Peak Matching -   To perform peak matching to an Accurate Mass and Time Tag Database (AMT DB) you need to specify", false);
            Logger.PrintMessage("                      the name of the database in the input file. ", false);
            Logger.PrintMessage("                      To do this with a local SQLite database use: ", false);
            Logger.PrintMessage("                              [Database]", false);
            Logger.PrintMessage("                              sqlite = pathOfDatabase.mdb", false);
            Logger.PrintMessage("                      To use a meta-sample text file (comma separated): ", false);
            Logger.PrintMessage("                              [Database]", false);
            Logger.PrintMessage("                              metasample = path to meta sample file", false);
            Logger.PrintMessage("                      To use one of the Mass Tag System's (MTS) databases use - PNNL Only: ", false);
            Logger.PrintMessage("                              [Database]", false);
            Logger.PrintMessage("                              database = nameOfDatabase", false);
            Logger.PrintMessage("                              server   = serverDatabaseLivesOn", false);
            Logger.PrintMessage("[Options]", false);
            Logger.PrintMessage(" ", false);
            Logger.PrintMessage("   -files  inputFile.txt ", false);
            Logger.PrintMessage("          ASCII Text file with input file names.", false);
            Logger.PrintMessage("          In list of files use asterik to indicate the baseline choice, e.g. 'dataset *'", false);
            Logger.PrintMessage("   -name analysisName  ", false);
            Logger.PrintMessage("          Name to give analysis.", false);
            Logger.PrintMessage("   -log logPath.txt", false);
            Logger.PrintMessage("          Path to provide for log files.", false);
            Logger.PrintMessage("   -h", false);
            Logger.PrintMessage("          Prints this help message.", false);
            Logger.PrintMessage("   -help", false);
            Logger.PrintMessage("          Prints this help message.", false);
            Logger.PrintMessage("   -html htmlPathName.html", false);
            Logger.PrintMessage("          Name to give output HTML plot file.", false);
            Logger.PrintMessage("   -params parameterFile.xml  ", false);
            Logger.PrintMessage("          XML file defining MultiAlign parameters.", false);
            Logger.PrintMessage("   -path  AnalysisPath      ", false);
            Logger.PrintMessage("          File directory of where to put MultiAlign output.  Can be relative or absolute.", false);
            Logger.PrintMessage("   -centroid      ", false);
            Logger.PrintMessage("          To use centroid distance as clustering algorithm.", false);
            Logger.PrintMessage("   -factors factorFilePath", false);
            Logger.PrintMessage("          Path to factor definition file to be loaded.", false);
            Logger.PrintMessage("   -exportClusters clusterFileName     ", false);
            Logger.PrintMessage("          Exports clusters and their LC-MS features to the file name specified.  This file will be sent to the analysis path folder you specified.", false);
            Logger.PrintMessage("   -exportCrossTab  crossTabFileName     ", false);
            Logger.PrintMessage("          Exports clusters and their LC-MS features in cross tab fashion.  Each row is a cluster.  No mass tags are exported.  This file will be sent to the analysis path folder you specified.", false);
            Logger.PrintMessage("   -exportAbundances  crossTabFileName     ", false);
            Logger.PrintMessage("          Exports cluster ids and the abundances of their LC-MS features in cross tab fashion.  Each row is a cluster.  No mass tags are exported.  This file will be sent to the analysis path folder you specified.", false);
            Logger.PrintMessage("   -exportDTA      ", false);
            Logger.PrintMessage("          Exports all MS/MS spectra in the DTA format.", false);
            Logger.PrintMessage("   -plots   [databaseName]  ", false);
            Logger.PrintMessage("          Creates plots for final analysis.  If [databaseName] specified when not running analysis, this will create plots post-analysis.", false);
            Logger.PrintMessage("   -useFactors ", false);
            Logger.PrintMessage("          Flags MultiAlign to use factors.  If no factor file is provided, then MultiAlign will attempt to contact DMS -- PNNL Network only.", false);
        }/// <summary>
        /// Writes the parameters to the log file and database.
        /// </summary>
        /// <param name="analysis"></param>
        private void PrintParameters(MultiAlignAnalysis analysis, bool insertIntoDatabase)
        {
            Logger.PrintMessage("Parameters Loaded");
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("MS Linker Options", analysis.Options.MSLinkerOptions);
            options.Add("UMC Finding Options", analysis.Options.FeatureFindingOptions);
            options.Add("Feature Filtering Options", analysis.Options.FeatureFilterOptions);
            options.Add("Mass Tag Database Options", analysis.Options.MassTagDatabaseOptions);
            options.Add("Alignment Options", analysis.Options.AlignmentOptions);
            options.Add("Drift Time Alignment Options", analysis.Options.DriftTimeAlignmentOptions);
            options.Add("Cluster Options", analysis.Options.ClusterOptions);
            options.Add("STAC Options", analysis.Options.STACOptions);

            List<ParameterHibernateMapping> allmappings = new List<MultiAlignCore.IO.Parameters.ParameterHibernateMapping>();
            foreach (string key in options.Keys)
            {
                object o = options[key];
                Logger.PrintMessage(key, true);
                List<string> parameters = ParameterUtility.ConvertParameterObjectToStrings(o);
                foreach (string parameter in parameters)
                {
                    Logger.PrintMessage("\t" + parameter, true);
                }

                List<ParameterHibernateMapping> mappings = ParameterUtility.ExtractParameterMapObjects(o, key);
                allmappings.AddRange(mappings);
            }

            ParameterHibernateMapping assemblyMap = new ParameterHibernateMapping();
            string assemblyData = ApplicationUtility.GetAssemblyData();
            assemblyMap.OptionGroup = "Assembly Info";
            assemblyMap.Parameter = "Version";
            assemblyMap.Value = assemblyData;
            allmappings.Add(assemblyMap);

            ParameterHibernateMapping systemMap = new ParameterHibernateMapping();
            string systemData = ApplicationUtility.GetSystemData();
            systemMap.OptionGroup = "Assembly Info";
            systemMap.Parameter = "System Info";
            systemMap.Value = systemData;
            allmappings.Add(systemMap);

            if (insertIntoDatabase)
            {
                Logger.PrintMessage("Writing parameters to the analysis database.");
                GenericDAOHibernate<ParameterHibernateMapping> parameterCache = new GenericDAOHibernate<MultiAlignCore.IO.Parameters.ParameterHibernateMapping>();
                parameterCache.AddAll(allmappings);
            }
        }

        #endregion
        
        #region Exporting
        private int ExportData(FeatureDataAccessProviders providers,
                                string databasePath,
                                List<DatasetInformation> datasets)
        {
            if (m_config.ClusterExporters.Count > 0)
            {
                List<UMCClusterLight> clusters = providers.ClusterCache.FindAll();                                                        
                if (clusters.Count < 1)
                {
                    Logger.PrintMessage("No clusters present in the database.");
                    return 1;
                }
                List<UMCLight> features = providers.FeatureCache.FindAllClustered();

                Dictionary<int, UMCClusterLight> clusterFeatureMap = new Dictionary<int, UMCClusterLight>();
                foreach (UMCClusterLight cluster in clusters)
                {
                    clusterFeatureMap[cluster.ID] = cluster;
                }
                foreach (UMCLight umc in features)
                {
                    clusterFeatureMap[umc.ClusterID].AddChildFeature(umc);
                }

                Logger.PrintMessage("Checking for mass tag matches");
                List<ClusterToMassTagMap> clusterMatches = providers.MassTagMatches.FindAll();

                Logger.PrintMessage("Checking for mass tags");
                List<PNNLOmics.Data.MassTags.MassTagLight> massTags = providers.MassTags.FindAll();

                Dictionary<int, List<ClusterToMassTagMap>> clusterMap = new Dictionary<int, List<ClusterToMassTagMap>>();
                if (clusterMatches.Count > 0)
                {
                    foreach (ClusterToMassTagMap map in clusterMatches)
                    {
                        if (!clusterMap.ContainsKey(map.ClusterId))
                        {
                            clusterMap.Add(map.ClusterId, new List<ClusterToMassTagMap>());
                        }
                        clusterMap[map.ClusterId].Add(map);
                    }
                }

                Dictionary<string, PNNLOmics.Data.MassTags.MassTagLight> tags = new Dictionary<string, PNNLOmics.Data.MassTags.MassTagLight>();
                if (massTags.Count > 0)
                {
                    foreach (PNNLOmics.Data.MassTags.MassTagLight tag in massTags)
                    {
                        string key = tag.ConformationID + "-" + tag.ID;
                        if (!tags.ContainsKey(key))
                        {
                            tags.Add(key, tag);
                        }
                    }
                }

                Logger.PrintMessage("Exporting Data");
                foreach (IFeatureClusterWriter writer in m_config.ClusterExporters)
                {
                    Logger.PrintMessage("Exporting in " + writer.ToString() + " format to " + m_config.AnalysisPath);
                    writer.WriteClusters(clusters,
                                         clusterMap,
                                         datasets,
                                         tags);
                }
            }
            return 0;
        }
        #endregion
        
        #region Construction
        private  void ConstructPlotPath()
        {
            Logger.PrintMessage("Creating Plot Thumbnail Path");
            // set the plot save path.
            m_config.plotSavePath = Path.Combine(m_config.AnalysisPath, m_reportCreator.PlotPath);

            // Find out where it's located.
            if (!Directory.Exists(m_config.plotSavePath))
            {
                Directory.CreateDirectory(m_config.plotSavePath);
            }
        }
        /// <summary>
        /// Creates the analysis processor and synchronizs the events.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        private  MultiAlignAnalysisProcessor ConstructAnalysisProcessor(AlgorithmBuilder builder, FeatureDataAccessProviders providers)
        {
            MultiAlignAnalysisProcessor processor = new MultiAlignAnalysisProcessor();
            processor.AnalysisError         += new EventHandler<AnalysisErrorEventArgs>(processor_AnalysisError);
            processor.FeaturesAligned       += new EventHandler<FeaturesAlignedEventArgs>(processor_FeaturesAligned);
            processor.FeaturesLoaded        += new EventHandler<FeaturesLoadedEventArgs>(processor_FeaturesLoaded);
            processor.MassTagsLoaded        += new EventHandler<MassTagsLoadedEventArgs>(processor_MassTagsLoaded);
            processor.FeaturesClustered     += new EventHandler<FeaturesClusteredEventArgs>(processor_FeaturesClustered);
            processor.FeaturesPeakMatched   += new EventHandler<FeaturesPeakMatchedEventArgs>(processor_FeaturesPeakMatched);
            processor.AnalysisComplete      += new EventHandler<AnalysisCompleteEventArgs>(processor_AnalysisComplete);
            processor.Status                += new EventHandler<AnalysisStatusEventArgs>(processor_Status);
            processor.FeaturesAdjusted += new EventHandler<FeaturesAdjustedEventArgs>(processor_FeaturesAdjusted);
            processor.BaselineFeaturesLoaded += new EventHandler<BaselineFeaturesLoadedEventArgs>(processor_BaselineFeaturesLoaded);
            processor.FeaturesExtracted     += new EventHandler<MultiAlignCore.Algorithms.MSLinker.FeaturesExtractedEventArgs>(processor_FeaturesExtracted);
            m_config.Analysis.DataProviders          = providers;
            processor.AlgorithmProviders     = builder.GetAlgorithmProvider(m_config.Analysis.Options);

            return processor;
        }

        void processor_FeaturesAdjusted(object sender, FeaturesAdjustedEventArgs e)
        {
            FeatureAdjusterWriter writer = new FeatureAdjusterWriter();
            writer.WriteFeatures(Path.Combine(m_config.AnalysisPath, e.DatasetInformation.DatasetName + "_adjustedScans.csv"), e.Features, e.AdjustedFeatures);
        }

        void processor_BaselineFeaturesLoaded(object sender, BaselineFeaturesLoadedEventArgs e)
        {
            m_reportCreator.CreateBaselinePlots(e);
        }

        /// <summary>
        /// Sets up the analysis essentials including analysis path, log path, and prints the version and parameter information
        /// to the log.
        /// </summary>
        private  void SetupAnalysisEssentials()
        {
            // Create the analysis path and log file paths.
            ConstructAnalysisPath();
            string dateSuffix = ConstructLogPath();
            
            // Log the version information to the log.
            Logger.PrintVersion();
            Logger.PrintSpacer();

            // Build Plot Path
            m_config.plotSavePath = AnalysisPathUtils.BuildPlotPath(m_config.AnalysisPath);

            // Build analysis name                  
            bool containsExtensionDB3 = m_config.AnalysisName.EndsWith(".db3");
            if (!containsExtensionDB3)
            {
                m_config.AnalysisName += ".db3";
            }

            // create application and analysis.
            Logger.PrintMessage("Starting MultiAlign Console Application.");
            Logger.PrintMessage("Creating analysis: ");
            Logger.PrintMessage("\t" + m_config.AnalysisName);
            Logger.PrintMessage("Storing analysis: ");
            Logger.PrintMessage("\t" + Path.GetFullPath(m_config.AnalysisPath));
            if (m_config.InputPaths != null && m_config.InputPaths.Length > 0)
            {
                Logger.PrintMessage("Using Files:  ");
                Logger.PrintMessage("\tFull Path: " + Path.GetFullPath(m_config.InputPaths));
                Logger.PrintMessage("\tFile Name: " + Path.GetFileName(m_config.InputPaths));
            }
            else
            {
                Logger.PrintMessage("No input files specified.");
            }

            if (m_config.ParameterFile != null)
            {
                Logger.PrintMessage("Using Parameters: ");
                Logger.PrintMessage("\tFull Path: " + Path.GetFullPath(m_config.ParameterFile));
                Logger.PrintMessage("\tFile Name: " + Path.GetFileName(m_config.ParameterFile));
            }
            else
            {
                Logger.PrintMessage("No parameter file specified.");
            }
        }
        private  void ReadParameterFile()
        {
            // Setup the parameters.
            Logger.PrintMessage("Loading parameters.");
            // Make sure we have parameters!
            if (!File.Exists(m_config.ParameterFile))
            {
                Logger.PrintMessage("The parameter file does not exist.");
                //return 1;
            }
            XMLParamterFileReader reader = new XMLParamterFileReader();
            MultiAlignAnalysis analysis = m_config.Analysis;
            reader.ReadParameterFile(m_config.ParameterFile, ref analysis);
        }
        private  string ConstructLogPath()
        {
            // Create the LOG FILE.
            string dateSuffix = AnalysisPathUtils.BuildDateSuffix();
            if (m_config.logPath == null)
            {
                m_config.logPath = AnalysisPathUtils.BuildLogPath(m_config.AnalysisPath,
                                                            m_config.AnalysisName,
                                                            dateSuffix);                
            }
            else
            {                
                m_config.logPath = Path.Combine(m_config.AnalysisPath,
                                         m_config.logPath,
                                         dateSuffix);                
            }
            Logger.LogPath = m_config.logPath;
            return dateSuffix;
        }
        private  void ConstructAnalysisPath()
        {
            //Create the analysis directory.
            if (!Directory.Exists(m_config.AnalysisPath))
            {
                Logger.PrintMessage("Creating analysis path " + m_config.AnalysisPath);
                Directory.CreateDirectory(m_config.AnalysisPath);
            }
            else
            {
                Logger.PrintMessage("Analysis path " + m_config.AnalysisPath + " already exists.");
            }
        }
        private  MultiAlignAnalysis ConstructAnalysisObject(InputAnalysisInfo analysisSetupInformation)
        {
            Logger.PrintMessage("Creating Analysis Objects.");
            MultiAlignAnalysis analysis             = new MultiAlignAnalysis();
            analysis.MetaData.AnalysisPath          = m_config.AnalysisPath;
            analysis.MetaData.AnalysisName          = m_config.AnalysisName;
            analysis.Options.UseMassTagDBAsBaseline = true;

            analysis.MetaData.ParameterFile         = m_config.ParameterFile;
            analysis.MetaData.InputFileDefinition   = m_config.InputPaths;
            analysis.MetaData.AnalysisSetupInfo     = analysisSetupInformation;
            return analysis;
        }
        private MultiAlignAnalysis ConstructAnalysisObject(FeatureDataAccessProviders providers)
        {
            Logger.PrintMessage("Creating Analysis Objects.");
            MultiAlignAnalysis analysis             = new MultiAlignAnalysis();
            
            // Need to extract from the database.
            analysis.MetaData.AnalysisPath          = m_config.AnalysisPath;
            analysis.MetaData.AnalysisName          = m_config.AnalysisName;
            analysis.Options.UseMassTagDBAsBaseline = true;            
            analysis.MetaData.ParameterFile         = m_config.ParameterFile;
            analysis.MetaData.InputFileDefinition   = m_config.InputPaths;                        
            analysis.MetaData.AnalysisSetupInfo     = null;
            analysis.DataProviders                  = providers;
            analysis.Clusters                       = providers.ClusterCache.FindAll();
            analysis.MetaData.Datasets              = providers.DatasetCache.FindAll();

            MassTagDatabaseLoaderCache provider     = new MassTagDatabaseLoaderCache();
            provider.Provider                       = analysis.DataProviders.MassTags;
            analysis.MassTagDatabase                = provider.LoadDatabase();

            return analysis;
        }
        private  bool ReadInputDefinitionFile(out InputAnalysisInfo analysisSetupInformation, out bool useMTDB)
        {
            // Read the input datasets.
            if (!File.Exists(m_config.InputPaths))
            {
                Logger.PrintMessage(string.Format("The input file {0} does not exist.", m_config.InputPaths));
                //return 1;
            }
            else
            {
                Logger.PrintMessage("Copying input file to output directory.");
                try
                {
                    string dateSuffix = AnalysisPathUtils.BuildDateSuffix();
                    string newPath = Path.GetFileNameWithoutExtension(m_config.InputPaths);
                    newPath = newPath + "_" + dateSuffix + ".txt";
                    File.Copy(m_config.InputPaths, Path.Combine(m_config.AnalysisPath, newPath));
                }
                catch (Exception ex)
                {
                    Logger.PrintMessage("Could not copy the input file to the output directory.  " + ex.Message);
                }
            }

            Logger.PrintMessage("Parsing Input Filenames and Databases.");
            useMTDB = false;
            analysisSetupInformation = null;
            try
            {
                analysisSetupInformation = MultiAlignFileInputReader.ReadInputFile(m_config.InputPaths);
            }
            catch (Exception ex)
            {
                Logger.PrintMessage("The input file had some bad lines in it.  " + ex.Message);
                return false;
            }
            Logger.PrintMessage("Found " + analysisSetupInformation.Files.Count.ToString() + " files.");

            // Validate the mass tag database settings.
            try
            {
                useMTDB = analysisSetupInformation.Database.ValidateDatabaseType();
            }
            catch (AnalysisMTDBSetupException ex)
            {
                Logger.PrintMessage("There was a problem with the mass tag database specification.  " + ex.Message);
                return false;
            }
            return true;
        }
        private  void ExportParameterFile()
        {
            // Output the settings to INI for viewing.
            string outParamName = Path.GetFileNameWithoutExtension(m_config.ParameterFile);
            string outParamPath = Path.Combine(m_config.AnalysisPath, outParamName);
            XMLParameterFileWriter xmlWriter = new XMLParameterFileWriter();
            xmlWriter.WriteParameterFile(outParamPath + ".xml", m_config.Analysis);
        }
        /// <summary>
        /// Constructs the baseline databases.
        /// </summary>
        /// <param name="analysisSetupInformation"></param>
        /// <param name="useMTDB"></param>
        private  bool ConstructBaselines(InputAnalysisInfo analysisSetupInformation, AnalysisMetaData analysisMetaData, bool useMTDB)
        {
            Logger.PrintMessage("Confirming baseline selections.");
            if (useMTDB)
            {
                switch (analysisSetupInformation.Database.DatabaseFormat)
                {
                    case MassTagDatabaseFormat.Access:
                        Logger.PrintMessage("Using local Access Mass Tag Database at location: ");
                        Logger.PrintMessage(string.Format("\tFull Path: {0}", analysisSetupInformation.Database.LocalPath));
                        Logger.PrintMessage(string.Format("\tDatabase Name: {0}", Path.GetFileName(analysisSetupInformation.Database.LocalPath)));

                        m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseFilePath = analysisSetupInformation.Database.LocalPath;
                        m_config.Analysis.Options.MassTagDatabaseOptions.Server = analysisSetupInformation.Database.DatabaseServer;
                        m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseType = MassTagDatabaseType.ACCESS;
                        break;

                    case MassTagDatabaseFormat.SQL:
                        Logger.PrintMessage("Using Mass Tag Database:");
                        Logger.PrintMessage(string.Format("\tServer:        {0}", analysisSetupInformation.Database.DatabaseServer));
                        Logger.PrintMessage(string.Format("\tDatabase Name: {0}", analysisSetupInformation.Database.DatabaseName));
                        m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseName = analysisSetupInformation.Database.DatabaseName;
                        m_config.Analysis.Options.MassTagDatabaseOptions.Server = analysisSetupInformation.Database.DatabaseServer;
                        m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseType = MassTagDatabaseType.SQL;
                        break;
                    case MassTagDatabaseFormat.Sqlite:
                        Logger.PrintMessage("Using local Sqlite Mass Tag Database at location: ");
                        Logger.PrintMessage(string.Format("\tFull Path: {0}", analysisSetupInformation.Database.LocalPath));
                        Logger.PrintMessage(string.Format("\tDatabase Name: {0}", Path.GetFileName(analysisSetupInformation.Database.LocalPath)));

                        m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseFilePath = analysisSetupInformation.Database.LocalPath;
                        m_config.Analysis.Options.MassTagDatabaseOptions.Server = analysisSetupInformation.Database.DatabaseServer;
                        m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseType = MassTagDatabaseType.SQLite;
                        break;
                    case MassTagDatabaseFormat.MetaSample:
                        Logger.PrintMessage("Using local MetaSample Mass Tag Database at location: ");
                        Logger.PrintMessage(string.Format("\tFull Path: {0}", analysisSetupInformation.Database.LocalPath));
                        Logger.PrintMessage(string.Format("\tDatabase Name: {0}", Path.GetFileName(analysisSetupInformation.Database.LocalPath)));

                        m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseFilePath = analysisSetupInformation.Database.LocalPath;
                        m_config.Analysis.Options.MassTagDatabaseOptions.Server = analysisSetupInformation.Database.DatabaseServer;
                        m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseType = MassTagDatabaseType.MetaSample;
                        break;
                }

                // Validate the baseline
                if (analysisSetupInformation.BaselineFile == null)
                {
                    m_config.Analysis.Options.UseMassTagDBAsBaseline = true;
                    Logger.PrintMessage(string.Format("Using mass tag database {0} as the alignment baseline.", analysisSetupInformation.Database.DatabaseName));
                }
                else
                {
                    m_config.Analysis.Options.UseMassTagDBAsBaseline = false;
                    m_config.Analysis.MetaData.BaselineDataset = null;
                    foreach (DatasetInformation info in analysisMetaData.Datasets)
                    {
                        if (info.Features.Path == analysisSetupInformation.BaselineFile.Path)
                        {
                            m_config.Analysis.MetaData.BaselineDataset  = info;
                            info.IsBaseline                             = true;
                            m_config.Analysis.DataProviders.DatasetCache.Update(info);
                        }
                    }
                    Logger.PrintMessage(string.Format("Using dataset {0} as the alignment baseline.", m_config.Analysis.MetaData.BaselineDataset));
                }
            }
            else
            {
                m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseType = MassTagDatabaseType.None;
                m_config.Analysis.Options.UseMassTagDBAsBaseline = false;
                if (analysisSetupInformation.BaselineFile == null)
                {
                    Logger.PrintMessage("No baseline dataset or database was selected.");
                    return false;
                }
                else
                {
                    m_config.Analysis.MetaData.BaselineDataset = null;
                    foreach (DatasetInformation info in analysisMetaData.Datasets)
                    {
                        if (info.Features.Path == analysisSetupInformation.BaselineFile.Path)
                        {
                            info.IsBaseline = true;
                            m_config.Analysis.MetaData.BaselineDataset = info;
                            m_config.Analysis.DataProviders.DatasetCache.Update(info);
                        }
                    }
                    Logger.PrintMessage(string.Format("Using dataset {0} as the alignment baseline.", m_config.Analysis.MetaData.BaselineDataset));
                }
            }
            return true;
        }
        /// <summary>
        /// Loads factors from file or other.
        /// </summary>
        /// <param name="analysisSetupInformation"></param>
        /// <param name="datasets"></param>
        private  void ConstructFactorInformation(InputAnalysisInfo analysisSetupInformation, List<DatasetInformation> datasets, FeatureDataAccessProviders providers)
        {
            MultiAlignCore.IO.Factors.MAGEFactorAdapter mage = new MultiAlignCore.IO.Factors.MAGEFactorAdapter();

            if (analysisSetupInformation.FactorFile == null)
            {
                Logger.PrintMessage("Loading Factor Information from DMS");
                mage.LoadFactorsFromDMS(datasets, providers);
            }
            else
            {
                Logger.PrintMessage("Loading Factor Information from file: " + analysisSetupInformation.FactorFile);
                mage.LoadFactorsFromFile(analysisSetupInformation.FactorFile, datasets, providers);
            }
        }
        /// <summary>
        /// Constructs dataset infromation from the input analysis information.
        /// </summary>
        /// <param name="analysisSetupInformation"></param>
        /// <param name="analysis"></param>
        private  void ConstructDatasetInformation(InputAnalysisInfo analysisSetupInformation, MultiAlignAnalysis analysis, bool insertIntoDatabase)
        {
            // Create dataset information.
            Logger.PrintMessage("Creating dataset and other input information.");

            Dictionary<string, List<InputFile>> datasetMap = new Dictionary<string, List<InputFile>>();

            foreach (InputFile file in analysisSetupInformation.Files)
            {
                string name = Path.GetFileName(file.Path);  
                string datasetName = DatasetInformation.ExtractDatasetName(name);

                bool isEntryMade = datasetMap.ContainsKey(datasetName);
                if (!isEntryMade)
                {
                    datasetMap.Add(datasetName, new List<InputFile>());
                }

                datasetMap[datasetName].Add(file);
            }


            int i = 0;                
            foreach (string datasetName in datasetMap.Keys)
            {
                List<InputFile> files                   = datasetMap[datasetName];                
                DatasetInformation datasetInformation   = new DatasetInformation();
                datasetInformation.DatasetId            = i++;
                datasetInformation.DatasetName          = datasetName;
                                            
                foreach(InputFile file in files)                
                {
                    switch (file.FileType)
                    {
                        case InputFileType.Features:
                            datasetInformation.Features = file;
                            datasetInformation.Path     = file.Path;
                            Logger.PrintMessage("\tDataset Information:   " + file.Path);                            
                            break;
                        case InputFileType.Scans:
                            Logger.PrintMessage("\tDatabase Search Results Sequence Data Information:  " + file.Path);
                            datasetInformation.Scans = file;
                            break;
                        case InputFileType.Raw:
                            Logger.PrintMessage("\tRaw Data Information:  " + file.Path);
                            datasetInformation.Raw   = file;
                            break;
                        case InputFileType.Sequence:
                            Logger.PrintMessage("\tDatabase Search Results Sequence Data Information:  " + file.Path);
                            datasetInformation.Sequence = file;
                            break;
                    }                    
                }
                analysis.MetaData.Datasets.Add(datasetInformation);
            }
            if (insertIntoDatabase)
            {
                m_config.Analysis.DataProviders.DatasetCache.AddAll(analysis.MetaData.Datasets);
            }
        }
        /// <summary>
        /// Determine what exporting features need to be had.
        /// </summary>
        private  void ConstructExporting()
        {
            if (m_config.ExporterNames.ClusterScanPath != null)
            {
                m_config.ClusterExporters.Add(new UMCClusterScanWriter(Path.Combine(m_config.AnalysisPath, m_config.ExporterNames.ClusterScanPath)));
            }
            if (m_config.ExporterNames.CrossTabPath != null)
            {
                UMCClusterCrossTabWriter writer = new UMCClusterCrossTabWriter(Path.Combine(m_config.AnalysisPath, m_config.ExporterNames.CrossTabPath));
           
                MultiAlignCore.Algorithms.FeatureFinding.AbundanceReportingType reporting    = MultiAlignCore.Algorithms.FeatureFinding.AbundanceReportingType.Sum;
                MultiAlignCore.Algorithms.FeatureFinding.AbundanceReportingType umcAbundance = MultiAlignCore.Algorithms.FeatureFinding.AbundanceReportingType.Sum;
                if (m_config.Analysis != null)
                {                    
                    reporting    = m_config.Analysis.Options.ConsolidationOptions.AbundanceType;
                    umcAbundance = m_config.Analysis.Options.FeatureFindingOptions.UMCAbundanceReportingType;
                }
                writer.Consolidator = FeatureConsolidatorFactory.CreateConsolidator(reporting,
                                                                                    umcAbundance);
                m_config.ClusterExporters.Add(writer);
            }
            if (m_config.ExporterNames.CrossTabAbundance != null)
            {
                UMCClusterAbundanceCrossTabWriter writer = new UMCClusterAbundanceCrossTabWriter(Path.Combine(m_config.AnalysisPath,
                                                                                                 m_config.ExporterNames.CrossTabAbundance));

                MultiAlignCore.Algorithms.FeatureFinding.AbundanceReportingType reporting = MultiAlignCore.Algorithms.FeatureFinding.AbundanceReportingType.Sum;
                MultiAlignCore.Algorithms.FeatureFinding.AbundanceReportingType umcAbundance = MultiAlignCore.Algorithms.FeatureFinding.AbundanceReportingType.Sum;
                if (m_config.Analysis != null)
                {
                    reporting = m_config.Analysis.Options.ConsolidationOptions.AbundanceType;
                    umcAbundance = m_config.Analysis.Options.FeatureFindingOptions.UMCAbundanceReportingType;
                }
                writer.Consolidator = FeatureConsolidatorFactory.CreateConsolidator(reporting,
                                                                                    umcAbundance);

                m_config.ClusterExporters.Add(writer);
            }
        }
        /// <summary>
        /// Create the clustering algorithms.
        /// </summary>
        /// <param name="builder"></param>
        private  void ConstructClustering(AlgorithmBuilder builder)
        {
            // Setup algorithm providers.
            if (m_config.options.ContainsKey("-centroid"))
            {
                Logger.PrintMessage("Building centroid clusterer");
                builder.BuildClusterer(ClusteringAlgorithmType.Centroid);
            }
            else if (m_config.options.ContainsKey("-singlelinkage"))
            {

                Logger.PrintMessage("Building single linkage clusterer");
                builder.BuildClusterer(ClusteringAlgorithmType.SingleLinkage);
            }
            else
            {
                Logger.PrintMessage("Built average linkage clusterer.");
            }
        }
        #endregion

        #region Cleanup 
        public void CleanupOldAnalysisBranches(AnalysisConfig config)
        {
            switch (config.InitialStep)
            {
                case AnalysisStep.None:
                    break;
                case AnalysisStep.LoadMTDB:
                    break;
                case AnalysisStep.FindFeatures:
                    break;
                case AnalysisStep.Alignment:
                    config.Analysis.DataProviders.FeatureCache.ClearAlignmentData();
                    config.Analysis.DataProviders.ClusterCache.ClearAllClusters();
                    config.Analysis.DataProviders.MassTagMatches.ClearAllMatches();  
                    break;
                case AnalysisStep.Clustering:
                    config.Analysis.DataProviders.ClusterCache.ClearAllClusters();
                    config.Analysis.DataProviders.MassTagMatches.ClearAllMatches();  
                    break;
                case AnalysisStep.ClusterQC:
                    break;
                case AnalysisStep.PeakMatching:
                    config.Analysis.DataProviders.MassTagMatches.ClearAllMatches();                        
                    break;
                default:
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Processes the MA analysis data.
        /// </summary>
        /// <param name="args"></param>
        /// 
        public int StartMultiAlign(AnalysisConfig config, IAnalysisReportGenerator reporter)
        {
            m_reportCreator = reporter;
            m_config        = config;

            // Builds the list of algorithm providers.
            AlgorithmBuilder builder = new AlgorithmBuilder();

            // Use this to signal when the analysis is done.              
            config.triggerEvent     = new ManualResetEvent(false);
            config.errorEvent       = new ManualResetEvent(false);
            config.errorException   = null;

            /// /////////////////////////////////////////////////////////////
            /// Print Help
            /// /////////////////////////////////////////////////////////////
            // See if the user wants help
            if (config.showHelp)
            {
                PrintHelp();
                config.errorEvent.Dispose();
                config.triggerEvent.Dispose();
                return 0;
            }

            /// /////////////////////////////////////////////////////////////
            /// Validate the command line
            /// /////////////////////////////////////////////////////////////            
            AnalysisType validated = AnalysisValidator.ValidateSetup(m_config);
            if (validated == AnalysisType.InvalidParameters)
            {

                PrintHelp();
                return 0;
            }

            /// /////////////////////////////////////////////////////////////
            /// Setup log path, analysis path, and print version to log file.            
            /// /////////////////////////////////////////////////////////////                        
            SetupAnalysisEssentials();

            /// /////////////////////////////////////////////////////////////
            /// Determine if we have specified a valid database to extract
            /// data from or to re-start an analysis.
            /// /////////////////////////////////////////////////////////////    
            string databasePath = Path.Combine(config.AnalysisPath, config.AnalysisName);
            bool databaseExists = File.Exists(databasePath);
            bool createDatabase = true;

            createDatabase = ShouldCreateDatabase(validated, databaseExists);

            // make sure that we were not told to skip to a new part of the analysis.
            if (config.InitialStep >= AnalysisStep.Alignment)
            {
                createDatabase = false;
            }

            switch (validated)
            {
                case AnalysisType.FactorImporting:
                    ImportFactors(config,  databaseExists);
                    break;
                case AnalysisType.Full:
                    PerformAnalysis(config, builder, validated, createDatabase);
                    break;
                case AnalysisType.ExportDataOnly:
                    ExportData(config, builder, databasePath, databaseExists);
                    break;
                case AnalysisType.ExportSICs:
                    PerformAnalysis(config, builder, validated, createDatabase);
                    break;
            }
            return 0;
        }

        /// <summary>
        /// Determines if the database should be created or not.
        /// </summary>
        /// <param name="validated"></param>
        /// <param name="databaseExists"></param>
        /// <returns></returns>
        private static bool ShouldCreateDatabase(AnalysisType validated, bool databaseExists)
        {
            bool createDatabase = true;

            if (validated != AnalysisType.Full && validated != AnalysisType.InvalidParameters)
            {
                if (databaseExists)
                {
                    createDatabase = false;
                }
            }

            return createDatabase;
        }

        #region Processing 
        /// <summary>
        /// Performs the analysis.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="builder"></param>
        /// <param name="providers"></param>
        /// <param name="processor"></param>
        /// <param name="validated"></param>
        /// <param name="analysisSetupInformation"></param>
        /// <param name="createDatabase"></param>
        /// <returns></returns>
        private int PerformAnalysis(AnalysisConfig config, AlgorithmBuilder builder, AnalysisType validated, bool createDatabase)
        {
            InputAnalysisInfo analysisSetupInformation = null;
            FeatureDataAccessProviders  providers = null;
            MultiAlignAnalysisProcessor processor = null;

            Logger.PrintMessage("Performing analysis.");

            /// /////////////////////////////////////////////////////////////            
            /// Read the input files.
            /// /////////////////////////////////////////////////////////////                                    
            bool useMTDB = false;
            bool isInputFileOk = ReadInputDefinitionFile(out analysisSetupInformation, out useMTDB);
            if (!isInputFileOk)
                return 1;

            /// /////////////////////////////////////////////////////////////            
            /// Figure out if the factors are defined.
            /// /////////////////////////////////////////////////////////////                                    
            if (config.options.ContainsKey("-factors"))
            {
                Logger.PrintMessage("Factor file specified.");
                string factorFile = config.options["-factors"][0];
                analysisSetupInformation.FactorFile = factorFile;
            }

            /// /////////////////////////////////////////////////////////////            
            /// Creates or connects to the underlying analysis database.
            /// ///////////////////////////////////////////////////////////// 
            providers = SetupDataProviders(createDatabase);

            /// /////////////////////////////////////////////////////////////
            /// Create the clustering, analysis, and plotting paths.
            /// /////////////////////////////////////////////////////////////                                    
            ConstructClustering(builder);

            config.Analysis                 = ConstructAnalysisObject(analysisSetupInformation);
            config.Analysis.DataProviders   = providers;
            config.Analysis.AnalysisType    = validated;
            ConstructPlotPath();

            /// /////////////////////////////////////////////////////////////
            /// Read the parameter files.
            /// /////////////////////////////////////////////////////////////        
            ReadParameterFile();

            /// /////////////////////////////////////////////////////////////
            /// Construct Dataset information
            /// /////////////////////////////////////////////////////////////            
            // Construct the dataset information for export.
            ConstructDatasetInformation(analysisSetupInformation, config.Analysis, createDatabase);

            if (config.ShouldUseFactors)
            {
                ConstructFactorInformation(analysisSetupInformation, config.Analysis.MetaData.Datasets, config.Analysis.DataProviders);
            }

            bool isBaselineSpecified = ConstructBaselines(analysisSetupInformation, config.Analysis.MetaData, useMTDB);
            if (!isBaselineSpecified)
            {
                return 1;
            }

            ExportParameterFile();
            Logger.PrintSpacer();
            PrintParameters(config.Analysis, createDatabase);
            Logger.PrintSpacer();

            /// /////////////////////////////////////////////////////////////
            /// Setup the processor.
            /// /////////////////////////////////////////////////////////////            
            processor = ConstructAnalysisProcessor(builder, providers);

            // Tell the processor whether to load data or not.
            processor.ShouldLoadData = createDatabase;


            // Give the processor somewhere to put the SIC images.
            if (validated == AnalysisType.ExportSICs)
            {
                processor.AnalaysisPath = Path.Combine(config.AnalysisPath, "SICs");
                if (!Directory.Exists(processor.AnalaysisPath))
                {
                    Directory.CreateDirectory(processor.AnalaysisPath);
                }
            }

            Logger.PrintMessage("Creating exporter options.");
            if (config.ExporterNames.CrossTabPath == null)
            {
                config.ExporterNames.CrossTabPath = config.AnalysisName.Replace(".db3", "");
            }
            if (config.ExporterNames.CrossTabAbundance == null)
            {
                config.ExporterNames.CrossTabAbundance = config.AnalysisName.Replace(".db3", "");
            }
            ConstructExporting();

            Logger.PrintMessage("Cleaning up old analysis branches.");
            CleanupOldAnalysisBranches(config);


            /// /////////////////////////////////////////////////////////////
            /// Start the analysis
            /// /////////////////////////////////////////////////////////////            
            Logger.PrintMessage("Analysis Started.");
            processor.StartAnalysis(config);
            int handleID = WaitHandle.WaitAny(new WaitHandle[] { config.triggerEvent, config.errorEvent });

            if (handleID == 1)
            {
                Logger.PrintMessage("There was an error during processing.");
                return 1;
            }

            /// /////////////////////////////////////////////////////////////
            /// Finalize the analysis plots etc.
            /// /////////////////////////////////////////////////////////////
            try
            {
                m_reportCreator.CreateFinalAnalysisPlots(providers.FeatureCache, providers.ClusterCache);
                m_reportCreator.CreatePlotReport();
            }
            catch (Exception ex)
            {
                Logger.PrintMessage("There was an error when trying to create the final analysis plots, however, the data analysis is complete.");
                Logger.PrintMessage(ex.Message);
                Logger.PrintMessage(ex.StackTrace);
            }

            config.Analysis.Dispose();
            config.triggerEvent.Dispose();
            config.errorEvent.Dispose();
            processor.Dispose();
            CleanupDataProviders();
            Logger.PrintMessage("Analysis Complete.");
            return 0;
        }
        private int ExportData(AnalysisConfig config, AlgorithmBuilder builder, string databasePath, bool databaseExists)
        {
            InputAnalysisInfo analysisSetupInformation  = null;
            FeatureDataAccessProviders providers        = null;
            MultiAlignAnalysisProcessor processor       = null;

            Logger.PrintMessage("Exporting data only selected.");
            if (!databaseExists)
            {
                Logger.PrintMessage("The database you specified to extract data from does not exist.");
                return 1;
            }

            // Create access to data.
            providers = SetupDataProviders(false);
            if (providers == null)
            {
                Logger.PrintMessage("Could not create connection to database.");
                return 1;
            }

            // Find all the datasets 
            List<DatasetInformation> datasets = providers.DatasetCache.FindAll();
            if (datasets == null || datasets.Count == 0)
            {
                Logger.PrintMessage("There are no datasets present in the current database.");
                CleanupDataProviders();
                return 1;
            }

            ConstructExporting();
            ExportData(providers, databasePath, datasets);
            CleanupDataProviders();
            return 0;
        }
        private int ImportFactors(AnalysisConfig config,  bool databaseExists)
        {

            FeatureDataAccessProviders providers = null;
            MultiAlignAnalysisProcessor processor = null;

            Logger.PrintMessage("Updating factors");
            if (!databaseExists)
            {
                Logger.PrintMessage("The database you specified to extract data from does not exist.");
                return 1;
            }

            // Create access to data.
            providers = SetupDataProviders(false);
            if (providers == null)
            {
                Logger.PrintMessage("Could not create connection to database.");
                return 1;
            }

            // Find all the datasets 
            List<DatasetInformation> datasetsFactors = providers.DatasetCache.FindAll();
            if (datasetsFactors == null || datasetsFactors.Count == 0)
            {
                Logger.PrintMessage("There are no datasets present in the current database.");
                CleanupDataProviders();
                return 1;
            }

            InputAnalysisInfo info = new InputAnalysisInfo();

            if (config.options.ContainsKey("-factors"))
            {
                Logger.PrintMessage("Factor file specified.");
                string factorFile = config.options["-factors"][0];
                info.FactorFile = factorFile;
            }
            ConstructFactorInformation(info, datasetsFactors, providers);
            CleanupDataProviders();
            return 0;
        }
        #endregion
    }
}
