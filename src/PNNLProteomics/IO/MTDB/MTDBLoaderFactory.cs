using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="options"></param>
        /// <returns></returns>
        public static clsMassTagDB LoadMassTagDB(clsMassTagDatabaseOptions options)            
        {
            clsMassTagDB database = null;
            
            //TODO: Make this load based on ACCESS, SQL, or TXT.  We are currently loading in a single object.  
            // This is a good example of HIGH coupling :(
            clsMTDBLoader loader = new clsMTDBLoader(options);
            database = loader.LoadMassTagDatabase();

            return database;
        }
    }
}
