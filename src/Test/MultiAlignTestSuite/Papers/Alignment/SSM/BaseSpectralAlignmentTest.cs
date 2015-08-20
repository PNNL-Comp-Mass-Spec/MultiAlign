#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.RawData;
using MultiAlignTestSuite.Algorithms;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment.SSM
{
    /// <summary>
    ///     Base class for setting up tests`
    /// </summary>
    public abstract class BaseSpectralAlignmentTest
    {
        protected void PrintHistogram(Histogram histogram)
        {
            Print("Histogram: " + histogram.Name);
            for (var i = 0; i < histogram.Bins.Count; i++)
            {
                Print(string.Format("{0},{1}", histogram.Bins[i], histogram.Data[i]));
            }
        }

        protected void PrintHistogram(string title, List<Histogram> histograms)
        {
            Print("Histograms: " + title);
            for (var i = 0; i < histograms[0].Bins.Count; i++)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("{0},", histograms[0].Bins[i]);
                for (var j = 0; j < histograms.Count; j++)
                {
                    builder.AppendFormat("{0},", histograms[j].Data[i]);
                }
                Print(builder.ToString().TrimEnd(','));
            }
        }

        /// <summary>
        ///     Returns a list of MS/MS spectra
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        protected List<MSSpectra> GetSpectra(List<UMCLight> features)
        {
            var spectra = new List<MSSpectra>();
            foreach (var feature in features)
            {
                feature.MsFeatures.ForEach(x => spectra.AddRange(x.MSnSpectra));
            }
            return spectra;
        }

        /// <summary>
        ///     Prints meta-data about how many features match up
        /// </summary>
        /// <param name="features"></param>
        protected void PrintFeatureMsMsData(List<UMCLight> features)
        {
            var count = 0;
            var doubleCount = 0;
            foreach (var feature in features)
            {
                var singleCount = 0;
                foreach (var msFeature in feature.MsFeatures)
                {
                    if (msFeature.MSnSpectra.Count > 0)
                    {
                        count++;
                        singleCount++;
                    }
                }
                doubleCount += (singleCount > 0) ? 1 : 0;
            }

            Console.WriteLine("{0} Features Have {1} MS/MS spectra - {2} have more than one", features.Count, count,
                doubleCount);
        }

        /// <summary>
        ///     Retrieves a list of features.
        /// </summary>
        /// <param name="rawFile"></param>
        /// <param name="featureFile"></param>
        /// <returns></returns>
        public List<UMCLight> FindFeatures(string rawFile, string featureFile)
        {
            List<UMCLight> features;
            using (ISpectraProvider raw = new InformedProteomicsReader())
            {
                // Read the raw file summary data...
                raw.AddDataFile(rawFile, 0);

                var info = new DatasetInformation();
                info.Features = new InputFile();
                info.Features.Path = featureFile;

                var finder = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.TreeBased);

                var tolerances = new FeatureTolerances
                {
                    Mass = 8,
                    Net = .005
                };
                var options = new LcmsFeatureFindingOptions(tolerances);


                // Load and create features
                var msFeatures = UmcLoaderFactory.LoadMsFeatureData(info.Features.Path);
                var provider = RawLoaderFactory.CreateFileReader(rawFile);
                features = finder.FindFeatures(msFeatures, options, provider);
            }
            return features;
        }

        /// <summary>
        ///     Gets the root data path
        /// </summary>
        public string RootDataPath { get; set; }

        /// <summary>
        ///     Creates the root test path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetTestPath(string path)
        {
            return Path.Combine(RootDataPath, path);
        }

        #region Peptide Utility

        #endregion

        #region Spectra Retrieval

        #endregion

        #region Peptide file reading

        /// <summary>
        ///     Gets a list of the peptide matches from the path provided.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<PeptideMatch> GetPeptideMatches(string path)
        {
            var lines = File.ReadAllLines(path).ToList();

            var matches = new List<PeptideMatch>();
            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (data.Length < 6)
                {
                    continue;
                }

                try
                {
                    var match = new PeptideMatch();
                    match.Peptide = data[5];
                    match.ScanX = Convert.ToInt32(data[1]);
                    match.ScanY = Convert.ToInt32(data[3]);
                    matches.Add(match);
                }
                catch
                {
                }
            }

            return matches;
        }

        /// <summary>
        ///     Creates a dictionary for scan to peptide match
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Dictionary<int, Dictionary<int, PeptideMatch>> CreateMatches(List<PeptideMatch> matches, int type)
        {
            var matchesX = new Dictionary<int, Dictionary<int, PeptideMatch>>();
            foreach (var match in matches)
            {
                var scanx = match.ScanX;
                var scany = match.ScanY;
                if (type == 1)
                {
                    scanx = match.ScanY;
                    scany = match.ScanX;
                }


                if (!matchesX.ContainsKey(scanx))
                {
                    matchesX.Add(scanx, new Dictionary<int, PeptideMatch>());
                }

                if (!matchesX[scanx].ContainsKey(scany))
                {
                    matchesX[scanx].Add(scany, match);
                }
            }

            return matchesX;
        }

        /// <summary>
        ///     Reads a peptide file.
        /// </summary>
        /// <param name="peptidePathX"></param>
        /// <returns></returns>
        protected Dictionary<int, PeptideTest> ReadPeptideFile(string peptidePath)
        {
            var peptideMap = new Dictionary<int, PeptideTest>();
            var lines = File.ReadAllLines(peptidePath);

            var header = lines[0];
            var headerData = header.Split('\t');

            var scanIndex = 0;
            var peptideIndex = 0;
            var fdrIndex = 0;
            var scoreIndex = 0;

            var i = 0;
            foreach (var x in headerData)
            {
                switch (x.ToLower())
                {
                    case "scan":
                        scanIndex = i;
                        break;
                    case "peptide":
                        peptideIndex = i;
                        break;
                    case "fdr":
                        fdrIndex = i;
                        break;
                    case "msgfdb_specprob":
                        scoreIndex = i;
                        break;
                    default:
                        break;
                }
                i++;
            }

            // Map all of the lines.
            for (i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var data = line.Split('\t');
                var pep = data[peptideIndex];
                var scan = Convert.ToInt32(data[scanIndex]);
                var score = Convert.ToDouble(data[scoreIndex]);
                var fdr = Convert.ToDouble(data[fdrIndex]);

                var p = new PeptideTest();
                p.Sequence = pep;
                p.Score = score;
                p.FDR = fdr;

                if (!peptideMap.ContainsKey(scan))
                {
                    peptideMap.Add(scan, p);
                }
            }
            return peptideMap;
        }

        #endregion

        protected void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}