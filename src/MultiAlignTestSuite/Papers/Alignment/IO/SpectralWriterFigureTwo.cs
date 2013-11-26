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
    public class SpectralWriterFigureTwo : ISpectralAnalysisWriter
    {
        public string Path { get; set; }

        /// <summary>
        /// Serializes the data provided.
        /// </summary>
        /// <param name="analysis"></param>
        public void Write(SpectralAnalysis analysis)
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.WriteLine("NET,{0}", analysis.Options.NetTolerance);
            Console.WriteLine("Mass,{0}", analysis.Options.MzTolerance);            
        }
    }
}
