using System;
using System.IO;
using System.Linq;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.TextFiles;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.FeatureFinding
{

    [TestFixture]
    public class FeatureFindingTest: TestBase
    {
        /// <summary>
        /// Creates a simple LC-MS features file from the provided paths
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="outputPath"></param>
        [Test]
        [TestCase(@"Data\QC_SHEW\QC_Shew_13_04_1b_6Oct13_Cougar_13-06-14_isos.csv",
                  @"Data\QC_SHEW\QC_Shew_13_04_1b_6Oct13_Cougar_13-06-14.features", Ignore = false)]
        [TestCase(@"Data\QC_SHEW\QC_Shew_13_04_1b_18Sep13_Cougar_13-06-14_isos.csv",
                  @"Data\QC_SHEW\QC_Shew_13_04_1b_18Sep13_Cougar_13-06-14.features", Ignore = false)]
        [TestCase(@"Data\chronicFatigue\data\169091_Schutzer_CF_10937_18Jan10_Owl_09-08-18_isos.csv",
                  @"Data\chronicFatigue\data\169091_Schutzer_CF_10937_18Jan10_Owl_09-08-18.features", Ignore = false)]
        [TestCase(@"Data\chronicFatigue\data\169114_Schutzer_CF_10818_18Jan10_Owl_09-08-18_isos.csv",
                  @"Data\chronicFatigue\data\169114_Schutzer_CF_10818_18Jan10_Owl_09-08-18.features", Ignore = false)]
        public void CreateFeaturesTest(string relativePath, string outputPath)
        {            
            var path = GetPath(relativePath);

            var tolerances  = new FeatureTolerances
            {
                Mass =  13,
                Net = .01,
                DriftTime =  30,
                FragmentationWindowSize = .5
            };

            var reader      = new MsFeatureLightFileReader();
            var rawFeatures = reader.ReadFile(path);
            var msFilterOptions = new MsFeatureFilteringOptions
            {
                ChargeRange =  new FilterRange(1,6),
                MinimumIntensity = 200000,
                ShouldUseDeisotopingFilter =  true,
                ShouldUseIntensityFilter   =  true
            };
            rawFeatures = LcmsFeatureFilters.FilterMsFeatures(rawFeatures, msFilterOptions);

            var finder      = new MsFeatureTreeClusterer<MSFeatureLight, UMCLight> {Tolerances = tolerances};
            finder.Progress += (sender, args) => Console.WriteLine(args.Message);
            var features    = finder.Cluster(rawFeatures.ToList());

            var filterOptions = new LcmsFeatureFilteringOptions
            {
                FeatureLengthRangeScans = new FilterRange
                {
                    Maximum = 30,
                    Minimum = 10
                }                
            };
            features        = LcmsFeatureFilters.FilterFeatures(features, filterOptions);

            Console.WriteLine(@"Found - {0} features", features.Count);
            using (var writer = File.CreateText(GetPath(outputPath)))
            {
                var index = 0;
                foreach (var feature in features)
                {
                    feature.Id = index++;
                    feature.CalculateStatistics(ClusterCentroidRepresentation.Mean);
                    writer.WriteLine("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}",
                        TextDelimiter,
                        feature.Net,
                        feature.ChargeState,
                        feature.Mz,
                        feature.Scan,
                        feature.MassMonoisotopic,
                        feature.MassMonoisotopicAligned,
                        feature.Id,
                        feature.ScanStart,
                        feature.ScanEnd,
                        feature.ScanAligned
                        );
                }
            }
        }
    }
}
