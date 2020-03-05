using System;
using System.Collections.Generic;
using System.IO;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.IO.TextFiles
{
    /// <summary>
    /// Decon2ls (MS Feature) file text reader.
    /// </summary>
    public class MsFeatureFileReader : BaseTextFileReader<MSFeatureLight>
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

        /// <summary>
        /// Parses the column header text into a map of name column index.
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
        protected override Dictionary<string, int> CreateColumnMapping(TextReader textReader)
        {
            var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var readLine = textReader.ReadLine();
            if (readLine == null) return columnMap;

            var columnTitles = readLine.Split('\t', '\n');
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
        /// Saves the MS feature data to a list.
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="columnMapping"></param>
        /// <returns></returns>
        protected override IEnumerable<MSFeatureLight> SaveFileToEnumerable(TextReader textReader, Dictionary<string, int> columnMapping)
        {
            var features = new List<MSFeatureLight>();
            var currentId = 0;
            string line;

            // Detect if the data comes from an IMS platform.
            var hasDriftTimeData = columnMapping.ContainsKey(FRAME_NUMBER);

            while ((line = textReader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(new[] { Delimiter }, 0, StringSplitOptions.RemoveEmptyEntries);
                var feature = new MSFeatureLight
                {
                    Id = currentId
                };

                // In case this file does not have drift time, we need to make sure we clean up the
                // feature so the downstream processing can complete successfully.
                if (!hasDriftTimeData)
                {
                    if (columnMapping.ContainsKey(SCAN_NUMBER)) feature.Net = float.Parse(columns[columnMapping[SCAN_NUMBER]]);
                }
                else
                {
                    if (columnMapping.ContainsKey(FRAME_NUMBER)) feature.Net = float.Parse(columns[columnMapping[FRAME_NUMBER]]);
                    if (columnMapping.ContainsKey(SCAN_NUMBER)) feature.Scan = int.Parse(columns[columnMapping[SCAN_NUMBER]]);
                }

                if (columnMapping.ContainsKey(CHARGE)) feature.ChargeState = int.Parse(columns[columnMapping[CHARGE]]);
                if (columnMapping.ContainsKey(ABUNDANCE)) feature.Abundance = long.Parse(columns[columnMapping[ABUNDANCE]]);
                if (columnMapping.ContainsKey(MZ)) feature.Mz = float.Parse(columns[columnMapping[MZ]]);
                if (columnMapping.ContainsKey(ISOTOPIC_FIT)) feature.Score = float.Parse(columns[columnMapping[ISOTOPIC_FIT]]);
                if (columnMapping.ContainsKey(MONO_MASS)) feature.MassMonoisotopic = float.Parse(columns[columnMapping[MONO_MASS]]);

                features.Add(feature);
                currentId++;
            }
            return features;
        }
    }
}
