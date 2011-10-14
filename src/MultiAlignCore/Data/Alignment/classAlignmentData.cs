using System;
using MultiAlignEngine.Alignment;

namespace MultiAlignCore.Data.Alignment
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
        public double[,]    driftErrorHistogram;
        public double       NETRsquared;
        public double       NETSlope;
        public double       NETIntercept;
        public double       MassMean;
        public double       NETMean;
        public double       MassStandardDeviation;
        public double       NETStandardDeviation;

        
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

        public double MassKurtosis
        {
            get
            {
                return Math.Pow(MassMean, 4) / Math.Pow(MassStandardDeviation, 4);
            }
        }
        public double NETKurtosis
        {
            get
            {
                return Math.Pow(NETMean, 4) / Math.Pow(NETStandardDeviation, 4);
            }
        }
    }
}
