using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.Algorithms.SpectralProcessing;

namespace MultiAlignTestSuite.Papers.Alignment.IO
{

    /// <summary>
    /// Writes data from an analysis for figure 1
    /// </summary>
    public class SpectralWriterFigureTwo : PaperFileWriter, ISpectralAnalysisWriter
    {
        public SpectralWriterFigureTwo(string name, string path)
            : base(name, path, false)
        {
        }

        /// <summary>
        /// Serializes the data provided.
        /// </summary>
        /// <param name="analysis"></param>
        public void Write(SpectralAnalysis analysis)
        {            
            WriteLine(string.Format("NET,{0}", analysis.Options.NetTolerance));
            WriteLine(string.Format("Mass,{0}", analysis.Options.MzTolerance));            
        }
    }
}
