using System.Collections.Generic;
using PNNLOmics.Data;
using PNNLProteomics.Data.MassTags;

using MultiAlignEngine.MassTags;
namespace PNNLProteomics.IO.MTDB
{
    /// <summary>
    /// Interface for loading mass tag databases.
    /// </summary>
    public interface IMtdbLoader
    {
        /// <summary>
        /// Loads the database.
        /// </summary>
        /// <returns></returns>
        MassTagDatabase LoadDatabase();

        /// <summary>
        /// Gets or sets the database options.
        /// </summary>
        clsMassTagDatabaseOptions Options
        {
            get;
            set;
        }
    }
}
