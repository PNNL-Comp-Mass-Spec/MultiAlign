namespace FeatureAlignment.Algorithms.Options
{
    public class MassTagDatabaseOptions
    {
        public MassTagDatabaseOptions()
        {
            MinimumObservationCountFilter = 5;
            OnlyLoadTagsWithDriftTime = false;
            MinimumXCorr = 0.0F;
            MinimumPmtScore = 2;
            MinimumDiscriminant = 0;
            MinimumPeptideProphetScore = 0;
            ExperimentExclusionFilter = string.Empty;
            ExperimentFilter = string.Empty;
            MinimumNet = 0;
            MaximumNet = 1.0;
            MinimumMass = 0;
            MaximumMass = 100000;
        }

        public string ExperimentFilter { get; set; }
        public string ExperimentExclusionFilter { get; set; }
        public bool OnlyLoadTagsWithDriftTime { get; set; }

        public double MinimumXCorr { get; set; }
        public int MinimumObservationCountFilter { get; set; }
        public double MinimumPmtScore { get; set; }

        public double MinimumDiscriminant { get; set; }
        public double MinimumPeptideProphetScore { get; set; }

        public double MinimumNet { get; set; }
        public double MaximumNet { get; set; }
        public double MinimumMass { get; set; }
        public double MaximumMass { get; set; }
    }
}