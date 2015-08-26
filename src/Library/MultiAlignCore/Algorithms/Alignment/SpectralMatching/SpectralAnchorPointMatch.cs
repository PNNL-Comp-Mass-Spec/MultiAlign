namespace MultiAlignCore.Algorithms.Alignment.SpectralMatching
{

    /// <summary>
    /// Defines an anchor point match
    /// </summary>
    public class SpectralAnchorPointMatch
    {
        public SpectralAnchorPointMatch()
        {
            AnchorPointX = new SpectralAnchorPoint();
            AnchorPointY = new SpectralAnchorPoint();
            IsValidMatch = AnchorPointMatchType.FalseMatch;
        }


        public int Id { get; set; }

        public SpectralAnchorPoint AnchorPointX { get; set; }
        public SpectralAnchorPoint AnchorPointY { get; set; }
        public double SimilarityScore { get; set; }

        /// <summary>
        /// Gets or sets whether this match is valid, or invalid (false positive)
        /// </summary>
        public AnchorPointMatchType IsValidMatch { get; set; }
    }
}