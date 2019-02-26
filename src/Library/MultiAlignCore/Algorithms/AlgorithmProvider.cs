#region

using System;
using System.Collections.Generic;
using FeatureAlignment.Algorithms;
using FeatureAlignment.Algorithms.Alignment;
using FeatureAlignment.Data.Alignment;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    /// Class that holds the algorithms to use.
    /// </summary>
    public class AlgorithmProvider : IProgressNotifer
    {
        /// <summary>
        /// Fired when a status message needs to be logged.
        /// </summary>
        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        /// Gets or sets the clustering algorithm used.
        /// </summary>
        public Clustering.IClusterer<UMCLight, UMCClusterLight> Clusterer { get; set; }

        /// <summary>
        /// Gets or sets the feature/database aligner.
        /// </summary>
        public IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData> DatasetAligner { get;
            set; }

        /// <summary>
        /// Gets or sets the database aligner.
        /// </summary>
        public IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, AlignmentData> DatabaseAligner { get; set; }

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
        private void RegisterEvents(params IProgressNotifer[] providers)
        {
            foreach (var provider in providers)
            {
                if (provider != null)
                {
                    provider.Progress += provider_Progress;
                }
            }
        }

        private void provider_Progress(object sender, ProgressNotifierArgs e)
        {
            Progress?.Invoke(sender, e);
        }
    }
}