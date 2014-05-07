using System;

using MultiAlignCore.Data.MassTags;
using MultiAlignEngine.MassTags;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Data.MassTags;

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
        public static MassTagDatabase LoadMassTagDB(InputDatabase databaseDefinition, Algorithms.Options.MassTagDatabaseOptions options)            
        {
            IMtdbLoader loader       = null;

            switch (databaseDefinition.DatabaseFormat)
            {
                case MassTagDatabaseFormat.DirectInfusionIms:
                    loader = new DriftTimeTextFileDatabaseLoader(databaseDefinition.LocalPath);
                    break;
                case MassTagDatabaseFormat.APE:
                    loader = new ApeMassTagDatabaseLoader(databaseDefinition.LocalPath);
                    break;
                case MassTagDatabaseFormat.SQL:
                    loader = new MTSMassTagDatabaseLoader(databaseDefinition.DatabaseName, databaseDefinition.DatabaseServer);
                    break;
                case MassTagDatabaseFormat.Sqlite:
                    loader = new SQLiteMassTagDatabaseLoader(databaseDefinition.LocalPath);
                    break;
                case MassTagDatabaseFormat.MetaSample:
                    loader = new MetaSampleDatbaseLoader(databaseDefinition.LocalPath);
                    break;               
            }

            if (loader == null)
            {
                throw new NullReferenceException("The type of mass tag database format is not supported: " + databaseDefinition.DatabaseFormat.ToString());
            }

            loader.Options  = options;
            return loader.LoadDatabase();                        
        }
    }
}
