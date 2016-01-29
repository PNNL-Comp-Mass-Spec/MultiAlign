using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignTestSuite.IO
{
    using System.IO;

    using InformedProteomics.Backend.MassSpecData;

    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Algorithms.FeatureMatcher;
    using MultiAlignCore.Algorithms.FeatureMatcher.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MassTags;
    using MultiAlignCore.IO.MTDB;
    using MultiAlignCore.IO.RawData;
    using MultiAlignCore.IO.TextFiles;

    using NUnit.Framework;

    [TestFixture]
    public class LiquidReading
    {
        [TestCase(@"C:\Users\wilk011\Documents\DataFiles\OHSUblotter\Normal_1_Pos.tsv")]
        public void TestFileReading(string liquidResultsPath)
        {
            var fileReader = new LiquidResultsFileLoader(liquidResultsPath);
            var database = fileReader.LoadDatabase();
        }

        [TestCase(@"C:\Users\wilk011\Documents\DataFiles\OHSUblotter\Normal_1_Pos.tsv",
                  @"C:\Users\wilk011\Documents\DataFiles\OHSUblotter\OHSUblotter_2014_normal_lipid_pooled_1_POS_150mm_16Sept15_Polaroid_HSST3-02_isos.csv",
                  @"C:\Users\wilk011\Documents\DataFiles\OHSUblotter\OHSUblotter_2014_normal_lipid_pooled_1_POS_150mm_16Sept15_Polaroid_HSST3-02.raw")]
        public void CompareMs2IdsToMs1Ids(string liquidResultsPath, string isosFile, string rawFile)
        {
            // Read mass tags.
            var massTagReader = new LiquidResultsFileLoader(liquidResultsPath);
            var massTags = massTagReader.LoadDatabase();

            // Get identifications - this rereads the liquid results file, but I'm leaving it that way
            // for now because this is just a test.
            var scansToIds = this.GetIds(liquidResultsPath);

            // Read raw data file.
            var spectraProviderCache = new SpectraProviderCache();
            var spectraProvider = spectraProviderCache.GetSpectraProvider(rawFile);

            // Read isos features
            var isosReader = new MsFeatureLightFileReader();
            isosReader.IsosFilteroptions = new DeconToolsIsosFilterOptions { MaximumIsotopicFit = 0.15 };
            var msFeatures = isosReader.ReadFile(isosFile).ToList();

            // Get LCMS features
            var msFeatureClusterer = new MsToLcmsFeatures(spectraProvider);
            var lcmsFeatures = msFeatureClusterer.Convert(msFeatures);
            lcmsFeatures.ForEach(feature => { feature.NetAligned = feature.Net; feature.MassMonoisotopicAligned = feature.MassMonoisotopic; });

            // Create clusters - Since this is only working on a single dataset, there should be a 1:1 mapping
            // between LCMS features and clusters.
            var clusters = new List<UMCClusterLight> { Capacity = lcmsFeatures.Count };
            foreach (var lcmsFeature in lcmsFeatures)
            {
                var cluster = new UMCClusterLight(lcmsFeature);
                cluster.CalculateStatistics(ClusterCentroidRepresentation.Median);
                clusters.Add(cluster);
            }

            // Do STAC AMT matching
            var stacAdapter = new STACAdapter<UMCClusterLight>
            {
                Options = new FeatureMatcherParameters
                {
                    ShouldCalculateShiftFDR = false,
                    UsePriors = true,
                    UseEllipsoid = true,
                    UseDriftTime = false,
                    ShouldCalculateSTAC = true,
                }
            };
            var amtMatches = stacAdapter.PerformPeakMatching(clusters, massTags);

            // Group AMT matches by cluster, convert MassTags to Protein objects (represents lipid ID,
            // rather than Protein ID here) for simplicity in comparing them to the MS/MS IDs.
            var ms1Matches = clusters.ToDictionary(cluster => cluster, cluster => new List<Protein>());
            foreach (var amtMatch in amtMatches)
            {
                var cluster = amtMatch.Observed;
                var massTag = amtMatch.Target;
                ms1Matches[cluster].Add(new Protein
                {
                    Name = massTag.ProteinName,
                    Sequence = massTag.PeptideSequence,
                    ChemicalFormula = massTag.PeptideSequence
                });
            }

            // Now we need to backtrack MS/MS identifications -> clusters
            var ms2Matches = new Dictionary<UMCClusterLight, List<Protein>>();
            foreach (var cluster in clusters)
            {
                ms2Matches.Add(cluster, new List<Protein>());
                foreach (var lcmsFeature in cluster.UmcList)
                {
                    foreach (var msFeature in lcmsFeature.MsFeatures)
                    {
                        foreach (var msmsFeature in msFeature.MSnSpectra)
                        {
                            if (scansToIds.ContainsKey(msmsFeature.Scan))
                            {
                                ms2Matches[cluster].AddRange(scansToIds[msmsFeature.Scan]);
                            }
                        }
                    }
                }
            }

            // How many clusters have IDs from MS/MS?
            var clusterMs1IdCount = ms1Matches.Values.Count(value => value.Any());
            var clusterMs2IdCount = ms2Matches.Values.Count(value => value.Any());

            int overlapCount = 0; // Number of IDs that overlapped between MS1 and MS2 identifications.

            // Finally compare the MS1 IDs to the MS2 IDs.
            foreach (var cluster in clusters)
            {
                // For now only comparing by name
                var ms1Ids = ms1Matches[cluster];
                var ms1Lipids = ms1Ids.Select(id => id.Name);

                var ms2Ids = ms2Matches[cluster];
                var ms2Lipids = ms2Ids.Select(id => id.Name);

                // Compare MS1 IDs for the cluster vs MS2 IDs for the cluster.
                var ms1OnlyIds = ms1Lipids.Where(lipid => !ms2Lipids.Contains(lipid));
                var ms2OnlyIds = ms2Lipids.Where(lipid => !ms1Lipids.Contains(lipid));

                overlapCount += ms1OnlyIds.Intersect(ms2OnlyIds).Count();

                // Write Results
                if (ms1OnlyIds.Any() || ms2OnlyIds.Any())
                {
                    Console.WriteLine("Cluster {0}:", cluster.Id);
                    if (ms1OnlyIds.Any())
                    {
                        Console.WriteLine("\tMs1 Only IDs:");
                        foreach (var id in ms1OnlyIds)
                        {
                            Console.WriteLine("\t\t{0}", id);
                        }
                    }

                    if (ms2OnlyIds.Any())
                    {
                        Console.WriteLine("\tMs2 Only IDs:");
                        foreach (var id in ms2OnlyIds)
                        {
                            Console.WriteLine("\t\t{0}", id);
                        }
                    }
                }
            }

            Console.WriteLine("Overlap: {0}", overlapCount);
        }

        private Dictionary<int, List<Protein>> GetIds(string liquidResultsFilePath)
        {
            var headers = new Dictionary<string, int>();

            var ids = new Dictionary<int, List<Protein>>();

            // Parse lines of file into mass tags.
            int lineCount = 0;
            foreach (var line in File.ReadLines(liquidResultsFilePath))
            {
                var parts = line.Split('\t');
                if (lineCount++ == 0)
                {   // Header line, store header indices
                    for (int i = 0; i < parts.Length; i++)
                    {
                        headers.Add(parts[i], i);
                    }

                    continue;
                }

                var scan = Convert.ToInt32(parts[headers["MS/MS Scan"]]);
                var mass = Convert.ToDouble(parts[headers["Exact m/z"]]);

                var name = parts[headers["Common Name"]];
                var formula = parts[headers["Formula"]];

                if (!ids.ContainsKey(scan))
                {
                    ids.Add(scan, new List<Protein>());
                }

                // Data line, create ID
                ids[scan].Add(
                    new Protein
                    {
                        Name = name,
                        Sequence = formula,
                        MassMonoisotopic = mass,
                    });
            }

            return ids;
        }
    }
}
