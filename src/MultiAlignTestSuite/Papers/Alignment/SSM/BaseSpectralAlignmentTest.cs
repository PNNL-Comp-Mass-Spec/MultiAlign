using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.Algorithms;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;
using MultiAlignTestSuite.Algorithms.SpectralProcessing;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms;
using System.IO;
using PNNLOmics.Algorithms.FeatureMatcher.MSnLinker;

namespace MultiAlignTestSuite.Papers.Alignment.SSM
{

    /// <summary>
    /// Base class for setting up tests
    /// </summary>
    public abstract class BaseSpectralAlignmentTest
    {

        protected void PrintHistogram(Histogram histogram)
        {
            Print("Histogram: " + histogram.Name);
            for (int i = 0; i < histogram.Bins.Count; i++)
            {
                Print(string.Format("{0},{1}", histogram.Bins[i], histogram.Data[i]));
            }
        }
        protected void PrintHistogram(string title, List<Histogram> histograms)
        {
            Print("Histograms: " + title);
            for (int i = 0; i < histograms[0].Bins.Count; i++)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("{0},", histograms[0].Bins[i]);
                for (int j = 0; j < histograms.Count; j++)
                {
                    builder.AppendFormat("{0},", histograms[j].Data[i]);
                }
                Print(builder.ToString().TrimEnd(','));
            }
        }

        /// <summary>
        /// Returns a list of MS/MS spectra
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        protected List<MSSpectra> GetSpectra(List<UMCLight> features)
        {
            List<MSSpectra> spectra = new List<MSSpectra>();
            foreach (UMCLight feature in features)
            {
                feature.MSFeatures.ForEach(x => spectra.AddRange(x.MSnSpectra));
            }
            return spectra;
        }

        /// <summary>
        /// Prints meta-data about how many features match up
        /// </summary>
        /// <param name="features"></param>
        protected void PrintFeatureMsMsData(List<UMCLight> features)
        {
            int count = 0;
            int doubleCount = 0;
            foreach (var feature in features)
            {
                int singleCount = 0;
                foreach (var msFeature in feature.MSFeatures)
                {
                    if (msFeature.MSnSpectra.Count > 0)
                    {
                        count++;
                        singleCount++;
                    }
                }
                doubleCount += (singleCount > 0) ? 1 : 0;
            }

            Console.WriteLine("{0} Features Have {1} MS/MS spectra - {2} have more than one", features.Count, count, doubleCount);
        }
        /// <summary>
        /// Retrieves a list of features.
        /// </summary>
        /// <param name="rawFile"></param>
        /// <param name="featureFile"></param>
        /// <returns></returns>
        public List<UMCLight> FindFeatures(string rawFile, string featureFile)
        {
            List<UMCLight> features = new List<UMCLight>();
            using (ISpectraProvider raw = new ThermoRawDataFileReader())
            {
                // Read the raw file summary data...
                raw.AddDataFile(rawFile, 0);

                DatasetInformation info = new DatasetInformation();
                info.Features = new InputFile();
                info.Features.Path = featureFile;

                IFeatureFinder finder = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.DeconToolsCSV);
                LCMSFeatureFindingOptions options = new LCMSFeatureFindingOptions();

                // Load and create features
                List<MSFeatureLight> msFeatures = UMCLoaderFactory.LoadMsFeatureData(info, null);
                features = finder.FindFeatures(msFeatures, options);
                List<MSSpectra> msms = raw.GetRawSpectra(0);

                // Link the features
                IMSnLinker linker = MSnLinkerFactory.CreateLinker(MSnLinkerType.BoxMethod);
                linker.Tolerances.Mass = .05;
                linker.LinkMSFeaturesToMSn(msFeatures, msms);
            }
            return features;
        }
        /// <summary>
        /// Default root test path.
        /// </summary>
        private const string CONST_ROOT_PATH = @"m:\data\proteomics\";

        /// <summary>
        /// Gets the root data path
        /// </summary>
        public string RootDataPath { get; set; }

        /// <summary>
        /// Creates the root test path
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
        /// Gets a list of the peptide matches from the path provided.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<PeptideMatch> GetPeptideMatches(string path)
        {
            List<string> lines = File.ReadAllLines(path).ToList();

            List<PeptideMatch> matches = new List<PeptideMatch>();
            foreach (string line in lines)
            {
                string[] data = line.Split(',');
                if (data.Length < 6)
                {
                    continue;
                }

                try
                {
                    PeptideMatch match = new PeptideMatch();
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
        /// Creates a dictionary for scan to peptide match
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Dictionary<int, Dictionary<int, PeptideMatch>> CreateMatches(List<PeptideMatch> matches, int type)
        {
            Dictionary<int, Dictionary<int, PeptideMatch>> matchesX = new Dictionary<int, Dictionary<int, PeptideMatch>>();
            foreach (PeptideMatch match in matches)
            {
                int scanx = match.ScanX;
                int scany = match.ScanY;
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
        /// Reads a peptide file.
        /// </summary>
        /// <param name="peptidePathX"></param>
        /// <returns></returns>
        protected Dictionary<int, PeptideTest> ReadPeptideFile(string peptidePath)
        {
            Dictionary<int, PeptideTest> peptideMap = new Dictionary<int, PeptideTest>();
            string[] lines = File.ReadAllLines(peptidePath);

            string header = lines[0];
            string[] headerData = header.Split('\t');

            int scanIndex = 0;
            int peptideIndex = 0;
            int fdrIndex = 0;
            int scoreIndex = 0;

            int i = 0;
            foreach (string x in headerData)
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
                string line = lines[i];
                string[] data = line.Split('\t');
                string pep = data[peptideIndex];
                int scan = Convert.ToInt32(data[scanIndex]);
                double score = Convert.ToDouble(data[scoreIndex]);
                double fdr = Convert.ToDouble(data[fdrIndex]);

                PeptideTest p = new PeptideTest();
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
            System.Console.WriteLine(message);
        }
    }
}
