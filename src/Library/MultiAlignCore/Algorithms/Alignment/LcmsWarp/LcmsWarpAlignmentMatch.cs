using System;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Object to hold the Alignment Match Data from LCMS Warp
    /// Contains the Normalized elution time start and end for both the alignee and the baseline
    /// as well as the section start and end. Also contains the alignment score and the match score
    /// </summary>
    public class LcmsWarpAlignmentMatch : IComparable<LcmsWarpAlignmentMatch>
    {
        /// <summary>
        /// Id for use only by NHibernate; do not use, it is set when persisted.
        /// </summary>
        public int Id { get; private set; } // for storing in the database, set when stored.

        /// <summary>
        /// Gets or sets the alignee section NET start.
        /// This is typically scan number
        /// </summary>
        public double AligneeNetStart { get; set; }

        /// <summary>
        /// Gets or sets the alignee section NET end.
        /// This is typically scan number
        /// </summary>
        public double AligneeNetEnd { get; set; }

        /// <summary>
        /// Gets or sets the alignee start section index.
        /// </summary>
        public int AligneeSectionStart { get; set; }

        /// <summary>
        /// Gets or sets the alignee end section index.
        /// </summary>
        public int AligneeSectionEnd { get; set; }

        /// <summary>
        /// Gets or sets the baseline section NET start.
        /// </summary>
        public double BaselineNetStart { get; set; }

        /// <summary>
        /// Gets or sets the baseline section NET end.
        /// </summary>
        public double BaselineNetEnd { get; set; }

        /// <summary>
        /// Gets or sets the baseline start section index.
        /// </summary>
        public int BaselineSectionStart { get; set; }

        /// <summary>
        /// Gets or sets the baseline end section index.
        /// </summary>
        public int BaselineSectionEnd { get; set; }

        /// <summary>
        /// Gets or sets the score of just the match between the two and their sections
        /// </summary>
        public double MatchScore { get; set; }

        /// <summary>
        /// Gets or sets the score of alignments between the two up to and including this section
        /// </summary>
        public double AlignmentScore { get; set; }

        /// <summary>
        /// Set match data - Those ending in 'A' are alignee, those ending in 'B' are baseline
        /// </summary>
        /// <param name="netStartA">The alignee section NET start.</param>
        /// <param name="netEndA">The alignee section NET end.</param>
        /// <param name="sectStartA">The alignee start section index.</param>
        /// <param name="sectEndA">The alignee end section index.</param>
        /// <param name="netStartB">The baseline section NET start.</param>
        /// <param name="netEndB">The baseline section NET end.</param>
        /// <param name="sectStartB">The baseline start section index.</param>
        /// <param name="sectEndB">The baseline end section index.</param>
        /// <param name="alignScore">The alignment score between the alignee section and baseline section.</param>
        /// <param name="matchScore">The match score between the alignee section and baseline section.</param>
        public void Set(
                        double netStartA,
                        double netEndA,
                        int sectStartA,
                        int sectEndA,
                        double netStartB,
                        double netEndB,
                        int sectStartB,
                        int sectEndB,
                        double alignScore,
                        double matchScore)
        {
            this.AligneeNetStart = netStartA;
            this.AligneeNetEnd = netEndA;

            this.BaselineNetStart = netStartB;
            this.BaselineNetEnd = netEndB;

            this.AligneeSectionStart = sectStartA;
            this.AligneeSectionEnd = sectEndA;

            this.BaselineSectionStart = sectStartB;
            this.BaselineSectionEnd = sectEndB;

            this.AlignmentScore = alignScore;
            this.MatchScore = matchScore;
        }

        /// <summary>
        /// Compare an <see cref="LcmsWarpAlignmentMatch" /> to this match
        /// by comparing the alignee start section if the are not equal,
        /// or the baseline start section if they are.
        /// </summary>
        /// <param name="compareFeature">The feature to compare this to.</param>
        /// <returns>
        /// A value indicating whether this feature is greater than, less than,
        /// or equal to the other feature.
        /// </returns>
        public int CompareTo(LcmsWarpAlignmentMatch compareFeature)
        {
            if (compareFeature == null)
            {
                return 1;
            }
            if (this.AligneeSectionStart != compareFeature.AligneeSectionStart)
            {
                return this.AligneeSectionStart.CompareTo(compareFeature.AligneeSectionStart);
            }
            return this.BaselineSectionStart.CompareTo(compareFeature.BaselineSectionStart);
        }
    }
}