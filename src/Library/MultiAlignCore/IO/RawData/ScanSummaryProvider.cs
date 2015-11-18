using System;
using System.Collections.Generic;
using FluentNHibernate.Utils;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Hibernate;
using MultiAlignCore.IO.TextFiles;

namespace MultiAlignCore.IO.RawData
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using InformedProteomics.Backend.Utils;

    public class ScanSummaryProvider: IScanSummaryProvider
    {
        private DatasetSummary _summary;

        /// <summary>
        /// The total number of scans.
        /// </summary>
        private int _totalScans;

        /// <summary>
        /// The lowest scan number.
        /// </summary>
        private int _minScan;

        /// <summary>
        /// The highest scan number.
        /// </summary>
        private int _maxScan;

        /// <summary>
        /// Scans file for lazy loading.
        /// </summary>
        private string _scansFilePath;

        /// <summary>
        /// ScanSummaryDAO for lazy loading.
        /// </summary>
        private ScanSummaryDAOHibernate _scanSummaryDao;

        public ScanSummaryProvider(int groupId)
        {
            this.GroupId = groupId;
            this._summary = null;
            this._totalScans = -1;
            this._minScan = -1;
            this._maxScan = -1;
            this.IsBackedByFile = true;
        }

        public ScanSummaryProvider(int groupId, string scansFilePath) : this(groupId)
        {
            this._scansFilePath = scansFilePath;
        }

        public ScanSummaryProvider(int groupId, ScanSummaryDAOHibernate scanSummaryDao)
            : this(groupId)
        {
            this._scanSummaryDao = scanSummaryDao;
            this.IsBackedByFile = false;
        }

        /// <summary>
        /// Gets the group ID for the dataset this scan summary provider is for.
        /// </summary>
        public int GroupId { get; private set; }

        public ScanSummary GetPreviousScanSummary(int scan, int msLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the total number of scans.
        /// </summary>
        public int TotalScans
        {
            get
            {
                if (this._totalScans < 0)
                {   // Lazy load scans file.
                    this.LoadScans();
                }

                return this._totalScans;
            }
            private set { this._totalScans = value; }
        }

        /// <summary>
        /// Gets the lowest scan number.
        /// </summary>
        public int MinScan
        {
            get
            {
                if (this._minScan < 0)
                {   // Lazy load scans file.
                    this.LoadScans();
                }

                return this._minScan;
            }
            private set { this._minScan = value; }
        }

        /// <summary>
        /// Gets the highest scan number.
        /// </summary>
        public int MaxScan
        {
            get
            {
                if (this._maxScan < 0)
                {   // Lazy load scans file.
                    this.LoadScans();
                }

                return this._maxScan;
            }
            private set { this._maxScan = value; }
        }

        /// <summary>
        /// Retrieves the scan header from the underlying stream.
        /// </summary>
        public ScanSummary GetScanSummary(int scan)
        {
            if (this._summary == null)
            {
                this.LoadScans();
            }

            if (!_summary.ScanMetaData.ContainsKey(scan))
            {
                throw new ScanOutOfRangeException("The requested scan is out of range.");
            }

            return _summary.ScanMetaData[scan];
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
            if (this._summary == null)
            {
                this.LoadScans();
            }

            if (msLevel == 0)
            {
                return this._summary.ScanMetaData.Values.ToList();
            }

            return this._summary.ScanMetaData.Values.Where(sum => sum.MsLevel == msLevel).ToList();
        }

        public ScanSummary GetNextScanSummary(int scan, int msLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Whether the scan summary provider is populated from a file, or from something else (i.e., the database)
        /// </summary>
        public bool IsBackedByFile { get; private set; }

        /// <summary>
        /// A method that forces the provider to initializes itself if it uses lazy loading.
        /// </summary>
        /// <param name="progress">The progress of the initialization process.</param>
        public void Initialize(IProgress<ProgressData> progress = null)
        {
            this.LoadScans();
        }

        /// <summary>
        /// A method that forces the provider to initializes itself if it uses lazy loading asynchronously.
        /// </summary>
        /// <param name="progress">The progress of the initialization process.</param>
        public Task InitializeAsync(IProgress<ProgressData> progress = null)
        {
            return Task.Run(() => this.Initialize(progress));
        }

        /// <summary>
        /// Load scan summaries from a scans file.
        /// </summary>
        /// <param name="scansFilePath">The file path to the scans file.</param>
        public void LoadScansFile(string scansFilePath)
        {
            this._scansFilePath = scansFilePath;
            this._summary = new DatasetSummary();
            var reader = new ScansFileReader { Delimiter = ',' };
            var scans = reader.ReadFile(scansFilePath);
            var dict = this._summary.ScanMetaData;
            dict.Clear();
            var minScan = int.MaxValue;
            var maxScan = int.MinValue;
            foreach (var scan in scans)
            {
                scan.DatasetId = this.GroupId;
                dict.Add(scan.Scan, scan);
                minScan = Math.Min(minScan, scan.Scan);
                maxScan = Math.Max(maxScan, scan.Scan);
            }

            this.MinScan = minScan;
            this.MaxScan = maxScan;
            this.TotalScans = maxScan - minScan; // Not truly the total number present; Scans file contains only MS1 scans
            this.SetNets();
            this.IsBackedByFile = true;
        }

        /// <summary>
        /// Load scan summaries from database from dataset specified by <see cref="GroupId" />.
        /// </summary>
        public void LoadFromDatabase()
        {
            var dao = new ScanSummaryDAOHibernate();
            this.LoadFromDatabase(dao);
        }

        /// <summary>
        /// Load scan summaries from database from dataset specified by <see cref="GroupId" />,
        /// given an existing data-access-object to use.
        /// </summary>
        /// <param name="dao">The existing data-access-object to use to access the database.</param>
        public void LoadFromDatabase(ScanSummaryDAOHibernate dao)
        {
            this._scanSummaryDao = dao;
            var data = dao.FindByDatasetId(this.GroupId);

            foreach (var summary in data)
            {
                this._summary.ScanMetaData.Add(summary.Scan, summary);

                this.MinScan = Math.Min(this.MinScan, summary.Scan);
                this.MaxScan = Math.Max(this.MaxScan, summary.Scan);
            }

            this.TotalScans = this.MaxScan - this.MinScan;
            this.IsBackedByFile = false;
        }

        /// <summary>
        /// Load scans from scans file, if available, or database.
        /// </summary>
        private void LoadScans()
        {
            if (!string.IsNullOrEmpty(this._scansFilePath) && File.Exists(this._scansFilePath))
            {
                this.LoadScansFile(this._scansFilePath);
            }
            else if (this._scanSummaryDao != null)
            {
                this.LoadFromDatabase(this._scanSummaryDao);
            }
            else
            {
                this.LoadFromDatabase();
            }
        }

        /// <summary>
        /// Calculate the nets for every scan.
        /// </summary>
        private void SetNets()
        {
            var minScanSum = this.GetScanSummary(this.MinScan);
            var maxScanSum = this.GetScanSummary(this.MaxScan);
            var timeDiff = maxScanSum.Time - minScanSum.Time;

            foreach (var summary in this._summary.ScanMetaData.Values)
            {
                summary.Net = (summary.Time - minScanSum.Time) / timeDiff;
            }
        }

        /// <summary>
        /// Dispose of scan summary provider.
        /// </summary>
        public void Dispose()
        {
            _summary.ScanMetaData.Clear();
        }
    }
}
