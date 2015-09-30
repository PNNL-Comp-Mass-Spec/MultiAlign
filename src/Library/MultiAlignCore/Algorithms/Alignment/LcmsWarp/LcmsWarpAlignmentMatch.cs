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
        #region Public properties

        public double AligneeNetStart { get; set; }
        public double AligneeNetEnd { get; set; }
        public int AligneeSectionStart { get; set; }
        public int AligneeSectionEnd { get; set; }

        public double BaselineNetStart { get; set; }
        public double BaselineNetEnd { get; set; }
        public int BaselineSectionStart { get; set; }
        public int BaselineSectionEnd { get; set; }

        /// <summary>
        /// Score of just the match between the two and their sections
        /// </summary>
        public double MatchScore { get; set; }

        /// <summary>
        /// Score of alignments between the two up to and including this section
        /// </summary>
        public double AlignmentScore { get; set; }

        #endregion

        public void Set(double netStartA, double netEndA, int sectStartA, int sectEndA,
            double netStartB, double netEndB, int sectStartB, int sectEndB,
            double alignScore, double matchScore)
        {
            AligneeNetStart = netStartA;
            AligneeNetEnd = netEndA;
            BaselineNetStart = netStartB;
            BaselineNetEnd = netEndB;

            AligneeSectionStart = sectStartA;
            AligneeSectionEnd = sectEndA;
            BaselineSectionStart = sectStartB;
            BaselineSectionEnd = sectEndB;

            AlignmentScore = alignScore;
            MatchScore = matchScore;
        }

        public int CompareTo(LcmsWarpAlignmentMatch compareFeature)
        {
            if (compareFeature == null)
            {
                return 1;
            }
            if (AligneeSectionStart != compareFeature.AligneeSectionStart)
            {
                return AligneeSectionStart.CompareTo(compareFeature.AligneeSectionStart);
            }
            return BaselineSectionStart.CompareTo(compareFeature.BaselineSectionStart);
        }
    }
}