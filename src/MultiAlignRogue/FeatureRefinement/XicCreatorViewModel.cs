namespace MultiAlignRogue.FeatureRefinement
{
    using GalaSoft.MvvmLight.Messaging;

    using MultiAlignCore.Algorithms.Chromatograms;
    using MultiAlignCore.IO.InputFiles;

    using MultiAlignRogue.Utils;
    using MultiAlignRogue.ViewModels;

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
            get { return this.xicCreator.ShouldRefine; }
            set
            {
                if (this.xicCreator.ShouldRefine != value)
                {
                    this.xicCreator.ShouldRefine = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the relative intensity threshold for snipping XIC tails.
        /// </summary>
        public double RelativeIntensityThreshold
        {
            get { return this.xicCreator.XicRefiner.RelativeIntensityThreshold; }
            set
            {
                if (!this.xicCreator.XicRefiner.RelativeIntensityThreshold.Equals(value))
                {
                    this.xicCreator.XicRefiner.RelativeIntensityThreshold = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
