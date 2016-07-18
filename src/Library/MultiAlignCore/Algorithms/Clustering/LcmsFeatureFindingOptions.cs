namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// LC-MS Feature Filtering Options.  These options filter LC-MS features
    /// </summary>
    public class LcmsFeatureFindingOptions
    {
        /// <summary>
        /// Enum for which MS feature clusterer to use.
        /// </summary>
        public enum MsFeatureClusterers
        {
            TreeClusterer,
            SingleLinkageClusterer
        };

        public LcmsFeatureFindingOptions(FeatureTolerances tolerances) : this()
        {
            InstrumentTolerances = tolerances;
        }

        public LcmsFeatureFindingOptions()
        {
            InstrumentTolerances = new FeatureTolerances();

            this.FirstPassClusterer = MsFeatureClusteringAlgorithmType.SingleLinkage;
            this.SecondPassClusterer = GenericClusteringAlgorithmType.BinarySearchTree;
            this.FindXics = true;
            this.RefineXics = true;
            this.SmoothingWindowSize = 5;
            this.SmoothingPolynomialOrder = 2;
            this.XicRelativeIntensityThreshold = 0.05;
            this.SecondPassClustering = true;

            MaximumScanRange = 50;
            MaximumNetRange = .005;
        }

        public MsFeatureClusteringAlgorithmType FirstPassClusterer { get; set; }
        public GenericClusteringAlgorithmType SecondPassClusterer { get; set; }

        public bool FindXics { get; set; }
        public bool RefineXics { get; set; }
        public int SmoothingWindowSize { get; set; }
        public int SmoothingPolynomialOrder { get; set; }
        public double XicRelativeIntensityThreshold { get; set; }

        public bool SecondPassClustering { get; set; }

        public FeatureTolerances InstrumentTolerances { get; set; }
        public int               MaximumScanRange { get; set; }
        public double            MaximumNetRange { get; set; }
    }
}