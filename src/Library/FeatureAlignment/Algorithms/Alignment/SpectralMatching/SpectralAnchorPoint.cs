using FeatureAlignment.Data;
using FeatureAlignment.Data.MassTags;

namespace FeatureAlignment.Algorithms.Alignment.SpectralMatching
{
    /// <summary>
    /// Defines an anchor point
    /// </summary>
    public class SpectralAnchorPoint
    {
        public Peptide   Peptide    { get; set; }
        public MSSpectra Spectrum   { get; set; }
        public double    Net        { get; set; }
        public double    NetAligned { get; set; }
        public double    Mass       { get; set; }
        public double    Mz         { get; set; }
        public double    MzAligned  { get; set; }
        public int       Scan       { get; set; }

        public bool IsTrue { get; set; }
    }
}
