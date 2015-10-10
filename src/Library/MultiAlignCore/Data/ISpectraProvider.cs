using System;
using System.Collections.Generic;

namespace MultiAlignCore.Data
{    
    /// <summary>
    /// Interface for objects that have access to raw data.
    /// </summary>
    public interface ISpectraProvider: IScanSummaryProvider, IDisposable
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
    }
}