﻿#region


#endregion

using FeatureAlignment.Data.MassTags;

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
    }
}