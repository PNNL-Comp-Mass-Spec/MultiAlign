using System;
using System.Collections.Generic;

namespace MultiAlignCore.Data
{
    using System.Threading.Tasks;

    using InformedProteomics.Backend.Utils;

    /// <summary>
    /// Interface for objects that need access to scan summary data
    /// </summary>
    public interface IScanSummaryProvider : IDisposable
    {
        /// <summary>
        /// Gets the group ID for the dataset this scan summary provider is for.
        /// </summary>
        int GroupId { get; }

        /// <summary>
        /// Retrieves the scan header from the underlying stream.
        /// </summary>
        ScanSummary GetScanSummary(int scan);

        /// <summary>
        /// Get scan summaries for a particular ms level.
        /// </summary>
        /// <param name="msLevel">
        /// The mslevel to get scan summaries for.
        /// 0 = MS1 and MS2
        /// </param>
        /// <returns>All scan summaries.</returns>
        List<ScanSummary> GetScanSummaries(int msLevel = 0);

        /// <summary>
        /// Get the summary for the next scan following the given scan for the given MS level.
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="msLevel"></param>
        /// <returns></returns>
        ScanSummary GetNextScanSummary(int scan, int msLevel);

        /// <summary>
        /// Get the summary for the previous scan following the given scan for the given MS level.
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="msLevel"></param>
        /// <returns></returns>
        ScanSummary GetPreviousScanSummary(int scan, int msLevel);

        /// <summary>
        /// Gets the total number of scans
        /// </summary>
        int TotalScans { get; }

        /// <summary>
        /// Gets the lowest scan number
        /// </summary>
        int MinScan { get; }

        /// <summary>
        /// Gets the highest scan number
        /// </summary>
        int MaxScan { get; }

        /// <summary>
        /// Whether the scan summary provider is populated from a file, or from something else (i.e., the database)
        /// </summary>
        bool IsBackedByFile { get; }

        /// <summary>
        /// A method that forces the provider to initializes itself if it uses lazy loading.
        /// </summary>
        /// <param name="progress">The progress of the initialization process.</param>
        void Initialize(IProgress<ProgressData> progress = null);

        /// <summary>
        /// A method that forces the provider to initializes itself if it uses lazy loading asynchronously.
        /// </summary>
        /// <param name="progress">The progress of the initialization process.</param>
        Task InitializeAsync(IProgress<ProgressData> progress = null);
    }
}
