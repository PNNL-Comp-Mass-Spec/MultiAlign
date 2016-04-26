namespace MultiAlignRogue.DataLoading
{
    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.DatasetLoaders;

    using MultiAlignRogue.Clustering;
    using MultiAlignRogue.Utils;

    using NHibernate.Util;

    /// <summary>
    /// View model for selecting loading and filtering settings for DeconTools datasets.
    /// </summary>
    public sealed class DeconToolsLoaderViewModel : DatasetLoaderViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeconToolsLoaderViewModel" /> class. 
        /// </summary>
        /// <param name="loader">The Promex dataset loader/filter model object.</param>
        public DeconToolsLoaderViewModel(DeconToolsLoader loader) : base(loader)
        {
            this.DeconToolsLoader = loader;
            this.ElutionTimeRange = new ElutionTimeRangeViewModel(this.DeconToolsLoader.ElutionTimeRange);
            this.ElutionLengthRange = new ElutionTimeRangeViewModel(this.DeconToolsLoader.ElutionLengthRange);

            this.ElutionLengthRange = new ElutionTimeRangeViewModel(loader.ElutionLengthRange)
            {
                DefaultElutionRangeValues =
                {
                    [ElutionUnitNames.Scans] = new ElutionTimeRange<IElutionTimePoint>(new ScanTimePoint(3), new ScanTimePoint(20)),
                    [ElutionUnitNames.Minutes] = new ElutionTimeRange<IElutionTimePoint>(new ElutionTimePoint(0.06), new ElutionTimePoint(20)),
                    [ElutionUnitNames.Net] = new ElutionTimeRange<IElutionTimePoint>(new NetTimePoint(0.01), new NetTimePoint(0.1))
                }
            };

            // Set up clustering settings
            this.ClustererSettings = new ClusterAlgorithmSettingsViewModel(this.DeconToolsLoader.ClusteringOptions);
            this.ClustererSettings.ClusteringAlgorithmTypes.Clear();
            ClusteringAlgorithms.MsFeatureClusteringAlgorithms.ForEach(
                                algorithm => this.ClustererSettings.ClusteringAlgorithmTypes.Add(algorithm));
        }

        /// <summary>
        /// Gets or sets the dataset loader.
        /// </summary>
        private DeconToolsLoader DeconToolsLoader
        {
            get { return this.DatasetLoader as DeconToolsLoader; }
            set { this.DatasetLoader = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the feature should be filtered by their isotopic fit score.
        /// </summary>
        /// <remarks>True when the isotopic fit filter should be used because MaximumIsotopicFit is greater than 0.</remarks>
        public bool UseIsotopicFitFilter
        {
            get { return this.DeconToolsLoader.Filter.UseIsotopicFitFilter; }
            set
            {
                if (this.DeconToolsLoader.Filter.UseIsotopicFitFilter != value)
                {
                    this.DeconToolsLoader.Filter.UseIsotopicFitFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum isotopic fit value to allow.
        /// </summary>
        /// <remarks>If 0 or negative, isotopic fit filtering is not applied</remarks>
        public double MaximumIsotopicFit
        {
            get { return this.DeconToolsLoader.Filter.MaximumIsotopicFit; }
            set
            {
                if (!this.DeconToolsLoader.Filter.MaximumIsotopicFit.Equals(value))
                {
                    this.DeconToolsLoader.Filter.MaximumIsotopicFit = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the features should be filtered by abundance.
        /// </summary>
        /// <remarks>True when the abundance filter should be used because AbundanceMinimum is less than AbundanceMaximum and AbundanceMaximum is greater than 0.</remarks>
        public bool UseAbundanceFilter
        {
            get { return this.DeconToolsLoader.Filter.UseAbundanceFilter; }
            set
            {
                if (this.DeconToolsLoader.Filter.UseAbundanceFilter != value)
                {
                    this.DeconToolsLoader.Filter.UseAbundanceFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum abundance value.
        /// </summary>
        public double MinimumAbundance
        {
            get { return this.DeconToolsLoader.Filter.MinimumAbundance; }
            set
            {
                if (!this.DeconToolsLoader.Filter.MinimumAbundance.Equals(value))
                {
                    this.DeconToolsLoader.Filter.MinimumAbundance = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum abundance value.
        /// </summary>
        /// <remarks>If 0 or negative, abundance filtering is not applied.  Filtering is also skipped if AbundanceMinimum > AbundanceMaximum</remarks>
        public double MaximumAbundance
        {
            get { return this.DeconToolsLoader.Filter.MaximumAbundance; }
            set
            {
                if (!this.DeconToolsLoader.Filter.MaximumAbundance.Equals(value))
                {
                    this.DeconToolsLoader.Filter.MaximumAbundance = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether features should be discarded if their
        /// normalized elution range falls outside of selected elution time range.
        /// </summary>
        public bool UseTimeRangeFilter
        {
            get { return this.DeconToolsLoader.Filter.UseScanFilter; }
            set
            {
                if (this.DeconToolsLoader.Filter.UseScanFilter != value)
                {
                    this.DeconToolsLoader.Filter.UseScanFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model for editing the elution time range.
        /// </summary>
        public ElutionTimeRangeViewModel ElutionTimeRange { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether features should be discarded if their
        /// normalized elution length falls outside of <see cref="ElutionLengthRange" />.
        /// </summary>
        public bool UseNetLengthFilter
        {
            get { return this.DeconToolsLoader.Filter.UseFeatureLengthFilter; }
            set
            {
                if (this.DeconToolsLoader.Filter.UseFeatureLengthFilter != value)
                {
                    this.DeconToolsLoader.Filter.UseFeatureLengthFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model for selecting the minimum and maximum lengths of features to retain.
        /// </summary>
        public ElutionTimeRangeViewModel ElutionLengthRange { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MSFeatures should be filtered to contain
        /// only those with charge states between <see cref="MinChargeState" /> and <see cref="MaxChargeState" />.
        /// </summary>
        public bool UseChargestateFilter
        {
            get { return this.DeconToolsLoader.Filter.UseChargestateFilter; }
            set
            {
                if (this.DeconToolsLoader.Filter.UseChargestateFilter != value)
                {
                    this.DeconToolsLoader.Filter.UseChargestateFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the lowest possible charge state for MSFeatures.
        /// </summary>
        public int MinChargeState
        {
            get { return this.DeconToolsLoader.Filter.MinChargeState; }
            set
            {
                if (this.DeconToolsLoader.Filter.MinChargeState != value)
                {
                    this.DeconToolsLoader.Filter.MinChargeState = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the highest possible charge state for MSFeatures.
        /// </summary>
        public int MaxChargeState
        {
            get { return this.DeconToolsLoader.Filter.MaxChargeState; }
            set
            {
                if (this.DeconToolsLoader.Filter.MaxChargeState != value)
                {
                    this.DeconToolsLoader.Filter.MaxChargeState = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the MSFeatures should be filtered to contain
        /// only those with M/Zs between <see cref="MinMz" /> and <see cref="MaxMz" />.
        /// </summary>
        public bool UseMzFilter
        {
            get { return this.DeconToolsLoader.Filter.UseMzFilter; }
            set
            {
                if (this.DeconToolsLoader.Filter.UseMzFilter != value)
                {
                    this.DeconToolsLoader.Filter.UseMzFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the smallest possible M/Z for MSFeatures.
        /// </summary>
        public double MinMz
        {
            get { return this.DeconToolsLoader.Filter.MinMz; }
            set
            {
                if (!this.DeconToolsLoader.Filter.MinMz.Equals(value))
                {
                    this.DeconToolsLoader.Filter.MinMz = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the largest possible M/Z for MSFeatures.
        /// </summary>
        public double MaxMz
        {
            get { return this.DeconToolsLoader.Filter.MaxMz; }
            set
            {
                if (!this.DeconToolsLoader.Filter.MaxMz.Equals(value))
                {
                    this.DeconToolsLoader.Filter.MaxMz = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of data points in the smallest possible LCMS feature.
        /// </summary>
        public int MinimumDataPointsPerLcmsFeature
        {
            get { return this.DeconToolsLoader.Filter.MinimumDataPointsPerLcmsFeature; }
            set
            {
                if (this.DeconToolsLoader.Filter.MinimumDataPointsPerLcmsFeature != value)
                {
                    this.DeconToolsLoader.Filter.MinimumDataPointsPerLcmsFeature = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a maximum number of points should be kept.
        /// </summary>
        /// <remarks>True when the data count filter should be used because MaximumDataPoints is greater than 0.</remarks>
        public bool UseDataCountFilter
        {
            get { return this.DeconToolsLoader.Filter.UseDataCountFilter; }
            set
            {
                if (this.DeconToolsLoader.Filter.UseDataCountFilter != value)
                {
                    this.DeconToolsLoader.Filter.UseDataCountFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of data points to load (lower abundance data is discarded)
        /// </summary>
        public int MaximumDataPoints
        {
            get { return this.DeconToolsLoader.Filter.MaximumDataPoints; }
            set
            {
                if (this.DeconToolsLoader.Filter.MaximumDataPoints != value)
                {
                    this.DeconToolsLoader.Filter.MaximumDataPoints = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the settings for the clustering algorithm used to cluster
        /// MS features into LCMS features.
        /// </summary>
        public ClusterAlgorithmSettingsViewModel ClustererSettings { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the RAW file should be read.
        /// </summary>
        /// <remarks>
        /// True = raw file will be read (if available). False = Scans file will be read.
        /// </remarks>
        public bool LoadRawData
        {
            get { return this.DeconToolsLoader.ShouldLoadRawData; }
            set
            {
                if (this.DeconToolsLoader.ShouldLoadRawData != value)
                {
                    this.DeconToolsLoader.ShouldLoadRawData = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether MS feature information should
        /// be persisted to the database.
        /// </summary>
        public bool ShouldPersistMsFeatures
        {
            get { return this.DeconToolsLoader.ShouldPersistMsFeatures; }
            set
            {
                if (this.DeconToolsLoader.ShouldPersistMsFeatures != value)
                {
                    this.DeconToolsLoader.ShouldPersistMsFeatures = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
