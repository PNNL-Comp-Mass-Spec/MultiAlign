namespace MultiAlignRogue.FeatureRefinement
{
    using MultiAlignCore.Algorithms.FeatureRefinement;

    using MultiAlignRogue.Utils;

    /// <summary>
    /// This class is for editing the settings for MASIC peak finding.
    /// </summary>
    public class MasicPeakFinderViewModel : SettingsEditorViewModelBase
    {
        /// <summary>
        /// The peak finder model that this view model edits.
        /// </summary>
        private readonly MasicPeakFinder masicPeakFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasicPeakFinderViewModel" /> class.
        /// </summary>
        /// <param name="model">The peak finder model that this view model edits.</param>
        public MasicPeakFinderViewModel(MasicPeakFinder model)
            : base(model)
        {
            this.masicPeakFinder = model;
        }

        /// <summary>
        /// Gets or sets the minimum intensity allowed to be considered a peak.
        /// </summary>
        /// <remarks>Default: 0</remarks>
        public double AbsoluteMinimumIntensityThreshold
        {
            get { return this.masicPeakFinder.AbsoluteMinimumIntensityThreshold; }
            set
            {
                if (!this.masicPeakFinder.AbsoluteMinimumIntensityThreshold.Equals(value))
                {
                    this.masicPeakFinder.AbsoluteMinimumIntensityThreshold = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum intensity relative to the largest data point to
        /// be considered a peak.
        /// </summary>
        /// <remarks>Default: 0.01</remarks>
        public double RelativeIntensityThreshold
        {
            get { return this.masicPeakFinder.RelativeIntensityThreshold; }
            set
            {
                if (!this.masicPeakFinder.RelativeIntensityThreshold.Equals(value))
                {
                    this.masicPeakFinder.RelativeIntensityThreshold = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum distance that the edge of an identified peak can
        /// be away from the scan number that the parent ion was observed in if the identified
        /// peak does not contain the parent ion.
        /// </summary>
        /// <remarks>Default: 0</remarks>
        public int MaxScansWithNoOverlap
        {
            get { return this.masicPeakFinder.MaxScansWithNoOverlap;}
            set
            {
                if (this.masicPeakFinder.MaxScansWithNoOverlap != value)
                {
                    this.masicPeakFinder.MaxScansWithNoOverlap = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum fraction of the peak maximum that an upward spike can be
        /// to be included in the peak.
        /// </summary>
        /// <remarks>Default: 0.20 which means the maximum allowable spike is 20% of the peak maximum</remarks>
        public double MaxAllowedUpwardSpikeFraction
        {
            get { return this.masicPeakFinder.MaxAllowedUpwardSpikeFraction;}
            set
            {
                if (!this.masicPeakFinder.MaxAllowedUpwardSpikeFraction.Equals(value))
                {
                    this.masicPeakFinder.MaxAllowedUpwardSpikeFraction = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
