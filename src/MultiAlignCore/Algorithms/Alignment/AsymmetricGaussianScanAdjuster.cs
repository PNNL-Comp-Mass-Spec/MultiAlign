using System;
using System.Collections.Generic;
using PNNLOmics.Algorithms;
using System.IO;
using PNNLOmics.Data.Features;
using PNNLOmics.Data;
using MultiAlignCore.Extensions;
using System.Diagnostics;

namespace MultiAlignCore.Algorithms.Alignment
{
    /// <summary>
    /// Adjusts the scans for features. 
    /// </summary>
    public class AsymmetricGaussianScanAdjuster: ILcScanAdjuster
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        public AsymmetricGaussianScanAdjuster()
        {
            PythonExePath   = @"C:\Python26\python.exe";
            PythonPath      = @"scripts\fit_elution_profile.py";
            DirectoryPath   = "sic";
        }

        public string PythonExePath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the path to the python script that generates the SIC
        /// </summary>
        public string PythonPath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the path to write the temporary SIC's
        /// </summary>
        public string DirectoryPath
        {
            get;
            set;
        }

        public int ChargeStateMin
        {
            get;
            set;
        }
        public int ChargeStateMax
        {
            get;
            set;
        }

        /// <summary>
        /// Builds the sic path for the python script based on charge etc.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="featureID"></param>
        /// <param name="charge"></param>
        /// <returns></returns>
        private string BuildPath(int dataset, int featureID, int charge)
        {
            return string.Format("sic-{0}-{1}-{2}.csv", dataset, featureID, charge);
        }

        private bool RationalTest(UMCLight feature, SicFitData fit, int minScan, int maxScan)
        {            
            // Make sure that the fit is ok
            if (!fit.FitOk)
                return false;

            if (fit.HNegative < 0 || fit.HPositive < 0)
                return false;
    
            // is the feature in range of the original experiment?
            if (fit.Scan > maxScan)
                return false;

            if (fit.Scan < minScan)
                return false;

            if (fit.Scan + fit.HPositive > maxScan)
                return false;

            if (fit.Scan - fit.HNegative < minScan)
                return false;

            if (fit.HNegative > fit.HPositive)
                return false;

            return true;
        }

