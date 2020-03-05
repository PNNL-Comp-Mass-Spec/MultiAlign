using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.Features;
using NHibernate.Cache.Entry;

namespace MultiAlignCore.IO.TextFiles
{
    /// <summary>
    /// DeconTools (MS Feature) file text reader.
    /// </summary>
    /// <remarks>Column delimiter is a comma</remarks>
    public class MsFeatureLightFileReader : BaseTextFileReader<MSFeatureLight>
    {
        #region Column constants
        private const string SCAN_NUMBER = "scan";
        private const string FRAME_NUMBER = "frame";
        private const string MZ = "mz";
        private const string ABUNDANCE = "abundance";
        private const string ISOTOPIC_FIT = "fit";
        private const string CHARGE = "charge";
        private const string MONO_MASS = "monoMass";
        private const string AVERAGE_MASS = "averageMass";
        private const string ABUNDANT_MASS = "abundantMass";
        private const string FWHM = "fwhm";
        private const string MONO_ABUNDANCE = "monoAbundance";
        private const string MONO_2_ABUNDANCE = "mono2Abundance";
        private const string SNR = "SNR";
        #endregion

        #region Data Filters

        public DeconToolsIsosFilterOptions IsosFilteroptions { get; set; }

        #endregion

        /// <summary>
        /// Parses the column header text into a map of name column index.
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns>Mapping from column name to column index</returns>
        protected override Dictionary<string, int> CreateColumnMapping(TextReader textReader)
        {
            var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            mDataPointsLoaded = 0;
            mDataPointsSkipped = 0;

            var readLine = textReader.ReadLine();
            if (readLine == null) return columnMap;

            var columnTitles = readLine.Split(',', '\n');
            var numOfColumns = columnTitles.Length;

            for (var i = 0; i < numOfColumns; i++)
            {
                var column = columnTitles[i].ToLower().Trim();
                switch (column)
                {
                    case "frame_num":
                        columnMap.Add(FRAME_NUMBER, i);
                        break;
                    case "scan_num":
                        columnMap.Add(SCAN_NUMBER, i);
                        break;
                    case "charge":
                        columnMap.Add(CHARGE, i);
                        break;
                    case "abundance":
                        columnMap.Add(ABUNDANCE, i);
                        break;
                    case "mz":
                        columnMap.Add(MZ, i);
                        break;
                    case "fit":
                        columnMap.Add(ISOTOPIC_FIT, i);
                        break;
                    case "average_mw":
                        columnMap.Add(AVERAGE_MASS, i);
                        break;
                    case "monoisotopic_mw":
                        columnMap.Add(MONO_MASS, i);
                        break;
                    case "mostabundant_mw":
                        columnMap.Add(ABUNDANT_MASS, i);
                        break;
                    case "fwhm":
                        columnMap.Add(FWHM, i);
                        break;
                    case "signal_noise":
                        columnMap.Add(SNR, i);
                        break;
                    case "mono_abundance":
                        columnMap.Add(MONO_ABUNDANCE, i);
                        break;
                    case "mono_plus2_abundance":
                        columnMap.Add(MONO_2_ABUNDANCE, i);
                        break;
                }
            }
            return columnMap;
        }

