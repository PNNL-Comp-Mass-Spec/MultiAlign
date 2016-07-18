namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    public enum SpectraFilters
    {
        RawThreshold,
        TopPercent
    }

    public class SpectrumFilterFactory
    {
        public static ISpectraFilter CreateFilter(SpectraFilters filterType)
        {
            ISpectraFilter filter = null;
            switch (filterType)
            {
                case SpectraFilters.RawThreshold:
                    filter = new ThresholdSpectralFilter();
                    break;
                case SpectraFilters.TopPercent:
                    filter = new TopPercentSpectralFilter();
                    break;
                default:
                    break;
            }
            return filter;
        }
    }
}
