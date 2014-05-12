namespace MultiAlignCore.Algorithms.Options
{
    public class MassTagDatabaseOptions
    {
        public MassTagDatabaseOptions()
        {
            MinimumObservationCountFilter = 0;
            OnlyLoadTagsWithDriftTime = false;
            MinimumXCorr = 0.0F;
            MinimumPmtScore = 1;
            MinimumDiscriminant = 0;
            MinimumPeptideProphetScore = 0.5;
            ExperimentExclusionFilter = "";
            ExperimentFilter = "";
        }

        public string ExperimentFilter { get; set; }
        public string ExperimentExclusionFilter { get; set; }
        public bool OnlyLoadTagsWithDriftTime { get; set; }

        public double MinimumXCorr { get; set; }
        public int MinimumObservationCountFilter { get; set; }
        public double MinimumPmtScore { get; set; }

        public double MinimumDiscriminant { get; set; }
        public double MinimumPeptideProphetScore { get; set; }
    }
}