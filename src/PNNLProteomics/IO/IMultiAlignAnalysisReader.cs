using System;
using PNNLProteomics.Data.Analysis;

namespace PNNLProteomics.IO
{    
    /// <summary>
    /// 
    /// </summary>
    public interface IMultiAlignAnalysisReader
    {        
        /// <summary>
        /// Writes the analysis to the path provided.
        /// </summary>
        MultiAlignAnalysis ReadAnalysis(string path);        
    }
}
