namespace MultiAlignCore.IO.TextFiles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.DatasetLoaders;

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

        #endregion

        /// <summary>
        /// Parses the column header text into a map of name column index.
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns>Mapping from column name to column index</returns>
        protected override Dictionary<string, int> CreateColumnMapping(TextReader textReader)
        {
            var columnMap = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
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
        /// <param name="filter"></param>
        /// <returns>Enumerable list of MSFeatureLight objects</returns>
        /// <remarks>Column delimiter is a comma</remarks>
        protected override IEnumerable<MSFeatureLight> SaveFileToEnumerable(TextReader textReader, Dictionary<string, int> columnMapping, IFeatureFilter<MSFeatureLight> filter = null)
        {
            string line;

            // Detect if the data comes from an IMS platform.
            var hasDriftTimeData = columnMapping.ContainsKey(FRAME_NUMBER);

            var featureList = new LinkedList<MSFeatureLight>();

            var deconToolsFilter = filter as DeconToolsFilter;

            while ((line = textReader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(',');
                var feature = new MSFeatureLight
                {
                    Scan = this.GetScanNumber(columns, columnMapping, hasDriftTimeData) ?? 0,
                    Abundance = this.GetAbundance(columns, columnMapping) ?? 0,
                    DriftTime = hasDriftTimeData && columnMapping.ContainsKey(SCAN_NUMBER) ? 
                                    int.Parse(columns[columnMapping[SCAN_NUMBER]]) : 0,
                };

                feature.Net = Convert.ToDouble(feature.Scan);

                if (columnMapping.ContainsKey(AVERAGE_MASS)) feature.MassMonoisotopicAverage = double.Parse(columns[columnMapping[AVERAGE_MASS]]);
                if (columnMapping.ContainsKey(ABUNDANT_MASS)) feature.MassMonoisotopicMostAbundant = double.Parse(columns[columnMapping[ABUNDANT_MASS]]);
                if (columnMapping.ContainsKey(CHARGE)) feature.ChargeState = int.Parse(columns[columnMapping[CHARGE]]);
                if (columnMapping.ContainsKey(MZ)) feature.Mz = double.Parse(columns[columnMapping[MZ]]);
                if (columnMapping.ContainsKey(MONO_MASS)) feature.MassMonoisotopic = double.Parse(columns[columnMapping[MONO_MASS]]);
                if (columnMapping.ContainsKey(ISOTOPIC_FIT)) feature.Score = double.Parse(columns[columnMapping[ISOTOPIC_FIT]]);

                bool shouldTestFeature = deconToolsFilter == null || !deconToolsFilter.UseDataCountFilter ||
                                         (featureList.Count == deconToolsFilter.MaximumDataPoints &&
                                         feature.Abundance >= featureList.Last.Value.Abundance);

                if (shouldTestFeature || filter == null || filter.ShouldKeepFeature(feature))
                {
                    // Add feature to feature list
                    var currentNode = featureList.First;
                    while (currentNode != null && feature.Abundance < currentNode.Value.Abundance)
                    {
                        currentNode = currentNode.Next;
                    }

                    if (currentNode == null)
                    {
                        featureList.AddLast(feature);
                    }
                    else if ((int)currentNode.Value.Abundance == (int)feature.Abundance && feature.Score > currentNode.Value.Score)
                    {   // DeconTools scores: the lower the better. Insert behind the current node if the feature has a worse score (higher)
                        featureList.AddAfter(currentNode, feature);
                    }
                    else
                    {
                        featureList.AddBefore(currentNode, feature);
                    }

                    if (deconToolsFilter != null && deconToolsFilter.UseDataCountFilter && featureList.Count > deconToolsFilter.MaximumDataPoints)
                    {
                        featureList.RemoveLast();
                    }
                }
            }

            int id = 0;
            var features = featureList.ToList();
            features.ForEach(feature => feature.Id = id++);
            return features;
        }

    }
}
