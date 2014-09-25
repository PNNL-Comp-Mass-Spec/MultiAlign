using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.IO.Features;
using MultiAlignTestSuite.Papers.Alignment.IO;
using NUnit.Framework;
using PNNLOmics.Algorithms.Alignment.SpectralMatches;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;
using PNNLOmics.Algorithms.SpectralProcessing;

namespace AlignmentPaperTestSuite.Figures
{
    [TestFixture]
    public class Figure4: FigureBase
    {

        [TestCase(
            @"M:\doc\papers\paperAlignment\Data\figure4\15-timepoints",
            SpectralComparison.CosineDotProduct,
            .5, // mz bin size when retrieving spectra
            1, // m/z
            .25, // NET
            .6, // Similarity Cutoff Score            
            1, // MSGF+ Score
            .01, // Peptide FDR
            .8, // Ion Percent            
            32, // Required peaks
            "results-figure4-metaMatches",
            Ignore = false,
            Description =
                "This test case only looks at a subset of the large scale study to see if we can remove spectra with a low number of peaks"
            )]
        public void GenerateFigure4_MetaMatches(string directory,
            SpectralComparison comparerType,
            double mzBinSize,
            double mzTolerance,
            double netTolerance,
            double similarityScoreCutoff,
            double peptideScore,
            double peptideFdr,
            double ionPercent,
            int numberOfRequiredPeaks,
            string name)
        {
            AlignmentAnalysisWriterFactory.BasePath = @"M:\doc\papers\paperAlignment\Data\figure4";

            Console.WriteLine(@"Post-Pre Tests For {0}", directory);

            var cacheFiles = Directory.GetFiles(directory, "*.mscache");
            Console.WriteLine(@"Building data cache");
            var data = cacheFiles.Select(path => new FigureBase.PathCache { Cache = path }).ToList();

            // The options for the analysis 
            var options = new SpectralOptions
            {
                MzBinSize = mzBinSize,
                MzTolerance = mzTolerance,
                NetTolerance = netTolerance,
                SimilarityCutoff = similarityScoreCutoff,
                TopIonPercent = ionPercent,
                IdScore = peptideScore,
                ComparerType = comparerType,
                Fdr = peptideFdr,
                RequiredPeakCount = numberOfRequiredPeaks
            };

            var comparison = 0;
            for (var i = 0; i < data.Count; i++)
            {
                var cachex = data[i];
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                var rawPathX = ScanSummaryCache.ReadPath(cachex.Cache);
                var datasetX = new AlignmentDataset(rawPathX, "", cachex.Msgf);

                // create a raw file reader for the datasets
                using (var readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
                {
                    // wrap it in the cached object so we can load scan meta-data
                    var cacheReaderX = new RawLoaderCache(readerX);
                    var cacheDataX = ScanSummaryCache.ReadCache(cachex.Cache);

                    readerX.AddDataFile(rawPathX, 0);
                    cacheReaderX.AddCache(0, cacheDataX);

                    for (var j = i + 1; j < data.Count; j++)
                    {
                        var cachey = data[j];
                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        var rawPathY = ScanSummaryCache.ReadPath(cachey.Cache);
                        var datasetY = new AlignmentDataset(rawPathY, "", cachey.Msgf);

                        // create a raw file reader for the datasets
                        using (var readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                        {
                            // Then the writer for creating a report
                            var writer =
                                AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure3, name + comparison);
                            comparison++;

                            // wrap it in the cached object so we can load scan meta-data
                            var cacheReaderY = new RawLoaderCache(readerY);
                            var cacheDataY = ScanSummaryCache.ReadCache(cachey.Cache);
                            cacheReaderY.AddCache(0, cacheDataY);
                            readerY.AddDataFile(rawPathY, 0);
                            var names = new List<string> { data[i].Cache, data[j].Cache };

                            var analysis = MatchDatasets(comparerType,
                                readerX,
                                readerY,
                                options,
                                datasetX,
                                datasetY,
                                names);

                            AlignMatches(analysis, writer);
                            writer.Close();
                        }
                    }
                }
            }
        }

    }
}
