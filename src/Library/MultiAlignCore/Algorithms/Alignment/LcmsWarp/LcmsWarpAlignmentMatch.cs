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

        public double NetStart { get; set; }
        public double NetStart2 { get; set; }
        public double NetEnd { get; set; }
        public double NetEnd2 { get; set; }
        public int SectionStart { get; set; }
        public int SectionStart2 { get; set; }
        public int SectionEnd { get; set; }
        public int SectionEnd2 { get; set; }

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
            NetStart = netStartA;
            NetEnd = netEndA;
            NetStart2 = netStartB;
            NetEnd2 = netEndB;

            SectionStart = sectStartA;
            SectionEnd = sectEndA;
            SectionStart2 = sectStartB;
            SectionEnd2 = sectEndB;

            AlignmentScore = alignScore;
            MatchScore = matchScore;
        }

        public int CompareTo(LcmsWarpAlignmentMatch compareFeature)
        {
            if (compareFeature == null)
            {
                return 1;
            }
            if (SectionStart != compareFeature.SectionStart)
            {
                return SectionStart.CompareTo(compareFeature.SectionStart);
            }
            return SectionStart2.CompareTo(compareFeature.SectionStart2);
        }
    }
}
