using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.MTDB;
using NUnit.Framework;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlignTestSuite.IO.MTDB
{
    [TestFixture]
    public class MTDBLoaderTests
    {
        [Test]
        [TestCase("elmer", "MT_Human_Sarcopenia_MixedLC_P692", "mtdbTestParams.xml")] 
        public void OnlyLoadWithDriftTime(string server, string databaseName, string path)
        {
            MultiAlignAnalysis analysis                          = new MultiAlignAnalysis();
            analysis.Options.MassTagDatabaseOptions.OnlyLoadTagsWithDriftTime = true;
            
            analysis.MetaData.Database = new InputDatabase(MassTagDatabaseFormat.SQL);            
            analysis.MetaData.Database.DatabaseServer   = server;
            analysis.MetaData.Database.DatabaseName     = databaseName;

            MassTagDatabaseLoader   loader      = new MTSMassTagDatabaseLoader(databaseName, server);
            loader.Options                      = analysis.Options.MassTagDatabaseOptions;
            var         database                = loader.LoadDatabase();
            
            foreach(MassTagLight tag in database.MassTags)
            {
                Assert.IsTrue(tag.DriftTime > 0);
            }
        }
        [Test]
        [TestCase("elmer", "MT_Human_Sarcopenia_MixedLC_P692", "mtdbTestParams.xml")]
        public void LoadWithoutDriftTime(string server, string databaseName, string path)
        {
            MultiAlignAnalysis analysis                                         = new MultiAlignAnalysis();
            analysis.Options.MassTagDatabaseOptions.OnlyLoadTagsWithDriftTime   = false;

            analysis.MetaData.Database = new InputDatabase(MassTagDatabaseFormat.SQL);
            analysis.MetaData.Database.DatabaseServer = server;
            analysis.MetaData.Database.DatabaseName = databaseName;

            MassTagDatabaseLoader loader    = new MTSMassTagDatabaseLoader(databaseName, server);
            loader.Options                  = analysis.Options.MassTagDatabaseOptions;
            MassTagDatabase database        = loader.LoadDatabase();
            
            MassTagDatabaseLoader loader2   = new MTSMassTagDatabaseLoader(databaseName, server);
            loader2.Options                 = analysis.Options.MassTagDatabaseOptions;
            loader2.Options.OnlyLoadTagsWithDriftTime = true;
            MassTagDatabase database2        = loader.LoadDatabase();


            Assert.Greater(database.MassTags.Count, database2.MassTags.Count);
        }
    }
}
