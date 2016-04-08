namespace MultiAlignRogue.DataLoading
{
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.DatasetLoaders;

    using MultiAlignRogue.Utils;

    /// <summary>
    /// View model for selecting loading and filtering settings for Promex datasets.
    /// </summary>
    public sealed class PromexLoaderViewModel : DatasetLoaderViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromexLoaderViewModel" /> class. 
        /// </summary>
        /// <param name="loader">The Promex dataset loader/filter model object.</param>
        public PromexLoaderViewModel(PromexFilter loader)
        {
            this.PromexLoader = loader;
            this.SupportedDatasetType = MultiAlignCore.Data.DatasetLoader.SupportedDatasetTypes.Promex;

            this.ElutionTimeRange = new ElutionTimeRangeViewModel(loader.ElutionTimeRange);

            this.ElutionLengthRange = new ElutionTimeRangeViewModel(loader.ElutionLengthRange)
            {
                DefaultElutionRangeValues =
                {
                    [ElutionUnitNames.Scans] = new ElutionTimeRange<IElutionTimePoint>(new ScanTimePoint(3), new ScanTimePoint(20)),
                    [ElutionUnitNames.Minutes] = new ElutionTimeRange<IElutionTimePoint>(new ElutionTimePoint(0.06), new ElutionTimePoint(20)),
                    [ElutionUnitNames.Net] = new ElutionTimeRange<IElutionTimePoint>(new NetTimePoint(0.01), new NetTimePoint(0.1))
                }
            };
        }

        /// <summary>
        /// Gets or sets the dataset loader.
        /// </summary>
        private PromexFilter PromexLoader
        {
            get { return this.DatasetLoader as PromexFilter; }
            set { this.DatasetLoader = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether features should be discarded if their
        /// normalized elution range falls outside of <see cref="ElutionTimeRange" />.
        /// </summary>
        public bool UseTimeRangeFilter
        {
            get { return this.PromexLoader.UseTimeRangeFilter; }
            set
            {
                if (this.PromexLoader.UseTimeRangeFilter != value)
                {
                    this.PromexLoader.UseTimeRangeFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model for selecting the minimum and maximum elution time to retain.
        /// </summary>
        public ElutionTimeRangeViewModel ElutionTimeRange { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether features should be discarded if their
        /// normalized elution length falls outside of <see cref="ElutionLengthRange" />.
        /// </summary>
        public bool UseNetLengthFilter
        {
            get { return this.PromexLoader.UseNetLengthFilter; }
            set
            {
                if (this.PromexLoader.UseNetLengthFilter != value)
                {
                    this.PromexLoader.UseNetLengthFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model for selecting the minimum and maximum lengths of features to retain.
        /// </summary>
        public ElutionTimeRangeViewModel ElutionLengthRange { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether features should be discarded if their
        /// likelihood ratio is below <see cref="MinLikelihoodRatio" />.
        /// </summary>
        public bool UseLikelihoodRatioFilter
        {
            get { return this.PromexLoader.UseLikelihoodRatioFilter; }
            set
            {
                if (this.UseLikelihoodRatioFilter != value)
                {
                    this.UseLikelihoodRatioFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum feature likelihood ratio to retain.
        /// </summary>
        public double MinLikelihoodRatio
        {
            get { return this.PromexLoader.MinLikelihoodRatio; }
            set
            {
                if (!this.PromexLoader.MinLikelihoodRatio.Equals(value))
                {
                    this.PromexLoader.MinLikelihoodRatio = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether features should be discarded if their
        /// charge range falls outside of <see cref="MinChargeState" /> and <see cref="MaxChargeState" />.
        /// </summary>
        public bool UseChargeStateFilter
        {
            get { return this.PromexLoader.UseChargeStateFilter; }
            set
            {
                if (this.PromexLoader.UseChargeStateFilter != value)
                {
                    this.PromexLoader.UseChargeStateFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the smallest charge state to retain.
        /// </summary>
        public int MinChargeState
        {
            get { return this.PromexLoader.MinChargeState; }
            set
            {
                if (this.PromexLoader.MinChargeState != value)
                {
                    this.PromexLoader.MinChargeState = value;
                    this.RaisePropertyChanged();
                }  
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the largest charge state to retain.
        /// </summary>
        public int MaxChargeState
        {
            get { return this.PromexLoader.MaxChargeState; }
            set
            {
                if (this.PromexLoader.MaxChargeState != value)
                {
                    this.PromexLoader.MaxChargeState = value;
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
            get { return this.PromexLoader.UseAbundanceFilter; }
            set
            {
                if (this.PromexLoader.UseAbundanceFilter != value)
                {
                    this.PromexLoader.UseAbundanceFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum abundance value.
        /// </summary>
        public double MinimumAbundance
        {
            get { return this.PromexLoader.MinimumAbundance; }
            set
            {
                if (!this.PromexLoader.MinimumAbundance.Equals(value))
                {
                    this.PromexLoader.MinimumAbundance = value;
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
            get { return this.PromexLoader.MaximumAbundance; }
            set
            {
                if (!this.PromexLoader.MaximumAbundance.Equals(value))
                {
                    this.PromexLoader.MaximumAbundance = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
