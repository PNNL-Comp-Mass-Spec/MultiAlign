
using System;

namespace MultiAlignCore.Data.MassTags
{
    /// <summary>
    /// Format of the database.
    /// </summary>
    public enum MassTagDatabaseFormat
    {
        /// <summary>
        /// Mass Tag System (MTS) based mass tag databases
        /// </summary>
        MassTagSystemSql,
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
        /// </summary>        ]
        DelimitedTextFile,
        /// <summary>
        /// APE created databases
        /// </summary>
        [Obsolete]
        Ape,
        /// <summary>
        /// For Direct Infusionn based experiments, where no alignment should be made.
        /// </summary>
        SkipAlignment,
        /// <summary>
        /// New version of Mass Tag Database Creator databases.
        /// </summary>
        MtdbCreator
    }
}
