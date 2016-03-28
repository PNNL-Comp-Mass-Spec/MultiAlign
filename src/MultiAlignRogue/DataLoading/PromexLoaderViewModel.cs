namespace MultiAlignRogue.DataLoading
{
    using System;
    using System.Collections.Generic;

    using System.Collections.ObjectModel;

    using GalaSoft.MvvmLight.Messaging;

    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.DatasetLoaders;

    /// <summary>
    /// View model for selecting loading and filtering settings for Promex datasets.
    /// </summary>
    public class PromexLoaderViewModel : DatasetLoaderViewModelBase
    {
        /// <summary>
        /// The default time ranges to display for each type of unit.
        /// </summary>
        private readonly Dictionary<ElutionUnitNames, Tuple<double, double>> defaultElutionRangeValues;

        /// <summary>
        /// The default elution length values for each type of unit.
        /// </summary>
        private readonly Dictionary<ElutionUnitNames, Tuple<double, double>> defaultElutionLengthValues;

        /// <summary>
        /// The type of elution unit to use for the elution time range.
        /// </summary>
        private ElutionUnitNames selectedElutionTimeUnit;

        /// <summary>
        /// The type of unit for the elution length.
        /// </summary>
        private ElutionUnitNames selectedElutionLengthUnit;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromexLoaderViewModel" /> class. 
        /// </summary>
        /// <param name="loader">The Promex dataset loader/filter model object.</param>
        public PromexLoaderViewModel(PromexFilter loader)
        {
            this.PromexLoader = loader;

            this.ElutionTimeUnits = new ObservableCollection<ElutionUnitNames>
            {
                ElutionUnitNames.Net,
                ElutionUnitNames.Scans,
                ElutionUnitNames.Minutes,
            };

            // Set the default time ranges to display for each type of unit.
            this.defaultElutionRangeValues = new Dictionary<ElutionUnitNames, Tuple<double, double>>
            {
                { ElutionUnitNames.Net, new Tuple<double, double>(0.0, 1.0) },
                { ElutionUnitNames.Scans, new Tuple<double, double>(0, 10000) },
                { ElutionUnitNames.Minutes, new Tuple<double, double>(0, 90) },
            };

            // Set the default elution length values for each type of unit.
            this.defaultElutionLengthValues = new Dictionary<ElutionUnitNames, Tuple<double, double>>
            {
                { ElutionUnitNames.Net, new Tuple<double, double>(0.01, 0.2) },
                { ElutionUnitNames.Scans, new Tuple<double, double>(3, 20) },
                { ElutionUnitNames.Minutes, new Tuple<double, double>(1, 5) },
            };

            // Update the elution range values when the elution range unit changes.
            this.MessengerInstance.Register<PropertyChangedMessage<ElutionUnitNames>>(this,
                msg =>
            {
                var values = this.defaultElutionRangeValues[msg.NewValue];
                IElutionTimePoint minValue;
                IElutionTimePoint maxValue;
                switch (msg.NewValue)
                {
                    case ElutionUnitNames.Minutes:
                        minValue = new ElutionTimePoint(values.Item1);
                        maxValue = new ElutionTimePoint(values.Item2);
                        break;
                    case ElutionUnitNames.Scans:
                        minValue = new ScanTimePoint((int)values.Item1);
                        maxValue = new ScanTimePoint((int)values.Item2);
                        break;
                    default:
                        minValue = new NetTimePoint(values.Item1);
                        maxValue = new NetTimePoint(values.Item2);
                        break;
                }

                this.PromexLoader.ElutionTimeRange = new ElutionTimeRange<IElutionTimePoint>(minValue, maxValue);
            });

            // Update the elution length values when the elution length unit changes.
            this.MessengerInstance.Register<PropertyChangedMessage<ElutionUnitNames>>(this,
                msg =>
            {
                var values = this.defaultElutionLengthValues[msg.NewValue];
                this.MinElutionLength = values.Item1;
                this.MaxElutionLength = values.Item2;
            });
        }

        /// <summary>
        /// Gets or sets the dataset loader.
        /// </summary>
        private PromexFilter PromexLoader
        {
            get { return this.DatasetLoader as PromexFilter; }
            set
            {
                this.DatasetLoader = value;
            }
        }

        /// <summary>
        /// Gets the names of the possible elution time units.
        /// </summary>
        public ObservableCollection<ElutionUnitNames> ElutionTimeUnits { get; private set; }

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
        /// Gets or sets the minimum elution time of features to retain.
        /// </summary>
        public double MinElutionTime
        {
            get { return this.PromexLoader.ElutionTimeRange.MinValue.Value; }
            set
            {
                if (!this.PromexLoader.ElutionTimeRange.MinValue.Value.Equals(value))
                {
                    this.PromexLoader.ElutionTimeRange.MinValue.Value = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum elution time of features to retain.
        /// </summary>
        public double MaxElutionTime
        {
            get { return this.PromexLoader.ElutionTimeRange.MaxValue.Value; }
            set
            {
                if (!this.PromexLoader.ElutionTimeRange.MaxValue.Value.Equals(value))
                {
                    this.PromexLoader.ElutionTimeRange.MaxValue.Value = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of elution unit to use for the elution time range.
        /// </summary>
        public ElutionUnitNames SelectedElutionTimeUnit
        {
            get { return this.selectedElutionTimeUnit; }
            set
            {
                if (this.selectedElutionTimeUnit != value)
                {
                    var oldValue = this.selectedElutionTimeUnit;
                    this.selectedElutionTimeUnit = value;
                    this.RaisePropertyChanged(nameof(this.SelectedElutionTimeUnit), oldValue, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum and maximum normalized elution time to retain.
        /// </summary>
        public ElutionTimeRange<NetTimePoint> ElutionTimeRange { get; set; }

        /// <summary>
        /// Gets or sets the type of unit for the elution length.
        /// </summary>
        public ElutionUnitNames SelectedElutionLengthUnit
        {
            get { return this.selectedElutionLengthUnit; }
            set
            {
                if (this.selectedElutionLengthUnit != value)
                {
                    var oldValue = this.selectedElutionLengthUnit;
                    this.selectedElutionLengthUnit = value;
                    this.RaisePropertyChanged(nameof(this.SelectedElutionLengthUnit), oldValue, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether features should be discarded if their
        /// normalized elution length falls outside of <see cref="MinElutionLength" /> and
        /// <see cref="MaxElutionLength" />.
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
        /// Gets or sets the minimum normalized elution length of the features.
        /// </summary>
        public double MinElutionLength
        {
            get { return this.PromexLoader.MinElutionLength; }
            set
            {
                if (!this.PromexLoader.MinElutionLength.Equals(value))
                {
                    this.PromexLoader.MinElutionLength = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum normalized elution length of the features.  
        /// </summary>
        public double MaxElutionLength
        {
            get { return this.PromexLoader.MaxElutionLength; }
            set
            {
                if (!this.PromexLoader.MaxElutionLength.Equals(value))
                {
                    this.PromexLoader.MaxElutionLength = value;
                    this.RaisePropertyChanged();
                }
            }
        }

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
    }
}
