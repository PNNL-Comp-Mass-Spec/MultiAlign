#region

using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;
using MultiAlignCore.IO.RawData;

#endregion

namespace MultiAlignCore.IO.MsMs
{
    /// <summary>
    /// Finds the spectra for a given feature's scan
    /// </summary>
    public class ParentSpectraFinder
    {

        private static Dictionary<string, ISpectraProvider> mRawDataProviders;

        /// <summary>
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="path"></param>
        /// <param name="mz"></param>
        /// <param name="mzRange"></param>
        /// <returns></returns>
        public static List<XYData> GetParentSpectrum(string path, int scan, double minMz, double maxMz)
        {
            ISpectraProvider provider = GetProvider(path);

            if (provider == null)
            {
                return null;
            }

            List<XYData> spectrum = null;
            try
            {
                var summary = new ScanSummary();
                spectrum = provider.GetRawSpectra(scan, 1, out summary);
            }
            catch
            {
                Logger.PrintMessage("Could not load the raw spectra");
                return null;
            }

            if (spectrum == null)
            {
                return null;
            }

            var data = (from x in spectrum
                        where x.X > minMz && x.X < maxMz
                        select x).ToList();

            return data;
        }

        public static List<XYData> GetDaughterSpectrum(string path, int scan)
        {
            ISpectraProvider provider = GetProvider(path);

            if (provider == null)
            {
                return null;
            }

            List<XYData> spectrum = null;
            try
            {
                var summary = new ScanSummary();
                spectrum = provider.GetRawSpectra(scan, 2, out summary);
            }
            catch
            {
                Logger.PrintMessage("Could not load the raw spectra");
                return null;
            }

            return spectrum;
        }

        private static ISpectraProvider GetProvider(string path)
        {
            if (mRawDataProviders == null)
                mRawDataProviders = new Dictionary<string, ISpectraProvider>();

            ISpectraProvider provider;
            if (!mRawDataProviders.TryGetValue(path, out provider))
            {
                provider = new InformedProteomicsReader(0, path);
                mRawDataProviders.Add(path, provider);
            }

            return provider;
        }

    }
}