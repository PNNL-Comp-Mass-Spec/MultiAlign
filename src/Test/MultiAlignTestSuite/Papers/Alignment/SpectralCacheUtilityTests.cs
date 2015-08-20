#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.MTDB;
using MultiAlignCore.IO.RawData;
using MultiAlignTestSuite.Papers.Alignment.IO;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment
{
    [TestFixture]
    public class SpectralCacheUtilityTests
    {
        /// <summary>
        ///     Creates a cache file
        /// </summary>
        /// <param name="rawPath"></param>
        /// <param name="outPath"></param>
        public void CreateSpectraSummaryFile(string rawPath, string outPath)
        {
            using (var readerY = RawLoaderFactory.CreateFileReader(rawPath))
            {
                readerY.AddDataFile(rawPath, 0);
                var summary = readerY.GetScanData(0);


                ScanSummaryCache.WriteCache(outPath, summary, rawPath);
            }
        }

        [Test]
        [TestCase(@"M:\doc\papers\paperAlignment\Data\figure1\LargeScale",
            @"M:\doc\papers\paperAlignment\Data\figure1\100minute-released.txt",
            @"M:\doc\papers\paperAlignment\Data\figure1\LargeScale\dne"
            )]
        public void FixFolders(string directory, string path, string outpath)
        {
            var datasets = File.ReadAllLines(path);
            var files = Directory.GetFiles(directory);

            var maps = new Dictionary<string, string>();
            foreach (var dataset in datasets)
            {
                maps.Add(dataset.ToLower(), dataset);
            }

            foreach (var file in files)
            {
                var stripped = Path.GetFileNameWithoutExtension(file);
                stripped = stripped.ToLower().Replace("_msgfdb_fht", "");

                if (!maps.ContainsKey(stripped))
                {
                    File.Move(file, Path.Combine(outpath, Path.GetFileName(file)));
                }
            }
        }

        /// <summary>
        ///     Creates a cache file
        /// </summary>
        /// <param name="path">Path to raw files to create caches for </param>
        /// <param name="directory">output directory to put the cache files</param>
        [Test]
        [TestCase(@"M:\doc\papers\paperAlignment\Data\figure1\figure1-paths.txt",
            @"M:\doc\papers\paperAlignment\Data\figure1\",
            Ignore = true)]
        [TestCase(
            @"M:\doc\papers\paperAlignment\Data\figure4\biorumenInput.txt",
            @"M:\doc\papers\paperAlignment\Data\figure4",
            Ignore = true)]
        public void CreateCacheFiles(string path, string directory)
        {
            var files = File.ReadAllLines(path);

            Parallel.ForEach(files,
                new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount - 2},
                x =>
                {
                    Console.WriteLine("Processing...." + x);

                    var newPath = Path.GetFileNameWithoutExtension(x);
                    CreateSpectraSummaryFile(x, Path.Combine(directory, newPath + ".mscache"));
                }
                );
        }

        [Test]
        [TestCase(@"M:\doc\papers\paperAlignment\Data\figure1\figure1-paths.txt",
            @"M:\doc\papers\paperAlignment\Data\figure1\")]
        public void DownloadRequiredMsgfPlusResults(string path, string directory)
        {
            var files = Directory.GetFiles(directory, "*.mscache");

            Parallel.ForEach(files,
                new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount - 2},
                x =>
                {
                    var name = Path.GetFileNameWithoutExtension(x);
                    var loader = new MageMSGFFinderLoader();
                    var names = loader.LoadFiles(name);

                    if (names != null && names.Count == 1)
                    {
                        var downloader = new DMSFileDownloader();
                        var newName = name + "_msgfdb_fht.txt";
                        var archiveName = Path.Combine(names[0], newName);
                        var fullPath = Path.Combine(directory, newName);
                        downloader.Download(archiveName, fullPath);
                    }
                }
                );
        }

        [Test]
        [TestCase(@"M:\doc\papers\paperAlignment\Data\figure1\Test")]
        public void DownloadRequiredIsos(string path, string directory)
        {
        }
    }
}