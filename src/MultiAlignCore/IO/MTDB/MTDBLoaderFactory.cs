using System;

using MultiAlignCore.Data.MassTags;
using MultiAlignEngine.MassTags;

namespace MultiAlignCore.IO.MTDB
{
    /// <summary>
    /// Class that loads Mass Tag databases from a given source.
    /// </summary>
    public class MTDBLoaderFactory
    {
        /// <summary>
        /// Loads a mass tag database.
        /// </summary>
        /// <param name="options">Loading options.</param>
        /// <returns>The mass tag database.</returns>
        public static MassTagDatabase LoadMassTagDB(MassTagDatabaseOptions options)            
        {
            MassTagDatabase database = null;
            IMtdbLoader loader       = null;

            
            switch (options.DatabaseType)
            {
                case MassTagDatabaseType.APE:
                    loader = new ApeMassTagDatabaseLoader(options.DatabaseFilePath);
                    break;
                case MassTagDatabaseType.SQL:
                    loader = new MTSMassTagDatabaseLoader(options.DatabaseName, options.Server);
                    break;
                case MassTagDatabaseType.ACCESS:
                    loader = new AccessMassTagDatabaseLoader(options.DatabaseFilePath);                    
                    break;
                case MassTagDatabaseType.SQLite:
                    loader = new SQLiteMassTagDatabaseLoader(options.DatabaseFilePath);
                    break;
                case MassTagDatabaseType.MetaSample:
                    loader = new MetaSampleDatbaseLoader(options.DatabaseFilePath);
                    break;
                default:                    
                    break;
            }

            if (loader == null)
            {
                throw new NullReferenceException("The type of mass tag database format is not supported: "  + options.DatabaseType.ToString());
            }

            loader.Options  = options;
            database        = loader.LoadDatabase();            
            return database;
        }
    }
}
