#region

using System;
using System.Collections.Generic;
using System.IO;
using MSDataFileReader;
using PNNLOmics.Data;

#endregion

namespace MultiAlignCore.IO.RawData
{
    /// <summary>
    ///     Adapter for reading mzXML files from a reader by Matt Monroe.
    /// </summary>
    public class MzXMLReader : ISpectraProvider
    {
        /// <summary>
        ///     Readers for each dataset.
        /// </summary>
        private readonly Dictionary<int, clsMzXMLFileAccessor> m_readers;

        private readonly Dictionary<int, string> m_dataFiles;

        public MzXMLReader()
        {
            m_dataFiles = new Dictionary<int, string>();
            m_readers = new Dictionary<int, clsMzXMLFileAccessor>();
            BinSize = .5;
        }

        /// <summary>
        ///     Gets or sets the bin size for mass spectra
        /// </summary>
        public double BinSize { get; set; }

        public int GetMinScan(int groupId)
        {
            throw new NotImplementedException();
        }

        public int GetMaxScan(int groupId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Reads a list of MSMS Spectra header data from the mzXML file.
        /// </summary>
        /// <param name="file">file to read.</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> ReadMSMSSpectra(string file)
        {
            var spectra = new List<MSSpectra>();
            var reader = new clsMzXMLFileReader {SkipBinaryData = true};
            var opened = reader.OpenFile(file);

            if (!opened)
            {
                throw new IOException("Could not open the mzXML file " + file);
            }

            var totalScans = reader.ScanCount;

            for (var i = 0; i < totalScans; i++)
            {
                var info = new clsSpectrumInfo();
                reader.GetSpectrumByScanNumber(i, ref info);
                if (info.MSLevel > 1)
                {
                    var spectrum = new MSSpectra
                    {
                        MsLevel         = info.MSLevel,
                        RetentionTime   = info.RetentionTimeMin,
                        Scan            = i,
                        PrecursorMz     = info.ParentIonMZ,
                        TotalIonCurrent = info.TotalIonCurrent,
                        CollisionType   = CollisionType.Other
                    };

                    // Need to make this a standard type of collision based off of the data.
                    spectra.Add(spectrum);
                }
            }
            reader.CloseFile();
            return spectra;
        }

        public List<MSSpectra> GetRawSpectra(int group)
        {
            return new List<MSSpectra>();
        }

        public List<XYData> GetRawSpectra(int scan, int group, out ScanSummary summary)
        {
            return GetRawSpectra(scan, group, -11, out summary);
        }


        public void AddDataFile(string path, int groupId)
        {
            if (m_dataFiles.ContainsKey(groupId) == false)
            {
                m_dataFiles.Add(groupId, path);
            }
            else
            {
                m_dataFiles[groupId] = path;
            }
        }

        public List<MSSpectra> GetMSMSSpectra(int group)
        {
            return GetMSMSSpectra(group, new Dictionary<int, int>());
        }

        public List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap)
        {
            return GetMSMSSpectra(group, excludeMap, false);
        }

        public List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap, bool loadPeaks)
        {
            var spectra = new List<MSSpectra>();

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
                var reader = new clsMzXMLFileAccessor();
                m_readers.Add(group, reader);

                var opened = reader.OpenFile(path);
                if (!opened)
                {
                    throw new IOException("Could not open the mzXML file " + path);
                }
            }
            var rawReader = m_readers[group];

            var numberOfScans = rawReader.ScanCount;
            var info = new clsSpectrumInfo();

            for (var i = 0; i < numberOfScans; i++)
            {
                // This scan is not to be used.
                var isInExcludeMap = excludeMap.ContainsKey(i);
                if (isInExcludeMap)
                    continue;

                var header = new clsSpectrumInfo();

                rawReader.GetSpectrumHeaderInfoByIndex(i, ref header);
                if (header.MSLevel > 1)
                {
                    var spectrum = new MSSpectra();
                    spectrum.MsLevel = header.MSLevel;
                    spectrum.RetentionTime = header.RetentionTimeMin;
                    spectrum.Scan = i;
                    spectrum.PrecursorMz = header.ParentIonMZ;
                    spectrum.TotalIonCurrent = header.TotalIonCurrent;
                    spectrum.CollisionType = CollisionType.Other;
                    spectra.Add(spectrum);
                }
            }

            return spectra;
        }

        /// <summary>
        ///     Gets the total number of scans for the group id provided
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public int GetTotalScans(int group)
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
                var reader = new clsMzXMLFileAccessor();
                m_readers.Add(group, reader);

                var opened = reader.OpenFile(path);
                if (!opened)
                {
                    throw new IOException("Could not open the mzXML file " + path);
                }
            }
            var rawReader = m_readers[group];
            return rawReader.ScanCount;
        }

        public Dictionary<int, ScanSummary> GetScanData(int groupId)
        {
            return new Dictionary<int, ScanSummary>();
        }

        public void Dispose()
        {
        }

        public List<XYData> GetRawSpectra(int scan, int groupId, int scanLevel, out ScanSummary summary)
        {
            List<XYData> spectrum = null;

            var info = GetScanInfo(scan, groupId, out summary);
            if (info == null)
                return null;

            if (info.MSLevel == scanLevel || scanLevel < 1)
            {
                spectrum = new List<XYData>();
                for (var j = 0; j < info.MZList.Length; j++)
                {
                    spectrum.Add(new XYData(info.MZList[j], info.IntensityList[j]));
                }
            }

            return spectrum;
        }

        private clsSpectrumInfo GetScanInfo(int scan, int groupId, out ScanSummary summary)
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
                var reader = new clsMzXMLFileAccessor();
                m_readers.Add(groupId, reader);

                var opened = reader.OpenFile(path);
                if (!opened)
                {
                    throw new IOException("Could not open the mzXML file " + path);
                }
            }
            var rawReader = m_readers[groupId];

            var totalScans = rawReader.ScanCount;
            var info = new clsSpectrumInfo();

            rawReader.GetSpectrumByScanNumber(scan, ref info);

            summary = new ScanSummary
            {
                Bpi = Convert.ToInt64(info.BasePeakIntensity),
                BpiMz = info.BasePeakMZ,
                MsLevel = info.MSLevel,
                PrecursorMz = info.ParentIonMZ,
                TotalIonCurrent = Convert.ToInt64(info.TotalIonCurrent)
            };

            return info;
        }

        /// <summary>
        /// Gets the scan header information for the specified scan
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="groupId"></param>
        public ScanSummary GetScanSummary(int scan, int groupId)
        {
            ScanSummary summary;

            var info = GetScanInfo(scan, groupId, out summary);

            return summary;
        }

        public MSSpectra GetSpectrum(int scan, int group, int scanLevel, out ScanSummary summary, bool loadPeaks)
        {
            throw new NotImplementedException();
        }
    }
}