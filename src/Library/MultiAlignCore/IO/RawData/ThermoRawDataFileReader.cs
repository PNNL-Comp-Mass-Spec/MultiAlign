#region

using System;
using System.Collections.Generic;
using System.IO;
using MultiAlignCore.IO.RawData;
using PNNLOmics.Data;
using ThermoRawFileReaderDLL.FinniganFileIO;

#endregion

namespace MultiAlignCore.IO.Features
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
        /// This is a cache so we dont have to allocate a ton of memory each time we are reading large blocks of data over and over
        /// </summary>
        private double[] m_mzArray;
        /// <summary>
        /// This is a cache so we dont have to allocate a ton of memory each time we are reading large blocks of data over and over
        /// </summary>
        private double[] m_intensityArray;

        /// <summary>
        ///     Gets or sets the map
        /// </summary>
        private readonly Dictionary<int, DatasetSummary> m_datasetMetaData;

        public ThermoRawDataFileReader()
        {
            m_datasetMetaData   = new Dictionary<int, DatasetSummary>();
            m_dataFiles         = new Dictionary<int, string>();
            m_readers           = new Dictionary<int, XRawFileIO>();
            m_opened            = new Dictionary<int, bool>();
        }

        public int GetTotalScans(int group)
        {
            if (!m_readers.ContainsKey(group))
            {
                var reader = new XRawFileIO();
                m_readers.Add(group, reader);
            }

            var rawReader = m_readers[group];
            if (!m_opened[group])
            {
                var opened = rawReader.OpenRawFile(m_dataFiles[group]);
                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }
            return rawReader.GetNumScans();
        }

        /// <summary>
        ///     Adds a dataset file to the reader map so when in use the application
        ///     knows where to get raw data from.
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

        public
            List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap)
        {
            return GetMSMSSpectra(group, excludeMap, false);
        }

        /// <summary>
        ///     Reads a list of MSMS Spectra header data from the mzXML file.
        /// </summary>
        /// <param name="file">file to read.</param>
        /// <param name="excludeMap">Dictionary indicating which scans and related feature ID's to ignore.</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap, bool loadPeaks)
        {
            var spectra = new List<MSSpectra>();

            if (!m_readers.ContainsKey(group))
            {
                var path = m_dataFiles[group];
                var reader = new XRawFileIO();
                m_readers.Add(group, reader);
            }

            var rawReader = m_readers[group];
            if (!m_opened[group])
            {
                var opened = rawReader.OpenRawFile(m_dataFiles[group]);
                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }


            var numberOfScans = rawReader.GetNumScans();
            for (var i = 0; i < numberOfScans; i++)
            {
                // This scan is not to be used.
                var isInExcludeMap = excludeMap.ContainsKey(i);
                if (isInExcludeMap)
                    continue;

                var header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
                rawReader.GetScanInfo(i, out header);

                if (header.MSLevel > 1)
                {
                    var spectrum = new MSSpectra();
                    spectrum.MsLevel = header.MSLevel;
                    spectrum.RetentionTime = header.RetentionTime;
                    spectrum.Scan = i;
                    spectrum.PrecursorMz = header.ParentIonMZ;
                    spectrum.TotalIonCurrent = header.TotalIonCurrent;
                    spectrum.CollisionType = CollisionType.Other;


                    switch (header.CollisionMode)
                    {
                        case "cid":
                            spectrum.CollisionType = CollisionType.Cid;
                            break;
                        case "hcd":
                            spectrum.CollisionType = CollisionType.Hcd;
                            break;
                        case "etd":
                            spectrum.CollisionType = CollisionType.Etd;
                            break;
                        case "ecd":
                            spectrum.CollisionType = CollisionType.Ecd;
                            break;
                        case "hid":
                            spectrum.CollisionType = CollisionType.Hid;
                            break;
                    }

                    // Need to make this a standard type of collision based off of the data.
                    if (loadPeaks)
                    {
                        spectrum.Peaks = LoadRawSpectra(rawReader, i);
                    }
                    spectra.Add(spectrum);
                }
            }
            rawReader.CloseRawFile();

            return spectra;
        }

        /// <summary>
        ///     Reads a list of MSMS Spectra header data from the mzXML file.
        /// </summary>
        /// <param name="file">file to read.</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> GetMSMSSpectra(int group)
        {
            return GetMSMSSpectra(group, new Dictionary<int, int>(), false);
        }

        #endregion

        #region IMSMSDataSource Members

        /// <summary>
        ///     Gets the raw data from the data file.
        /// </summary>
        /// <param name="scan"></param>
        /// <returns></returns>
        public List<XYData> GetRawSpectra(int scan, int group, out ScanSummary summary)
        {
            return GetRawSpectra(scan, group, -1, out summary);
        }

        /// <summary>
        ///     Gets the raw data from the data file.
        /// </summary>
        public List<MSSpectra> GetRawSpectra(int group)
        {
            return GetMSMSSpectra(group, new Dictionary<int, int>(), true);
        }

        private List<XYData> LoadRawSpectra(XRawFileIO rawReader, int scan)
        {
            var header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
            rawReader.GetScanInfo(scan, out header);
            var N = header.NumPeaks;

            var mz = new double[N];
            var intensities = new double[N];
            rawReader.GetScanData(scan, ref mz, ref intensities, ref header);

            //rawReader.CloseRawFile();
            // construct the array.
            var data = new List<XYData>(mz.Length);
            for (var i = 0; i < mz.Length; i++)
            {
                var intensity = intensities[i];
                data.Add(new XYData(mz[i], intensity));
            }
            return data;
        }

        /// <summary>
        ///     Gets the raw data from the data file.
        /// </summary>
        public List<XYData> GetRawSpectra(int scan, int group, int msLevel)
        {
            var summary = new ScanSummary();
            return GetRawSpectra(scan, group, msLevel, out summary);
        }

        public List<XYData> GetRawSpectra(int scan, int group, int msLevel, out ScanSummary summary)
        {
            if (!m_dataFiles.ContainsKey(group))
            {
                throw new Exception("The group-dataset ID provided was not found.");
            }
            // If we dont have a reader, then create one for this group 
            // next time, it will be available and we won't have to waste time
            // opening the file.
            if (!m_readers.ContainsKey(group))
            {
                var path = m_dataFiles[group];
                var reader = new XRawFileIO();

                m_readers.Add(group, reader);
                var opened = reader.OpenRawFile(path);

                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }

            var spectra = new List<MSSpectra>();
            var rawReader = m_readers[group];


            var totalSpectra = rawReader.GetNumScans();

            if (scan > totalSpectra)
                throw new ScanOutOfRangeException("The requested scan is out of range.");

            var header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
            rawReader.GetScanInfo(scan, out header);

            summary = new ScanSummary
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

            if (header.MSLevel != msLevel && msLevel != -1)
                return null;

            var n = header.NumPeaks;

            if (m_mzArray == null || m_mzArray.Length < n)
                m_mzArray = new double[n];
            if (m_intensityArray == null || m_intensityArray.Length < n)
                m_intensityArray = new double[n];

            rawReader.GetScanData(scan, ref m_mzArray, ref m_intensityArray, ref header);


            // construct tAhe array.
            var data = new List<XYData>(m_mzArray.Length);
            for (var i = 0; i < m_mzArray.Length; i++)
            {
                var intensity = m_intensityArray[i];
                if (intensity  <=0 ) continue;
                data.Add(new XYData(m_mzArray[i], intensity));
            }
            return data;
        }

        /// <summary>
        ///     Determines if the scan is a precursor scan or not.
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool IsPrecursorScan(int scan, int group)
        {
            // If we dont have a reader, then create one for this group 
            // next time, it will be available and we won't have to waste time
            // opening the file.
            if (!m_readers.ContainsKey(group))
            {
                var path = m_dataFiles[group];
                var reader = new XRawFileIO();
                m_readers.Add(group, reader);

                var opened = reader.OpenRawFile(path);
                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }

            var rawReader = m_readers[group];
            var header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
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

        /// <summary>
        ///     Retrieves a dictionary of scan data
        /// </summary>
        /// <param name="groupId">Group ID to </param>
        /// <returns>Map between </returns>
        public Dictionary<int, ScanSummary> GetScanData(int group)
        {
            if (m_datasetMetaData.ContainsKey(group))
            {
                return m_datasetMetaData[group].ScanMetaData;
            }


            if (!m_readers.ContainsKey(group))
            {
                var path = m_dataFiles[group];
                var reader = new XRawFileIO();
                m_readers.Add(group, reader);
            }

            var rawReader = m_readers[group];
            if (!m_opened[group])
            {
                var opened = rawReader.OpenRawFile(m_dataFiles[group]);
                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }

            var datasetSummary = new DatasetSummary();
            var scanMap = new Dictionary<int, ScanSummary>();
            var numberOfScans = rawReader.GetNumScans();
            for (var i = 0; i < numberOfScans; i++)
            {
                var header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
                rawReader.GetScanInfo(i, out header);

                var summary = new ScanSummary();
                summary.MsLevel = header.MSLevel;
                summary.Time = header.RetentionTime;
                summary.Scan = i;
                summary.TotalIonCurrent = Convert.ToInt64(header.TotalIonCurrent);
                summary.PrecursorMz = header.ParentIonMZ;
                summary.CollisionType = CollisionType.Other;


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
                scanMap.Add(i, summary);
            }

            datasetSummary.ScanMetaData = scanMap;

            //rawReader.CloseRawFile();                        
            m_datasetMetaData.Add(group, datasetSummary);
            return datasetSummary.ScanMetaData;
        }

        #endregion

        public MSSpectra GetSpectrum(int scan, int group, int scanLevel, out ScanSummary summary, bool loadPeaks)
        {
            if (!m_dataFiles.ContainsKey(group))
            {
                throw new Exception("The group-dataset ID provided was not found.");
            }
            // If we dont have a reader, then create one for this group 
            // next time, it will be available and we won't have to waste time
            // opening the file.
            if (!m_readers.ContainsKey(group))
            {
                var path = m_dataFiles[group];
                var reader = new XRawFileIO();

                m_readers.Add(group, reader);
                var opened = reader.OpenRawFile(path);

                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }

            var spectra = new List<MSSpectra>();
            var rawReader = m_readers[group];


            var totalSpectra = rawReader.GetNumScans();

            if (scan > totalSpectra)
                throw new ScanOutOfRangeException("The requested scan is out of range.");

            var header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
            rawReader.GetScanInfo(scan, out header);

            summary = new ScanSummary
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


            var n = header.NumPeaks;
            var mz = new double[n];
            var intensities = new double[n];
            rawReader.GetScanData(scan, ref mz, ref intensities, ref header);


            // construct the array.
            var data = new List<XYData>(mz.Length);
            for (var i = 0; i < mz.Length; i++)
            {
                var intensity = intensities[i];
                data.Add(new XYData(mz[i], intensity));
            }


            var spectrum = new MSSpectra
            {
                MsLevel = header.MSLevel,
                RetentionTime = header.RetentionTime,
                Scan = scan,
                PrecursorMz = header.ParentIonMZ,
                TotalIonCurrent = header.TotalIonCurrent,
                CollisionType = CollisionType.Other
            };


            switch (header.CollisionMode)
            {
                case "cid":
                    spectrum.CollisionType = CollisionType.Cid;
                    break;
                case "hcd":
                    spectrum.CollisionType = CollisionType.Hcd;
                    break;
                case "etd":
                    spectrum.CollisionType = CollisionType.Etd;
                    break;
                case "ecd":
                    spectrum.CollisionType = CollisionType.Ecd;
                    break;
                case "hid":
                    spectrum.CollisionType = CollisionType.Hid;
                    break;
            }

            // Need to make this a standard type of collision based off of the data.
            if (loadPeaks)
            {
                spectrum.Peaks = LoadRawSpectra(rawReader, scan);
            }

            return spectrum;
        }
    }
}