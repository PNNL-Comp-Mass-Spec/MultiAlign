using FeatureAlignment.Data;

namespace MultiAlignCore.IO.RawData
{
    using System;
    using System.Collections.Generic;
    using MultiAlignCore.Data;
    using MultiAlignCore.IO.Hibernate;
    using MultiAlignCore.IO.TextFiles;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using InformedProteomics.Backend.Utils;

    /// <summary>
    /// This class stores the loaded Decon2ls scans file.
    /// </summary>
    public class ScanSummaryProvider: IScanSummaryProvider
    {
        /// <summary>
        /// The summary for the dataset, which contains the scan -> scan summary mapping.
        /// </summary>
        private DatasetSummary summary;

        /// <summary>
        /// The total number of scans.
        /// </summary>
        private int totalScans;

        /// <summary>
        /// The lowest scan number.
        /// </summary>
        private int minScan;

        /// <summary>
        /// The highest scan number.
        /// </summary>
        private int maxScan;

        /// <summary>
        /// Scans file for lazy loading.
        /// </summary>
        private string scansFilePath;

        /// <summary>
        /// ScanSummaryDAO for lazy loading.
        /// </summary>
        private ScanSummaryDAOHibernate scanSummaryDao;

        /// <summary>
        /// Sorted list of scan numbers.
        /// </summary>
        private List<int> knownScanNumbers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanSummaryProvider"/> class.
        /// For initializing an empty ScanSummaryProvider or a preloaded set of scan summaries.
        /// </summary>
        /// <param name="groupId">The identifier for the dataset that this provider is a part of.</param>
        /// <param name="scanSummaries">Preloaded scan summaries.</param>
        public ScanSummaryProvider(int groupId, IEnumerable<ScanSummary> scanSummaries = null)
        {
            this.GroupId = groupId;
            this.summary = null;
            this.totalScans = -1;
            this.minScan = -1;
            this.maxScan = -1;
            this.IsBackedByFile = true;
            this.knownScanNumbers = new List<int>();

            // Add preloaded scan summaries if available.
            if (scanSummaries != null)
            {
                this.summary = new DatasetSummary();
                foreach (var scanSummary in scanSummaries)
                {
                    this.summary.ScanMetaData.Add(scanSummary.Scan, scanSummary);
                }

                this.knownScanNumbers = this.summary.ScanMetaData.Keys.OrderBy(x => x).ToList();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanSummaryProvider"/> class.
        /// For initializing a ScanSummaryProvider from a scans file.
        /// </summary>
        /// <param name="groupId">The identifier for the dataset that this provider is a part of.</param>
        /// <param name="scansFilePath">The path to the scans file.</param>
        public ScanSummaryProvider(int groupId, string scansFilePath) : this(groupId)
        {
            this.scansFilePath = scansFilePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanSummaryProvider"/> class.
        /// For initializing a ScanSummaryProvider from a database data access object.
        /// </summary>
        /// <param name="groupId">The identifier for the dataset that this provider is a part of.</param>
        /// <param name="scanSummaryDao"></param>
        public ScanSummaryProvider(int groupId, ScanSummaryDAOHibernate scanSummaryDao)
            : this(groupId)
        {
            this.scanSummaryDao = scanSummaryDao;
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
                if (this.totalScans < 0)
                {   // Lazy load scans file.
                    this.LoadScans();
                }

                return this.totalScans;
            }
            private set { this.totalScans = value; }
        }

        /// <summary>
        /// Gets the lowest scan number.
        /// </summary>
        public int MinScan
        {
            get
            {
                if (this.minScan < 0)
                {   // Lazy load scans file.
                    this.LoadScans();
                }

                return this.minScan;
            }
            private set { this.minScan = value; }
        }

        /// <summary>
        /// Gets the highest scan number.
        /// </summary>
        public int MaxScan
        {
            get
            {
                if (this.maxScan < 0)
                {   // Lazy load scans file.
                    this.LoadScans();
                }

                return this.maxScan;
            }
            private set { this.maxScan = value; }
        }

        /// <summary>
        /// Retrieves the scan header from the underlying stream.
        /// </summary>
        public ScanSummary GetScanSummary(int scan)
        {
            if (this.summary == null)
            {
                this.LoadScans();
            }

            var scanSummary = this.summary.ScanMetaData.ContainsKey(scan) ?
                                          this.summary.ScanMetaData[scan] :
                                          this.InterpolateScanSummary(scan);

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
            if (this.summary == null)
            {
                this.LoadScans();
            }

            if (msLevel == 0)
            {
                return this.summary.ScanMetaData.Values.ToList();
            }

            return this.summary.ScanMetaData.Values.Where(sum => sum.MsLevel == msLevel).ToList();
        }

        public ScanSummary GetNextScanSummary(int scan, int msLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value that indicates whether the scan summary provider is populated from a file, or from something else (i.e., the database)
        /// </summary>
        public bool IsBackedByFile { get; private set; }

        /// <summary>
        /// A method that forces the provider to initializes itself if it uses lazy loading.
        /// </summary>
        /// <param name="progress">The progress of the initialization process.</param>
        public void Initialize(IProgress<PRISM.ProgressData> progress = null)
        {
            this.LoadScans();
        }

        /// <summary>
        /// A method that forces the provider to initializes itself if it uses lazy loading asynchronously.
        /// </summary>
        /// <param name="progress">The progress of the initialization process.</param>
        public Task InitializeAsync(IProgress<PRISM.ProgressData> progress = null)
        {
            return Task.Run(() => this.Initialize(progress));
        }

        /// <summary>
        /// Load scan summaries from a scans file.
        /// </summary>
        /// <param name="filePath">The file path to the scans file.</param>
        public void LoadScansFile(string filePath)
        {
            this.scansFilePath = filePath;
            this.summary = new DatasetSummary();
            var reader = new ScansFileReader { Delimiter = ',' };
            var scans = reader.ReadFile(this.scansFilePath);
            var dict = this.summary.ScanMetaData;
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
            this.knownScanNumbers = dict.Values.OrderBy(value => value.Scan).Select(value => value.Scan).ToList();
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
            this.scanSummaryDao = dao;
            var data = dao.FindByDatasetId(this.GroupId);

            foreach (var summary in data)
            {
                this.summary.ScanMetaData.Add(summary.Scan, summary);

                this.MinScan = Math.Min(this.MinScan, summary.Scan);
                this.MaxScan = Math.Max(this.MaxScan, summary.Scan);
            }

            this.TotalScans = this.MaxScan - this.MinScan;
            this.IsBackedByFile = false;
        }

        /// <summary>
        /// Dispose of scan summary provider.
        /// </summary>
        public void Dispose()
        {
            summary.ScanMetaData.Clear();
        }

        /// <summary>
        /// Load scans from scans file, if available, or database.
        /// </summary>
        private void LoadScans()
        {
            if (!string.IsNullOrEmpty(this.scansFilePath) && File.Exists(this.scansFilePath))
            {
                this.LoadScansFile(this.scansFilePath);
            }
            else if (this.scanSummaryDao != null)
            {
                this.LoadFromDatabase(this.scanSummaryDao);
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

            foreach (var summary in this.summary.ScanMetaData.Values)
            {
                summary.Net = (summary.Time - minScanSum.Time) / timeDiff;
            }
        }

        /// <summary>
        /// If scan summary is not known from the scans file/databse,
        /// calculate the retention time by interpolating between the two
        /// nearest scans.
        /// </summary>
        /// <param name="scanNumber">The target scan number.</param>
        /// <returns>The scan summary for the target scan number.</returns>
        private ScanSummary InterpolateScanSummary(int scanNumber)
        {
            // Exact match not found; find the elution time of the nearest scan
            var indexNearest = ~this.knownScanNumbers.BinarySearch(scanNumber);

            ScanSummary scanSummary;

            if (indexNearest <= 0)
            {
                scanSummary = this.summary.ScanMetaData[this.knownScanNumbers[0]];
            }
            else if (indexNearest >= this.knownScanNumbers.Count)
            {
                scanSummary = this.summary.ScanMetaData[this.knownScanNumbers[this.knownScanNumbers.Count - 1]];
            }
            else
            {
                var lowScanSummary = this.summary.ScanMetaData[this.knownScanNumbers[indexNearest - 1]];
                var highScanSummary = this.summary.ScanMetaData[this.knownScanNumbers[indexNearest]];
                var lowScan = lowScanSummary.Scan;
                var highScan = highScanSummary.Scan;
                var lowRt = lowScanSummary.Time;
                var highRt = highScanSummary.Time;

                var retentionTime = lowRt + (highRt - lowRt) * ((double)(scanNumber - lowScan) / (highScan - lowScan));

                scanSummary = new ScanSummary
                {
                    DatasetId = this.GroupId,
                    Scan = scanNumber,
                    Time = retentionTime,
                    Net = this.CalculateNet(retentionTime),
                    MsLevel = 1,
                    CollisionType = CollisionType.None
                };
            }

            return scanSummary;
        }

        /// <summary>
        /// Calculate normal elution time from retention time.
        /// </summary>
        /// <param name="retentionTime"></param>
        /// <returns></returns>
        private double CalculateNet(double retentionTime)
        {
            var minScanSummary = this.summary.ScanMetaData[this.MinScan];
            var maxScanSummary = this.summary.ScanMetaData[this.MaxScan];

            return (retentionTime - minScanSummary.Time) / (maxScanSummary.Time - minScanSummary.Time);
        }
    }
}
