using System;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Object to hold the Alignment Match Data from LCMS Warp
    /// Contains the Normalized elution time start and end for both the alignee and the baseline
    /// as well as the section start and end. Also contains the alignment score and the match score
    /// </summary>
    class LcmsWarpAlignmentMatch : IComparable<LcmsWarpAlignmentMatch>
    {
        private double m_netStart;
        private double m_netEnd;
        private int m_sectionStart;
        private int m_sectionEnd;

        private double m_netStart2;
        private double m_netEnd2;
        private int m_sectionStart2;
        private int m_sectionEnd2;

        // Score of alignments between the two up to and including this section
        private double m_alignmentScore;
        // Score of just the match between the two and their sections
        private double m_matchScore;

        #region Public properties
        public double NetStart
        {
            get { return m_netStart; }
            set { m_netStart = value; }
        }
        public double NetStart2
        {
            get { return m_netStart2; }
            set { m_netStart2 = value; }
        }
        public double NetEnd
        {
            get { return m_netEnd; }
            set { m_netEnd = value; }
        }
        public double NetEnd2
        {
            get { return m_netEnd2; }
            set { m_netEnd2 = value; }
        }
        public int SectionStart
        {
            get { return m_sectionStart; }
            set { m_sectionStart = value; }
        }
        public int SectionStart2
        {
            get { return m_sectionStart2; }
            set { m_sectionStart2 = value; }
        }
        public int SectionEnd
        {
            get { return m_sectionEnd; }
            set { m_sectionEnd = value; }
        }
        public int SectionEnd2
        {
            get { return m_sectionEnd2; }
            set { m_sectionEnd2 = value; }
        }

        public double MatchScore
        {
            get { return m_matchScore; }
            set { m_matchScore = value; }
        }
        public double AlignmentScore
        {
            get { return m_alignmentScore; }
            set { m_alignmentScore = value; }
        }
        #endregion

        public void Set(double netStartA, double netEndA, int sectStartA, int sectEndA,
                        double netStartB, double netEndB, int sectStartB, int sectEndB,
                        double alignScore, double matchScore)
        {
            m_netStart = netStartA;
            m_netEnd = netEndA;
            m_netStart2 = netStartB;
            m_netEnd2 = netEndB;

            m_sectionStart = sectStartA;
            m_sectionEnd = sectEndA;
            m_sectionStart2 = sectStartB;
            m_sectionEnd2 = sectEndB;

            m_alignmentScore = alignScore;
            m_matchScore = matchScore;
        }

        public int CompareTo(LcmsWarpAlignmentMatch compareFeature)
        {
            if (compareFeature == null)
            {
                return 1;
            }
            if (m_sectionStart != compareFeature.m_sectionStart)
            {
                return m_sectionStart.CompareTo(compareFeature.m_sectionStart);
            }
            return m_sectionStart2.CompareTo(compareFeature.m_sectionStart2);
        }
    }
}
