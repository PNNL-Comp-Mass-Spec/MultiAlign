
namespace PNNLProteomics.Data.MassTags
{
    /// <summary>
    /// Format of the database.
    /// </summary>
    public enum MassTagDatabaseFormat
    {
        /// <summary>
        /// Local MTDB.
        /// </summary>
        Access,
        /// <summary>
        /// MTS enabled MTDB
        /// </summary>
        SQL,
        /// <summary>
        /// No database specified.
        /// </summary>
        None,
        /// <summary>
        /// Not used, but future local database format.
        /// </summary>
        Sqlite
    }
}
