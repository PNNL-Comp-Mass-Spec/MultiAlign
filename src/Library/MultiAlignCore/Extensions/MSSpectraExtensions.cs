#region

using System.Collections.Generic;
using PNNLOmics.Data;

#endregion

namespace MultiAlignCore.Extensions
{
    public static class MSSpectraExtensions
    {
        public static Dictionary<int, List<MSSpectra>> CreateScanMapsForMsMs<T>(this List<T> spectra) where T : MSSpectra
        {
            var map = new Dictionary<int, List<MSSpectra>>();
            foreach (var spectrum in spectra)
            {
                if (!map.ContainsKey(spectrum.Scan))
                {
                    map.Add(spectrum.Scan, new List<MSSpectra>());
                }
                map[spectrum.Scan].Add(spectrum);
            }
            return map;
        }
    }
}
