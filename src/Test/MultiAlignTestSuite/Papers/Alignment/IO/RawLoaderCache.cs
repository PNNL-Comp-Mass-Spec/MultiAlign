#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Data;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment.IO
{
    public class RawLoaderCache : ISpectraProvider
    {
        private readonly Dictionary<int, Dictionary<int, ScanSummary>> m_summaryMap;
        private readonly ISpectraProvider m_provider;


        public RawLoaderCache(ISpectraProvider provider)
        {
            m_summaryMap = new Dictionary<int, Dictionary<int, ScanSummary>>();
            m_provider = provider;
        }

        public Dictionary<int, ScanSummary> GetScanData(int group)
        {
            if (!m_summaryMap.ContainsKey(group))
            {
                m_summaryMap[group] = m_provider.GetScanData(group);
            }

            return m_summaryMap[group];
        }

        #region ISpectraProvider Members

        public int GetMinScan(int group)
        {
            return m_provider.GetMinScan(group);
        }

        public int GetMaxScan(int group)
        {
            return m_provider.GetMaxScan(group);
        }

        public void AddDataFile(string path, int groupId)
        {
            m_provider.AddDataFile(path, groupId);
        }

        public void AddCache(int groupID, Dictionary<int, ScanSummary> cache)
        {
            if (m_summaryMap.ContainsKey(groupID))
                throw new Exception("The scan summary already exists for this group.");

            m_summaryMap.Add(groupID, cache);
        }

        public List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap, bool loadPeaks)
        {
            return m_provider.GetMSMSSpectra(group, excludeMap, loadPeaks);
        }

        public List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap)
        {
            return m_provider.GetMSMSSpectra(group, excludeMap);
        }

        public List<MSSpectra> GetMSMSSpectra(int group)
        {
            return m_provider.GetMSMSSpectra(group);
        }


        public List<MSSpectra> GetRawSpectra(int group)
        {
            return m_provider.GetRawSpectra(group);
        }

        public ScanSummary GetScanSummary(int scan, int @group)
        {
            return m_provider.GetScanSummary(scan, group);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_provider.Dispose();
        }

        #endregion

        public List<XYData> GetRawSpectra(int scan, int group, int scanLevel, out ScanSummary summary)
        {
            return m_provider.GetRawSpectra(scan, group, scanLevel, out summary);
        }


        public List<XYData> GetRawSpectra(int scan, int group, out ScanSummary summary)
        {
            return m_provider.GetRawSpectra(scan, group, -1, out summary);
        }


        public int GetTotalScans(int group)
        {
            return m_provider.GetTotalScans(group);
        }


        public MSSpectra GetSpectrum(int scan, int group, int scanLevel, out ScanSummary summary, bool loadPeaks)
        {
            return m_provider.GetSpectrum(scan, group, scanLevel, out summary, loadPeaks);
        }
    }
}