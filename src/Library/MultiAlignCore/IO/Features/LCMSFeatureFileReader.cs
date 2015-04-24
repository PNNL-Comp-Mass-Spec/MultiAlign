#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignCore.IO.Features
{
    public class LCMSFeatureFileReader : BaseLCMSFeatureReader<UMCLight>
    {
        #region Constructors

        /// <summary>
        ///     Constructor for passing in a StreamReader object
        /// </summary>
        /// <param name="streamReader">StreamReader object for UMC csv file to be read</param>
        public LCMSFeatureFileReader(StreamReader streamReader)
            : base(streamReader)
        {
            m_umcList = SaveDataToUmcList();
        }

        /// <summary>
        ///     Constructor for passing in a String containing the location of the UMC csv file
        /// </summary>
        /// <param name="filePath">String containing the location of the UMC csv file</param>
        public LCMSFeatureFileReader(string filePath)
            : base(filePath)
        {
            m_umcList = SaveDataToUmcList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Saves the data from a UMC csv file to an array of clsUMC Objects.
        /// </summary>
        protected override sealed List<UMCLight> SaveDataToUmcList()
        {
            var umcList = new List<UMCLight>();
            var previousId = -99;
            var idIndex = 0;

            var minScan = int.MaxValue;
            var maxScan = int.MinValue;

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

                umcList.Add(umc);

                minScan = Math.Min(umc.Scan, minScan);
                maxScan = Math.Max(umc.Scan, maxScan);

                previousId = umc.Id;
            }

            foreach (var x in umcList)
            {
                x.MassMonoisotopicAligned = x.MassMonoisotopic;
                x.ScanAligned = x.Scan;
                x.Net = (Convert.ToDouble(x.Scan - minScan) / Convert.ToDouble(maxScan - minScan));
                x.NetAligned = x.Net;
                x.Net = (Convert.ToDouble(x.Scan - minScan) / Convert.ToDouble(maxScan - minScan));
            }
            return umcList;
        }

        #endregion
    }
}