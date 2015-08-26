using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlign.ViewModels.Instruments;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Alignment.LcmsWarp;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Algorithms.SpectralProcessing;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.RawData;
using MultiAlignTestSuite;
using NUnit.Framework;

namespace AlignmentPaperTestSuite.Figures
{
    [TestFixture]
    public class Figure5 : TestBase
    {
        [Test]
        [TestCase(  @"QC-Shew-Annotated2\shewAnnotated-features.mta",
                    @"AlignmentPaper\Figure5\",
                    "figure5-qc-shew",
                    FeatureAlignmentType.SPECTRAL_ALIGNMENT,
                    LcmsFeatureClusteringAlgorithmType.AverageLinkage,
                    Ignore = true
                    )]

        [TestCase(@"QC-Shew-Annotated2\shewAnnotated-features.mta",
                    @"AlignmentPaper\Figure5\",
                    "figure5-qc-shew",
                    FeatureAlignmentType.SPECTRAL_ALIGNMENT,
                    LcmsFeatureClusteringAlgorithmType.AverageLinkage,
                    Ignore = true
                    )]
        [TestCase(@"bad-03\qc-alignment-03.mta",
                   @"AlignmentPaper\Figure5\",
                   "figure5-qc-shew",
                   FeatureAlignmentType.SPECTRAL_ALIGNMENT,
                   LcmsFeatureClusteringAlgorithmType.AverageLinkage,
                   Ignore = false
                   )]
        public void GenerateClusterAlignmentStatistics(string relativeDatabasePath,
            string relativeName,
            string name,
            FeatureAlignmentType alignmentType,
            LcmsFeatureClusteringAlgorithmType clusterType)
        {
            var databasePath    = GetPath(relativeDatabasePath);
            var outputPath      = GetOutputPath(relativeName);
            
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // Connect to the NHibernate database
            var providers = DataAccessFactory.CreateDataAccessProviders(databasePath, false);

            // Setup our alignment options
            var alignmentOptions = new AlignmentOptions();
            var spectralOptions = new SpectralOptions
            {
                ComparerType = SpectralComparison.CosineDotProduct,
                Fdr          = .01,
                IdScore      = 1e-09,
                MzBinSize    = .5,
                MzTolerance  = .5,
                NetTolerance = .1,
                RequiredPeakCount   = 32,
                SimilarityCutoff    = .75,
                TopIonPercent       = .8
            };

            // Options setup
            var instrumentOptions = InstrumentPresetFactory.Create(InstrumentPresets.LtqOrbitrap);
            var featureTolerances = new FeatureTolerances
            {
                Mass        = instrumentOptions.Mass + 6,
                Net         = instrumentOptions.NetTolerance,
                DriftTime   = instrumentOptions.DriftTimeTolerance
            };

            UpdateStatus("Retrieving all datasets for test.");
            var datasets = providers.DatasetCache.FindAll();

            // Create our algorithms
            var aligner     = FeatureAlignerFactory.CreateDatasetAligner(alignmentType,
                alignmentOptions.LCMSWarpOptions,
                spectralOptions);
            var clusterer   = ClusterFactory.Create(clusterType);
            clusterer.Parameters = new FeatureClusterParameters<UMCLight>
            {
                Tolerances       = featureTolerances
            };

            RegisterProgressNotifier(aligner);
            RegisterProgressNotifier(clusterer);

            for (var i = 0; i < datasets.Count - 1; i++)
            {
                var matchPath = string.Format("{0}-{1}-matches.txt", name, i);
                var errorPath = string.Format("{0}-{1}-errors.txt", name, i);

                matchPath = Path.Combine(outputPath, matchPath);
                errorPath = Path.Combine(outputPath, errorPath);



                var aligneeDataset      = datasets[i + 1];
                var baselineDataset     = datasets[i];

                // Load the baseline reference set
                using (var rawProviderX = RawLoaderFactory.CreateFileReader(baselineDataset.RawPath))
                {
                    rawProviderX.AddDataFile(baselineDataset.RawPath, 0);
                    // Load the baseline reference set
                    using (var rawProviderY = RawLoaderFactory.CreateFileReader(aligneeDataset.RawPath))
                    {
                        rawProviderY.AddDataFile(aligneeDataset.RawPath, 0);

                        var baselineFeatures = RetrieveFeatures(baselineDataset.DatasetId, providers);
                        var aligneeFeatures  = RetrieveFeatures(aligneeDataset.DatasetId,  providers);
                        var providerX        = new CachedFeatureSpectraProvider(rawProviderX, baselineFeatures);
                        var providerY        = new CachedFeatureSpectraProvider(rawProviderY, aligneeFeatures);

                        AlignDatasets(  baselineFeatures,
                                        aligneeFeatures,
                                        providerX,
                                        providerY,
                                        aligner,
                                        clusterer,
                                        matchPath,
                                        errorPath);
                    }
                }
            }
        }

