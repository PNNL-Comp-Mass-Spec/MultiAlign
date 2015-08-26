using System.Collections.Generic;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Chromatogram for a given m/z holding X scan.
    /// </summary>
    public class Chromatogram
    {
        protected Chromatogram()
        {
            Points = new List<XYData>();
        }

        public int ChargeState
        {
            get;
            set;
        }
        public double Mz
        {
            get;
            set;
        }
        public int StartScan
        {
            get;
            set;
        }
        public int EndScan
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the points defining the chromatogram
        /// </summary>
        public List<XYData> Points
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the points that were fit to a chromatogram
        /// </summary>
        public List<XYData> FitPoints
        {
            get;
            set;
        }        
        /// <summary>
        /// Gets or sets the coefficients that describes the fit profile.
        /// </summary>
        public double[] FitCoefficients
        {
            get;
            set;
        }

        public double Area { get; set; }
    }
}
