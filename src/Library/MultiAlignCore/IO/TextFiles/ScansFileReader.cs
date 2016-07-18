using System;
using System.Collections.Generic;
using System.IO;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.TextFiles
{
    public class ScansFileReader : BaseTextFileReader<ScanSummary>
    {
        #region Column constants
        private const string SCAN_NUMBER = "scan";
        private const string SCAN_TIME = "scanTime";
        private const string FRAME_NUM = "frame";
        private const string FRAME_TIME = "frameTime";
        private const string TYPE = "abundance";
        private const string BPI = "fit";
        private const string BPI_MZ = "bpimz";
        private const string TIC = "tic";
        private const string NUM_PEAKS = "numberOfPeaks";
        private const string NUM_DEISOTOPED = "numberOfDeisotoped";
        #endregion

        /// <summary>
        /// Maps the .scans file header to a dictionary for column mapping.
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
        protected override Dictionary<string, int> CreateColumnMapping(TextReader textReader)
        {
            var columnMap = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);

            var readLine = textReader.ReadLine();
            if (readLine == null) return columnMap;

            string[] columnTitles;

            // Check for a null delimiter
            if (Delimiter == char.MinValue)
                columnTitles = readLine.Split(',', '\t');
            else
                columnTitles = readLine.Split(Delimiter);

            var numOfColumns = columnTitles.Length;

            for (var i = 0; i < numOfColumns; i++)
            {
                var column = columnTitles[i].ToLower().Trim();
                switch (column)
                {
                    case "scan_num":
                        columnMap.Add(SCAN_NUMBER, i);
                        break;
                    case "frame_num":
                        columnMap.Add(FRAME_NUM, i);
                        break;
                    case "scan_time":
                        columnMap.Add(SCAN_TIME, i);
                        break;
                    case "frame_time":
                        columnMap.Add(FRAME_TIME, i);
                        break;
                    case "type":
                        columnMap.Add(TYPE, i);
                        break;
                    case "bpi":
                        columnMap.Add(BPI, i);
                        break;
                    case "bpi_mz":
                        columnMap.Add(BPI_MZ, i);
                        break;
                    case "tic":
                        columnMap.Add(TIC, i);
                        break;
                    case "num_peaks":
                        columnMap.Add(NUM_PEAKS, i);
                        break;
                    case "num_deisotoped":
                        columnMap.Add(NUM_DEISOTOPED, i);
                        break;
                }
            }
            return columnMap;
        }

        protected override IEnumerable<ScanSummary> SaveFileToEnumerable(TextReader textReader, Dictionary<string, int> columnMapping)
        {
            var scans = new List<ScanSummary>();
            string line;

            char[] delimiters;

            // Check for a null delimiter
            if (Delimiter == char.MinValue)
                delimiters = new[] {',', '\t'};
            else
                delimiters = new[] { Delimiter };

            while ((line = textReader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                var scan = new ScanSummary();

                if (columnMapping.ContainsKey(SCAN_NUMBER)) scan.Scan = int.Parse(columns[columnMapping[SCAN_NUMBER]]);
                else if (columnMapping.ContainsKey(FRAME_NUM)) scan.Scan = int.Parse(columns[columnMapping[FRAME_NUM]]);

                if (columnMapping.ContainsKey(SCAN_TIME)) scan.Time = double.Parse(columns[columnMapping[SCAN_TIME]]);
                else if (columnMapping.ContainsKey(FRAME_TIME)) scan.Time = double.Parse(columns[columnMapping[FRAME_TIME]]);

                if (columnMapping.ContainsKey(TYPE)) scan.MsLevel = int.Parse(columns[columnMapping[TYPE]]);
                if (columnMapping.ContainsKey(BPI)) scan.Bpi = double.Parse(columns[columnMapping[BPI]]);
                if (columnMapping.ContainsKey(BPI_MZ)) scan.BpiMz = double.Parse(columns[columnMapping[BPI_MZ]]);
                if (columnMapping.ContainsKey(TIC)) scan.TotalIonCurrent = double.Parse(columns[columnMapping[TIC]]);
                if (columnMapping.ContainsKey(NUM_PEAKS)) scan.NumberOfPeaks = int.Parse(columns[columnMapping[NUM_PEAKS]]);
                if (columnMapping.ContainsKey(NUM_DEISOTOPED)) scan.NumberOfDeisotoped = int.Parse(columns[columnMapping[NUM_DEISOTOPED]]);

                scans.Add(scan);
            }

            return scans;
        }
    }
}
