
namespace MultiAlignCore.Data.MassTags
{
    /// <summary>
    /// Format of the database.
    /// </summary>
    public enum MassTagDatabaseFormat
    {
        /// <summary>
        /// MTS enabled MTDB
        /// </summary>
        SQL,
        /// <summary>
        /// No database specified.
        /// </summary>
        None,
        /// <summary>
        /// Local database format.
        /// </summary>
        Sqlite,
        /// <summary>
        /// Database built from a collection of samples using clusters.
        /// </summary>
        MetaSample,
        /// <summary>
        /// Ape enabled databases
        /// </summary>
        APE
    }

}
