namespace MultiAlignRogue.FeatureRefinement
{
    using MultiAlignCore.Algorithms.Chromatograms;

    using MultiAlignRogue.Utils;

    /// <summary>
    /// This class is a view model for editing the <see cref="XicCreator" /> settings.
    /// </summary>
    public sealed class XicCreatorViewModel : SettingsEditorViewModelBase
    {
        /// <summary>
        /// The model that this view model edits.
        /// </summary>
        private readonly XicCreator xicCreator;

        /// <summary>
        /// Initializes new instance of the <see cref="XicCreatorViewModel" /> class.
        /// </summary>
        /// <param name="xicCreator">The model that this view model edits.</param>
        public XicCreatorViewModel(XicCreator xicCreator) : base(xicCreator)
        {
            this.xicCreator = xicCreator;
        }

        /// <summary>
        /// Gets or sets the mass error in PPM for the 
        /// </summary>
        public double MassError
        {
            get { return this.xicCreator.MassError; }
            set
            {
                if (!this.xicCreator.MassError.Equals(value))
                {
                    this.xicCreator.MassError = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether refinement (smoothing and tail snipping)
        /// should be run on the XICs.
        /// </summary>
        public bool ShouldRefine
        {
            get { return this.xicCreator.ShouldSmooth; }
            set
            {
                if (this.xicCreator.ShouldSmooth != value)
                {
                    this.xicCreator.ShouldSmooth = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the relative intensity threshold for snipping XIC tails.
        /// </summary>
        public double RelativeIntensityThreshold
        {
            get { return this.xicCreator.RelativeIntensityThreshold; }
            set
            {
                if (!this.xicCreator.RelativeIntensityThreshold.Equals(value))
                {
                    this.xicCreator.RelativeIntensityThreshold = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number scans to pad the feature scan range by when extracting the XIC.
        /// </summary>
        /// <remarks>This is useful when using this feature as an input to the peak finder.</remarks>
        public int ScanTolerance
        {
            get { return this.xicCreator.ScanTolerance; }
            set
            {
                if (this.xicCreator.ScanTolerance != value)
                {
                    this.xicCreator.ScanTolerance = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
