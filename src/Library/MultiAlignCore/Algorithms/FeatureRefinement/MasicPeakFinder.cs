namespace MultiAlignCore.Algorithms.FeatureRefinement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using InformedProteomics.Backend.Utils;

    using MagnitudeConcavityPeakFinder;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;

    /// <summary>
    /// Peak finder that uses the Magnitude Concavity peak finding algorithm from MASIC.
    /// </summary>
    public class MasicPeakFinder : ISettingsContainer
    {
        /// <summary>
        /// Initializes new instance of the <see cref="MasicPeakFinder" /> class.
        /// </summary>
        public MasicPeakFinder()
        {
            this.RestoreDefaults();
        }

        /// <summary>
        /// Gets or sets the minimum intensity allowed to be considered a peak.
        /// </summary>
        /// <remarks>Default: 0</remarks>
        public double AbsoluteMinimumIntensityThreshold { get; set; }

        /// <summary>
        /// Gets or sets the minimum intensity relative to the largest data point to
        /// be considered a peak.
        /// </summary>
        /// <remarks>Default: 0.01</remarks>
        public double RelativeIntensityThreshold { get; set; }

        /// <summary>
        /// Gets or sets the maximum distance that the edge of an identified peak can
        /// be away from the scan number that the parent ion was observed in if the identified
        /// peak does not contain the parent ion.
        /// </summary>
        /// <remarks>Default: 0</remarks>
        public int MaxScansWithNoOverlap { get; set; }

        /// <summary>
        /// Gets or sets the maximum fraction of the peak maximum that an upward spike can be
        /// to be included in the peak.
        /// </summary>
        /// <remarks>Default: 0.20 which means the maximum allowable spike is 20% of the peak maximum</remarks>
        public double MaxAllowedUpwardSpikeFraction { get; set; }

        /// <summary>
        /// Restore settings back to their default values.
        /// </summary>
        public void RestoreDefaults()
        {
            this.AbsoluteMinimumIntensityThreshold = 0;
            this.RelativeIntensityThreshold = 0.01;
            this.MaxScansWithNoOverlap = 0;
            this.MaxAllowedUpwardSpikeFraction = 0.2;
        }

        /// <summary>
        /// Run peak finder on a collection of features. Each feature may contain 0 to many peaks.
        /// </summary>
        /// <param name="features">The raw features to run the peak finder on.</param>
        /// <param name="scanSummaryProvider">Scan summary provider for mapping scans to NET.</param>
        /// <param name="progress">The progress reporter.</param>
        /// <returns>A list of LCMS features where each feature represents a single LC peak.</returns>
        public List<UMCLight> FindPeaks(List<UMCLight> features, IScanSummaryProvider scanSummaryProvider, IProgress<ProgressData> progress = null)
        {
            progress = progress ?? new Progress<ProgressData>();
            var progressData = new ProgressData();
            var peaksFound = new List<UMCLight>();

            // Run peak finder on each existing LCMS feature.
            for (int index = 0; index < features.Count; index++)
            {
                peaksFound.AddRange(this.FindPeaks(features[index], scanSummaryProvider));
                progress.Report(progressData.UpdatePercent((100.0 * index) / features.Count));
            }

            return peaksFound;
        }

        /// <summary>
        /// Find peaks within a single feature. Feature may contain 0 to many peaks.
        /// </summary>
        /// <param name="feature">The feature to find peaks in.</param>
        /// <param name="scanSummaryProvider">Scan summary provider for mapping scans to NET.</param>
        /// <returns>List of peaks found within the feature as new LCMS features.</returns>
        public List<UMCLight> FindPeaks(UMCLight feature, IScanSummaryProvider scanSummaryProvider)
        {
            // Set up peak finder.
            var peakFinder = new PeakDetector();

            // Initialize MASIC peak finder options
            var options = new PeakDetector.udtSICPeakFinderOptionsType
            {
                UseButterworthSmooth = false,
                UseSavitzkyGolaySmooth = false,
                FindPeaksOnSmoothedData = false,
            };

            // Split out x and y values for peak finder
            var scans = feature.MsFeatures.Select(msFeature => msFeature.Scan).ToArray();
            var intensities = feature.MsFeatures.Select(msFeature => msFeature.Abundance).ToArray();
            List<double> xyData;

            // Run peak finder on feature
            var peaks = peakFinder.FindPeaks(options, scans, intensities, feature.Scan, out xyData)
                                  .Where(peak => peak.IsValid);

            // Make each peak a new LCMS feature
            var newFeatures = new List<UMCLight>();
            foreach (var peak in peaks)
            {
                var newFeature = new UMCLight();

                // Repackage MS feaures.
                for (int i = peak.LeftEdge; i <= peak.RightEdge; i++)
                {
                    var scan = scans[i];
                    var net = scanSummaryProvider.GetScanSummary(scan).Net;
                    var intensity = xyData[i];
                    newFeature.AddChildFeature(new MSFeatureLight
                    {
                        Scan = scan,
                        Net = net,
                        NetAligned = net,
                        Abundance = intensity
                    });
                }

                // Create new UMC
                newFeature.CalculateStatistics();
                newFeatures.Add(newFeature);
            }

            return newFeatures;
        } 
    }
}
