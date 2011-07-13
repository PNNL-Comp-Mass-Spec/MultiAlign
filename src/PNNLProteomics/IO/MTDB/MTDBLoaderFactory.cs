using System;
using System.Collections.Generic;
using System.Text;

using PNNLProteomics.Data.MassTags;
using MultiAlignEngine.MassTags;

namespace PNNLProteomics.IO.MTDB
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
        public static MassTagDatabase LoadMassTagDB(clsMassTagDatabaseOptions options, MassTagDatabaseFormat format)            
        {
            MassTagDatabase database = null;
            IMtdbLoader loader       = null;

            //TODO: Finish setting up the mass tag database to load from a strategy based pattern.
            switch (format)
            {
                case MassTagDatabaseFormat.SQL:
                    loader = new MTSMassTagDatabaseLoader(options.mstrDatabase, options.mstrServer);
                    break;
                case MassTagDatabaseFormat.Access:
                    loader = new AccessMassTagDatabaseLoader(options.mstr_databaseFilePath);                    
                    break;
                default:                    
                    break;
            }

            if (loader == null)
            {
                throw new NullReferenceException("The type of mass tag database format is not supported: "  + format.ToString());
            }

            loader.Options  = options;
            database        = loader.LoadDatabase();            
            return database;
        }
    }
}
