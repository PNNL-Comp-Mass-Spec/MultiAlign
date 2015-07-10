using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PNNLOmics.Data;
using InformedProteomics.Backend;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.MassSpecData;

namespace MultiAlignCore.IO.RawData
{
    public class InformedProteomicsReader : ISpectraProvider, IRawDataFileReader
    {
        /// <summary>
        ///     Readers for each dataset.
        /// </summary>
        private readonly Dictionary<int, LcMsRun> m_readers = new Dictionary<int, LcMsRun>();

        private readonly Dictionary<int, string> m_dataFiles = new Dictionary<int, string>();
        private readonly Dictionary<int, bool> m_opened = new Dictionary<int, bool>();

        /// <summary>
        ///     Gets or sets the map
        /// </summary>
        private readonly Dictionary<int, DatasetSummary> m_datasetMetaData = new Dictionary<int, DatasetSummary>();

        public InformedProteomicsReader()
        {
            
        }

        #region ISpectraProvider and IRawDataFileReader

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
                        //m_readers[groupId].Close();
                        m_readers.Remove(groupId);
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

        #endregion
        #region ISpectraProvider

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

            var numberOfScans = rawReader.NumSpectra;
            for (var i = 0; i < numberOfScans; i++)
            {
                var isInExcludeMap = excludeMap.ContainsKey(i);
                if (isInExcludeMap)
                {
                    // This scan is not to be used.
                    continue;
                }

                var summary = GetScanSummary(i, rawReader);

                if (summary.MsLevel > 1)
                {
                    var spectrum = new MSSpectra
                    {
                        MsLevel = summary.MsLevel,
                        RetentionTime = summary.Time,
                        Scan = i,
                        PrecursorMz = summary.PrecursorMz,
                        TotalIonCurrent = summary.TotalIonCurrent,
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

        public List<XYData> GetRawSpectra(int scan, int groupId, int scanLevel, out ScanSummary summary)
        {
            var rawReader = GetScanSummaryAndReader(scan, groupId, out summary);
            if (rawReader == null)
                return null;

            if (summary.MsLevel != scanLevel && scanLevel > 0)
                return null;

            var data = LoadRawSpectra(rawReader, scan);

            return data;
        }

        /// <summary>
        /// Gets the raw data from the data file.
        /// </summary>
        public List<MSSpectra> GetRawSpectra(int groupId)
        {
            return GetMSMSSpectra(groupId, new Dictionary<int, int>(), true);
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
            var numberOfScans = rawReader.NumSpectra;
            for (var i = 0; i < numberOfScans; i++)
            {
                var summary = GetScanSummary(i, rawReader);

                scanMap.Add(i, summary);
            }

            datasetSummary.ScanMetaData = scanMap;

            m_datasetMetaData.Add(groupId, datasetSummary);
            return datasetSummary.ScanMetaData;
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

        private LcMsRun GetScanSummaryAndReader(int scan, int groupId, out ScanSummary summary)
        {
            // Get the RawFileReader for this group
            var rawReader = GetReaderForGroup(groupId);

            ValidateScanNumber(scan, rawReader);

            summary = GetScanSummary(scan, rawReader);

            return rawReader;
        }

        public MSSpectra GetSpectrum(int scan, int groupId, int scanLevel, out ScanSummary summary, bool loadPeaks)
        {
            // Get the RawFileReader for this group
            var rawReader = GetReaderForGroup(groupId);

            ValidateScanNumber(scan, rawReader);

            summary = GetScanSummary(scan, rawReader);

            var spectrum = new MSSpectra
            {
                MsLevel = summary.MsLevel,
                RetentionTime = summary.Time,
                Scan = scan,
                PrecursorMz = summary.PrecursorMz,
                TotalIonCurrent = summary.TotalIonCurrent,
                CollisionType = summary.CollisionType
            };

            // Need to make this a standard type of collision based off of the data.
            if (loadPeaks)
            {
                spectrum.Peaks = LoadRawSpectra(rawReader, scan);
            }

            return spectrum;
        }

        private List<XYData> LoadRawSpectra(LcMsRun rawReader, int scan)
        {
            var spec = rawReader.GetSpectrum(scan);

            if (spec.Peaks == null)
                return new List<XYData>();

            //var data = new List<XYData>(spec.Peaks.Length);
            //for (var i = 0; i < spec.Peaks.Length; i++)
            //{
            //    data.Add(new XYData(spec.Peaks[i].Mz, spec.Peaks[i].Intensity));
            //}
            //return data;
            return spec.Peaks.Select(peak => new XYData(peak.Mz, peak.Intensity)).ToList();
        }

        public int GetTotalScans(int groupId)
        {
            // Get the ILcMsRun for this group
            var reader = GetReaderForGroup(groupId);

            return reader.NumSpectra;
        }

        public void Dispose()
        {
            //foreach (var key in m_readers.Keys)
            //{
            //    try
            //    {
            //        m_readers[key].Close();
            //    }
            //    catch
            //    {
            //        // We don't want it to fail.  Just go with it.
            //    }
            //}
            m_readers.Clear();
        }

        #endregion
        #region IRawDataFileReader

        public List<MSSpectra> ReadMSMSSpectra(string file)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region New public functions
        public LcMsRun GetReaderForGroup(int groupId)
        {
            if (!m_dataFiles.ContainsKey(groupId))
            {
                throw new Exception("The group-dataset ID provided was not found.");
            }

            // If we don't have a reader, then create one for this group 
            // next time, it will be available and we won't have to waste time
            // opening the file.

            if (!m_readers.ContainsKey(groupId))
            {
                var path = m_dataFiles[groupId];
                var reader = PbfLcMsRun.GetLcMsRun(path);

                m_readers.Add(groupId, reader);
            }

            var rawReader = m_readers[groupId];

            return rawReader;
        }
        #endregion

        #region Private functions

        private ScanSummary GetScanSummary(int scan, LcMsRun rawReader)
        {
            var spec = rawReader.GetSpectrum(scan);

            var summary = new ScanSummary
            {
                MsLevel = spec.MsLevel,
                Time = spec.ElutionTime,
                Scan = scan,
                //TotalIonCurrent = Convert.ToInt64(header.TotalIonCurrent), // Only used in PNNLOmics.Algorithms.Chromatograms.XicCreator.CreateXic(...)
                PrecursorMz = 0,
                CollisionType = CollisionType.Other,
            };

            if (spec is ProductSpectrum)
            {
                var pspec = spec as ProductSpectrum;
                if (pspec.IsolationWindow.MonoisotopicMass != null)
                {
                    summary.PrecursorMz = pspec.IsolationWindow.MonoisotopicMass.Value;
                }

                switch (pspec.ActivationMethod)
                {
                    case ActivationMethod.CID:
                        summary.CollisionType = CollisionType.Cid;
                        break;
                    case ActivationMethod.HCD:
                        summary.CollisionType = CollisionType.Hcd;
                        break;
                    case ActivationMethod.ETD:
                        summary.CollisionType = CollisionType.Etd;
                        break;
                    case ActivationMethod.ECD:
                        summary.CollisionType = CollisionType.Ecd;
                        break;
                    case ActivationMethod.PQD:
                        //summary.CollisionType = CollisionType.Hid; // HID? what is HID?
                        break;
                }
            }
            return summary;
        }

        private int ValidateScanNumber(int scan, LcMsRun rawReader)
        {
            var totalSpectra = rawReader.NumSpectra;

            if (scan > totalSpectra)
            {
                throw new ScanOutOfRangeException("The requested scan is out of range.");
            }

            return scan;
        }
        #endregion
    }
}