        private IEnumerable<UMCLight> RetrieveFeatures(int datasetId, FeatureDataAccessProviders providers)
        {
            var features     = providers.FeatureCache.FindByDatasetId(datasetId);            
            var spectra      = providers.MSnFeatureCache.FindByDatasetId(datasetId);
            if (spectra == null) 
                throw new ArgumentNullException(@"There were no MS/MS spectra in the database");

            var sequences    = providers.DatabaseSequenceCache.FindAll();
            var sequenceMaps = providers.SequenceMsnMapCache.FindByDatasetId(datasetId);
            var spectraMaps  = providers.MSFeatureToMSnFeatureCache.FindByDatasetId(datasetId);
            var msFeatures   = providers.MSFeatureCache.FindByDatasetId(datasetId);

            // Make a one pass through each enumerable list, 
            // then use the maps to join the data together
            var dictFeatures = new Dictionary<int, UMCLight>();
            var dictSpectra  = new Dictionary<int, MSSpectra>();
            var dictPeptide  = new Dictionary<int, Peptide>();
            var dictMsFeatures = new Dictionary<int, MSFeatureLight>();

            foreach (var sequence in sequences)
            {
                if (sequence.GroupId != datasetId)
                    continue;

                var peptide = new Peptide
                {
                    Sequence =  sequence.Sequence,
                    Id       =  sequence.Id,
                };                 
                dictPeptide.Add(peptide.Id, peptide);
            }

            msFeatures.ForEach(x => dictMsFeatures.Add(x.Id, x));
            features.ForEach(x => dictFeatures.Add(x.Id, x));
            spectra.ForEach(x  => dictSpectra.Add(x.Id, x));

            var count = 0;
            // Map the MSMS
            foreach (var map in sequenceMaps)
            {
                MSSpectra spectrum;
                Peptide   peptide;
                var workedSpectra = dictSpectra.TryGetValue(map.MsnFeatureId, out spectrum);
                var workedPeptide = dictPeptide.TryGetValue(map.SequenceId, out peptide);

                if (workedSpectra && workedPeptide)
                {
                    spectrum.Peptides.Add(peptide);
                    peptide.Spectrum = spectrum;
                    count++;
                }
            }

            Console.WriteLine("Mapped {0} peptides to spectra", count);
            count = 0;

            // Map the spectra....
            foreach (var map in spectraMaps)
            {
                UMCLight  feature;
                MSSpectra spectrum;
                MSFeatureLight msFeature;
                
                var workedFeatures  = dictFeatures.TryGetValue(map.LCMSFeatureID, out feature);
                var workedSpectra   = dictSpectra.TryGetValue(map.MSMSFeatureID,  out spectrum);
                var workedMsFeature = dictMsFeatures.TryGetValue(map.MSFeatureID, out msFeature);
                if (!workedFeatures || !workedSpectra || !workedMsFeature)
                    continue;

                var metaData = new ScanSummary
                {
                    MsLevel = 2,
                    PrecursorMz = spectrum.PrecursorMz,
                    Scan = spectrum.Scan
                };
                spectrum.ScanMetaData = metaData;

                msFeature.MSnSpectra.Add(spectrum);
                spectrum.ParentFeature = msFeature;
                feature.AddChildFeature(msFeature);
                msFeature.Umc = feature;
                count++;
            }

            Console.WriteLine("Mapped {0} spectra to parent Features", count);            
            return features;
        }

