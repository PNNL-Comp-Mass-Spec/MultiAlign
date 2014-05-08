namespace MultiAlignSTACRunner
{
    public sealed class FilteringOptions
    {
        private FilteringOptions(int totalDatasetMembers, double minRatio, double maxRatio)
        {
            TotalDatasetMembers = totalDatasetMembers;
            MinRatio            = minRatio;
            MaxRatio            = maxRatio;
        }

        public FilteringOptions()
            : this(20, 1, 1.5)
        {
            
        }

        public int TotalDatasetMembers { get; private set; }
        public double MinRatio { get; private set; }
        public double MaxRatio { get; private set; }
    }
}