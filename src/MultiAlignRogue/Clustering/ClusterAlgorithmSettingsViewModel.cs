namespace MultiAlignRogue.Clustering
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using GalaSoft.MvvmLight;

    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Algorithms.Distance;
    using MultiAlignCore.Data.Features;

    /// <summary>
    /// This is the view model for the ClusterAlgorithmSettings User Control.
    /// </summary>
    public class ClusterAlgorithmSettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// The clustering options model that this view model edits.
        /// </summary>
        private readonly LcmsClusteringOptions clusteringOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterAlgorithmSettingsViewModel" /> class. 
        /// </summary>
        /// <param name="clusteringOptions">The clustering options model that this view model edits.</param>
        public ClusterAlgorithmSettingsViewModel(LcmsClusteringOptions clusteringOptions)
        {
            this.clusteringOptions = clusteringOptions;
            this.ClusteringAlgorithmTypes = new ObservableCollection<ClusteringAlgorithmTypes>(ClusteringAlgorithms.GenericClusteringAlgorithms);
            this.DistanceFunctions = new ObservableCollection<DistanceMetric>(Enum.GetValues(typeof(DistanceMetric)).Cast<DistanceMetric>());
            this.ClusterCentroidRepresentations = new ObservableCollection<ClusterCentroidRepresentation>
            {
                ClusterCentroidRepresentation.Mean,
                ClusterCentroidRepresentation.Median
            };
        }

        /// <summary>
        /// Gets or sets a value indicating whether features with different charge states should
        /// be kept separate.
        /// </summary>
        public bool ShouldSeparateCharge
        {
            get { return this.clusteringOptions.ShouldSeparateCharge; }
            set
            {
                if (this.clusteringOptions.ShouldSeparateCharge != value)
                {
                    this.clusteringOptions.ShouldSeparateCharge = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the options for distance calculation between two features.
        /// </summary>
        public ObservableCollection<DistanceMetric> DistanceFunctions { get; private set; }

        /// <summary>
        /// Gets or sets the method to use for calculating distance between features.
        /// </summary>
        public DistanceMetric SelectedDistanceFunction
        {
            get { return this.clusteringOptions.DistanceFunction; }
            set
            {
                if (this.clusteringOptions.DistanceFunction != value)
                {
                    this.clusteringOptions.DistanceFunction = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the options for possible clustering algorithms for selection.
        /// </summary>
        public ObservableCollection<ClusteringAlgorithmTypes> ClusteringAlgorithmTypes { get; private set; }

        /// <summary>
        /// Gets or sets the type of algorithm to use for clustering.
        /// </summary>
        public ClusteringAlgorithmTypes SelectedClusteringAlgorithm
        {
            get { return this.clusteringOptions.LcmsFeatureClusteringAlgorithm; }
            set
            {
                if (this.clusteringOptions.LcmsFeatureClusteringAlgorithm != value)
                {
                    this.clusteringOptions.LcmsFeatureClusteringAlgorithm = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the possible parts of the peak to use as the centroid of the peak.
        /// </summary>
        public ObservableCollection<ClusterCentroidRepresentation> ClusterCentroidRepresentations { get; private set; }

        /// <summary>
        /// Gets or sets the part of the peak to use as the centroid of the peak.
        /// </summary>
        public ClusterCentroidRepresentation SelectedClusterCentroidRepresentation
        {
            get { return this.clusteringOptions.ClusterCentroidRepresentation; }
            set
            {
                if (this.clusteringOptions.ClusterCentroidRepresentation != value)
                {
                    this.clusteringOptions.ClusterCentroidRepresentation = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the mass tolerance.
        /// </summary>
        public double MassTolerance
        {
            get { return this.clusteringOptions.InstrumentTolerances.Mass; }
            set
            {
                if (!this.clusteringOptions.InstrumentTolerances.Mass.Equals(value))
                {
                    this.clusteringOptions.InstrumentTolerances.Mass = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the normalized elution time (NET) tolerance.
        /// </summary>
        public double NetTolerance
        {
            get { return this.clusteringOptions.InstrumentTolerances.Net; }
            set
            {
                if (!this.clusteringOptions.InstrumentTolerances.Net.Equals(value))
                {
                    this.clusteringOptions.InstrumentTolerances.Net = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