        /// <summary>
        ///     Runs the MultiAlign analysis
        /// </summary>
        public void AlignDatasets(  IEnumerable<UMCLight>   baselineFeatures,
                                    IEnumerable<UMCLight>   aligneeFeatures,
                                    ISpectraProvider        providerX,
                                    ISpectraProvider        providerY,
                                    IFeatureAligner<IEnumerable<UMCLight>,
                                        IEnumerable<UMCLight>,
                                        AlignmentData> aligner,
                                    IClusterer<UMCLight, UMCClusterLight> clusterer,
                                    string matchPath,
                                    string errorPath)
        {

            // cluster before we do anything else....
            var allFeatures = new List<UMCLight>();
            allFeatures.AddRange(baselineFeatures);
            allFeatures.AddRange(aligneeFeatures);


            var maxBaseline = baselineFeatures.Max(x => x.Scan);
            var minBaseline = baselineFeatures.Min(x => x.Scan);

            var maxAlignee  = aligneeFeatures.Max(x => x.Scan);
            var minAlignee  = aligneeFeatures.Min(x => x.Scan);

            foreach (var feature in aligneeFeatures)
            {
                feature.Net = Convert.ToDouble(feature.Scan - minAlignee) / Convert.ToDouble(maxAlignee - minAlignee);
                feature.MassMonoisotopicAligned = feature.MassMonoisotopic;
            }

            foreach (var feature in baselineFeatures)
            {
                feature.Net = Convert.ToDouble(feature.Scan - minBaseline) / Convert.ToDouble(maxBaseline - minBaseline);
                feature.MassMonoisotopicAligned = feature.MassMonoisotopic;
            }

            // This tells us the differences before we align.
            var clusters     = clusterer.Cluster(allFeatures);
            var clusterId    = 0;
            foreach (var cluster in clusters)
            {
                cluster.Id = clusterId++;
            }
            var scorer       = new GlobalPeptideClusterScorer();
            var preAlignment = scorer.Score(clusters);

            aligner.AligneeSpectraProvider  = providerY;
            aligner.BaselineSpectraProvider = providerX;

            UpdateStatus("Aligning data");
            // Aligner data
            var data    = aligner.Align(baselineFeatures, aligneeFeatures);
            var matches = data.Matches;

            // create anchor points for LCMSWarp alignment
            var massPoints = new List<RegressionPoint>();
            var netPoints = new List<RegressionPoint>();
            foreach (var match in matches)
            {
                var massError   = FeatureLight.ComputeMassPPMDifference(match.AnchorPointX.Mz,
                                                    match.AnchorPointY.Mz);
                var netError    = match.AnchorPointX.Net - match.AnchorPointY.Net;
                var massPoint   = new RegressionPoint(match.AnchorPointX.Mz, 0, massError, netError);
                massPoints.Add(massPoint);

                var netPoint    = new RegressionPoint(match.AnchorPointX.Net, 0, massError, netError);
                netPoints.Add(netPoint);
            }

            foreach (var feature in allFeatures)
            {
                feature.UmcCluster = null;
                feature.ClusterId = -1;
            }
            // Then cluster after alignment!
            UpdateStatus("clustering data");
            clusters = clusterer.Cluster(allFeatures);
            var postAlignment = scorer.Score(clusters);

            UpdateStatus("Note\tSame\tDifferent");
            UpdateStatus(string.Format("Pre\t{0}\t{1}", 
                            preAlignment.SameCluster,
                            preAlignment.DifferentCluster));
            UpdateStatus(string.Format("Post\t{0}\t{1}", 
                            postAlignment.SameCluster,
                            postAlignment.DifferentCluster));

            matches = FilterMatches(matches, 40);

            SaveMatches(matchPath, matches);
            DeRegisterProgressNotifier(aligner);
            DeRegisterProgressNotifier(clusterer);
        }

        private IEnumerable<SpectralAnchorPointMatch> FilterMatches(IEnumerable<SpectralAnchorPointMatch> matches, double ppm)
        {
            return
                matches.Where(x => 
                        ppm > Math.Abs(FeatureLight.ComputeMassPPMDifference(x.AnchorPointX.Spectrum.ParentFeature.Mz,
                                                                    x.AnchorPointY.Spectrum.ParentFeature.Mz)));
        }

      
        private void SaveMatches(string path, IEnumerable<SpectralAnchorPointMatch> matches)
        {
            using (var writer = File.CreateText(path))
            {
                writer.WriteLine("[Header]");
                writer.WriteLine("p mz = parentMz - A and B denote dataset A and dataset B");
                writer.WriteLine("[Data]");
                writer.WriteLine("Net-A\tpMz-A\tScan-A\tNet-B\tpMz-B\tScan-B\tMassErrorPpm\tSimScore");
                foreach (var match in matches)
                {
                    if (match.AnchorPointX.Spectrum == null)
                        continue;

                    if (match.AnchorPointY.Spectrum == null)
                        continue;

                    var parentFeatureX = match.AnchorPointX.Spectrum.ParentFeature;
                    var parentFeatureY = match.AnchorPointY.Spectrum.ParentFeature;

                    var data =
                        string.Format( "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",

                                        parentFeatureX.ParentFeature.Net,
                                        parentFeatureX.ParentFeature.Mz,
                                        parentFeatureX.ParentFeature.Scan,

                                        parentFeatureY.ParentFeature.Net,
                                        parentFeatureY.ParentFeature.Mz,
                                        parentFeatureY.ParentFeature.Scan,

                                        FeatureLight.ComputeMassPPMDifference(parentFeatureX.Mz, parentFeatureY.Mz),
                                        match.SimilarityScore);

                    writer.WriteLine(data);
                }
            }
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
    }
}