        /// <summary>
        /// Prescan the _isos.csv file to determine the minimum abundance to keep such that MaximumDataPoints are retained when we actually load the data
        /// </summary>
        /// <param name="isosFile"></param>
        /// <param name="columnMapping"></param>
        /// <returns></returns>
        private double PreScanFile(FileInfo isosFile, Dictionary<string, int> columnMapping)
        {
            // Note: this code is adapted from project DeconIsosFilter

            var maxDataPointsOverall = IsosFilteroptions.MaximumDataPoints;

            var cullThreshold = (int)(maxDataPointsOverall * 1.5);
            if (cullThreshold < 10)
                cullThreshold = 10;

            var abundances = new double[cullThreshold];
            var targetIndex = 0;

            var dtLastProgress = DateTime.UtcNow;
            var totalLines = 0;
            long bytesRead = 0;

            Console.WriteLine("Pre-scanning the data file to determine the minimum abundance to retain");

            var hasDriftTimeData = columnMapping.ContainsKey(FRAME_NUMBER);

            using (var reader = new StreamReader(new FileStream(isosFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                while (!reader.EndOfStream)
                {
                    var dataLine = reader.ReadLine();
                    totalLines++;
                    bytesRead += GetLength(dataLine);

                    if (string.IsNullOrEmpty(dataLine))
                        continue;

                    var columns = dataLine.Split(',');
                    if (totalLines == 1 && char.IsLetter(columns[0][0]))
                    {
                        // This is the header line; skip it
                        continue;
                    }

                    var scanNum = GetScanNumber(columns, columnMapping, hasDriftTimeData);

                    var abundance = GetAbundance(columns, columnMapping);

                    if (scanNum == null || abundance == null)
                        continue;

                    abundances[targetIndex] = abundance ?? 0;
                    targetIndex++;

                    if (DateTime.UtcNow.Subtract(dtLastProgress).TotalSeconds >= 1)
                    {
                        dtLastProgress = DateTime.UtcNow;

                        var percentComplete = bytesRead / (double)isosFile.Length * 100.0;
                        Console.WriteLine("{0,3:0}% complete, scan {1:#,##0}", percentComplete, scanNum);
                    }

                    if (targetIndex < cullThreshold)
                    {
                        continue;
                    }

                    Array.Sort(abundances);
                    Array.Reverse(abundances);
                    targetIndex = maxDataPointsOverall;

                }
            }

            Console.WriteLine();
            Console.WriteLine("Read {0:#,##0} data points; will keep {1:#,##0} points", totalLines, maxDataPointsOverall);
            Console.WriteLine();

            double minimumAbundanceToKeep;

            if (targetIndex <= maxDataPointsOverall)
            {
                minimumAbundanceToKeep = abundances.Take(targetIndex).Min();
            }
            else
            {
                Array.Sort(abundances, 0, targetIndex);
                minimumAbundanceToKeep = abundances[targetIndex - maxDataPointsOverall];
            }

            return minimumAbundanceToKeep;
        }

        /// <summary>
        /// Return the length of the data line, including the line terminator
        /// </summary>
        /// <param name="dataLine"></param>
        /// <param name="lineTerminatorSize"></param>
        /// <returns></returns>
        private int GetLength(string dataLine, byte lineTerminatorSize = 2)
        {
            if (string.IsNullOrEmpty(dataLine))
                return lineTerminatorSize;

            return dataLine.Length + lineTerminatorSize;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="columnMapping"></param>
        /// <param name="hasDriftTimeData"></param>
        /// <returns>Scan number, or null if a problem</returns>
        private int? GetScanNumber(string[] columns, Dictionary<string, int> columnMapping, bool hasDriftTimeData)
        {
            int? scanNumber = null;

            if (hasDriftTimeData)
            {
                if (columnMapping.ContainsKey(FRAME_NUMBER))
                {
                    scanNumber = int.Parse(columns[columnMapping[FRAME_NUMBER]]);
                }
            }
            else
            {
                if (columnMapping.ContainsKey(SCAN_NUMBER))
                {
                    scanNumber = int.Parse(columns[columnMapping[SCAN_NUMBER]]);
                }
            }

            return scanNumber;
        }

        /// <summary>
        /// Return the abudance value, as a double
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="columnMapping"></param>
        /// <returns>Abundance, or null if a problem</returns>
        private double? GetAbundance(string[] columns, Dictionary<string, int> columnMapping)
        {
            double? abundance = null;

            if (columnMapping.ContainsKey(ABUNDANCE))
            {
                // DeconTools auto processor sometimes writes the abundances as doubles; convert to int

                var data = columns[columnMapping[ABUNDANCE]];
                long longAbundance;
                var successInt = long.TryParse(data, out longAbundance);
                if (successInt)
                {
                    abundance = longAbundance;
                }
                else
                {

                    double abundanceDouble;
                    var successDbl = double.TryParse(data, out abundanceDouble);
                    if (successDbl)
                    {
                        abundance = abundanceDouble;
                    }
                    else
                    {
                        throw new InvalidDataException("Non-numeric abundance value in _isos file: " + data);
                    }
                }

            }

            return abundance;
        }

        /// <summary>
        /// Reads the MS feature data from a text file and returns an enumerable list
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="columnMapping"></param>
        /// <returns>Enumerable list of MSFeatureLight objects</returns>
        /// <remarks>Column delimiter is a comma</remarks>
        protected override IEnumerable<MSFeatureLight> SaveFileToEnumerable(TextReader textReader, Dictionary<string, int> columnMapping)
        {
            var features = new List<MSFeatureLight>();
            var currentId = 0;
            string line;

            if (IsosFilteroptions == null)
                IsosFilteroptions = new DeconToolsIsosFilterOptions();

            double dataCountFilterMinimumAbundance;

            if (IsosFilteroptions.UseDataCountFilter && !string.IsNullOrWhiteSpace(mTextFilePath))
            {
                var dataFile = new FileInfo(mTextFilePath);

                dataCountFilterMinimumAbundance = PreScanFile(dataFile, columnMapping);
            }
            else
            {
                dataCountFilterMinimumAbundance = -1;
            }

            // Detect if the data comes from an IMS platform.
            var hasDriftTimeData = columnMapping.ContainsKey(FRAME_NUMBER);

            while ((line = textReader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(',');
                var feature = new MSFeatureLight { Id = currentId };

                //If mono_plus2_abundance is 0, we don't want to count this as a real feature.
                //TODO: Make an optional filter. This biases against low abundance features.
                //if (columnMapping.ContainsKey(MONO_2_ABUNDANCE))
                //    if (double.Parse(columns[columnMapping[MONO_2_ABUNDANCE]]) <= 0) continue;

                // In case this file does not have drift time, we need to make sure we clean up the
                // feature so the downstream processing can complete successfully.

                feature.Scan = GetScanNumber(columns, columnMapping, hasDriftTimeData) ?? 0;

                if (hasDriftTimeData && columnMapping.ContainsKey(SCAN_NUMBER))
                {
                    // Frame number has already been stored in feature.Scan
                    // Now parse out the IMS scan number and store as DriftTime
                    feature.DriftTime = int.Parse(columns[columnMapping[SCAN_NUMBER]]);
                }

                feature.Net = Convert.ToDouble(feature.Scan);

                if (IsosFilteroptions.UseScanFilter && (feature.Scan < IsosFilteroptions.LCScanStart || feature.Scan > IsosFilteroptions.LCScanEnd))
                {
                    mDataPointsSkipped++;
                    continue;
                }

                if (columnMapping.ContainsKey(CHARGE)) feature.ChargeState = int.Parse(columns[columnMapping[CHARGE]]);

                feature.Abundance = GetAbundance(columns, columnMapping) ?? 0;

                if (feature.Abundance < dataCountFilterMinimumAbundance ||
                     IsosFilteroptions.UseAbundanceFilter &&
                     (feature.Abundance < IsosFilteroptions.AbundanceMinimum || feature.Abundance > IsosFilteroptions.AbundanceMaximum))
                {
                    mDataPointsSkipped++;
                    continue;
                }

                if (columnMapping.ContainsKey(MZ)) feature.Mz = double.Parse(columns[columnMapping[MZ]]);
                if (columnMapping.ContainsKey(MONO_MASS)) feature.MassMonoisotopic = double.Parse(columns[columnMapping[MONO_MASS]]);
                if (columnMapping.ContainsKey(ISOTOPIC_FIT))
                {
                    feature.Score = double.Parse(columns[columnMapping[ISOTOPIC_FIT]]);

                    if (IsosFilteroptions.UseIsotopicFitFilter && feature.Score > IsosFilteroptions.MaximumIsotopicFit)
                    {
                        mDataPointsSkipped++;
                        continue;
                    }
                }

                if (columnMapping.ContainsKey(AVERAGE_MASS)) feature.MassMonoisotopicAverage = double.Parse(columns[columnMapping[AVERAGE_MASS]]);
                if (columnMapping.ContainsKey(ABUNDANT_MASS)) feature.MassMonoisotopicMostAbundant = double.Parse(columns[columnMapping[ABUNDANT_MASS]]);

                features.Add(feature);
                currentId++;
                mDataPointsLoaded++;
            }

            return features;
        }

    }
}
