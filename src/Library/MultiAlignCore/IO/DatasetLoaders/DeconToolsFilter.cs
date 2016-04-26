namespace MultiAlignCore.IO.DatasetLoaders
{
    using System.Collections.Generic;
    using System.Linq;

    using MultiAlignCore.Data.Features;

    /// <summary>
    /// This class filters DeconTools feature datasets.
    /// </summary>
    public class DeconToolsFilter : IFeatureFilter<MSFeatureLight>, IFeatureFilter<UMCLight>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeconToolsLoader" /> class. 
        /// </summary>
        public DeconToolsFilter()
        {
            this.RestoreDefaults();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the feature should be filtered by their isotopic fit score.
        /// </summary>
        /// <remarks>True when the isotopic fit filter should be used because MaximumIsotopicFit is greater than 0.</remarks>
        public bool UseIsotopicFitFilter { get; set; }

        /// <summary>
        /// Gets or sets the maximum isotopic fit value to allow.
        /// </summary>
        /// <remarks>If 0 or negative, isotopic fit filtering is not applied</remarks>
        public double MaximumIsotopicFit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the features should be filtered by abundance.
        /// </summary>
        /// <remarks>True when the abundance filter should be used because AbundanceMinimum is less than AbundanceMaximum and AbundanceMaximum is greater than 0.</remarks>
        public bool UseAbundanceFilter { get; set; }

        /// <summary>
        /// Gets or sets the minimum abundance value.
        /// </summary>
        public double MinimumAbundance { get; set; }

        /// <summary>
        /// Gets or sets the maximum abundance value.
        /// </summary>
        /// <remarks>If 0 or negative, abundance filtering is not applied.  Filtering is also skipped if AbundanceMinimum > AbundanceMaximum</remarks>
        public double MaximumAbundance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the features should be filtered to contain only those between <see cref="ScanRange" />.
        /// </summary>
        /// <remarks>True when the scan filter should be used because ScanStart is less than or equal to ScanEnd and ScanEnd is greater than 0.</remarks>
        public bool UseScanFilter { get; set; }

        /// <summary>
        /// Gets or sets the elution time range to limit features in.
        /// </summary>
        public ElutionTimeRange<ScanTimePoint> ScanRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether features should be discarded if their
        /// normalized elution length falls outside of <see cref="ElutionLengthRange" />.
        /// </summary>
        public bool UseFeatureLengthFilter { get; set; }

        /// <summary>
        /// Gets or sets the minimum and maximum lengths of features to retain.
        /// </summary>
        public ElutionTimeRange<NetTimePoint> ElutionLengthRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MSFeatures should be filtered to contain
        /// only those with charge states between <see cref="MinChargeState" /> and <see cref="MaxChargeState" />.
        /// </summary>
        public bool UseChargestateFilter { get; set; }

        /// <summary>
        /// Gets or sets the lowest possible charge state for MSFeatures.
        /// </summary>
        public int MinChargeState { get; set; }

        /// <summary>
        /// Gets or sets the highest possible charge state for MSFeatures.
        /// </summary>
        public int MaxChargeState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MSFeatures should be filtered to contain
        /// only those with M/Zs between <see cref="MinMz" /> and <see cref="MaxMz" />.
        /// </summary>
        public bool UseMzFilter { get; set; }

        /// <summary>
        /// Gets or sets the smallest possible M/Z for MSFeatures.
        /// </summary>
        public double MinMz { get; set; }

        /// <summary>
        /// Gets or sets the largest possible M/Z for MSFeatures.
        /// </summary>
        public double MaxMz { get; set; }

        /// <summary>
        /// Gets or sets the number of data points in the smallest possible LCMS feature.
        /// </summary>
        public int MinimumDataPointsPerLcmsFeature { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a maximum number of points should be kept.
        /// </summary>
        /// <remarks>True when the data count filter should be used because MaximumDataPoints is greater than 0.</remarks>
        public bool UseDataCountFilter { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of data points to load (lower abundance data is discarded)
        /// </summary>
        public int MaximumDataPoints { get; set; }

        /// <summary>
        /// Reset all settings to their default settings.
        /// </summary>
        public void RestoreDefaults()
        {
            this.UseIsotopicFitFilter = true;
            this.MaximumIsotopicFit = 0.15;

            this.UseAbundanceFilter = false;
            this.MinimumAbundance = 0;
            this.MaximumAbundance = 1e15;

            this.UseScanFilter = false;
            this.ScanRange = new ElutionTimeRange<ScanTimePoint>(new ScanTimePoint(), new ScanTimePoint(10000));

            this.UseFeatureLengthFilter = true;
            this.ElutionLengthRange = new ElutionTimeRange<NetTimePoint>(new NetTimePoint(0.01), new NetTimePoint(0.2));

            this.UseDataCountFilter = false;
            this.MaximumDataPoints = 800000;
            this.MinimumDataPointsPerLcmsFeature = 3;
        }

        /// <summary>
        /// Filters an entire list of MSFeatures, retaining only those which pass through all filters.
        /// </summary>
        /// <param name="features">The features to filter.</param>
        /// <returns>The filtered features.</returns>
        public List<MSFeatureLight> FilterFeatures(List<MSFeatureLight> features)
        {
            return features.Where(this.ShouldKeepFeature)
                           .OrderBy(feature => feature.Abundance)
                           .Take(this.MaximumDataPoints)
                           .ToList();
        }

        /// <summary>
        /// Determines whether a given feature passes all filters and should be retained.
        /// </summary>
        /// <param name="feature">The feature to test.</param>
        /// <returns>A value indicating whether the feature should be retained or discarded.</returns>
        public bool ShouldKeepFeature(MSFeatureLight feature)
        {
            bool keepFeature = true;
            keepFeature &= !this.UseScanFilter || (feature.Scan >= this.ScanRange.MinValue.Value && feature.Scan <= this.ScanRange.MaxValue.Value);
            keepFeature &= !this.UseAbundanceFilter
                           || (feature.Abundance >= this.MinimumAbundance && feature.Abundance <= this.MaximumAbundance);
            keepFeature &= !this.UseIsotopicFitFilter && feature.Score <= this.MaximumIsotopicFit;

            return keepFeature;
        }

        /// <summary>
        /// Filters an entire list of LCMSFeatures, retaining only those which pass through all filters.
        /// </summary>
        /// <param name="features">The features to filter.</param>
        /// <returns>The filtered features.</returns>
        public List<UMCLight> FilterFeatures(List<UMCLight> features)
        {
            return features.FindAll(this.ShouldKeepFeature);
        }

        /// <summary>
        /// Determines whether a given feature passes all filters and should be retained.
        /// </summary>
        /// <param name="feature">The feature to test.</param>
        /// <returns>A value indicating whether the feature should be retained or discarded.</returns>
        public bool ShouldKeepFeature(UMCLight feature)
        {
            bool keepFeature = true;
            keepFeature &= feature.Features.Count >= this.MinimumDataPointsPerLcmsFeature;

            var featureLength = feature.NetEnd - feature.NetStart;
            keepFeature &= !this.UseFeatureLengthFilter ||
                           (featureLength >= this.ElutionLengthRange.MinValue.Value &&
                            featureLength <= this.ElutionLengthRange.MaxValue.Value);

            return keepFeature;
        }
    }
}
