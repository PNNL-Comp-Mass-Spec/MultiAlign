namespace MultiAlignRogue.FeatureRefinement
{
    using System.Collections.ObjectModel;

    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Algorithms.FeatureRefinement;

    using MultiAlignRogue.Utils;

    /// <summary>
    /// This is a view model for editing the options for a <see cref="DeisotopingCorrector" />.
    /// </summary>
    public class DeisotopingCorrectorViewModel : SettingsEditorViewModelBase
    {
        /// <summary>
        /// The deisotoping options model that this view model edits.
        /// </summary>
        private readonly DeisotopingCorrector deisotopingCorrector;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeisotopingCorrectorViewModel" /> class. 
        /// </summary>
        /// <param name="deisotopingCorrector">The deisotoping options model that this view model edits.</param>
        public DeisotopingCorrectorViewModel(DeisotopingCorrector deisotopingCorrector) : base(deisotopingCorrector)
        {
            this.deisotopingCorrector = deisotopingCorrector;
            this.ClusteringAlgorithmTypes = new ObservableCollection<ClusteringAlgorithmTypes>(ClusteringAlgorithms.GenericClusteringAlgorithms);
        }

        /// <summary>
        /// Gets or sets the options for the clustering algorithm.
        /// </summary>
        public ObservableCollection<ClusteringAlgorithmTypes> ClusteringAlgorithmTypes { get; private set; }

        /// <summary>
        /// Gets or sets the type of clustering algorithm to use.
        /// </summary>
        public ClusteringAlgorithmTypes SelectedClusteringAlgorithm
        {
            get { return this.deisotopingCorrector.ClusteringAlgorithm; }
            set
            {
                if (this.deisotopingCorrector.ClusteringAlgorithm != value)
                {
                    this.deisotopingCorrector.ClusteringAlgorithm = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the mass tolerance.
        /// </summary>
        public double MassTolerance
        {
            get { return this.deisotopingCorrector.Tolerances.Mass; }
            set
            {
                if (!this.deisotopingCorrector.Tolerances.Mass.Equals(value))
                {
                    this.deisotopingCorrector.Tolerances.Mass = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the NET tolerance.
        /// </summary>
        public double NetTolerance
        {
            get { return this.deisotopingCorrector.Tolerances.Net; }
            set
            {
                if (!this.deisotopingCorrector.Tolerances.Net.Equals(value))
                {
                    this.deisotopingCorrector.Tolerances.Net = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tolerance for the mobility dimension.
        /// </summary>
        public double DiftTimeTolerance
        {
            get { return this.deisotopingCorrector.Tolerances.DriftTime; }
            set
            {
                if (!this.deisotopingCorrector.Tolerances.DriftTime.Equals(value))
                {
                    this.deisotopingCorrector.Tolerances.DriftTime = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of isotopes to compare features by.
        /// </summary>
        /// <remarks>The higher this number, the slower it is.</remarks>
        public int NumberOfIsotopes
        {
            get { return this.deisotopingCorrector.NumberOfIsotopes; }
            set
            {
                if (this.deisotopingCorrector.NumberOfIsotopes != value)
                {
                    this.deisotopingCorrector.NumberOfIsotopes = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether potential isotope peaks
        /// should be combined even though they are different charge states.
        /// </summary>
        public bool ShouldSeparateChargeStates
        {
            get { return this.deisotopingCorrector.ShouldSeparateChargeStates; }
            set
            {
                if (this.deisotopingCorrector.ShouldSeparateChargeStates != value)
                {
                    this.deisotopingCorrector.ShouldSeparateChargeStates = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Restore defaults back to their original values.
        /// </summary>
        public void RestoreDefaults()
        {
            this.deisotopingCorrector.RestoreDefaults();
        }
    }
}
