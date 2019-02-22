using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.MsMs;

namespace MultiAlign.IO
{
    //TODO: Load a spectra
    /// <summary>
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
            if (info != null && info.RawFile != null && info.RawFile.Path != null)
            {
                peaks = ParentSpectraFinder.GetDaughterSpectrum(info.RawFile.Path, spectrum.Scan);
            }
            return peaks;
        }
    }
}