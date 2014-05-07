using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Data.Alignment;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using System;
using System.Collections.Generic;

namespace MultiAlignCore.Algorithms
{

    /// <summary>
    /// Class that holds the algorithms to use.
    /// </summary>
    public class AlgorithmProvider: IProgressNotifer
    {
        /// <summary>
        /// Fired when a status message needs to be logged.
        /// </summary> 
        public event EventHandler<ProgressNotifierArgs> Progress;
    
        /// <summary>
        /// Gets or sets the clustering algorithm used.
        /// </summary>
        public IClusterer<UMCLight, UMCClusterLight> Clusterer { get; set; }        
        /// <summary>
        /// Gets or sets the feature/database aligner.
        /// </summary>
        public IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, classAlignmentData> DatasetAligner { get; set; }
        /// <summary>
        /// Gets or sets the database aligner.
        /// </summary>
        public IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, classAlignmentData> DatabaseAligner { get; set; }
        /// <summary>
        /// Gets or sets the peak matcher object.
        /// </summary>
        public IPeakMatcher<UMCClusterLight> PeakMatcher { get; set; }
                
        /// <summary>
        /// Registers events for algorithms.
        /// </summary>
        public void RegisterEvents()
        {
            RegisterEvents(PeakMatcher, DatasetAligner, DatabaseAligner, Clusterer);
        }
        /// <summary>
        /// Registers status event handlers for each algorithm type.
        /// </summary>
        /// <param name="providers"></param>
        private void RegisterEvents(params IProgressNotifer [] providers)
        {
            foreach (var provider in providers)
            {
                if (provider != null)
                {
                    provider.Progress += provider_Progress;
                }
            }
        }

        void provider_Progress(object sender, ProgressNotifierArgs e)
        {
            if (Progress != null)
            {
                Progress(sender, e);
            }
        }                
    }
}
