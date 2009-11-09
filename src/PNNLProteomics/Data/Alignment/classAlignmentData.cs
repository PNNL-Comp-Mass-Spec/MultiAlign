using System;
using MultiAlignEngine.Alignment;

namespace PNNLProteomics.Data.Alignment
{
    [Serializable()]
    public class classAlignmentData
    {
        private classAlignmentResidualData mobj_residualData;

        public clsAlignmentFunction alignmentFunction;
        public string       aligneeDataset;
        public float[,]     heatScores;
        public int          minScanBaseline;
        public int          maxScanBaseline;
        public float        minMTDBNET;
        public float        maxMTDBNET;
        public double[,]    massErrorHistogram;
        public double[,]    netErrorHistogram;
        public double       NETRsquared;
        public double       NETSlope;
        public double       NETIntercept;

        /// <summary>
        /// Gets or sets the residual alignment data.
        /// </summary>        
        public classAlignmentResidualData ResidualData
        {
            get
            {
                return mobj_residualData;
            }
            set
            {
                mobj_residualData = value;
            }
        }
    }
}
