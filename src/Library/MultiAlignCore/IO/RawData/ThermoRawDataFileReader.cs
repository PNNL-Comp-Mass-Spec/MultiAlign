#region

using System;
using System.Collections.Generic;
using System.IO;
using PNNLOmics.Data;
using ThermoRawFileReaderDLL.FinniganFileIO;

#endregion

namespace MultiAlignCore.IO.RawData
{
    /// <summary>
    ///     Adapter for the Thermo Finnigan file format reader made by Matt Monroe.
    /// </summary>
    public class ThermoRawDataFileReader : ISpectraProvider
    {
        /// <summary>
        ///     Readers for each dataset.
        /// </summary>
        private readonly Dictionary<int, XRawFileIO> m_readers;

        private readonly Dictionary<int, string> m_dataFiles;
        private readonly Dictionary<int, bool> m_opened;

        /// <summary>
        ///     Gets or sets the map
        /// </summary>
        private readonly Dictionary<int, DatasetSummary> m_datasetMetaData;

        /// <summary>
        /// Constructor
        /// </summary>
        public ThermoRawDataFileReader()
        {
            m_datasetMetaData = new Dictionary<int, DatasetSummary>();
            m_dataFiles = new Dictionary<int, string>();
            m_readers = new Dictionary<int, XRawFileIO>();
            m_opened = new Dictionary<int, bool>();
        }

        /// <summary>
        /// Get the total number of scans for the given dataset
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public int GetTotalScans(int groupId)
        {
            // Get the RawFileReader for this group
            var rawReader = GetReaderForGroup(groupId);

            return rawReader.GetNumScans();
        }

        /// <summary>
        /// Adds a dataset file to the reader map so when in use the application
        /// knows where to get raw data from.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="groupId"></param>
        public void AddDataFile(string path, int groupId)
        {
            if (m_opened.ContainsKey(groupId))
            {
                try
                {
                    if (m_readers.ContainsKey(groupId))
                    {
                        m_readers[groupId].CloseRawFile();
                    }
                }
                catch
                {
                }

                m_opened[groupId] = false;
                m_dataFiles[groupId] = path;

                return;
            }
            m_dataFiles.Add(groupId, path);
            m_opened.Add(groupId, false);
        }

        #region IRawDataFileReader Members

        /// <summary>
        /// Reads a list of MSMS Spectra header data from the Raw file
        /// </summary>
        /// <param name="groupId">File group ID</param>
        /// <param name="excludeMap">Dictionary indicating which scans and related feature ID's to ignore.</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> GetMSMSSpectra(int groupId, Dictionary<int, int> excludeMap)
        {
            return GetMSMSSpectra(groupId, excludeMap, false);
        }

        /// <summary>
        /// Reads a list of MSMS Spectra header data from the Raw file
        /// </summary>
        /// <param name="groupId">File group ID</param>
        /// <param name="excludeMap">Dictionary indicating which scans and related feature ID's to ignore.</param>
        /// <param name="loadPeaks">True to also load the mass/intensity pairs for each spectrum</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> GetMSMSSpectra(int groupId, Dictionary<int, int> excludeMap, bool loadPeaks)
        {
            // Get the RawFileReader for this group
            var rawReader = GetReaderForGroup(groupId);

            var spectra = new List<MSSpectra>();

            var numberOfScans = rawReader.GetNumScans();
            for (var i = 0; i < numberOfScans; i++)
            {
                var isInExcludeMap = excludeMap.ContainsKey(i);
                if (isInExcludeMap)
                {
                    // This scan is not to be used.
                    continue;
                }

                FinniganFileReaderBaseClass.udtScanHeaderInfoType header;
                var summary = GetScanSummary(i, rawReader, out header);

                if (header.MSLevel > 1)
                {
                    var spectrum = new MSSpectra
                    {
                        MsLevel = header.MSLevel,
                        RetentionTime = header.RetentionTime,
                        Scan = i,
                        PrecursorMz = header.ParentIonMZ,
                        TotalIonCurrent = header.TotalIonCurrent,
                        CollisionType = summary.CollisionType
                    };
                   
                    // Need to make this a standard type of collision based off of the data.
                    if (loadPeaks)
                    {
                        spectrum.Peaks = LoadRawSpectra(rawReader, i);
                    }
                    spectra.Add(spectrum);
                }
            }

            return spectra;
        }

