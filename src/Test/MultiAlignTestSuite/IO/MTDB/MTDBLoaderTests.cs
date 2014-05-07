using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.MTDB;
using NUnit.Framework;

namespace MultiAlignTestSuite.IO.MTDB
{
    [TestFixture]
    public sealed class MtdbLoaderTests
    {
        [Test]
        [TestCase("elmer", "MT_Human_Sarcopenia_MixedLC_P692", Description = "MTS Based Tests")] 
        public void OnlyLoadWithDriftTime(string server, string databaseName)
        {
            var analysis                          = new MultiAlignAnalysis();
            analysis.Options.MassTagDatabaseOptions.OnlyLoadTagsWithDriftTime = true;
            
            analysis.MetaData.Database = new InputDatabase(MassTagDatabaseFormat.MassTagSystemSql)
            {
                DatabaseServer = server,
                DatabaseName = databaseName
            };

            var loader      = new MTSMassTagDatabaseLoader(databaseName, server, analysis.Options.MassTagDatabaseOptions);            
            var database    = loader.LoadDatabase();
            
            foreach(var tag in database.MassTags)
            {
                Assert.IsTrue(tag.DriftTime > 0);
            }
        }
        [Test]
        [TestCase("elmer", "MT_Human_Sarcopenia_MixedLC_P692")]
        public void LoadWithoutDriftTime(string server, string databaseName)
        {
            var analysis                                         = new MultiAlignAnalysis();
            analysis.Options.MassTagDatabaseOptions.OnlyLoadTagsWithDriftTime   = false;

            analysis.MetaData.Database = new InputDatabase(MassTagDatabaseFormat.MassTagSystemSql)
            {
                DatabaseServer = server,
                DatabaseName = databaseName
            };

            MassTagDatabaseLoader loader = new MTSMassTagDatabaseLoader(databaseName, server, analysis.Options.MassTagDatabaseOptions);            
            var database                 = loader.LoadDatabase();

            MassTagDatabaseLoader loader2   = new MTSMassTagDatabaseLoader(databaseName, server, analysis.Options.MassTagDatabaseOptions);            
            loader2.Options.OnlyLoadTagsWithDriftTime = true;
            var database2                   = loader.LoadDatabase();


            Assert.Greater(database.MassTags.Count, database2.MassTags.Count);
        }
        [Test]
        [TestCase(@"m:\testDatabase.mtdb", MassTagDatabaseFormat.MtdbCreator)]
        public void TestLocalMtdbLoader(string path, MassTagDatabaseFormat format)
        {
            var input = new InputDatabase
            {
                DatabaseFormat  = format,
                LocalPath       = path
            };

            var options  = new MassTagDatabaseOptions();
            var loader   = MtdbLoaderFactory.Create(input, options);
            Assert.NotNull(loader);

            var database = loader.LoadDatabase();
            Assert.NotNull(database);
            Assert.Greater(database.MassTags.Count, 100);
        }
    }
}
