using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MetaData;

namespace MultiAlignRogue.Alignment
{
    using System;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    /// <summary>
    /// This class is a view model for configuring a <see cref="AlignmentDimensionSettings" />
    /// settings for a single separation dimension.
    /// </summary>
    public class AlignmentDimensionSettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// The dimension settings model.
        /// </summary>
        private readonly AlignmentDimensionSettings dimensionSettings;

        /// <summary>
        /// The name of the selected baseline dataset.
        /// </summary>
        private string selectedBaselineDatasetName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlignmentDimensionSettingsViewModel"/> class.
        /// </summary>
        /// <param name="dimensionSettings">Dialog service for opening dialogs from view model.</param>
        public AlignmentDimensionSettingsViewModel(AlignmentDimensionSettings dimensionSettings)
        {
            // Initialize properties to values in model.
            this.dimensionSettings = dimensionSettings;
            this.Tolerance = this.dimensionSettings.Tolerance;
            this.NumSections = this.dimensionSettings.NumSections;
            this.ContractionFactor = this.dimensionSettings.ContractionFactor;
            this.ShouldAlignDimension = this.dimensionSettings.ShouldAlignDimension;

            this.MoveUpCommand = new RelayCommand(
                () =>
                    {
                        if (this.MoveUpRequested != null)
                        {
                            this.MoveUpRequested(this, EventArgs.Empty);
                        }
                    },
                    () => this.ShouldAlignDimension);

            this.MoveDownCommand = new RelayCommand(
                () =>
                {
                    if (this.MoveDownRequested != null)
                    {
                        this.MoveDownRequested(this, EventArgs.Empty);
                    }
                },
                () => this.ShouldAlignDimension);
        }

        /// <summary>
        /// Event that is triggered to indicate to the parent view model that this view model
        /// wants to be moved up in its containing list.
        /// </summary>
        public event EventHandler MoveUpRequested;

        /// <summary>
        /// Event that is triggered to indicate to the parent view model that this view model
        /// wants to be moved down in its containing list.
        /// </summary>
        public event EventHandler MoveDownRequested;

        /// <summary>
        /// Gets a command that triggers the <see cref="MoveUpRequested" /> event.
        /// </summary>
        public RelayCommand MoveUpCommand { get; }

        /// <summary>
        /// Gets a command that triggers the <see cref="MoveDownRequested" /> event.
        /// </summary>
        public RelayCommand MoveDownCommand { get; }

        /// <summary>
        /// Gets the type of separation that this dimension represents.
        /// </summary>
        public SeparationTypes SeparationType => this.dimensionSettings.SeparationType;

        /// <summary>
        /// Gets or sets the tolerance used when matching features in this dimension.
        /// </summary>
        public double Tolerance
        {
            get => this.dimensionSettings.Tolerance;
            set
            {
                if (!this.dimensionSettings.Tolerance.Equals(value))
                {
                    this.dimensionSettings.Tolerance = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of sections to discrete-ize this dimension into.
        /// </summary>
        public int NumSections
        {
            get => this.dimensionSettings.NumSections;
            set
            {
                if (this.dimensionSettings.NumSections != value)
                {
                    this.dimensionSettings.NumSections = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Get or sets a value indicating how much this dimension's sections can
        /// expand or contract.
        ///
        /// Each section can expand to (ContractionFactor)^2 or
        /// contract to 1 / ContractionFactor.
        /// </summary>
        public int ContractionFactor
        {
            get => this.dimensionSettings.ContractionFactor;
            set
            {
                if (this.dimensionSettings.ContractionFactor != value)
                {
                    this.dimensionSettings.ContractionFactor = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this dimension should be warped.
        /// </summary>
        public bool ShouldAlignDimension
        {
            get => this.dimensionSettings.ShouldAlignDimension;
            set
            {
                if (this.ShouldAlignDimension != value)
                {
                    this.ShouldAlignDimension = value;
                    this.MoveUpCommand.RaiseCanExecuteChanged();
                    this.MoveDownCommand.RaiseCanExecuteChanged();
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the selected baseline dataset.
        /// </summary>
        public IDataset SelectedBaselineDataset
        {
            get => this.dimensionSettings.BaselineDataset;
            private set
            {
                if (this.dimensionSettings.BaselineDataset != value)
                {
                    this.dimensionSettings.BaselineDataset = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