        /// <summary>
        /// Reads a list of MSMS Spectra header data from the mzXML file.
        /// </summary>
        /// <param name="groupId">File Group ID</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> GetMSMSSpectra(int groupId)
        {
            return GetMSMSSpectra(groupId, new Dictionary<int, int>(), false);
        }

        #endregion

        #region IMSMSDataSource Members

        /// <summary>
        ///     Gets the raw data from the data file.
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="groupId">File Group ID</param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public List<XYData> GetRawSpectra(int scan, int groupId, out ScanSummary summary)
        {
            return GetRawSpectra(scan, groupId, -1, out summary);
        }

        /// <summary>
        /// Gets the raw data from the data file.
        /// </summary>
        public List<MSSpectra> GetRawSpectra(int groupId)
        {
            return GetMSMSSpectra(groupId, new Dictionary<int, int>(), true);
        }

        private List<XYData> LoadRawSpectra(XRawFileIO rawReader, int scan)
        {

            double[] mz;
            double[] intensities;
            rawReader.GetScanData(scan, out mz, out intensities);

            if (mz == null)
                return new List<XYData>();

            var data = new List<XYData>(mz.Length);
            for (var i = 0; i < mz.Length; i++)
            {
                var intensity = intensities[i];
                data.Add(new XYData(mz[i], intensity));
            }
            return data;
        }

        /// <summary>
        /// Gets the raw data from the data file.
        /// </summary>
        public List<XYData> GetRawSpectra(int scan, int groupId, int msLevel)
        {
            ScanSummary summary;
            return GetRawSpectra(scan, groupId, msLevel, out summary);
        }

        public List<XYData> GetRawSpectra(int scan, int groupId, int msLevel, out ScanSummary summary)
        {

            var rawReader = GetScanSummaryAndReader(scan, groupId, out summary);
            if (rawReader == null)
                return null;
            
            if (summary.MsLevel != msLevel && msLevel > 0)
                return null;

            var data = LoadRawSpectra(rawReader, scan);

            return data;
        }

        private XRawFileIO GetReaderForGroup(int groupId)
        {
            if (!m_dataFiles.ContainsKey(groupId))
            {
                throw new Exception("The group-dataset ID provided was not found.");
            }

            // If we dont have a reader, then create one for this group 
            // next time, it will be available and we won't have to waste time
            // opening the file.

            if (!m_readers.ContainsKey(groupId))
            {
                var path = m_dataFiles[groupId];
                var reader = new XRawFileIO();

                m_readers.Add(groupId, reader);

                var opened = reader.OpenRawFile(path);
                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[groupId]);
                }
            }

            var rawReader = m_readers[groupId];

