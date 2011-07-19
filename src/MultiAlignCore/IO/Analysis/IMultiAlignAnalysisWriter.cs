using System;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMultiAlignAnalysisWriter
    {
        /// <summary>
        /// Writes the analysis to the path provided.
        /// </summary>
        void WriteAnalysis(string path, MultiAlignAnalysis analysis);
    }
}
