using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    /// Analysis Type to be performed.  Some types are modifications to the 
    /// </summary>
    public enum AnalysisType
    {
        /// <summary>
        /// Import factors into a database.
        /// </summary>
        FactorImporting,
        /// <summary>
        /// Do everything, as is tradition.
        /// </summary>
        Full,          
        /// <summary>
        /// Export data only.
        /// </summary>
        ExportDataOnly,
        /// <summary>
        /// Invalid analysis specified.
        /// </summary>
        InvalidParameters        
    }
    
}
