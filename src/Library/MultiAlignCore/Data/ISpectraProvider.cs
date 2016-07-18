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
        List<XYData> GetRawSpectra(int scan, out ScanSummary summary);

        /// <summary>
        /// Gets a list of all raw spectra
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        List<MSSpectra> GetRawSpectra();

        MSSpectra       GetSpectrum(int scan, int scanLevel, out ScanSummary summary, bool loadPeaks);

        /// <summary>
        /// Retrieves the scan from the underlying stream including the scan summary
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="group"></param>
        /// <param name="scanLevel"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        List<XYData> GetRawSpectra(int scan, int scanLevel, out ScanSummary summary);

        /// <summary>
        /// Retrieves a list of MS/MS spectra from the given group.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        List<MSSpectra> GetMSMSSpectra();

        /// <summary>
        /// Gets the fragmentation spectra associated with a given scan.
        /// </summary>
        /// <param name="scan">The MS1 scan to get MS/MS scans for.</param>
        /// <returns></returns>
        List<MSSpectra> GetMSMSSpectra(int prevMsScan, double mz, bool loadPeaks);

            /// <summary>
        /// Get a list of MS/MS spectra, but exclude if it exists in the dictionary of provided scans.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="excludeMap"></param>
        /// <returns></returns>
        List<MSSpectra> GetMSMSSpectra(Dictionary<int, int> excludeMap);

        /// <summary>
        ///
        /// </summary>
        /// <param name="group"></param>
        /// <param name="excludeMap"></param>
        /// <param name="loadPeaks"></param>
        /// <returns></returns>
        List<MSSpectra> GetMSMSSpectra(Dictionary<int, int> excludeMap, bool loadPeaks);
    }
}