namespace MultiAlignRogue.Utils
{
    using System;
    using System.Collections.Generic;

    using System.Collections.ObjectModel;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Messaging;

    using MultiAlignCore.Data.Features;

    /// <summary>
    /// View model for an elution time range where the elution time unit can be changed.
    /// </summary>
    public class ElutionTimeRangeViewModel : ViewModelBase
    {
        /// <summary>
        /// The default time ranges to display for each type of unit.
        /// </summary>
        private readonly Dictionary<ElutionUnitNames, ElutionTimeRange<IElutionTimePoint>> defaultElutionRangeValues;

        /// <summary>
        /// The default minimum, maximum, and increment values for each elution unit.
        /// </summary>
        private readonly Dictionary<ElutionUnitNames, Tuple<double, double, double>> elutionRangeExtremaValues;

        /// <summary>
        /// The type of elution unit to use for the elution time range.
        /// </summary>
        private ElutionUnitNames selectedElutionTimeUnit;

        /// <summary>
        /// Gets the absolute minimum for the current elution time unit.
        /// </summary>
        private double elutionRangeAbsoluteMinimum;

        /// <summary>
        /// The absolute maximum for the current elution time unit.
        /// </summary>
        private double elutionRangeAbsoluteMaximum;

        /// <summary>
        /// How much the the elution time can be increased/decreased at a time.
        /// </summary>
        private double elutionRangeIncrement;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElutionTimeRangeViewModel" /> class. 
        /// </summary>
        /// <param name="timeRange">The time range model that this view model edits.</param>
        public ElutionTimeRangeViewModel(ElutionTimeRange<IElutionTimePoint> timeRange)
        {
            this.ElutionTimeRange = timeRange;

            this.ElutionTimeUnits = new ObservableCollection<ElutionUnitNames>
            {
                ElutionUnitNames.Net,
                ElutionUnitNames.Scans,
                ElutionUnitNames.Minutes,
            };

            // Set default elution time range.
            this.SelectedElutionTimeUnit = this.ElutionTimeRange.MinValue.ElutionUnitName;
            this.MinElutionTime = this.ElutionTimeRange.MinValue.Value;
            this.MaxElutionTime = this.ElutionTimeRange.MaxValue.Value;

            // Set the default time ranges to display for each type of unit.
            this.defaultElutionRangeValues = new Dictionary<ElutionUnitNames, ElutionTimeRange<IElutionTimePoint>>
            {
                { ElutionUnitNames.Net, new ElutionTimeRange<IElutionTimePoint>(new NetTimePoint(), new NetTimePoint(1.0)) },
                { ElutionUnitNames.Scans, new ElutionTimeRange<IElutionTimePoint>(new ScanTimePoint(), new ScanTimePoint(10000)) },
                { ElutionUnitNames.Minutes, new ElutionTimeRange<IElutionTimePoint>(new ElutionTimePoint(), new ElutionTimePoint(90)) },
            };

            // Default minimum, maximum, and increment value for each type of unit.
            this.elutionRangeExtremaValues = new Dictionary<ElutionUnitNames, Tuple<double, double, double>>
            {
                { ElutionUnitNames.Net, new Tuple<double, double, double>(0, 1.0, 0.01) },
                { ElutionUnitNames.Scans, new Tuple<double, double, double>(0, 100000, 1) },
                { ElutionUnitNames.Minutes, new Tuple<double, double, double>(0, 100000, 1) }
            };

            // Update the elution range values when the elution range unit changes.
            this.MessengerInstance.Register<PropertyChangedMessage<ElutionUnitNames>>(this,
                msg =>
            {
                // Set default values for the selected unit
                var elutionTimeRange = this.defaultElutionRangeValues[msg.NewValue];
                this.ElutionTimeRange.MinValue = elutionTimeRange.MinValue;
                this.ElutionTimeRange.MaxValue = elutionTimeRange.MaxValue;

                // Set extrema for the selected unit
                var elutionRangeExtrema = this.elutionRangeExtremaValues[msg.NewValue];
                this.ElutionRangeAbsoluteMinimum = elutionRangeExtrema.Item1;
                this.ElutionRangeAbsoluteMaximum = elutionRangeExtrema.Item2;
                this.ElutionRangeIncrement = elutionRangeExtrema.Item3;

                this.RaisePropertyChanged(nameof(this.MinElutionTime));
                this.RaisePropertyChanged(nameof(this.MaxElutionTime));
            });
        }

        /// <summary>
        /// Gets the model elution time range.
        /// </summary>
        public ElutionTimeRange<IElutionTimePoint> ElutionTimeRange { get; private set; }

        /// <summary>
        /// Gets the names of the possible elution time units.
        /// </summary>
        public ObservableCollection<ElutionUnitNames> ElutionTimeUnits { get; private set; }

        /// <summary>
        /// Gets or sets the minimum elution time of features to retain.
        /// </summary>
        public double MinElutionTime
        {
            get { return this.ElutionTimeRange.MinValue.Value; }
            set
            {
                if (!this.ElutionTimeRange.MinValue.Value.Equals(value))
                {
                    this.ElutionTimeRange.MinValue.Value = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum elution time of features to retain.
        /// </summary>
        public double MaxElutionTime
        {
            get { return this.ElutionTimeRange.MaxValue.Value; }
            set
            {
                if (!this.ElutionTimeRange.MaxValue.Value.Equals(value))
                {
                    this.ElutionTimeRange.MaxValue.Value = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the absolute minimum for the current elution time unit.
        /// </summary>
        public double ElutionRangeAbsoluteMinimum
        {
            get { return this.elutionRangeAbsoluteMinimum; }
            private set
            {
                if (!this.elutionRangeAbsoluteMinimum.Equals(value))
                {
                    this.elutionRangeAbsoluteMinimum = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the absolute maximum for the current elution time unit.
        /// </summary>
        public double ElutionRangeAbsoluteMaximum
        {
            get { return this.elutionRangeAbsoluteMaximum; }
            private set
            {
                if (!this.elutionRangeAbsoluteMaximum.Equals(value))
                {
                    this.elutionRangeAbsoluteMaximum = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets how much the the elution time can be increased/decreased at a time.
        /// </summary>
        public double ElutionRangeIncrement
        {
            get { return this.elutionRangeIncrement; }
            private set
            {
                if (!this.elutionRangeIncrement.Equals(value))
                {
                    this.elutionRangeIncrement = value;
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
    }
}
