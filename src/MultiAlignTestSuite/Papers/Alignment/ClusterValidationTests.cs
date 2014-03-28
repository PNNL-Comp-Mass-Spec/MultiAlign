using AForge;
using MultiAlign.IO;
using MultiAlign.ViewModels.Charting;
using MultiAlign.ViewModels.Instruments;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Algorithms.Workflow;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using MultiAlignEngine.Alignment;
using NUnit.Framework;
using OxyPlot.WindowsForms;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.Alignment.SpectralMatches;
using PNNLOmics.Algorithms.Distance;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PNNLOmicsIO.IO;
using PNNLOmics.Utilities;

namespace MultiAlignTestSuite.Papers.Alignment
{
    [TestFixture]
    public class Figure4ClusterValidationTests : WorkflowBase
    {
        [Test]
        [TestCase(@"M:\data\proteomics\Papers\AlignmentPaper\data\annotated-QC-6\test-annotated-6.db3", 2)]
        public void TestClusters(string databasetPath, int minMsMsCount)
        {

            var providers = DataAccessFactory.CreateDataAccessProviders(databasetPath, false);            
            var clusters = providers.ClusterCache.FindAll();
            
            Console.WriteLine(@"Cluster ID\tCluster Size\tMs Ms Total\tMatching");
            foreach (var cluster in clusters)
            {
                var clusterPeptideMap = new ClusterIdentificationStatistic();

                if (cluster.MsMsCount > minMsMsCount)

                cluster.ReconstructUMCCluster(providers, true, true);

                var hasIdentifications = false;
                foreach (var umc in cluster.UMCList)
                {                    
                    foreach (var feature in umc.Features)
                    {
                        foreach (var spectrum in feature.MSnSpectra)
                        {
                            foreach (var peptide in spectrum.Peptides)
                            {
                                var sequence = peptide.Sequence;
                                if (!clusterPeptideMap.Peptides.ContainsKey(sequence))
                                {
                                    clusterPeptideMap.Peptides.Add(sequence, 0);
                                    clusterPeptideMap.PeptideDatasets.Add(sequence, new List<int>());
                                }
                                clusterPeptideMap.PeptideDatasets[sequence].Add(umc.GroupID);
                                clusterPeptideMap.Peptides[sequence]++;
                                hasIdentifications = true;
                            }
                        }
                    }

                    
                }

                if (hasIdentifications)
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}", cluster.ID, cluster.UMCList.Count, cluster.MsMsCount, clusterPeptideMap.TotalDatasetsObserved);

                cluster.Clear();
            }            
        }

