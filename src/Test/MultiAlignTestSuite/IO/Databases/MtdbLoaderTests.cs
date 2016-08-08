#region

using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.MTDB;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.IO.Databases
{
    [TestFixture]
    public sealed class MtdbLoaderTests
    {
        /// <summary>
        /// Tests local database loading.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="format"></param>
        /// <param name="expectedNumberOfTargets"></param>
        [Test]
        [TestCase(@"m:\testDatabase-100-3.mtdb", MassTagDatabaseFormat.MtdbCreator, 100, Ignore = "Local test file")]
        public void TestLocalMtdbLoader(string path, MassTagDatabaseFormat format, int expectedNumberOfTargets)
        {
            var input = new InputDatabase
            {
                DatabaseFormat  = format,
                LocalPath       = path
            };

            var options = new MassTagDatabaseOptions();
            var loader = MtdbLoaderFactory.Create(input, options);
            Assert.NotNull(loader);

            var database = loader.LoadDatabase();
            Assert.NotNull(database);
            Assert.AreEqual(database.MassTags.Count, expectedNumberOfTargets);
        }
    }
}