using System;
using System.Collections.Generic;

using MultiAlignEngine.Features;

using PNNLProteomics.Filters;
using PNNLProteomics.Data.Analysis;

namespace MultiAlignWin.IO
{    
    public interface IAnalysisWriter
    {
        void WriteAnalysis( string                    path, 
                            MultiAlignAnalysis        analysis, 
                            List<IFilter<clsUMC>>     umcFilters, 
                            List<IFilter<clsCluster>> clusterFilters);
    }
}
