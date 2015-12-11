namespace MultiAlignRogue.Alignment
{
    using System;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using MultiAlignCore.Data.Features;

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
                    });

            this.MoveDownCommand = new RelayCommand(
                () =>
                {
                    if (this.MoveDownRequested != null)
                    {
                        this.MoveDownRequested(this, EventArgs.Empty);
                    }
                });
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
        /// Gets a commmand that triggers the <see cref="MoveUpRequested" /> event.
        /// </summary>
        public RelayCommand MoveUpCommand { get; private set; }

        /// <summary>
        /// Gets a command that triggers the <see cref="MoveDownRequested" /> event.
        /// </summary>
        public RelayCommand MoveDownCommand { get; private set; }

        /// <summary>
        /// Gets the type of separation that this dimension represents.
        /// </summary>
        public FeatureLight.SeparationTypes SeparationType { get { return this.dimensionSettings.SeparationType; } }

        /// <summary>
        /// Gets or sets the tolerance used when matching features in this dimension.
        /// </summary>
        public double Tolerance
        {
            get { return this.dimensionSettings.Tolerance; }
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
        /// Gets or sets the number of sections to discretize this dimension into.
        /// </summary>
        public int NumSections
        {
            get { return this.dimensionSettings.NumSections; }
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
            get { return this.dimensionSettings.ContractionFactor; }
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
            get { return this.dimensionSettings.ShouldAlignDimension; }
            set
            {
                if (this.ShouldAlignDimension != value)
                {
                    this.ShouldAlignDimension = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
