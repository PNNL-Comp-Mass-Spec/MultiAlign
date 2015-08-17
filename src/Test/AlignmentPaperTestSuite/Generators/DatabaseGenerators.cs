using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlign.IO;
using MultiAlign.ViewModels.Instruments;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Hibernate;
using MultiAlignCore.IO.RawData;
using MultiAlignTestSuite;
using NUnit.Framework;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Alignment.SpectralMatches;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Utilities;
using PNNLOmicsIO.IO;

namespace AlignmentPaperTestSuite.Generators
{
    /// <summary>
    /// The purpose of this class is to generate data so that it can be used in the other tests and does not have to recreated
    /// for simple unit testing.
    /// </summary>
    [TestFixture]
    public class DatabaseGenerators: TestBase
    {
        /// <summary>
        /// Creates a database file so that other unit tests can use the data
        /// </summary>
       [Test]
       [TestCase(@"QC-Shew-Annotated2", @"QC-Shew-Annotated2\shewAnnotated-features.mta")]
       [TestCase(@"Bad-03", @"Bad-03\qc-alignment-03.mta")]
        public void CreateFeatureDatabase(string directoryPath, string databasePath)
        {
            var directory  = GetPath(directoryPath);
            databasePath   = GetPath(databasePath);
            
            // Loads the supported MultiAlign types 
            var supportedTypes = DatasetInformation.SupportedFileTypes;
            var extensions = new List<string>();
            supportedTypes.ForEach(x => extensions.Add("*" + x.Extension));

            // Find our datasets
            var inputFiles = DatasetSearcher.FindDatasets(directory,
                extensions,
                SearchOption.TopDirectoryOnly);
            var datasets = DatasetInformation.ConvertInputFilesIntoDatasets(inputFiles);

            // Options setup
            var instrumentOptions = InstrumentPresetFactory.Create(InstrumentPresets.LtqOrbitrap);
            var featureTolerances = new FeatureTolerances
            {
                Mass = instrumentOptions.Mass + 6,
                Net = instrumentOptions.NetTolerance,
                DriftTime = instrumentOptions.DriftTimeTolerance
            };
            var featureFindingOptions = new LcmsFeatureFindingOptions(featureTolerances)
            {
                MaximumNetRange = .002,
                MaximumScanRange = 50
            };
            var lcmsFilters = new LcmsFeatureFilteringOptions
            {
                FeatureLengthRange = new FilterRange(50, 300)
            };
            var msFilterOptions = new MsFeatureFilteringOptions
            {
                MinimumIntensity = 5000,
                ChargeRange = new FilterRange(1, 6),
                ShouldUseChargeFilter = true,
                ShouldUseDeisotopingFilter = true,
                ShouldUseIntensityFilter = true
            };
            var spectralOptions = new SpectralOptions
            {
                ComparerType = SpectralComparison.CosineDotProduct,
                Fdr = .01,
                IdScore = 1e-09,
                MzBinSize = .5,
                MzTolerance = .5,
                NetTolerance = .1,
                RequiredPeakCount = 32,
                SimilarityCutoff = .75,
                TopIonPercent = .8
            };
            var finder = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.TreeBased);
            NHibernateUtil.CreateDatabase(databasePath);
            // Synchronization and IO for serializing all data to the database.
            var providers   = DataAccessFactory.CreateDataAccessProviders(databasePath, true);
            var cache       = new FeatureLoader
            {
               Providers = providers
            };

            var datasetId = 0;
            foreach(var dataset in datasets)
            {
                dataset.DatasetId = datasetId++;
                var features = FindFeatures(dataset,
                                            featureFindingOptions,
                                            msFilterOptions,
                                            lcmsFilters,
                                            spectralOptions,
                                            finder);                

                cache.CacheFeatures(features);                
            }
            providers.DatasetCache.AddAll(datasets);

        }

        /// <summary>
        ///  Finds features given a dataset
        /// </summary>
        private IList<UMCLight> FindFeatures(  DatasetInformation               information,
                                                    LcmsFeatureFindingOptions   featureFindingOptions,
                                                    MsFeatureFilteringOptions   msFilterOptions,
                                                    LcmsFeatureFilteringOptions lcmsFilterOptions,
                                                    SpectralOptions             peptideOptions,
                                                    MultiAlignCore.Algorithms.FeatureFinding.IFeatureFinder              featureFinder)
                                               
        {
            UpdateStatus("Loading baseline features.");
            var msFeatures  = UmcLoaderFactory.LoadMsFeatureData(information.Features.Path);
            msFeatures      = LcmsFeatureFilters.FilterMsFeatures(msFeatures, msFilterOptions);

            // Load the baseline reference set
            using (var rawProviderX  = RawLoaderFactory.CreateFileReader(information.RawPath))
            {
                rawProviderX.AddDataFile(information.RawPath, 0);
                UpdateStatus("Creating LCMS Features.");
                var features    = featureFinder.FindFeatures(msFeatures,
                                                             featureFindingOptions,
                                                             rawProviderX);
                features        = LcmsFeatureFilters.FilterFeatures(features, lcmsFilterOptions);

                var datasetId = information.DatasetId;
                foreach (var feature in features)
                {
                    var lightEntry = new List<MSFeatureLight>();
                    feature.GroupId = datasetId;
                    foreach (var msFeature in feature.MsFeatures)
                    {
                        msFeature.GroupId = datasetId;
                        foreach (var msmsFeature in msFeature.MSnSpectra)
                        {
                            msmsFeature.GroupId = datasetId;
                            foreach (var peptide in msmsFeature.Peptides)
                            {
                                peptide.GroupId = datasetId;
                            }
                           
                        }

                        if (msFeature.MSnSpectra.Count > 0)
                            lightEntry.Add(msFeature);
                    }
                    
                    // We are doing this so that we dont have a ton of MS features in the database
                    feature.MsFeatures.Clear();
                    feature.MsFeatures.AddRange(lightEntry);
                }

                LinkPeptidesToFeatures(information.SequencePath,
                                        features, 
                                        peptideOptions.Fdr,
                                        peptideOptions.IdScore);
                
                DeRegisterProgressNotifier(featureFinder);
                return features;
            }
        }

        /// <summary>
        /// Links a list of peptides to the features provided if the dataset has knowledge of the sequence file file
        /// </summary>
        private void LinkPeptidesToFeatures(string sequencePath, IEnumerable<UMCLight> aligneeFeatures, double fdr,
            double idScore)
        {
            // Get the peptides associated with this feature set.
            var peptideReaderY = PeptideReaderFactory.CreateReader(sequencePath);
            if (peptideReaderY == null)
                return;

            // Load the peptide Y
            UpdateStatus("Linking peptides to ms/ms");
            var linker = new PeptideMsMsLinker();
            var peptides = peptideReaderY.Read(sequencePath).ToList();
            var filteredPeptides = peptides.ToList().Where(x => PeptideUtility.PassesCutoff(x, idScore, fdr)).ToList();

            var msnSpectra = new List<MSSpectra>();
            foreach (var feature in aligneeFeatures)
            {
                foreach (var msFeature in feature.MsFeatures)
                {
                    msnSpectra.AddRange(msFeature.MSnSpectra);
                }
            }


            linker.LinkPeptidesToSpectra(msnSpectra, filteredPeptides);
        }
    }
}
