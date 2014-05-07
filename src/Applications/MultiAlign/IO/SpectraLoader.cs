using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;
using MultiAlignCore.Data;
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
            List<XYData> peaks = new List<XYData>();
            DatasetInformation info = SingletonDataProviders.GetDatasetInformation(spectrum.GroupID);
            if (info != null && info.Raw != null && info.RawPath != null)
            {
                peaks = ParentSpectraFinder.GetDaughterSpectrum(info.RawPath, spectrum.Scan);                
            }
            return peaks;
        }
    }
}
