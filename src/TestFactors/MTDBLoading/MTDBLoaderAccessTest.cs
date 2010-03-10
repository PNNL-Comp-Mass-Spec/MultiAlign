using System;
using System.Collections.Generic;

using MultiAlignEngine.MassTags;

namespace Test.MTDBLoading
{
    public class MTDBLoaderAccessTest
    {
        public void TestAccess()
        {
            clsMassTagDatabaseOptions options = new clsMassTagDatabaseOptions();
            options.mstr_databaseFilePath = @"C:\Documents and Settings\d3m276\Desktop\QCShewAndEColiK12.mdb";
            options.mintMinObservationCountFilter = 0;
            options.mfltMinXCorr = 0;
            options.mdblMinDiscriminant = 0;
            options.mbyteNETValType = 0;
            options.mdblPeptideProphetVal = 0;
            options.mdecimalMinPMTScore = 0;

            clsMTDBLoader loader    = new clsMTDBLoader(options);
            clsMassTagDB  database  = loader.LoadMassTagDatabase();

            Console.WriteLine(database.GetMassTagCount().ToString());
        }

        public void RunTests()
        {
            TestAccess();
        }
    }
}
