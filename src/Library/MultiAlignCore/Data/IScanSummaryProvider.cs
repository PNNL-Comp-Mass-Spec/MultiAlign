using System;
using System.Collections.Generic;

namespace MultiAlignCore.Data
{
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
    }
}