            return rawReader;
        }

        /// <summary>
        /// Gets the scan header information for the specified scan
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="groupId"></param>
        public ScanSummary GetScanSummary(int scan, int groupId)
        {
            ScanSummary summary;
            GetScanSummaryAndReader(scan, groupId, out summary);
            return summary;
        }

        private XRawFileIO GetScanSummaryAndReader(int scan, int groupId, out ScanSummary summary)
        {
            // Get the RawFileReader for this group
            var rawReader = GetReaderForGroup(groupId);

            ValidateScanNumber(scan, rawReader);

            FinniganFileReaderBaseClass.udtScanHeaderInfoType header;
            summary = GetScanSummary(scan, rawReader, out header);

            return rawReader;
        }

        /// <summary>
        ///     Determines if the scan is a precursor scan or not.
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool IsPrecursorScan(int scan, int groupId)
        {
            // Get the RawFileReader for this group
            var rawReader = GetReaderForGroup(groupId);

            FinniganFileReaderBaseClass.udtScanHeaderInfoType header;
            rawReader.GetScanInfo(scan, out header);

            return header.MSLevel == 1;
        }



        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Cleanup closing each of the raw file readers if they were read.
        /// </summary>
        public void Dispose()
        {
            foreach (var key in m_readers.Keys)
            {
                try
                {
                    m_readers[key].CloseRawFile();
                }
                catch
                {
                    // We don't want it to fail.  Just go with it.
                }
            }
            m_readers.Clear();
        }

        #endregion

        #region ISpectraProvider Members

        public int GetMinScan(int groupId)
        {
            return 1;
        }

        public int GetMaxScan(int groupId)
        {
            return GetReaderForGroup(groupId).GetNumScans();
        }

        /// <summary>
        ///     Retrieves a dictionary of scan data
        /// </summary>
        /// <param name="groupId">Group ID for dataset</param>
        /// <returns>Map between </returns>
        public Dictionary<int, ScanSummary> GetScanData(int groupId)
        {
            if (m_datasetMetaData.ContainsKey(groupId))
            {
                return m_datasetMetaData[groupId].ScanMetaData;
            }

            // Get the RawFileReader for this group
            var rawReader = GetReaderForGroup(groupId);

            var datasetSummary = new DatasetSummary();
            var scanMap = new Dictionary<int, ScanSummary>();
            var numberOfScans = rawReader.GetNumScans();
            for (var i = 0; i < numberOfScans; i++)
            {
                FinniganFileReaderBaseClass.udtScanHeaderInfoType header;
                var summary = GetScanSummary(i, rawReader, out header);
                
                scanMap.Add(i, summary);
            }

            datasetSummary.ScanMetaData = scanMap;

            m_datasetMetaData.Add(groupId, datasetSummary);
            return datasetSummary.ScanMetaData;
        }

        #endregion

        public MSSpectra GetSpectrum(int scan, int groupId, int scanLevel, out ScanSummary summary, bool loadPeaks)
        {
            // Get the RawFileReader for this group
            var rawReader = GetReaderForGroup(groupId);

            ValidateScanNumber(scan, rawReader);

            FinniganFileReaderBaseClass.udtScanHeaderInfoType header;
            summary = GetScanSummary(scan, rawReader, out header);

            var spectrum = new MSSpectra
            {
                MsLevel = header.MSLevel,
                RetentionTime = header.RetentionTime,
                Scan = scan,
                PrecursorMz = header.ParentIonMZ,
                TotalIonCurrent = header.TotalIonCurrent,
                CollisionType = summary.CollisionType
            };

            // Need to make this a standard type of collision based off of the data.
            if (loadPeaks)
            {
                spectrum.Peaks = LoadRawSpectra(rawReader, scan);
            }

            return spectrum;
        }


        private ScanSummary GetScanSummary(int scan, XRawFileIO rawReader, out FinniganFileReaderBaseClass.udtScanHeaderInfoType header)
        {
            rawReader.GetScanInfo(scan, out header);

            var summary = new ScanSummary
            {
                MsLevel = header.MSLevel,
                Time = header.RetentionTime,
                Scan = scan,
                TotalIonCurrent = Convert.ToInt64(header.TotalIonCurrent),
                PrecursorMz = header.ParentIonMZ,
                CollisionType = CollisionType.Other,
            };

            switch (header.CollisionMode)
            {
                case "cid":
                    summary.CollisionType = CollisionType.Cid;
                    break;
                case "hcd":
                    summary.CollisionType = CollisionType.Hcd;
                    break;
                case "etd":
                    summary.CollisionType = CollisionType.Etd;
                    break;
                case "ecd":
                    summary.CollisionType = CollisionType.Ecd;
                    break;
                case "hid":
                    summary.CollisionType = CollisionType.Hid;
                    break;
            }
            return summary;
        }

        private int ValidateScanNumber(int scan, XRawFileIO rawReader)
        {
            var totalSpectra = rawReader.GetNumScans();

            if (scan > totalSpectra)
            {
                throw new ScanOutOfRangeException("The requested scan is out of range.");
            }

            return scan;
        }
    }
}