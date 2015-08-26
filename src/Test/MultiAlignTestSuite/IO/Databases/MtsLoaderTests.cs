#region

using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.MTDB;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.IO.Databases
{
    [TestFixture]
    public sealed class MtsLoaderTests
    {
        [Test]
        [TestCase("elmer", "MT_Human_Sarcopenia_P789", Description = "MTS Based Tests")]
        public void OnlyLoadWithDriftTime(string server, string databaseName)
        {
            var analysis = new MultiAlignAnalysis();
            analysis.Options.MassTagDatabaseOptions.OnlyLoadTagsWithDriftTime = true;

            analysis.MetaData.Database = new InputDatabase(MassTagDatabaseFormat.MassTagSystemSql)
            {
                DatabaseServer = server,
                DatabaseName = databaseName
            };

            var loader = new MtsMassTagDatabaseLoader(databaseName, server, analysis.Options.MassTagDatabaseOptions);
            var database = loader.LoadDatabase();

            foreach (var tag in database.MassTags)
            {
                Assert.IsTrue(tag.DriftTime > 0);
            }
        }

        [Test]
        [TestCase("elmer", "MT_Human_Sarcopenia_P789")]
        public void LoadWithoutDriftTime(string server, string databaseName)
        {
            var analysis = new MultiAlignAnalysis();
            analysis.Options.MassTagDatabaseOptions.OnlyLoadTagsWithDriftTime = false;

            analysis.MetaData.Database = new InputDatabase(MassTagDatabaseFormat.MassTagSystemSql)
            {
                DatabaseServer = server,
                DatabaseName = databaseName
            };

            MassTagDatabaseLoader loader = new MtsMassTagDatabaseLoader(databaseName, server,
                analysis.Options.MassTagDatabaseOptions);
            var database = loader.LoadDatabase();

            MassTagDatabaseLoader loader2 = new MtsMassTagDatabaseLoader(databaseName, server,
                analysis.Options.MassTagDatabaseOptions);
            loader2.Options.OnlyLoadTagsWithDriftTime = true;
            var database2 = loader.LoadDatabase();


            Assert.Greater(database.MassTags.Count, database2.MassTags.Count);
        }
    }
}