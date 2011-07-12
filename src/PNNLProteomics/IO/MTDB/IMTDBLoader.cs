using System.Collections.Generic;
using PNNLOmics.Data;

namespace PNNLProteomics.IO.MTDB
{
    /// <summary>
    /// Interface for loading mass tag databases.
    /// </summary>
    public interface IMtdbLoader
    {
        List<MassTag> GetMassTags();        
    }
}