        /// <summary>
        /// Adjusts the scans for the set of features.
        /// </summary>
        /// <param name="features"></param>
        public List<UMCLight> AdjustScans(List<UMCLight> features)
        {
            if (string.IsNullOrEmpty(DirectoryPath))
                throw new Exception("The SIC path was not provided.");

            bool doesExist = Directory.Exists(DirectoryPath);

            if (!doesExist)
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            Dictionary<int, List<string>> featureToScanPath = new Dictionary<int, List<string>>();

            //features.Clear();
            string featureFile = Path.Combine(DirectoryPath, "sic-features2.csv");
            
            using (TextWriter writer = File.CreateText(featureFile))
            {
                foreach (UMCLight feature in features)
                {
                    featureToScanPath.Add(feature.ID, new List<string>());

                    Dictionary<int, List<XYZData>> sicMap = feature.CreateChargeSIC();
                    foreach (int chargeState in sicMap.Keys)
                    {
                        string subPath = BuildPath(feature.GroupID, feature.ID, chargeState);
                        List<XYZData> chargeMap = sicMap[chargeState];
                        {
                            writer.WriteLine("feature,{0}", subPath);
                            writer.WriteLine("mz,scan,intensity");                            
                            chargeMap.ForEach(x => writer.WriteLine("{0},{1},{2}", x.Z, x.X, Math.Log(x.Y)));
                        }
                        featureToScanPath[feature.ID].Add(subPath);
                    }
                }
            }
            // Now run the scripts
            string arguments = string.Format("{1} -d {0} -f {0}\\fits.csv", Path.GetFullPath( DirectoryPath), Path.GetFullPath( PythonPath));
            using (Process python       = new Process())
            {
                python.StartInfo.UseShellExecute        = false;
                python.StartInfo.FileName               = "python.exe";
                python.StartInfo.Arguments              = arguments;                                
                bool started = python.Start();                
                if (!started)
                {
                    started = true;
                }
                python.WaitForExit();
            }
            
            
            string[] data = File.ReadAllLines(Path.Combine(DirectoryPath, "fits.csv"));            
            SicFitFileParser fitReader = new SicFitFileParser();
            Dictionary<string, SicFitData>  fits = fitReader.ParseData(data);            

            List<UMCLight> newFeatures = new List<UMCLight>();            
            int lastMinScan = int.MaxValue;
            int lastMaxScan = int.MinValue;
            foreach (UMCLight feature in features)
            {
                lastMinScan = Math.Min(lastMinScan, feature.Scan);
                lastMaxScan = Math.Max(lastMaxScan, feature.Scan);
            }

            foreach (UMCLight feature in features)
            {
                // Find the best fit for the feature.
                List<string> paths  = featureToScanPath[feature.ID];
                SicFitData bestFit  = null;
                foreach (string path in paths)
                {
                    if (!fits.ContainsKey(path))
                        continue;

                    SicFitData fit = fits[path];

                    if (!fit.FitOk)
                        continue;

                    bool doesFitPassRationalTests = RationalTest(feature, fit, lastMinScan, lastMaxScan);
                    if (!doesFitPassRationalTests)
                        continue;

                    if (bestFit == null)
                    {
                        bestFit = fit;
                    }
                    else
                    {                        
                        if (bestFit.Rms > fit.Rms)
                        {
                            bestFit = fit;
                        }
                    }
                }

                if (bestFit != null)
                {
                    UMCLight newFeature = new UMCLight(feature);
                    feature.Scan        = Convert.ToInt32(bestFit.Scan);
                    newFeatures.Add(newFeature);
                }
                else
                {
                    newFeatures.Add(new UMCLight(feature));
                }
            }

            features.ForEach(x => x.RetentionTime = Convert.ToDouble(x.Scan - lastMinScan) / Convert.ToDouble(lastMaxScan - lastMinScan));
            return newFeatures;
        }
    }

    /// <summary>
    /// Holds data about characterizing an SIC
    /// </summary>
    public class SicFitData
    {
        public string Source
        {
            get;
            set;
        }
        public int ScanMin
        {
            get;
            set;
        }
        public int ScanMax
        {
            get;
            set;
        }
        public int ScanAbundant
        {
            get;
            set;
        }
        public bool FitOk
        {
            get;
            set;
        }
        public double Scan
        {
            get;
            set;
        }
        public long Intensity
        {
            get;
            set;
        }
        public double HPositive
        {
            get;
            set;
        }
        public double HNegative
        {
            get;
            set;
        }
        public double Rms
        {
            get;
            set;
        }
        public int NumberOfScans
        {
            get;
            set;
        }
    }
    /// <summary>
    /// Parses a list of SIC data read from a file.
    /// </summary>
    public class SicFitFileParser
    {
        public Dictionary<string, SicFitData> ParseData(string [] allLines)
        {
            Dictionary<string, SicFitData> fits = new Dictionary<string,SicFitData>();
            
            
            for (int i = 1; i < allLines.Length; i++)
            {
                string lowerLine = allLines[i].ToLower();
                if (lowerLine.Contains("warning:"))
                {
                    continue;
                }

                string[] lineData = lowerLine.Split(new string [] {","}, StringSplitOptions.RemoveEmptyEntries);
                if (lineData.Length < 11)
                    continue;

                SicFitData fit      = new SicFitData();
                fit.Source          = lineData[0];
                fit.NumberOfScans   = Convert.ToInt32(lineData[1]);
                fit.ScanMin         = Convert.ToInt32(lineData[2]);
                fit.ScanMax         = Convert.ToInt32(lineData[3]);
                fit.ScanAbundant    = Convert.ToInt32(lineData[4]);
                fit.FitOk = false;
                if (lineData[5] == "y")
                {
                    fit.FitOk = true;
                }

                fit.Scan            = Convert.ToInt32(Math.Round(Convert.ToDouble(lineData[6])));
                fit.Intensity       = Convert.ToInt64(Math.Round(Convert.ToDouble(lineData[7])));
                fit.HNegative       = Convert.ToDouble(lineData[8]);
                fit.HPositive       = Convert.ToDouble(lineData[9]);
                fit.Rms             = Convert.ToDouble(lineData[10]);

                fits.Add(fit.Source, fit);
            }
            return fits;
        }
    }
}
