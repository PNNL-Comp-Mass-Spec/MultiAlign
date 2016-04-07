#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;

#endregion

namespace MultiAlignCore.Data
{
    using System.Linq;

    using MultiAlignCore.IO.DatasetLoaders;

    public class MultiAlignAnalysis : IDisposable
    {
        #region Constructor

        /// <summary>
        ///     Default constructor for a MultiAlign analysis object.
        /// </summary>
        public MultiAlignAnalysis()
        {
            // Meta Data Information about the analysis and datasets.
            MetaData = new AnalysisMetaData {AnalysisName = string.Empty};
            Options = new MultiAlignAnalysisOptions();
            MassTagDatabase = new MassTagDatabase();

            // Alignment options and data.
            AlignmentData = new List<AlignmentData>();
            MatchResults = null;
        }

        #endregion

        /// <summary>
        ///     Dispose method that will kill the analysis thread.
        /// </summary>
        public void Dispose()
        {
            MetaData.Datasets.Clear();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the name of the analysis.
        /// </summary>
        public string AnalysisName { get; set; }

        /// <summary>
        /// The path to the analysis file.
        /// </summary>
        public string AnalysisPath { get; set; }

        /// <summary>
        ///     Objects that access data from the databases.
        /// </summary>
        public FeatureDataAccessProviders DataProviders { get; set; }

        /// <summary>
        ///     Gets or sets the list of data providers.
        /// </summary>
        public List<UMCClusterLight> Clusters { get; set; }

        /// <summary>
        ///     Gets or sets what kind of analysis to do.
        /// </summary>
        public AnalysisType AnalysisType { get; set; }

        /// <summary>
        ///     Gets or sets the analysis options.
        /// </summary>
        public MultiAlignAnalysisOptions Options { get; set; }

        /// <summary>
        ///     Gets or sets the cluster alignment data.
        /// </summary>
        public AlignmentData ClusterAlignmentData { get; set; }

        /// <summary>
        ///     Gets or sets the alignment data.
        /// </summary>
        public List<AlignmentData> AlignmentData { get; set; }

        /// <summary>
        ///     Gets or sets the analysis meta-data.
        /// </summary>
        public AnalysisMetaData MetaData { get; set; }

        /// <summary>
        ///     Gets the peak matching results
        /// </summary>
        public PeakMatchingResults<UMCClusterLight, MassTagLight> MatchResults { get; set; }

        /// <summary>
        ///     Gets or sets the mass tag database.
        /// </summary>
        public MassTagDatabase MassTagDatabase { get; set; }

        public IList<IDatasetLoader> GetDatasetLoaders(IEnumerable<DatasetInformation> datasets)
        {
            var datasetLoaders = new HashSet<IDatasetLoader>();
            foreach (var dataset in datasets)
            {
                var datasetLoader = this.GetDatasetLoader(dataset);
                if (!datasetLoaders.Contains(datasetLoader))
                {
                    datasetLoaders.Add(datasetLoader);
                }
            }

            return datasetLoaders.ToList();
        }

        public IDatasetLoader GetDatasetLoader(DatasetInformation dataset)
        {
            var path = dataset.FeaturePath;
            IDatasetLoader loader;
            if (path.EndsWith(".ms1ft"))
            {
                loader = this.Options.DatasetLoaders.FirstOrDefault(l => l  is PromexFilter) ??
                         new PromexFilter(this.DataProviders);
            }
            else if (path.EndsWith("_LCMSFeatures.txt"))
            {
                loader = this.Options.DatasetLoaders.FirstOrDefault(l => l is DeconToolsLoader) ??
                         new LcImsFeatureFilter(this.DataProviders);
            }
            else
            {
                loader = this.Options.DatasetLoaders.FirstOrDefault(l => l is DeconToolsLoader) ??
                         new DeconToolsLoader(this.DataProviders);
            }

            if (!this.Options.DatasetLoaders.Contains(loader))
            {
                this.Options.DatasetLoaders.Add(loader);
            }

            return loader;
        }

        #endregion
    }
}