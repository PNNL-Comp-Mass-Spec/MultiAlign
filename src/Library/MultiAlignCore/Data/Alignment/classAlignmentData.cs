using System;
using System.Collections.Generic;
using MultiAlignEngine.Alignment;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;

namespace MultiAlignCore.Data.Alignment
{
    [Serializable]
    public class classAlignmentData
    {
        private classAlignmentResidualData mobj_residualData;

        public int          DatasetID {get;set;}
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
        public double       NETRsquared {get;set;}
        public double       NETSlope {get;set;}
        public double       NETIntercept {get;set;}
        public double       MassMean {get;set;}
        public double       NETMean {get;set;}
        public double       MassStandardDeviation {get;set;}
        public double       NETStandardDeviation {get;set;}


        /// <summary>
        /// Gets or sets the anchor poitns defined by spectral matching if that algorithm was used.
        /// This is a stop gap until I can port this all to better objects.
        /// </summary>
        public IEnumerable<SpectralAnchorPointMatch> Matches { get; set; }

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



        public override bool Equals(object obj)
        {
            var factor = (classAlignmentData)obj;

            if (factor == null)
            {
                return false;
            }
            if (!DatasetID.Equals(factor.DatasetID))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash * 23 + DatasetID.GetHashCode();            

            return hash;
        }
    }
}
