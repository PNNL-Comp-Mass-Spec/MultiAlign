using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PNNLOmics.Data;
using MultiAlignCore.IO.Features;
using MultiAlignTestSuite.Papers.Alignment.IO;
using System.IO;
using System.Threading.Tasks;
using MultiAlignCore.IO.MTDB;

namespace MultiAlignTestSuite.Papers.Alignment
{

    [TestFixture]
    public class SpectralCacheUtility
    {
        /// <summary>
        /// Creates a cache file 
        /// </summary>
        /// <param name="rawPath"></param>
        /// <param name="outPath"></param>
        public void CreateSpectraSummaryFile(string rawPath, string outPath)
        {

            using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(rawPath))
            {
                readerY.AddDataFile(rawPath, 0);
                Dictionary<int, ScanSummary> summary = readerY.GetScanData(0);


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
            string[] datasets = File.ReadAllLines(path);
            string[] files    = Directory.GetFiles(directory);

            Dictionary<string, string> maps = new Dictionary<string, string>();
            foreach (var dataset in datasets)
            {
                maps.Add(dataset.ToLower(), dataset);
            }

            foreach (var file in files)
            {
                string stripped = Path.GetFileNameWithoutExtension(file);
                stripped = stripped.ToLower().Replace("_msgfdb_fht", "");                

                if (!maps.ContainsKey(stripped))
                {
                    File.Move(file,  Path.Combine(outpath, Path.GetFileName(file)));
                }
            }
        }

        /// <summary>
        /// Creates a cache file 
        /// </summary>
        /// <param name="path">Path to raw files to create caches for </param>
        /// <param name="directory">output directory to put the cache files</param>
        [Test]
        [TestCase(@"M:\doc\papers\paperAlignment\Data\figure1\figure1-paths.txt",
            @"M:\doc\papers\paperAlignment\Data\figure1\",
            Ignore=true)]

        [TestCase(@"M:\doc\papers\paperAlignment\Data\figure1\Figure\figure1-figurePath.txt",
                  @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\")]
        public void CreateCacheFiles(string path, string directory)
        {
            string[] files = File.ReadAllLines(path);

            Parallel.ForEach(files, 
                new ParallelOptions(){ MaxDegreeOfParallelism=Environment.ProcessorCount -2 },                
                x =>
                {
                    Console.WriteLine("Processing...." + x);

                    string newPath = Path.GetFileNameWithoutExtension(x);
                    CreateSpectraSummaryFile(x, Path.Combine(directory, newPath + ".mscache"));
                }
            
            );
        }
        [Test]
        [TestCase(  @"M:\doc\papers\paperAlignment\Data\figure1\figure1-paths.txt",
                    @"M:\doc\papers\paperAlignment\Data\figure1\")]
        public void DownloadRequiredMsgfPlusResults(string path, string directory)
        {
            string[] files = Directory.GetFiles(directory, "*.mscache");
            
            Parallel.ForEach(files,
                    new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount - 2 },
                    x =>
                    {
                        string name = Path.GetFileNameWithoutExtension(x);
                        MageMSGFFinderLoader loader = new MageMSGFFinderLoader();
                        List<string> names = loader.LoadFiles(name);

                        if (names != null && names.Count == 1)
                        {
                            DMSFileDownloader downloader = new DMSFileDownloader();
                            string newName      = name + "_msgfdb_fht.txt";
                            string archiveName  = Path.Combine(names[0], newName); 
                            string fullPath     = Path.Combine(directory, newName);
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
