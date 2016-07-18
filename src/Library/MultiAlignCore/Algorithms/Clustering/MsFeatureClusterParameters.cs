using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Options for MS feature clustering.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MsFeatureClusterParameters<T>:
        FeatureClusterParameters<T>
        where T : FeatureLight, new()

    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MsFeatureClusterParameters()
        {
            Clear();
        }
        /// <summary>
        /// Gets or sets the retention time weight.
        /// </summary>
        public double RetentionTimeWeight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the drift time weight to use.
        /// </summary>
        public double DriftTimeWeight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the log abundance weight
        /// </summary>
        public double AbundanceLogWeight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the scan weight to use.
        /// </summary>
        public double ScanWeight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the fit or score weight to use when calculating
        /// distance between two features.
        /// </summary>
        public double FitWeight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the mono mass weight.
        /// </summary>
        public double MassMonoisotopicWeight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the mass constraint for
        /// </summary>
        public double MassMonoisotopicAverageConstraint
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the monoisotopic mass average weight.
        /// </summary>
        public double MassMonoisotopicAverageWeight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the maximum distance to use.
        /// </summary>
        public double MaxDistance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to convert scans into a generic NET value based on the min and max scan.
        /// </summary>
        public bool UseNET
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the scane minimum of the feature cloud.
        /// </summary>
        public int ScanMinimum
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the scan maximum of the feature cloud.
        /// </summary>
        public int ScanMaximum
        {
            get;
            set;
        }
        /// <summary>
        /// Sets the internal values to their default state.
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            MassMonoisotopicWeight              = .01;
            MassMonoisotopicAverageWeight       = .01;
            MassMonoisotopicAverageConstraint   = 10;   // ppm
            AbundanceLogWeight                  = .1;
            ScanWeight                          = .01;
            FitWeight                           = .1;
            DriftTimeWeight                     = 1.0;
            RetentionTimeWeight                 = 15;   // Retention time weight
            MaxDistance                         = .1;
            ScanMinimum                         = int.MaxValue;
            ScanMaximum                         = int.MinValue;
        }
    }
}
