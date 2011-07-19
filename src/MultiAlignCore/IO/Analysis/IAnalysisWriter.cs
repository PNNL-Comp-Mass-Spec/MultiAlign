using System;
using System.Collections.Generic;

using MultiAlignEngine.Features;

using MultiAlignCore.Filters;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO
{    
    public interface IAnalysisWriter
    {
        void WriteAnalysis( string                    path, 
                            MultiAlignAnalysis        analysis, 
                            List<IFilter<clsUMC>>     umcFilters, 
                            List<IFilter<clsCluster>> clusterFilters);
    }
}
