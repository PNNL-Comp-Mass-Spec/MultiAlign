#region

using System.Collections.Generic;
using System.Linq;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignCore.IO.RawData
{
    /// <summary>
    ///     Allows the context to use the spectra provider, and filters spectra available by
    ///     only suggesting spectra that are associated with an LC-MS feature.
    /// </summary>
    public sealed class CachedFeatureSpectraProvider : ISpectraProvider
    {
        /// <summary>
        ///     Maps the scans to spectra that are related to features passed in.
        /// </summary>
        private readonly Dictionary<int, MSSpectra> m_spectraMap;

        private readonly ISpectraProvider m_reader;

        public CachedFeatureSpectraProvider(ISpectraProvider reader, IEnumerable<UMCLight> features)
        {
            m_reader = reader;
            m_spectraMap = new Dictionary<int, MSSpectra>();

            // Sort out the features to make a dictionary so we can look up spectra
            // and summary information later on without having to touch the disk again...and 
            // this restricts all possible spectra to those that came from deisotoped data.
            foreach (var feature in features)
            {
                foreach (var msFeature in feature.MsFeatures)
                {
                    foreach (var spectrum in msFeature.MSnSpectra)
                    {
                        if (!m_spectraMap.ContainsKey(spectrum.Scan))
                            m_spectraMap.Add(spectrum.Scan, spectrum);
                    }
                }
            }
        }

        public CachedFeatureSpectraProvider(ISpectraProvider reader, IEnumerable<MSSpectra> spectra)
        {
            m_reader     = reader;
            m_spectraMap = new Dictionary<int, MSSpectra>();

            // Sort out the features to make a dictionary so we can look up spectra
            // and summary information later on without having to touch the disk again...and 
            // this restricts all possible spectra to those that came from deisotoped data.
            foreach (var spectrum in spectra)
            {
                if (!m_spectraMap.ContainsKey(spectrum.Scan))
                    m_spectraMap.Add(spectrum.Scan, spectrum);             
            }
        }

        public List<XYData> GetRawSpectra(int scan, int group, out ScanSummary summary)
        {
            return m_reader.GetRawSpectra(scan, group, out summary);
        }

        public List<MSSpectra> GetRawSpectra(int group)
        {
            return m_spectraMap.Values.ToList();
        }

        public List<XYData> GetRawSpectra(int scan, int group, int scanLevel, out ScanSummary summary)
        {
            return m_reader.GetRawSpectra(scan, group, scanLevel, out summary);
        }

        public List<MSSpectra> GetMSMSSpectra(int group)
        {
            return m_spectraMap.Values.ToList();
        }

        public List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap)
        {
            return m_spectraMap.Values.Where(x => !excludeMap.ContainsKey(x.Scan)).ToList();
        }

        public List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap, bool loadPeaks)
        {
            if (loadPeaks)
                return m_reader.GetMSMSSpectra(group, excludeMap, true);

            return m_spectraMap.Values.Where(x => !excludeMap.ContainsKey(x.Scan)).ToList();
        }

        public void AddDataFile(string path, int groupId)
        {
            m_reader.AddDataFile(path, groupId);
        }

        public Dictionary<int, ScanSummary> GetScanData(int groupId)
        {
            return m_spectraMap.Keys.ToDictionary(scan => scan, scan => m_spectraMap[scan].ScanMetaData);
        }

        public int GetTotalScans(int group)
        {
            return m_reader.GetTotalScans(group);
        }

        public void Dispose()
        {
            m_reader.Dispose();
        }


        public MSSpectra GetSpectrum(int scan, int group, int scanLevel, out ScanSummary summary, bool loadPeaks)
        {
            if (m_spectraMap.ContainsKey(scan))
            {
                summary = m_spectraMap[scan].ScanMetaData;

                if (loadPeaks)
                {
                    m_spectraMap[scan].Peaks = GetRawSpectra(scan, group, scanLevel, out summary);
                }
                return m_spectraMap[scan];
            }

            return m_reader.GetSpectrum(scan, group, scanLevel, out summary, loadPeaks);
        }
    }
}