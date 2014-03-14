using MultiAlignCore.Data.MassTags;
using MultiAlignEngine.MassTags;

namespace MultiAlignCore.IO.MTDB
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
        Algorithms.Options.MassTagDatabaseOptions Options
        {
            get;
            set;
        }
    }
}
