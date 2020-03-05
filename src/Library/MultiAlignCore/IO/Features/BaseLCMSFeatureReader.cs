using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.IO.Features
{
    public abstract class BaseLCMSFeatureReader<T>
    {
        protected readonly StreamReader m_umcFileReader;
        protected readonly Dictionary<String, int> m_columnMap;
        protected List<T> m_umcList;

        #region Constructors

        /// <summary>
        /// Constructor for passing in a StreamReader object
        /// </summary>
        /// <param name="streamReader">StreamReader object for UMC csv file to be read</param>
        protected BaseLCMSFeatureReader(StreamReader streamReader)
        {
            m_umcFileReader = streamReader;
            m_columnMap = CreateColumnMapping();
        }

        /// <summary>
        /// Constructor for passing in a String containing the location of the UMC csv file
        /// </summary>
        /// <param name="filePath">String containing the location of the UMC csv file</param>
        protected BaseLCMSFeatureReader(string filePath)
        {
            m_umcFileReader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            m_columnMap = CreateColumnMapping();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the umcList contained in this class
        /// </summary>
        public List<T> GetUmcList()
        {
            return m_umcList;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fills in the Column Map with the appropriate values.
        /// The Map will have a Column Property (e.g. Umc.Mass) mapped to a Column Number.
        /// </summary>
        /// <returns>The column map as a Dictionary object</returns>
        private Dictionary<string, int> CreateColumnMapping()
        {
            var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var readLine = m_umcFileReader.ReadLine();
            if (readLine == null) return columnMap;

            var columnTitles = readLine.Split('\t', '\n');
            var numOfColumns = columnTitles.Length;

            for (var i = 0; i < numOfColumns; i++)
            {
                var column = columnTitles[i].ToLower().Trim();
                switch (column)
                {
                    case "saturated_member_count":
                        columnMap.Add("Umc.SaturatedCount", i);
                        break;
                    case "umcindex":
                    case "feature_index":
                        columnMap.Add("Umc.Id", i);
                        break;
                    case "scanstart":
                    case "scan_start":
                        columnMap.Add("Umc.ScanStart", i);
                        break;
                    case "scanend":
                    case "scan_end":
                        columnMap.Add("Umc.ScanEnd", i);
                        break;
                    case "scanclassrep":
                    case "scan":
                        columnMap.Add("Umc.Scan", i);
                        columnMap.Add("Umc.ScanAligned", i);
                        break;
                    case "netclassrep":
                        columnMap.Add("Umc.Net", i);
                        break;
                    case "umcmonomw":
                    case "monoisotopic_mass":
                        columnMap.Add("Umc.Mass", i);
                        columnMap.Add("Umc.MassCalibrated", i);
                        break;
                    case "umcmwstdev":
                        columnMap.Add("Umc.MassStandardDeviation", i);
                        break;
                    case "umcmzforchargebasis":
                    case "class_rep_mz":
                        columnMap.Add("Umc.MZForCharge", i);
                        break;
                    case "umcabundance":
                    case "abundance":
                        columnMap.Add("Umc.AbundanceSum", i);
                        break;
                    case "maxabundance":
                    case "max_abundance":
                        columnMap.Add("Umc.AbundanceMax", i);
                        break;
                    case "classstatschargebasis":
                    case "class_rep_charge":
                        columnMap.Add("Umc.ChargeRepresentative", i);
                        break;
                    case "chargestatemax":
                        columnMap.Add("Umc.ChargeMax", i);
                        break;
                    case "umcmembercount":
                    case "umc_member_count":
                        columnMap.Add("Umc.SpectralCount", i);
                        break;
                    case "drift_time":
                        columnMap.Add("Umc.DriftTime", i);
                        break;
                    case "drift_time_uncorrected":
                        columnMap.Add("Umc.DriftTimeUncorrected", i);
                        break;
                    case "avg_interference_score":
                        columnMap.Add("Umc.AverageInterferenceScore", i);
                        break;
                    case "conformation_fit_score":
                        columnMap.Add("Umc.ConformationFitScore", i);
                        break;
                    case "decon2ls_fit_score":
                        columnMap.Add("Umc.AverageDeconFitScore", i);
                        break;
                    case "members_percentage":
                        columnMap.Add("Umc.MembersPercentageScore", i);
                        break;
                    case "combined_score":
                        columnMap.Add("Umc.CombinedScore", i);
                        break;
                }
            }

            return columnMap;
        }

        protected UMCExtended ParseLine(string line, int previousId, ref int idIndex, out string[] columns)
        {
            columns = line.Split(',', '\t', '\n');

            int currentId;
            if (m_columnMap.ContainsKey("Umc.Id"))
            {
                currentId = int.Parse(columns[m_columnMap["Umc.Id"]]);
            }
            else
            {
                currentId = idIndex;
                idIndex++;
            }

            // If the UMC ID matches the previous UMC ID, then skip the UMC data.
            //      - It is the same UMC, different peptide, and we have already stored this UMC data.
            if (previousId == currentId)
            {
                return null;
            }

            var umc = new UMCExtended
            {
                Id = currentId
            };

            if (m_columnMap.ContainsKey("Umc.ScanStart"))
                umc.ScanStart = int.Parse(columns[m_columnMap["Umc.ScanStart"]]);

            if (m_columnMap.ContainsKey("Umc.ScanEnd"))
                umc.ScanEnd = int.Parse(columns[m_columnMap["Umc.ScanEnd"]]);

            if (m_columnMap.ContainsKey("Umc.Scan"))
                umc.Scan = int.Parse(columns[m_columnMap["Umc.Scan"]]);

            if (m_columnMap.ContainsKey("Umc.Net"))
                umc.Net = double.Parse(columns[m_columnMap["Umc.Net"]]);

            if (m_columnMap.ContainsKey("Umc.Mass"))
                umc.MassMonoisotopic = double.Parse(columns[m_columnMap["Umc.Mass"]]);

            if (m_columnMap.ContainsKey("Umc.AbundanceSum"))
            {
                try
                {
                    // To handle bugs from Feature Finder.
                    var data = columns[m_columnMap["Umc.AbundanceSum"]];
                    if (data.StartsWith("-"))
                    {
                        umc.AbundanceSum = 0;
                    }
                    else
                    {
                        umc.AbundanceSum = long.Parse(data, NumberStyles.AllowDecimalPoint);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            if (m_columnMap.ContainsKey("Umc.AbundanceMax"))
            {
                umc.Abundance = double.Parse(columns[m_columnMap["Umc.AbundanceMax"]]);
            }

            if (m_columnMap.ContainsKey("Umc.ChargeRepresentative"))
            {
                umc.ChargeState = int.Parse(columns[m_columnMap["Umc.ChargeRepresentative"]]);
            }

            if (m_columnMap.ContainsKey("Umc.SpectralCount"))
                umc.SpectralCount = int.Parse(columns[m_columnMap["Umc.SpectralCount"]]);

            if (m_columnMap.ContainsKey("Umc.MZForCharge"))
                umc.Mz = double.Parse(columns[m_columnMap["Umc.MZForCharge"]]);

            if (m_columnMap.ContainsKey("Umc.DriftTime"))
                umc.DriftTime = double.Parse(columns[m_columnMap["Umc.DriftTime"]]);

            if (m_columnMap.ContainsKey("Umc.AverageInterferenceScore"))
            {
                var d = double.Parse(columns[m_columnMap["Umc.AverageInterferenceScore"]]);
                if (double.IsNegativeInfinity(d))
                {
                    d = Convert.ToDouble(Decimal.MinValue / 100);
                }
                else if (double.IsPositiveInfinity(d))
                {
                    d = Convert.ToDouble(double.MaxValue / 100);
                }
                umc.AverageInterferenceScore = d;
            }

            if (m_columnMap.ContainsKey("Umc.ConformationFitScore"))
                umc.ConformationFitScore = double.Parse(columns[m_columnMap["Umc.ConformationFitScore"]]);

            if (m_columnMap.ContainsKey("Umc.AverageDeconFitScore"))
                umc.AverageDeconFitScore = double.Parse(columns[m_columnMap["Umc.AverageDeconFitScore"]]);

            if (m_columnMap.ContainsKey("Umc.SaturatedCount"))
                umc.SaturatedMemberCount = int.Parse(columns[m_columnMap["Umc.SaturatedCount"]]);

            return umc;
        }

        /// <summary>
        /// Saves the data from a UMC csv file to an array of clsUMC Objects.
        /// </summary>
        protected abstract List<T> SaveDataToUmcList();

        #endregion

    }
}
