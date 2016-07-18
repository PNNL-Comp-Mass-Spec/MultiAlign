#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.IO.Features
{
    public class UmcReader : BaseLCMSFeatureReader<UMCExtended>
    {
        #region Constructors

        /// <summary>
        ///     Constructor for passing in a StreamReader object
        /// </summary>
        /// <param name="streamReader">StreamReader object for UMC csv file to be read</param>
        public UmcReader(StreamReader streamReader)
            : base(streamReader)
        {
            m_umcList = SaveDataToUmcList();
        }

        /// <summary>
        ///     Constructor for passing in a String containing the location of the UMC csv file
        /// </summary>
        /// <param name="filePath">String containing the location of the UMC csv file</param>
        public UmcReader(string filePath)
            : base(filePath)
        {
            m_umcList = SaveDataToUmcList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves the data from a UMC csv file to an array of UMCExtended Objects.
        /// </summary>
        protected override sealed List<UMCExtended> SaveDataToUmcList()
        {
            var umcList = new List<UMCExtended>();
            var previousId = -99;
            var idIndex = 0;

            // Read the rest of the Stream, 1 line at a time, and save the appropriate data into new Objects
            while (!m_umcFileReader.EndOfStream)
            {
                var line = m_umcFileReader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] columns;

                var umc = ParseLine(line, previousId, ref idIndex, out columns);

                if (umc == null)
                {
                    // The new UMC ID matched the previous UMC ID
                    continue;
                }

                // Store fields specific to a UMC file

                if (m_columnMap.ContainsKey("Umc.ScanAligned"))
                {
                    umc.ScanAligned = int.Parse(columns[m_columnMap["Umc.ScanAligned"]]);
                }
                else
                {
                    umc.ScanAligned = umc.Scan;
                }

                if (m_columnMap.ContainsKey("Umc.MassCalibrated"))
                {
                    umc.MassMonoisotopicAligned = double.Parse(columns[m_columnMap["Umc.MassCalibrated"]]);
                }
                else
                {
                    umc.MassMonoisotopicAligned = umc.MassMonoisotopic;
                }

                if (m_columnMap.ContainsKey("Umc.ChargeRepresentative"))
                {
                    umc.ChargeRepresentative = int.Parse(columns[m_columnMap["Umc.ChargeRepresentative"]]);
                    umc.ChargeMax = Math.Max(umc.ChargeMax, umc.ChargeRepresentative);
                }

                if (m_columnMap.ContainsKey("Umc.ChargeMax"))
                {
                    umc.ChargeMax = int.Parse(columns[m_columnMap["Umc.ChargeMax"]]);
                }

                if (m_columnMap.ContainsKey("Umc.MZForCharge"))
                    umc.MZForCharge = double.Parse(columns[m_columnMap["Umc.MZForCharge"]]);

                if (m_columnMap.ContainsKey("Umc.DriftTimeUncorrected"))
                    umc.DriftTimeUncorrected = double.Parse(columns[m_columnMap["Umc.DriftTimeUncorrected"]]);

                umcList.Add(umc);

                previousId = umc.Id;
            }
            return umcList;
        }

        #endregion
    }
}