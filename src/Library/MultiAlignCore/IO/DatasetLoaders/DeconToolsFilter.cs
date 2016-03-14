namespace MultiAlignCore.IO.DatasetLoaders
{
    using System.Collections.Generic;
    using System.Linq;

    using MultiAlignCore.Data.Features;

    /// <summary>
    /// This class filters DeconTools features.
    /// </summary>
    public class DeconToolsFilter : IFeatureFilter<MSFeatureLight>
    {
        /// <summary>
        /// The maximum isotopic fit value to allow.
        /// </summary>
        private double maximumIsotopicFit;

        /// <summary>
        /// The minimum abundance value.
        /// </summary>
        private double abundanceMinimum;

        /// <summary>
        /// The maximum abundance value.
        /// </summary>
        private double abundanceMaximum;

        /// <summary>
        /// The maximum number of data points to load (lower abundance data is discarded).
        /// </summary>
        private int maximumDataPoints;

        /// <summary>
        /// The minimum scan number to retain.
        /// </summary>
        private int lcScanStart;

        /// <summary>
        /// The maximum scan number to retain.
        /// </summary>
        private int lcScanEnd;

        /// <summary>
        /// The lowest possible charge state for MSFeatures.
        /// </summary>
        private int minChargeState;

        /// <summary>
        /// The highest possible charge state for MSFeatures.
        /// </summary>
        private int maxChargeState;

        /// <summary>
        /// The smallest possible M/Z for MSFeatures.
        /// </summary>
        private double minMz;

        /// <summary>
        /// The largest possible M/Z for MSFeatures.
        /// </summary>
        private double maxMz;

        /// <summary>
        /// The number of datapoints encountered.
        /// </summary>
        private int dataPointCount;

        /// <summary>
        /// Gets or sets the maximum isotopic fit value to allow.
        /// </summary>
        /// <remarks>If 0 or negative, isotopic fit filtering is not applied</remarks>
        public double MaximumIsotopicFit
        {
            get
            {
                return this.maximumIsotopicFit;
            }
            set
            {
                this.maximumIsotopicFit = value;
                this.UpdateFilters();
            }
        }

        /// <summary>
        /// Gets or sets the minimum abundance value.
        /// </summary>
        public double AbundanceMinimum
        {
            get
            {
                return this.abundanceMinimum;
            }
            set
            {
                this.abundanceMinimum = value;
                this.UpdateFilters();
            }

        }

        /// <summary>
        /// Gets or sets the maximum abundance value.
        /// </summary>
        /// <remarks>If 0 or negative, abundance filtering is not applied.  Filtering is also skipped if AbundanceMinimum > AbundanceMaximum</remarks>
        public double AbundanceMaximum
        {
            get
            {
                return this.abundanceMaximum;
            }
            set
            {
                this.abundanceMaximum = value;
                this.UpdateFilters();
            }

        }

        /// <summary>
        /// Gets or sets the maximum number of data points to load (lower abundance data is discarded)
        /// </summary>
        public int MaximumDataPoints
        {
            get
            {
                return this.maximumDataPoints;
            }
            set
            {
                this.maximumDataPoints = value;
                this.UpdateFilters();
            }
        }

        /// <summary>
        /// Gets or sets the minimum scan number to retain.
        /// </summary>
        public int LcScanStart
        {
            get
            {
                return this.lcScanStart;
            }
            set
            {
                this.lcScanStart = value;
                this.UpdateFilters();
            }
        }

        /// <summary>
        /// Gets or sets the maximum scan number to retain.
        /// </summary>
        /// <remarks>If 0 or negative, scan filtering is not applied.  Filtering is also skipped if scanStart > ScanEnd</remarks>
        public int LcScanEnd
        {
            get
            {
                return this.lcScanEnd;
            }
            set
            {
                this.lcScanEnd = value;
                this.UpdateFilters();
            }
        }

        /// <summary>
        /// Gets or sets the lowest possible charge state for MSFeatures.
        /// </summary>
        public int MinChargeState
        {
            get { return this.minChargeState; }
            set
            {
                this.minChargeState = value;
                this.UpdateFilters();
            }
        }

        /// <summary>
        /// Gets or sets the highest possible charge state for MSFeatures.
        /// </summary>
        public int MaxChargeState
        {
            get { return this.maxChargeState; }
            set
            {
                this.maxChargeState = value;
                this.UpdateFilters();
            }
        }

        /// <summary>
        /// Gets or sets the smallest possible M/Z for MSFeatures.
        /// </summary>
        public double MinMz
        {
            get { return this.minMz; }
            set
            {
                this.minMz = value;
                this.UpdateFilters();
            }

        }

        /// <summary>
        /// Gets or sets the largest possible M/Z for MSFeatures.
        /// </summary>
        public double MaxMz
        {
            get { return this.maxMz; }
            set
            {
                this.maxMz = value;
                this.UpdateFilters();
            }
        }

        /// <summary>
        /// Gets or sets the number of data points in the smallest possible LCMS feature.
        /// </summary>
        public int MinimumDataPointsPerLcmsFeature { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MSFeatures should be filtered to contain
        /// only those with charge states between <see cref="MinChargeState" /> and <see cref="MaxChargeState" />.
        /// </summary>
        public bool UseChargestateFilter { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the MSFeatures should be filtered to contain
        /// only those with M/Zs between <see cref="MinMz" /> and <see cref="MaxMz" />.
        /// </summary>
        public bool UseMzFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the features should be filtered to contain only those between <see cref="LcScanEnd" />
        /// and <see cref="LcScanStart" />.
        /// </summary>
        /// <remarks>True when the scan filter should be used because ScanStart is less than or equal to ScanEnd and ScanEnd is greater than 0.</remarks>
        public bool UseScanFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the features should be filtered by abundance.
        /// </summary>
        /// <remarks>True when the abundance filter should be used because AbundanceMinimum is less than AbundanceMaximum and AbundanceMaximum is greater than 0.</remarks>
        public bool UseAbundanceFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the feature should be filtered by their isotopic fit score.
        /// </summary>
        /// <remarks>True when the isotopic fit filter should be used because MaximumIsotopicFit is greater than 0.</remarks>
        public bool UseIsotopicFitFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a maximum number of points should be kept.
        /// </summary>
        /// <remarks>True when the data count filter should be used because MaximumDataPoints is greater than 0.</remarks>
        public bool UseDataCountFilter { get; set; }

        /// <summary>
        /// Filters an entire list of MSFeatures, retaining only those which pass through all filters.
        /// </summary>
        /// <param name="features">The features to filter.</param>
        /// <returns>The filtered features.</returns>
        public List<MSFeatureLight> FilterFeatures(List<MSFeatureLight> features)
        {
            return features.Where(this.ShouldKeepFeature).ToList();
        }

        /// <summary>
        /// Filters an entire list of LCMSFeatures, retaining only those which pass through all filters.
        /// </summary>
        /// <param name="features">The features to filter.</param>
        /// <returns>The filtered features.</returns>
        public List<UMCLight> FilterLcmsFeature(List<UMCLight> features)
        {
            return features.Where(feature => feature.Features.Count >= this.MinimumDataPointsPerLcmsFeature).ToList();
        } 

        /// <summary>
        /// Determines whether a given feature passes all filters and should be retained.
        /// </summary>
        /// <param name="feature">The feature to test.</param>
        /// <returns>A value indicating whether the feature should be retained or discarded.</returns>
        public bool ShouldKeepFeature(MSFeatureLight feature)
        {
            bool keepFeature = true;
            keepFeature &= !this.UseScanFilter || (feature.Scan >= this.LcScanEnd && feature.Scan <= this.LcScanEnd);
            keepFeature &= !this.UseAbundanceFilter
                           || (feature.Abundance >= this.AbundanceMinimum && feature.Abundance <= this.AbundanceMaximum);
            keepFeature &= !this.UseIsotopicFitFilter && feature.Score <= this.MaximumIsotopicFit;
            keepFeature &= !this.UseDataCountFilter || this.dataPointCount++ <= this.MaximumDataPoints;

            return keepFeature;
        }

        /// <summary>
        /// Enables or disables the scan/abundance/fit/data count filters if they do not have valid values.
        /// </summary>
        private void UpdateFilters()
        {
            this.UseChargestateFilter = (this.MinChargeState <= this.MaxChargeState && this.MinChargeState > 0);

            this.UseMzFilter = (this.MinMz <= this.MaxMz && this.MaxMz > 0);

            this.UseScanFilter = (this.LcScanStart <= this.LcScanEnd && this.LcScanEnd > 0);

            this.UseAbundanceFilter = (this.AbundanceMinimum < this.AbundanceMaximum && this.AbundanceMaximum > 0);

            this.UseIsotopicFitFilter = (this.MaximumIsotopicFit > 0);

            this.UseDataCountFilter = (this.MaximumDataPoints > 0);
        }
    }
}
