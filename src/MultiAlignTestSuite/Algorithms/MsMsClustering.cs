using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MultiAlignCore.Algorithms.FeatureFinding;
using PNNLOmicsIO.IO;
using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms.FeatureClustering;
using MultiAlignCore.Data;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Data;
using MultiAlignCore.IO.Features;
using MultiAlignCore.Algorithms;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Algorithms.FeatureMatcher.MSnLinker;
using PNNLOmics.Algorithms;
using System.IO;

namespace MultiAlignTestSuite.Algorithms
{
    public class Triplet
    {
        public MSSpectra        Spectrum;
        public MSFeatureLight   Feature;
        public int              MatchingPeaks;
        public double           Error;
    }
    [TestFixture]
    public class MsMsClusterTests
    {
        public int CompareSpectra(Peptide p, MSSpectra s)
        {
            int matchingPeaks = 0;

            foreach (XYData point in p.Spectrum.Peaks)
            {
                double px = point.X;
                for(int i = 0; i < s.Peaks.Count - 1; i++)
                {
                    XYData iPoint = s.Peaks[i];
                    XYData jPoint = s.Peaks[i + 1];
                    
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

        public int LinkSpectraToPeptides(List<Peptide> peptides, 
                                         List<MSFeatureLight> features,
                                         string rawPath,
                                         int datasetId,
                                         double tol)
        {
            Dictionary<string, List<Triplet>> matches = new Dictionary<string, List<Triplet>>();

            int linked = 0;
            using (ThermoRawDataFileReader reader = new ThermoRawDataFileReader())
            {
                reader.AddDataFile(rawPath, datasetId);

                // Brute force approach to finding MSMS spectra peptide id's
                foreach (MSFeatureLight feature in features)
                {
                    foreach (MSSpectra spectrum in feature.MSnSpectra)
                    {
                        foreach (Peptide peptide in peptides)
                        {
                            double ppmError = Feature.ComputeMassPPMDifference(
                                                            peptide.Spectrum.PrecursorMZ,
                                                            feature.Mz);

                            if (Math.Abs(ppmError) < tol)
                            {
                                Console.WriteLine("\tDiff = {0} Spectra ID = {4} Sequence = {1}, peptide mz = {2},  spectrum mz = {3}",
                                                ppmError,
                                                peptide.Sequence,
                                                peptide.Spectrum.PrecursorMZ,
                                                feature.Mz,
                                                spectrum.ID);

                                List<XYData> spectra = reader.GetRawSpectra(spectrum.Scan, feature.GroupID);
                                spectrum.Peaks       = spectra;

                                int totalIonsMatcing = CompareSpectra(peptide, spectrum);

                                if (!matches.ContainsKey(peptide.Sequence))
                                {
                                    matches.Add(peptide.Sequence, new List<Triplet>());
                                }
                                Triplet triplet = new Triplet();
                                triplet.Feature         = feature;
                                triplet.Error           = ppmError;
                                triplet.MatchingPeaks   = totalIonsMatcing;
                                triplet.Spectrum        = spectrum;

                                matches[peptide.Sequence].Add(triplet);
                                linked++;
                                spectrum.Peptide.Sequence = peptide.Sequence;
                            }
                        }
                    }
                }
            }

            foreach (string peptide in matches.Keys)
            {
                Console.WriteLine("peptide: {0}", peptide);
                foreach (Triplet key in matches[peptide])
                {
                    Console.WriteLine("\tFeature ID = {0}\tSpectra ID = {1}\tPrecursor = {6}\tScan = {2}\tRetention Time = {5}\tTotal Ions = {4}\tError = {3}", 
                                                                                            key.Feature.ID,
                                                                                            key.Spectrum.ID,
                                                                                            key.Feature.Scan,
                                                                                            key.Error,
                                                                                            key.MatchingPeaks,
                                                                                            key.Spectrum.RetentionTime,
                                                                                            key.Feature.Mz);                    
                }
            }
            return linked;
        }

        [Test]
        [TestCase(
                "hideous-complexity-a-4",
                @"m:\data\proteomics\msmsalignment\HideousComplexity\Complex-plantHuman",
                @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\transition-mixture-light.csv",
                SequenceFileType.SkylineTransitionFile,
                @"M:\data\proteomics\MsMsAlignment\HideousComplexity\Complex-plantHuman\Align_complex_02_Run3_24Jul12_Falcon_12-06-01_isos.csv",
                @"M:\data\proteomics\MsMsAlignment\HideousComplexity\Complex-plantHuman\Align_complex_02_Run2_20Jul12_Falcon_12-06-01_isos.csv",
                .4
                )]
        [TestCase(
                "hideous-complexity-a-8",
                @"m:\data\proteomics\msmsalignment\HideousComplexity\Complex-plantHuman",
                @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\transition-mixture-light.csv",
                SequenceFileType.SkylineTransitionFile,
                @"M:\data\proteomics\MsMsAlignment\HideousComplexity\Complex-plantHuman\Align_complex_02_Run3_24Jul12_Falcon_12-06-01_isos.csv",
                @"M:\data\proteomics\MsMsAlignment\HideousComplexity\Complex-plantHuman\Align_complex_02_Run2_20Jul12_Falcon_12-06-01_isos.csv",
                .8
                )]
        
        [TestCase(
                "low-complexity-a-4",
                @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\",
                @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\transition-mixture-light.csv",
                SequenceFileType.SkylineTransitionFile,
                @"M:\data\proteomics\MsMsAlignment\LowComplexity\PepMix\Align_pepmix_01_Run2_30May12_Falcon_12-03-34_isos.csv",
                @"M:\data\proteomics\MsMsAlignment\LowComplexity\PepMix\Align_pepmix_01_Run1_30May12_Falcon_12-03-34_isos.csv",
                .4)]
        [TestCase(
                "low-complexity-a-8",
                @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\",
                @"m:\data\proteomics\msmsalignment\LowComplexity\PepMix\transition-mixture-light.csv",
                SequenceFileType.SkylineTransitionFile,
                @"M:\data\proteomics\MsMsAlignment\LowComplexity\PepMix\Align_pepmix_01_Run2_30May12_Falcon_12-03-34_isos.csv",
                @"M:\data\proteomics\MsMsAlignment\LowComplexity\PepMix\Align_pepmix_01_Run1_30May12_Falcon_12-03-34_isos.csv",
                .8)]
        public void ClusterMsMs(string name, 
                                string resultPath,
                                string sequencePath,
                                SequenceFileType type,
                                string baseline,
                                string features,
                                double percent)
        {
            string baselineRaw              = baseline.ToString().Replace("_isos.csv", ".raw");
            string sequencesBaselinePath    = baseline.ToString().Replace("_isos.csv", "_xt_MSGF.txt");
            string featuresRaw              = features.ToString().Replace("_isos.csv", ".raw");
            string sequencesFeaturesPath    = features.ToString().Replace("_isos.csv", "_xt_MSGF.txt");
            
            Console.WriteLine("Create Baseline Information");        

            DatasetInformation baselineInfo = new DatasetInformation();
            baselineInfo.DatasetId          = 0;
            baselineInfo.Features           = new InputFile();
            baselineInfo.Features.Path      = baseline;
            baselineInfo.Raw                = new InputFile();
            baselineInfo.Raw.Path           = baselineRaw;
            baselineInfo.Sequence           = new InputFile();
            baselineInfo.Sequence.Path      = sequencePath;

            Console.WriteLine("Create Alignee Information");        
            DatasetInformation aligneeInfo  = new DatasetInformation();
            aligneeInfo.DatasetId           = 1;
            aligneeInfo.Features            = new InputFile();
            aligneeInfo.Features.Path       = features;
            aligneeInfo.Raw                 = new InputFile();
            aligneeInfo.Raw.Path            = featuresRaw;
            aligneeInfo.Sequence            = new InputFile();            
            aligneeInfo.Sequence.Path       = sequencePath;

            MSFeatureLightFileReader reader       = new MSFeatureLightFileReader();

            Console.WriteLine("Reading Baseline Features");
            List<MSFeatureLight> baselineMsFeatures = reader.ReadFile(baseline).ToList();
            baselineMsFeatures.ForEach(x => x.GroupID = baselineInfo.DatasetId);          
            LinkMsMsSpectra(baselineInfo, baselineMsFeatures);

            Console.WriteLine("Reading Alignee Features");
            List<MSFeatureLight> aligneeMsFeatures  = reader.ReadFile(features).ToList();
            aligneeMsFeatures.ForEach(x => x.GroupID = aligneeInfo.DatasetId);
            LinkMsMsSpectra(aligneeInfo, aligneeMsFeatures);

            LCMSFeatureFindingOptions options   = new LCMSFeatureFindingOptions();
            UMCFeatureFinder finder             = new UMCFeatureFinder();

            Console.WriteLine("Reading Baseline Sequence Files");
            ISequenceFileReader sequenceReader  = PeptideReaderFactory.CreateReader(type);
            List<Peptide> baseLinePeptides      = sequenceReader.Read(baselineInfo.Sequence.Path).ToList();

            Console.WriteLine("Reading Alignee Sequence Files");
            List<Peptide> aligneePeptides       = sequenceReader.Read(aligneeInfo.Sequence.Path).ToList();

            double ppmTol   = .5;
            Console.WriteLine("Linking Known Peptide Precursors to Alignee MS/MS Spectra");
            int totalLinked = LinkSpectraToPeptides(aligneePeptides, 
                                                    aligneeMsFeatures, 
                                                    aligneeInfo.Raw.Path,
                                                    aligneeInfo.DatasetId,
                                                    ppmTol);
            Console.WriteLine("Alignee Linked: {0}", totalLinked);

            Console.WriteLine("Linking Known Peptide Precursors to Baseline MS/MS Spectra");
            totalLinked     = LinkSpectraToPeptides(baseLinePeptides,
                                                    baselineMsFeatures,
                                                    baselineInfo.Raw.Path,
                                                    baselineInfo.DatasetId, 
                                                    ppmTol);
            Console.WriteLine("Baseline Linked: {0}", totalLinked);

            Console.WriteLine("Detecting Baseline Features");
            List<UMCLight> baselineFeatures     = finder.FindFeatures(baselineMsFeatures, options);
            
            Console.WriteLine("Detecting Alignee Features");
            List<UMCLight> aligneeFeatures = finder.FindFeatures(aligneeMsFeatures, options);

            Console.WriteLine("Managing baseline and alignee features");        
            baselineFeatures.ForEach(x  => x.GroupID = baselineInfo.DatasetId);
            aligneeFeatures.ForEach(x   => x.GroupID = aligneeInfo.DatasetId);

            Console.WriteLine("Clustering MS/MS Spectra");        
            MSMSClusterer clusterer         = new MSMSClusterer();
            clusterer.MzTolerance           = .5;
            clusterer.MassTolerance         = 6;
            clusterer.SpectralComparer      = new SpectralNormalizedDotProductComparer()
            {
                TopPercent = percent
            };
            clusterer.SimilarityTolerance   = .5;            
            clusterer.ScanRange             = 905;                        
            clusterer.Progress              += new EventHandler<ProgressNotifierArgs>(clusterer_Progress);

            List<UMCLight> allFeatures      = new List<UMCLight>();
            allFeatures.AddRange(baselineFeatures);
            allFeatures.AddRange(aligneeFeatures);

            List<MSMSCluster> clusters      = null;
            using (ThermoRawDataFileReader rawReader = new ThermoRawDataFileReader())
            {
                rawReader.AddDataFile(baselineInfo.Raw.Path, baselineInfo.DatasetId);
                rawReader.AddDataFile(aligneeInfo.Raw.Path,  aligneeInfo.DatasetId);
                 
                clusters = clusterer.Cluster(allFeatures, rawReader);
                Console.WriteLine("Found {0} Total Clusters", clusters.Count);                
            }

            if (clusters != null)
            {
                DateTime now                = DateTime.Now;
                string testResultPath       = string.Format("{7}\\{0}-results-{1}-{2}-{3}-{4}-{5}-{6}_scans.txt", 
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
                    foreach (MSMSCluster cluster in clusters)
                    {
                        string scanData = "";
                        if (cluster.Features.Count == 2)
                        {
                            foreach (MSFeatureLight feature in cluster.Features)
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
                    foreach (MSMSCluster cluster in clusters)
                    {
                        string scanData = "";
                        string data = "";
                        foreach (MSFeatureLight feature in cluster.Features)
                        {
                            scanData += string.Format("{0},", feature.Scan);
                            data     += string.Format("{0},{1},{2},{3},{4},{5}",
                                                            feature.GroupID,
                                                            feature.ID,
                                                            feature.MassMonoisotopic,
                                                            feature.Mz,
                                                            feature.ChargeState,
                                                            feature.Scan);
                            foreach (MSSpectra spectrum in feature.MSnSpectra)
                            {
                                data += string.Format(",{0},{1}", spectrum.Peptide.Sequence, spectrum.Peptide.Score);
                            }
                        }
                        writer.WriteLine(scanData + "," + data);
                    }
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("[Clusters]");                    

                    foreach (MSMSCluster cluster in clusters)
                    {                        
                        writer.WriteLine("cluster id, cluster score");                                               
                        writer.WriteLine("{0}, {1}", cluster.ID, cluster.MeanScore);
                        writer.WriteLine("feature dataset id, id, monoisotopic mass, mz, charge, scan, peptides");

                        foreach(MSFeatureLight feature in cluster.Features)
                        {
                            string data = string.Format("{0},{1},{2},{3},{4},{5}",
                                                            feature.GroupID,
                                                            feature.ID,
                                                            feature.MassMonoisotopic,
                                                            feature.Mz,
                                                            feature.ChargeState,
                                                            feature.Scan);
                            foreach (MSSpectra spectrum in feature.MSnSpectra)
                            {
                                data += string.Format(",{0},{1}", spectrum.Peptide.Sequence, spectrum.Peptide.Score);
                            }
                            writer.WriteLine(data);
                        }
                    }


                }
            }
        }
        
        private static void MapSequencesToSpectra(List<MSFeatureLight> features, string path)
        {
            PNNLOmicsIO.IO.MsgfReader reader = new MsgfReader();
            reader.Delimeter                 = "\t";
            IEnumerable<Peptide> peptides    = reader.ReadFile(path);

            Dictionary<int, Peptide> scanMap = new Dictionary<int, Peptide>();
            foreach (Peptide p in peptides)
            {
                if (!scanMap.ContainsKey(p.Scan))
                {
                    scanMap.Add(p.Scan, p);
                }
            }

            foreach (MSFeatureLight feature in features)
            {
                foreach (MSSpectra spectrum in feature.MSnSpectra)
                {
                    bool hasScan = scanMap.ContainsKey(spectrum.Scan);
                    if (hasScan)
                    {
                        spectrum.Peptide = scanMap[spectrum.Scan];
                    }
                }
            }
        }
        private static void LinkMsMsSpectra(DatasetInformation baselineInfo,
                                            List<MSFeatureLight> baselineMsFeatures)
        {
            IMSnLinker linker       = MSnLinkerFactory.CreateLinker(MSnLinkerType.BoxMethod);
            linker.Tolerances       = new PNNLOmics.Algorithms.FeatureTolerances();
            linker.Tolerances.Mass  = .5;

            Console.WriteLine("Coordinating Dataset MS/MS Spectra");
            using (ThermoRawDataFileReader rawReader = new ThermoRawDataFileReader())
            {

                rawReader.AddDataFile(baselineInfo.Raw.Path, baselineInfo.DatasetId);

                Console.WriteLine("Reading MS/MS Spectra Header Information");
                List<MSSpectra> spectra = rawReader.GetMSMSSpectra(baselineInfo.DatasetId);
                Console.WriteLine("Found {0} MS/MS Spectra", spectra.Count);

                Console.WriteLine("Assigning indices to MS/MS spectra");
                int i = 0;
                spectra.ForEach(x => x.ID = i++);
                spectra.ForEach(x => x.GroupID = baselineInfo.DatasetId);

                Console.WriteLine("Linking MS/MS Spectra");
                Dictionary<int, int> linked = linker.LinkMSFeaturesToMSn(baselineMsFeatures, spectra);

                Console.WriteLine("Linked {0} total MS/MS Spectra", linked.Keys.Count);
            }
        }

        void clusterer_Progress(object sender, PNNLOmics.Algorithms.ProgressNotifierArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