        /// <summary>
        /// Matches Peptides and then asks what clusters they are in
        /// </summary>
        [Test]
        [TestCase(@"M:\data\proteomics\Papers\AlignmentPaper\data\annotated-QC-6\clusterTests-matches.txt",                     
                    900)]
        public void TestPeptides(string databasePath, int scanTolerance)
        {            
            var map  = new  Dictionary<string, int>();
            var data = File.ReadAllLines(databasePath).ToList();

            var currentPeptide   = "";
            var currentDataset   = 0;
            var currentClusterId = 0;
            var currentScan      = 0;

            int same    = 0;
            int notSame = 0;

            var peptides   = new List<Peptide>();
            var clusterMap = new Dictionary<string, int>();

            for (var i = 1; i < data.Count; i++)
            {                
                var columns = data[i].Split('\t');

                if (columns.Length < 5)
                    continue;

                var datasetId = Convert.ToInt32(columns[0]);
                var sequence  = columns[2];
                var scan      = Convert.ToInt32(columns[3]);
                var clusterId = Convert.ToInt32(columns[4]);

                var peptide = new Peptide()
                {
                    Scan = scan,
                    Sequence = sequence,
                    GroupId = datasetId
                };

                // We found a match
                if (sequence == currentPeptide && datasetId != currentDataset)
                {
                    if (Math.Abs(scan - currentScan) < scanTolerance)
                    {
                        if (currentClusterId == clusterId)
                            same++;
                        else
                            notSame++;
                    }
                }

                currentClusterId = clusterId;
                currentDataset   = datasetId;
                currentPeptide   = sequence;
                currentScan      = scan;
            }

            Console.WriteLine("Same: {0}\t Not Same{1}", same, notSame);
        }

        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew-Annotated2",
            FeatureAlignmentType.SpectralAlignment,
            LcmsFeatureClusteringAlgorithmType.AverageLinkage
            )]
        public void TestClustering(
            string directory,
            FeatureAlignmentType alignmentType,
            LcmsFeatureClusteringAlgorithmType clusterType)
        {

            // Loads the supported MultiAlign types 
            var supportedTypes = DatasetInformation.SupportedFileTypes;
            var extensions = new List<string>();
            supportedTypes.ForEach(x => extensions.Add("*" + x.Extension));

            // Find our datasets
            var inputFiles = DatasetSearcher.FindDatasets(directory,
                extensions,
                SearchOption.TopDirectoryOnly);
            var datasets = DatasetInformation.ConvertInputFilesIntoDatasets(inputFiles);

            // Setup our alignment options
            var warpOptions = new AlignmentOptions();
            var spectralOptions = new SpectralOptions
            {
                ComparerType = SpectralComparison.CosineDotProduct,
                Fdr = .01,
                IdScore =  1e-15,
                MzBinSize = .5,
                MzTolerance = .5,
                NetTolerance = .1,
                RequiredPeakCount = 32,
                SimilarityCutoff = .75,
                TopIonPercent = .8
            };
            

            // Options setup
            var instrumentOptions = InstrumentPresetFactory.Create(InstrumentPresets.LtqOrbitrap);
            var featureTolerances = new FeatureTolerances()
            {
                Mass = instrumentOptions.Mass,
                RetentionTime = instrumentOptions.NetTolerance,
                DriftTime = instrumentOptions.DriftTimeTolerance
            };
            var featureFindingOptions = new LcmsFeatureFindingOptions(featureTolerances)
            {
                MaximumNetRange = .002,
                MaximumScanRange = 50
            };

            // Create our algorithms
            var finder = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.TreeBased);
            var aligner = FeatureAlignerFactory.CreateDatasetAligner(alignmentType,
                warpOptions,
                spectralOptions);
            var clusterer = ClusterFactory.Create(clusterType);
            clusterer.Parameters = new FeatureClusterParameters<UMCLight>()
            {
                Tolerances = featureTolerances
            };


           // ((LcmsWarpFeatureAligner)aligner).Options.AlignmentType = enmAlignmentType.NET_MASS_WARP;
            

            RegisterProgressNotifier(aligner);
            RegisterProgressNotifier(finder);
            RegisterProgressNotifier(clusterer);

            var lcmsFilters = new LcmsFeatureFilteringOptions()
            {
                FeatureLengthRange = new FilterRange(50, 300)
            };
            var msFilterOptions = new MsFeatureFilteringOptions()
            {
                MinimumIntensity =  5000,
                ChargeRange = new FilterRange(1, 6),
                ShouldUseChargeFilter = true,
                ShouldUseDeisotopingFilter = true,
                ShouldUseIntensityFilter = true
            };

            for (int i = 0; i < 1; i++)
            {
                var aligneeDatasets = datasets.Where((t, j) => j != i).ToList();
                PerformMultiAlignAnalysis(  datasets[0],
                                            aligneeDatasets,  
                                            featureFindingOptions,
                                            msFilterOptions,
                                            lcmsFilters,
                                            spectralOptions,
                                            finder, 
                                            aligner,
                                            clusterer);                            
            }
        }
        /// <summary>
        /// Runs the MultiAlign analysis
        /// </summary>
        public void PerformMultiAlignAnalysis(  DatasetInformation               baselineDataset,                                    
                                                IEnumerable<DatasetInformation>  aligneeDatasets,
                                                LcmsFeatureFindingOptions        featureFindingOptions,
                                                MsFeatureFilteringOptions        msFilterOptions,   
                                                LcmsFeatureFilteringOptions      lcmsFilterOptions,  
                                                SpectralOptions                  peptideOptions,
                                                IFeatureFinder                   featureFinder,                           
                                                IFeatureAligner  <IEnumerable<UMCLight>, 
                                                                    IEnumerable<UMCLight>, 
                                                                        classAlignmentData> aligner,
                                                IClusterer<UMCLight, UMCClusterLight> clusterer)
        {

            UpdateStatus("Loading baseline features.");
            var msFeatures  = UmcLoaderFactory.LoadMsFeatureData(baselineDataset.Features.Path);
            msFeatures      = LcmsFeatureFilters.FilterMsFeatures(msFeatures, msFilterOptions); 

            // Load the baseline reference set
            using (var providerX = RawLoaderFactory.CreateFileReader(baselineDataset.RawPath))
            {
                providerX.AddDataFile(baselineDataset.RawPath, 0);
                UpdateStatus("Creating Baseline LCMS Features.");
                var baselineFeatures = featureFinder.FindFeatures(msFeatures,
                                                                  featureFindingOptions, 
                                                                  providerX);
                LinkPeptidesToFeatures(baselineDataset.SequencePath, baselineFeatures, peptideOptions.Fdr, peptideOptions.IdScore);

                providerX.AddDataFile(baselineDataset.RawPath, 0);

                // Then load the alignee dataset
                foreach (var dataset in aligneeDatasets)
                {
                    var aligneeMsFeatures   = UmcLoaderFactory.LoadMsFeatureData(dataset.Features.Path);
                    aligneeMsFeatures = LcmsFeatureFilters.FilterMsFeatures(aligneeMsFeatures, msFilterOptions); 

                    using (var providerY    = RawLoaderFactory.CreateFileReader(dataset.RawPath))
                    {
                        providerY.AddDataFile(dataset.RawPath, 0);


                        UpdateStatus("Finding alignee features");
                        var aligneeFeatures = featureFinder.FindFeatures(aligneeMsFeatures, 
                                                                         featureFindingOptions,
                                                                         providerY);
                        LinkPeptidesToFeatures(dataset.SequencePath, aligneeFeatures, peptideOptions.Fdr    , peptideOptions.IdScore);



                        var allFeatures = new List<UMCLight>();
                        allFeatures.AddRange(baselineFeatures);
                        allFeatures.AddRange(aligneeFeatures);

                        foreach (var feature in allFeatures)
                        {
                            feature.RetentionTime = feature.NET;
                            feature.MassMonoisotopicAligned = feature.MassMonoisotopic;
                        }


                        var clusters        = clusterer.Cluster(allFeatures);
                        var preAlignment =  AnalyzeClusters(clusters);

                        aligner.AligneeSpectraProvider  = providerY;
                        aligner.BaselineSpectraProvider = providerX;

                        UpdateStatus("Aligning data");
                        // Aligner data
                        var data = aligner.Align(baselineFeatures, aligneeFeatures);         
                        
                        
                        //ExportAlignmentData(data,
                        //                    baselineDataset, 
                        //                    dataset,
                        //                    baselineFeatures,
                        //                    aligneeFeatures);


                        foreach (var feature in allFeatures)
                        {
                            feature.UMCCluster = null;
                            feature.ClusterID  = -1;
                        }

                        UpdateStatus("Re-clustering data");
                        clusters = clusterer.Cluster(allFeatures);
                        var postAlignment = AnalyzeClusters(clusters);

                        UpdateStatus("Note\tSame\tDifferent");
                        UpdateStatus(string.Format("Pre\t{0}\t{1}", preAlignment.SameCluster,  preAlignment.DifferentCluster));
                        UpdateStatus(string.Format("Post\t{0}\t{1}", postAlignment.SameCluster, postAlignment.DifferentCluster));
                        
                    }
                }
            }

            DeRegisterProgressNotifier(aligner);
            DeRegisterProgressNotifier(featureFinder);
            DeRegisterProgressNotifier(clusterer);
        }
        /// <summary>
        /// Links a list of peptides to the features provided if the dataset has knowledge of the sequence file file 
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="aligneeFeatures"></param>
        private void LinkPeptidesToFeatures(string sequencePath, List<UMCLight> aligneeFeatures, double fdr, double idScore)
        {
            // Get the peptides associated with this feature set.
            var peptideReaderY = PeptideReaderFactory.CreateReader(sequencePath);
            if (peptideReaderY == null) return;

            // Load the peptide Y
            UpdateStatus("Linking peptides to ms/ms");
            var linker = new PeptideMsMsLinker();
            var peptides = peptideReaderY.Read(sequencePath).ToList();
            var filteredPeptides = peptides.ToList().Where(x => PeptideUtility.PassesCutoff(x, idScore, fdr)).ToList();

            var msnSpectra = new List<MSSpectra>();
            foreach (var feature in aligneeFeatures)
            {
                foreach (var msFeature in feature.MSFeatures)
                {
                    msnSpectra.AddRange(msFeature.MSnSpectra);
                }
            }


            linker.LinkPeptidesToSpectra(msnSpectra, filteredPeptides);
        }
        /// <summary>
        /// Extracts the peptides from the given features
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        private IEnumerable<Peptide> ExtractPeptides(IEnumerable<UMCLight> features)
        {
            var peptides = new List<Peptide>();
            foreach (var feature in features)
            {
                foreach (var msFeature in feature.MSFeatures)
                {
                    foreach (var spectrum in msFeature.MSnSpectra)
                    {
                        peptides.AddRange(spectrum.Peptides);
                    }
                }
            }
            return peptides;
        }

        private PeptideMatchesData AnalyzeClusters(List<UMCClusterLight> clusters)
        {
            var peptides = new Dictionary<string, List<Peptide>>();
            foreach (var cluster in clusters)
            {                
                var clusterPeptides = ExtractPeptides(cluster.Features);
                foreach (var peptide in clusterPeptides)
                {
                    if (!peptides.ContainsKey(peptide.Sequence))
                    {
                        peptides.Add(peptide.Sequence, new List<Peptide>());
                    }
                    peptides[peptide.Sequence].Add(peptide);
                }
            }

            // analyze now...
            var matches = new PeptideMatchesData();

            foreach (var sequence in peptides.Keys)
            {
                Dictionary<int, int> map = new Dictionary<int, int>();              
                foreach (var peptide in peptides[sequence])
                {
                    var parent = peptide.GetParentUmc();
                    if (parent != null)
                    {
                        if (parent.UMCCluster != null)
                        {
                            var id = parent.UMCCluster.ID;
                            if (!map.ContainsKey(id))
                            {
                                map.Add(id, 0);
                            }
                            map[id]++;
                        }
                    }
                }

                if (map.Count > 1)
                    matches.DifferentCluster++;
                else                
                    matches.SameCluster++;

            }
            return matches;
        }



        /// <summary>
        /// Updates the status to console 
        /// </summary>
        /// <param name="message"></param>
        protected override void UpdateStatus(string message)
        {
            Console.WriteLine("\t" + message); 
            base.UpdateStatus(message);            
        }

        private void ExportAlignmentData(classAlignmentData data,
                                         DatasetInformation baselineDatasetInformation,
                                         DatasetInformation alignDatasetInformation,
                                         IEnumerable<UMCLight> baselineFeatures,
                                         IEnumerable<UMCLight> aligneeFeatures)
        {

            var netValues = new List<double>();
            var massValues = new List<double>();
            

            var anchorPoints = data.Matches;
            foreach (var match in anchorPoints)
            {
                netValues.Add(match.AnchorPointX.Net   - match.AnchorPointY.Net);
                massValues.Add(match.AnchorPointX.Mass - match.AnchorPointY.Mass);
            }


            var netHist =
                MatchCountHistogramBuilder.CreateResidualHistogram(-.05, .05, .01, netValues);


            var netHistogram = new Dictionary<double, int>();
            
            Console.WriteLine();
            for(int i = 0; i < netHist.Bins.Count; i++)
            {
                netHistogram.Add(netHist.Bins[i],   Convert.ToInt32(netHist.Data[i]));                
                Console.WriteLine("{0}\t{1}", netHist.Bins[i], netHist.Data[i]);
            }            
        }

        public class PeptideMatchesData
        {
            public int SameCluster { get; set; }
            public int DifferentCluster { get; set; }
        }
    }
}
