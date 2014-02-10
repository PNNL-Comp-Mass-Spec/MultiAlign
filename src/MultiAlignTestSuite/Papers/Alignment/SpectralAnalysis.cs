using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.Papers.Alignment.SSM;
using MultiAlignTestSuite.Algorithms.SpectralProcessing;

namespace MultiAlignTestSuite.Papers.Alignment
{
    /// <summary>
    /// Holds information about each analysis window for true and false matches.
    /// </summary>
    public class SpectralAnalysis
    {        
        /// <summary>
        /// Constructor
        /// </summary>
        public SpectralAnalysis()
        {                        
            MassData    = new AlignmentMeasurement<double>();
            NetData     = new AlignmentMeasurement<double>(); 
            Matches     = new List<AnchorPointMatch>();
            Options     = new SpectralOptions();
            DatasetNames = new List<string>();
        }

        public List<string> DatasetNames { get; set; }
        public AlignmentMeasurement<double> MassData { get; set; }
        public AlignmentMeasurement<double> NetData { get; set; }

        public SpectralOptions Options { get; set; }
        /// <summary>
        /// Gets or sets the list of matches.
        /// </summary>
        public IEnumerable<AnchorPointMatch> Matches { get; set; }

        
    }

}
