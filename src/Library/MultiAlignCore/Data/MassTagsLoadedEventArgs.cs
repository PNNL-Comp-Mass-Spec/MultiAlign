#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.Data
{
    public sealed class MassTagsLoadedEventArgs : EventArgs
    {
        /// <summary>
        ///     Arguments that hold dataset information when features are loaded.
        /// </summary>
        public MassTagsLoadedEventArgs(List<MassTagLight> tags, MassTagDatabase database)
        {
            MassTags = tags;
            Database = database;
        }

        /// <summary>
        ///     Gets the dataset information.
        /// </summary>
        public List<MassTagLight> MassTags { get; private set; }

        public MassTagDatabase Database { get; private set; }
    }
}