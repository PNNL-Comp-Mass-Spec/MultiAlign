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
        /// Retrieves the scan data for the given dataset ID (i.e. group ID)
        /// </summary>
        /// <param name="groupId">Group identifier</param>
        /// <returns>Mapped scan header data based on scan ID</returns>
        Dictionary<int, ScanSummary> GetScanData(int groupId);

        /// <summary>
        /// Retrieves the scan header from the underlying stream.
        /// </summary>
        ScanSummary GetScanSummary(int scan, int group);

        /// <summary>
        /// Adds a file ID to the path for multi-file support.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="groupId"></param>
        void AddDataFile(string path, int groupId);

        /// <summary>
        /// Gets the total number of scans
        /// </summary>
        /// <param name="group">Group (or dataset) provider</param>
        /// <returns>Total scans for that dataset.</returns>
        int GetTotalScans(int group);

        /// <summary>
        /// Gets the lowest scan number
        /// </summary>
        /// <param name="group">Group (or dataset) provider</param>
        /// <returns>Lowest scan number for that dataset</returns>
        int GetMinScan(int group);

        /// <summary>
        /// Gets the highest scan number
        /// </summary>
        /// <param name="group">Group (or dataset) provider</param>
        /// <returns>Highest scan number for that dataset</returns>
        int GetMaxScan(int group);
    }
}
