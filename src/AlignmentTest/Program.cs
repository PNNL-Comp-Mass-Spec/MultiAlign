using System;
using System.IO;
using System.Collections.Generic;

using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms.Alignment;

using MultiAlignCore.Data;
using MultiAlignCore.Algorithms;
using MultiAlignCore.IO;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.Features;

namespace AlignmentTest
{
    class Program
    {

        /// <summary>
        /// Load the data from the dataset information objects to the cache at the analysis Path
        /// </summary>
        /// <param name="datasets">Datasets to load.</param>
        /// <param name="options">Options to use for UMC finding if required.</param>
        /// <param name="analysisPath">Path to save data to.</param>
        private static void LoadDatasetData(List<DatasetInformation> datasets,
                                     UMCFeatureFinderOptions options,
                                     string analysisPath)
        {
            IUmcDAO featureCache                        = m_analysis.DataProviders.FeatureCache;
            IMSFeatureDAO msFeatureCache                = m_analysis.DataProviders.MSFeatureCache;
            IGenericDAO<MSFeatureToLCMSFeatureMap> map  = m_analysis.DataProviders.MSFeatureToLCMSFeatureCache;

            foreach (DatasetInformation dataset in datasets)
            {
                UpdateStatus("Loading dataset " + dataset.DatasetName + ".");
                List<clsUMC> features = UMCLoaderFactory.LoadData(dataset,
                                                                    featureCache,
                                                                    msFeatureCache,
                                                                    map,
                                                                    options);

                UpdateStatus(string.Format("Filtering {0} features found.", features.Count));
                int preFiltered = features.Count;
                features = MultiAlignCore.Data.Features.LCMSFeatureFilters.FilterFeatures(features, m_analysis.Options.FeatureFilterOptions);
                UpdateStatus(string.Format("Filtered features from: {0} to {1}.", preFiltered, features.Count));
                UpdateStatus("Adding features to cache database.");
                featureCache.AddAll(features);
                if (FeaturesLoaded != null)
                {
                    FeaturesLoadedEventArgs args = new FeaturesLoadedEventArgs(dataset, features);
                    FeaturesLoaded(this, args);
                }
                features.Clear();
                features = null;
            }
        }  
        private static void ConstructDatasetInformation(InputAnalysisInfo analysisSetupInformation, MultiAlignAnalysis analysis)
        {
            // Create dataset information.
            int i = 0;
            
            foreach (InputFile file in analysisSetupInformation.Files)
            {
                switch (file.FileType)
                {
                    case InputFileType.Features:
                        DatasetInformation datasetInfo = new DatasetInformation();
                        datasetInfo.Path = file.Path;
                        datasetInfo.DatasetId = i++;
                        datasetInfo.DatasetName = Path.GetFileName(file.Path);
                        datasetInfo.DatasetName = datasetInfo.DatasetName.Replace("_isos.csv", "");
                        datasetInfo.DatasetName = datasetInfo.DatasetName.Replace(".pek", "");
                        datasetInfo.DatasetName = datasetInfo.DatasetName.Replace("_lcmsfeatures.txt", "");
                        datasetInfo.JobId = "";
                        datasetInfo.mstrResultsFolder = Path.GetDirectoryName(file.Path);
                        datasetInfo.ParameterFileName = "";
                        datasetInfo.Selected = true;
                        analysis.MetaData.Datasets.Add(datasetInfo);
                        break;
                    case InputFileType.Scans:
                        analysis.MetaData.OtherFiles.Add(file);
                        break;
                    case InputFileType.Raw:
                        analysis.MetaData.OtherFiles.Add(file);
                        break;
                }
            }            
        }


        static void Main(string[] args)
        {
            ThermoRawDataFileReader reader  = new ThermoRawDataFileReader();            
            IMSMSDataSource msms            = reader;
            MSMSAligner aligner             = new MSMSAligner();

            // Read the input file.
            InputAnalysisInfo info          = MultiAlignFileInputReader.ReadInputFile(args[0]);
            MultiAlignAnalysis analysis     = new MultiAlignAnalysis();
            
        }
    }
}
