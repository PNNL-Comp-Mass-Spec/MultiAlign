namespace MultiAlignCore.Algorithms.FeatureRefinement
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Algorithms.Chromatograms;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.Features;
    using MultiAlignCore.IO.RawData;

    /// <summary>
    /// Runs several different post-processors on the features in attempt to
    /// guarantee that each LCMS feature is a single deisotoped LCMS peak.
    /// </summary>
    public class FeatureRefiner : ISettingsContainer
    {
        /// <summary>
        /// Gets or sets a value indicating whether extracted ion chromatograms should
        /// be extracted from the raw data for each feature.
        /// </summary>
        public bool ShouldCreateXics { get; set; }

        /// <summary>
        /// Gets or sets the XIC creation settings.
        /// </summary>
        public XicCreator XicCreator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether deisotoping correction clustering should be
        /// performed on the LCMS features.
        /// </summary>
        public bool ShouldRunDeisotopingCorrection { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DeisotopingCorrector" /> to be used on the LCMS features.
        /// </summary>
        public DeisotopingCorrector DeiosotopingCorrector { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether peak detecting/splitting should be run on the LCMS features.
        /// </summary>
        public bool ShouldRunPeakFinding { get; set; }

        /// <summary>
        /// Gets or sets the peak finder to use for detecting and splitting peaks into separate LCMS features.
        /// </summary>
        public MasicPeakFinder PeakFinder { get; set; }

        /// <summary>
        /// Run the feature refinement process on a set of features given a DAO.
        /// </summary>
        /// <param name="umcDataAccessProvider">Provider for the features to run on.</param>
        /// <param name="scanSummaryProvider">Scan summary provider for the dataset.</param>
        /// <param name="progress">The progress of the feature refinement process.</param>
        /// <returns>The refined features.</returns>
        public void Run(
                        IUmcDAO umcDataAccessProvider,
                        IScanSummaryProvider scanSummaryProvider,
                        IProgress<ProgressData> progress = null)
        {
            // Get features
            var features = umcDataAccessProvider.FindByDatasetId(scanSummaryProvider.GroupId);

            // Run feature refinement
            var refinedFeatures = this.Run(features, scanSummaryProvider, progress);

            // Persist features
            umcDataAccessProvider.SaveFeaturesByDataset(refinedFeatures, scanSummaryProvider.GroupId, progress);
        }

        /// <summary>
        /// Run the feature refinement process on a set of features.
        /// </summary>
        /// <param name="features">The features to run on.</param>
        /// <param name="progress">The progress of the feature refinement process.</param>
        /// <param name="scanSummaryProvider">Scan summary provider for the dataset.</param>
        /// <returns>The refined features.</returns>
        public List<UMCLight> Run(
                                  List<UMCLight> features,
                                  IScanSummaryProvider scanSummaryProvider,
                                  IProgress<ProgressData> progress = null)
        {
            progress = progress ?? new Progress<ProgressData>();

            // 1. Find XICs
            if (this.ShouldCreateXics)
            {
                this.XicCreator.CreateXic(features, scanSummaryProvider as InformedProteomicsReader, progress);
            }

            // 2. Run peak finding
            if (this.ShouldRunPeakFinding)
            {
                features = this.PeakFinder.FindPeaks(features, scanSummaryProvider, progress);
            }

            // 3. Run deisotoping correction
            if (this.ShouldRunDeisotopingCorrection)
            {
                features = this.DeiosotopingCorrector.Run(features, progress);
            }

            return features;
        }

        /// <summary>
        /// Restore settings back to their default values.
        /// </summary>
        public void RestoreDefaults()
        {
            this.ShouldCreateXics = true;
            this.ShouldCreateXics = true;
            this.XicCreator = new XicCreator();
            this.ShouldRunDeisotopingCorrection = true;
            this.DeiosotopingCorrector = new DeisotopingCorrector();
        }
    }
}
