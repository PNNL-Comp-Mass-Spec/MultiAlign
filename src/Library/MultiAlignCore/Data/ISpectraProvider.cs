using System;
using System.Collections.Generic;

namespace MultiAlignCore.Data
{    
    /// <summary>
    /// Interface for objects that have access to raw data.
    /// </summary>
    public interface ISpectraProvider: IDisposable
    {
        /// <summary>
        /// Retrieves the scan from the underlying stream.
        /// </summary>
        List<XYData> GetRawSpectra(int scan, int group, out ScanSummary summary);

        /// <summary>
        /// Gets a list of all raw spectra
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        List<MSSpectra> GetRawSpectra(int group);

        MSSpectra       GetSpectrum(int scan, int group, int scanLevel, out ScanSummary summary, bool loadPeaks);

        /// <summary>
        /// Retrieves the scan from the underlying stream including the scan summary
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="group"></param>
        /// <param name="scanLevel"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        List<XYData> GetRawSpectra(int scan, int group, int scanLevel, out ScanSummary summary);

        /// <summary>
        /// Retrieves a list of MS/MS spectra from the given group.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        List<MSSpectra> GetMSMSSpectra(int group);

        /// <summary>
        /// Get a list of MS/MS spectra, but exclude if it exists in the dictionary of provided scans.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="excludeMap"></param>
        /// <returns></returns>
        List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="excludeMap"></param>
        /// <param name="loadPeaks"></param>
        /// <returns></returns>
        List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap, bool loadPeaks);

        /// <summary>
        /// Adds a file ID to the path for multi-file support.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="groupId"></param>
        void AddDataFile(string path, int groupId);  
    
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