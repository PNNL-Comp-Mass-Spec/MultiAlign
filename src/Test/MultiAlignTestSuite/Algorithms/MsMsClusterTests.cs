#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.SpectralProcessing;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.RawData;
using MultiAlignCore.IO.TextFiles;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.Algorithms
{
    [TestFixture]
    public sealed class MsMsClusterTests
    {
        public int CompareSpectra(Peptide p, MSSpectra s)
        {
            var matchingPeaks = 0;

            foreach (var point in p.Spectrum.Peaks)
            {
                var px = point.X;
                for (var i = 0; i < s.Peaks.Count - 1; i++)
                {
                    var iPoint = s.Peaks[i];
                    var jPoint = s.Peaks[i + 1];

                    if (px < jPoint.X && px >= iPoint.X)
                    {
                        if (iPoint.Y > 0)
                        {
                            matchingPeaks++;
                            break;
                        }
                    }
                }
            }

            return matchingPeaks;
        }


        [Test]
        [TestCase(
            "hideous-complexity-a-4",
            @"m:\data\proteomics\msmsalignment\HideousComplexity\Complex-plantHuman",
            @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\transition-mixture-light.csv",
            SequenceFileType.SkylineTransitionFile,
            @"M:\data\proteomics\MsMsAlignment\HideousComplexity\Complex-plantHuman\Align_complex_02_Run3_24Jul12_Falcon_12-06-01_isos.csv",
            @"M:\data\proteomics\MsMsAlignment\HideousComplexity\Complex-plantHuman\Align_complex_02_Run2_20Jul12_Falcon_12-06-01_isos.csv",
            .4,
            Ignore = true)]
        [TestCase(
            "hideous-complexity-a-8",
            @"m:\data\proteomics\msmsalignment\HideousComplexity\Complex-plantHuman",
            @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\transition-mixture-light.csv",
            SequenceFileType.SkylineTransitionFile,
            @"M:\data\proteomics\MsMsAlignment\HideousComplexity\Complex-plantHuman\Align_complex_02_Run3_24Jul12_Falcon_12-06-01_isos.csv",
            @"M:\data\proteomics\MsMsAlignment\HideousComplexity\Complex-plantHuman\Align_complex_02_Run2_20Jul12_Falcon_12-06-01_isos.csv",
            .8,
            Ignore = true)]
        [TestCase(
            "low-complexity-a-4",
            @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\",
            @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\transition-mixture-light.csv",
            SequenceFileType.SkylineTransitionFile,
            @"M:\data\proteomics\MsMsAlignment\LowComplexity\PepMix\Align_pepmix_01_Run2_30May12_Falcon_12-03-34_isos.csv",
            @"M:\data\proteomics\MsMsAlignment\LowComplexity\PepMix\Align_pepmix_01_Run1_30May12_Falcon_12-03-34_isos.csv",
            .4,
            Ignore = true)]
        [TestCase(
            "low-complexity-a-8",
            @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\",
            @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\transition-mixture-light.csv",
            SequenceFileType.SkylineTransitionFile,
            @"M:\data\proteomics\MsMsAlignment\LowComplexity\PepMix\Align_pepmix_01_Run2_30May12_Falcon_12-03-34_isos.csv",
            @"M:\data\proteomics\MsMsAlignment\LowComplexity\PepMix\Align_pepmix_01_Run1_30May12_Falcon_12-03-34_isos.csv",
            .8,
            Ignore = true)]
        public void ClusterMsMs(string name,
            string resultPath,
            string sequencePath,
            SequenceFileType type,
            string baseline,
            string features,
            double percent)
        {
            var baselineRaw = baseline.Replace("_isos.csv", ".raw");
            var featuresRaw = features.Replace("_isos.csv", ".raw");


            Console.WriteLine("Create Baseline Information");

            var baselineInfo = new DatasetInformation
            {
                DatasetId = 0,
            };

            baselineInfo.InputFiles.Add(new InputFile { Path = baseline, FileType = InputFileType.Features });
            baselineInfo.InputFiles.Add(new InputFile { Path = baselineRaw, FileType = InputFileType.Raw });
            baselineInfo.InputFiles.Add(new InputFile { Path = sequencePath, FileType = InputFileType.Sequence });

            Console.WriteLine("Create Alignee Information");
            var aligneeInfo = new DatasetInformation
            {
                DatasetId = 1,
            };

            aligneeInfo.InputFiles.Add(new InputFile { Path = features, FileType = InputFileType.Features });
            aligneeInfo.InputFiles.Add(new InputFile { Path = featuresRaw, FileType = InputFileType.Raw });
            aligneeInfo.InputFiles.Add(new InputFile { Path = sequencePath, FileType = InputFileType.Sequence });

            var reader = new MsFeatureLightFileReader();

            Console.WriteLine("Reading Baseline Features");
            var baselineMsFeatures = reader.ReadFile(baseline).ToList();
            baselineMsFeatures.ForEach(x => x.GroupId = baselineInfo.DatasetId);

            Console.WriteLine("Reading Alignee Features");
            var aligneeMsFeatures = reader.ReadFile(features).ToList();
            aligneeMsFeatures.ForEach(x => x.GroupId = aligneeInfo.DatasetId);


            var finder = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.TreeBased);
            var tolerances = new FeatureTolerances
            {
                Mass = 8,
                Net = .005
            };
            var options = new LcmsFeatureFindingOptions(tolerances);

            Console.WriteLine("Detecting Baseline Features");
            var baselineFeatures = finder.FindFeatures(baselineMsFeatures, options, null);

            Console.WriteLine("Detecting Alignee Features");
            var aligneeFeatures = finder.FindFeatures(aligneeMsFeatures, options, null);

            Console.WriteLine("Managing baseline and alignee features");
            baselineFeatures.ForEach(x => x.GroupId = baselineInfo.DatasetId);
            aligneeFeatures.ForEach(x => x.GroupId = aligneeInfo.DatasetId);

            Console.WriteLine("Clustering MS/MS Spectra");
            var clusterer = new MSMSClusterer();
            clusterer.MzTolerance = .5;
            clusterer.MassTolerance = 6;
            clusterer.SpectralComparer = new SpectralNormalizedDotProductComparer
            {
                TopPercent = percent
            };
            clusterer.SimilarityTolerance = .5;
            clusterer.ScanRange = 905;
            clusterer.Progress += clusterer_Progress;

            var allFeatures = new List<UMCLight>();
            allFeatures.AddRange(baselineFeatures);
            allFeatures.AddRange(aligneeFeatures);

            List<MsmsCluster> clusters = null;
            var spectraProviderCache = new SpectraProviderCache();
            spectraProviderCache.GetSpectraProvider(baselineInfo.RawFile.Path, baselineInfo.DatasetId);
            spectraProviderCache.GetSpectraProvider(aligneeInfo.RawFile.Path, aligneeInfo.DatasetId);


            clusters = clusterer.Cluster(allFeatures, spectraProviderCache);
            Console.WriteLine("Found {0} Total Clusters", clusters.Count);

            if (clusters != null)
            {
                var now = DateTime.Now;
                var testResultPath = string.Format("{7}\\{0}-results-{1}-{2}-{3}-{4}-{5}-{6}_scans.txt",
                    name,
                    now.Year,
                    now.Month,
                    now.Day,
                    now.Hour,
                    now.Minute,
                    now.Second,
                    resultPath
                    );
                using (TextWriter writer = File.CreateText(testResultPath))
                {
                    writer.WriteLine("[Data]");
                    writer.WriteLine("{0}", baseline);
                    writer.WriteLine("{0}", features);
                    writer.WriteLine("[Scans]");
                    writer.WriteLine();
                    foreach (var cluster in clusters)
                    {
                        var scanData = "";
                        if (cluster.Features.Count == 2)
                        {
                            foreach (var feature in cluster.Features)
                            {
                                scanData += string.Format("{0},", feature.Scan);
                            }
                            scanData += string.Format("{0}", cluster.MeanScore);

                            writer.WriteLine(scanData);
                        }
                    }
                }
                testResultPath = string.Format("{7}\\{0}-results-{1}-{2}-{3}-{4}-{5}-{6}.txt",
                    name,
                    now.Year,
                    now.Month,
                    now.Day,
                    now.Hour,
                    now.Minute,
                    now.Second,
                    resultPath
                    );
                using (TextWriter writer = File.CreateText(testResultPath))
                {
                    writer.WriteLine("[Data]");
                    writer.WriteLine("{0}", baseline);
                    writer.WriteLine("{0}", features);
                    writer.WriteLine("[Scans]");
                    foreach (var cluster in clusters)
                    {
                        var scanData = "";
                        var data = "";
                        foreach (var feature in cluster.Features)
                        {
                            scanData += string.Format("{0},", feature.Scan);
                            data += string.Format("{0},{1},{2},{3},{4},{5}",
                                feature.GroupId,
                                feature.Id,
                                feature.MassMonoisotopic,
                                feature.Mz,
                                feature.ChargeState,
                                feature.Scan);
                            foreach (var spectrum in feature.MSnSpectra)
                            {
                                foreach (var peptide in spectrum.Peptides)
                                {
                                    data += string.Format(",{0},{1}", peptide.Sequence, peptide.Score);
                                }
                            }
                        }
                        writer.WriteLine(scanData + "," + data);
                    }
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("[Clusters]");

                    foreach (var cluster in clusters)
                    {
                        writer.WriteLine("cluster id, cluster score");
                        writer.WriteLine("{0}, {1}", cluster.Id, cluster.MeanScore);
                        writer.WriteLine("feature dataset id, id, monoisotopic mass, mz, charge, scan, peptides");

                        foreach (var feature in cluster.Features)
                        {
                            var data = string.Format("{0},{1},{2},{3},{4},{5}",
                                feature.GroupId,
                                feature.Id,
                                feature.MassMonoisotopic,
                                feature.Mz,
                                feature.ChargeState,
                                feature.Scan);
                            foreach (var spectrum in feature.MSnSpectra)
                            {
                                foreach (var peptide in spectrum.Peptides)
                                {
                                    data += string.Format(",{0},{1}", peptide.Sequence, peptide.Score);
                                }
                            }
                            writer.WriteLine(data);
                        }
                    }
                }
            }
        }

        private static void MapSequencesToSpectra(List<MSFeatureLight> features, string path)
        {
            var reader = new MsgfReader { Delimiter = '\t' };
            var peptides = reader.ReadFile(path);

            var scanMap = new Dictionary<int, Peptide>();
            foreach (var p in peptides)
            {
                if (!scanMap.ContainsKey(p.Scan))
                {
                    scanMap.Add(p.Scan, p);
                }
            }

            foreach (var feature in features)
            {
                foreach (var spectrum in feature.MSnSpectra)
                {
                    var hasScan = scanMap.ContainsKey(spectrum.Scan);
                    if (hasScan)
                    {
                        spectrum.Peptides.Add(scanMap[spectrum.Scan]);
                    }
                }
            }
        }

        private void clusterer_Progress(object sender, ProgressNotifierArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}