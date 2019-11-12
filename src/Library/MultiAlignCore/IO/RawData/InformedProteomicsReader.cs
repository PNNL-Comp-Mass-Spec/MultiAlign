using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.MassSpecData;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.RawData
{
    using System.IO;
    using System.Threading.Tasks;

    using InformedProteomics.Backend.Utils;

    public class InformedProteomicsReader : ISpectraProvider
    {
        /// <summary>
        /// Readers for each dataset.
        /// </summary>
        private LcMsRun _lcmsRun;

        private readonly string _dataFilePath;

        /// <summary>
        /// Internal map of scan summaries, to avoid reading the scan summary data from the file each time.
        /// </summary>
        private readonly DatasetSummary _summary;

        public InformedProteomicsReader(int groupId, string dataFilePath)
        {
            this.GroupId = groupId;
            this._dataFilePath = dataFilePath;
            this._summary = new DatasetSummary();

            if (string.IsNullOrEmpty(dataFilePath) || !File.Exists(dataFilePath)
                || !MassSpecDataReaderFactory.MassSpecDataTypeFilterList.Any(
                    ext => dataFilePath.ToLower().EndsWith(ext)))
            {
                throw new ArgumentException(string.Format("Invalid RAW spectrum file: {0}", dataFilePath));
            }
        }

        #region IScanSummaryProvider, ISpectraProvider

        #endregion
        #region IScanSummaryProvider

        public LcMsRun LcMsRun
        {
            get
            {
                if (this._lcmsRun == null)
                {
                    this.LoadLcmsRun();
                }

                return this._lcmsRun;
            }
        }

        /// <summary>
        /// Gets the group ID for the dataset this scan summary provider is for.
        /// </summary>
        public int GroupId { get; private set; }

        /// <summary>
        /// Gets the total number of scans.
        /// </summary>
        public int TotalScans
        {
            get { return (this.LcMsRun.MaxLcScan - this.LcMsRun.MinLcScan); }
        }

        /// <summary>
        /// Gets the lowest scan number.
        /// </summary>
        public int MinScan
        {
            get { return this.LcMsRun.MinLcScan; }
        }

        /// <summary>
        /// Gets the highest scan number.
        /// </summary>
        public int MaxScan
        {
            get { return this.LcMsRun.MaxLcScan; }
        }

        /// <summary>
        /// Retrieves the scan header from the underlying stream.
        /// </summary>
        public ScanSummary GetScanSummary(int scan)
        {
            ScanSummary scanSummary;

            if (this._summary.ScanMetaData.ContainsKey(scan))
            {
                scanSummary = this._summary.ScanMetaData[scan];
            }
            else
            {
                scanSummary = this.LoadScanSummary(scan);
            }

            return scanSummary;
        }

        /// <summary>
        /// Get scan summaries for a particular ms level.
        /// </summary>
        /// <param name="msLevel">
        /// The mslevel to get scan summaries for.
        /// 0 = MS1 and MS2
        /// </param>
        /// <returns>All scan summaries.</returns>
        public List<ScanSummary> GetScanSummaries(int msLevel = 0)
        {
            List<int> scanNumbers;
            if (msLevel == 0)
            {
                scanNumbers = this.LcMsRun.GetScanNumbers(1).ToList();
                scanNumbers.AddRange(this.LcMsRun.GetScanNumbers(2));
                scanNumbers.Sort();
            }
            else
            {
                scanNumbers = this.LcMsRun.GetScanNumbers(msLevel).ToList();
            }

            return scanNumbers.Select(this.GetScanSummary).ToList();
        }

        public ScanSummary GetPreviousScanSummary(int scan, int msLevel)
        {
            if (this.LcMsRun == null)
            {
                this.LoadLcmsRun();
            }

            var prevScan = Math.Max(this.LcMsRun.GetPrevScanNum(scan, msLevel), this.LcMsRun.MinLcScan);
            return this.GetScanSummary(prevScan);
        }

        public ScanSummary GetNextScanSummary(int scan, int msLevel)
        {
            if (this.LcMsRun == null)
            {
                this.LoadLcmsRun();
            }

            var nextScan = Math.Min(this.LcMsRun.GetNextScanNum(scan, msLevel), this.LcMsRun.MaxLcScan);
            return this.GetScanSummary(nextScan);
        }

        /// <summary>
        /// Whether the scan summary provider is populated from a file, or from something else (i.e., the database)
        /// Always true for InformedProteomicsReader
        /// </summary>
        public bool IsBackedByFile
        {
            get { return true; }
        }

        /// <summary>
        /// A method that forces the provider to initializes itself if it uses lazy loading.
        /// </summary>
        /// <param name="progress">The progress of the initialization process.</param>
        public void Initialize(IProgress<PRISM.ProgressData> progress = null)
        {
            this.LoadLcmsRun(progress);
        }

        /// <summary>
        /// A method that forces the provider to initializes itself if it uses lazy loading asynchronously.
        /// </summary>
        /// <param name="progress">The progress of the initialization process.</param>
        public Task InitializeAsync(IProgress<PRISM.ProgressData> progress = null)
        {
            return Task.Run(() => this.LoadLcmsRun(progress));
        }

        #endregion
        #region ISpectraProvider

        public List<MSSpectra> GetMSMSSpectra(int prevMsScan, double mz, bool loadPeaks)
        {
            var nextMsScan = this.GetNextScanSummary(prevMsScan, 1).Scan;
            IEnumerable<int> fragmentationScans = this.LcMsRun.GetFragmentationSpectraScanNums(mz);
            fragmentationScans = fragmentationScans.Where(scan => scan > prevMsScan && scan < nextMsScan);

            var spectra = new List<MSSpectra>();
            foreach (var scan in fragmentationScans)
            {
                ScanSummary scanSummary;
                spectra.Add(this.GetSpectrum(scan, 2, out scanSummary, loadPeaks));
            }

            return spectra;
        }

        /// <summary>
        /// Reads a list of MSMS Spectra header data from the Raw file
        /// </summary>
        /// <param name="groupId">File group ID</param>
        /// <param name="excludeMap">Dictionary indicating which scans and related feature ID's to ignore.</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> GetMSMSSpectra(Dictionary<int, int> excludeMap)
        {
            return GetMSMSSpectra(excludeMap, false);
        }

        /// <summary>
        /// Reads a list of MSMS Spectra header data from the Raw file
        /// </summary>
        /// <param name="groupId">File group ID</param>
        /// <param name="excludeMap">Dictionary indicating which scans and related feature ID's to ignore.</param>
        /// <param name="loadPeaks">True to also load the mass/intensity pairs for each spectrum</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> GetMSMSSpectra(Dictionary<int, int> excludeMap, bool loadPeaks)
        {
            var spectra = new List<MSSpectra>();

            var numberOfScans = this._lcmsRun.NumSpectra;
            for (var i = 1; i <= numberOfScans; i++)
            {
                var isInExcludeMap = excludeMap.ContainsKey(i);
                if (isInExcludeMap)
                {
                    // This scan is not to be used.
                    continue;
                }

                var summary = this.GetScanSummary(i);

                if (summary.MsLevel > 1)
                {
                    var spectrum = new MSSpectra
                    {
                        MsLevel = summary.MsLevel,
                        RetentionTime = summary.Time,
                        Scan = summary.Scan,
                        PrecursorMz = summary.PrecursorMz,
                        TotalIonCurrent = summary.TotalIonCurrent,
                        CollisionType = summary.CollisionType
                    };

                    // Need to make this a standard type of collision based off of the data.
                    if (loadPeaks)
                    {
                        spectrum.Peaks = this.LoadSpectra(i);
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
        public List<MSSpectra> GetMSMSSpectra()
        {
            return GetMSMSSpectra(new Dictionary<int, int>(), false);
        }

        /// <summary>
        /// Gets the raw data from the data file.
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="groupId">File Group ID</param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public List<XYData> GetRawSpectra(int scan, out ScanSummary summary)
        {
            return GetRawSpectra(scan, -1, out summary);
        }

        public List<XYData> GetRawSpectra(int scan, int scanLevel, out ScanSummary summary)
        {
            if (this._lcmsRun == null)
            {
                this.LoadLcmsRun();
            }

            summary = this.GetScanSummary(scan);

            if (summary.MsLevel != scanLevel && scanLevel > 0)
            {
                return null;
            }

            var data = this.LoadSpectra(scan);

            return data;
        }

        /// <summary>
        /// Gets the raw data from the data file.
        /// </summary>
        public List<MSSpectra> GetRawSpectra()
        {
            return this.GetMSMSSpectra(new Dictionary<int, int>(), true);
        }

        public MSSpectra GetSpectrum(int scan, int scanLevel, out ScanSummary summary, bool loadPeaks)
        {
            summary = this.GetScanSummary(scan);

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
                spectrum.Peaks = this.LoadSpectra(scan);
            }

            return spectrum;
        }

        private List<XYData> LoadSpectra(int scan)
        {
            if (this._lcmsRun == null)
            {
                this.LoadLcmsRun();
            }

            var spec = this._lcmsRun.GetSpectrum(scan);
            return spec.Peaks.Select(peak => new XYData(peak.Mz, peak.Intensity)).ToList();
        }

        public void Dispose()
        {
            this._lcmsRun = null;
        }

        #endregion

        private ScanSummary LoadScanSummary(int scan)
        {
            // Peaks needed to calculate Total ion current
            var spec = this.LcMsRun.GetSpectrum(scan, true);

            var minEt = this.LcMsRun.GetElutionTime(this.LcMsRun.MinLcScan);
            var maxEt = this.LcMsRun.GetElutionTime(this.LcMsRun.MaxLcScan);
            var timeDiff = maxEt - minEt;

            var summary = new ScanSummary
            {
                MsLevel = spec.MsLevel,
                Net = (spec.ElutionTime - minEt) / timeDiff,
                Time = spec.ElutionTime,
                Scan = spec.ScanNum,
                TotalIonCurrent = Convert.ToInt64(spec.TotalIonCurrent), // Only used in MultiAlignCore.Algorithms.Chromatograms.XicCreator.CreateXic(...)
                PrecursorMz = 0,
                CollisionType = CollisionType.Other,
                DatasetId = this.GroupId,
            };

            if (spec is ProductSpectrum productSpectrum)
            {
                if (productSpectrum.IsolationWindow.MonoisotopicMass != null)
                {
                    summary.PrecursorMz = productSpectrum.IsolationWindow.MonoisotopicMass.Value;
                }

                switch (productSpectrum.ActivationMethod)
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
            this._summary.ScanMetaData.Add(scan, summary);
            return summary;
        }

        private void LoadLcmsRun(IProgress<PRISM.ProgressData> progress = null)
        {
            this._lcmsRun = PbfLcMsRun.GetLcMsRun(this._dataFilePath, progress);
        }
    }
}
