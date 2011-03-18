using System;
using PNNLProteomics.Data;

namespace PNNLProteomics.IO
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
