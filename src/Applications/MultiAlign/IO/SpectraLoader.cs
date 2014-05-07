using System.Collections.Generic;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data;

namespace MultiAlign.IO
{
    //TODO: Load a spectra
    /// <summary>
    /// 
    /// </summary>
    public class SpectraLoader
    {

        /// <summary>
        /// Loads the MS/MS spectrum from file.
        /// </summary>
        /// <param name="spectrum"></param>
        public static List<XYData> LoadSpectrum(MSSpectra spectrum)
        {
            var peaks = new List<XYData>();
            var info = SingletonDataProviders.GetDatasetInformation(spectrum.GroupId);
            if (info != null && info.Raw != null && info.RawPath != null)
            {
                peaks = ParentSpectraFinder.GetDaughterSpectrum(info.RawPath, spectrum.Scan);                
            }
            return peaks;
        }
    }
}
