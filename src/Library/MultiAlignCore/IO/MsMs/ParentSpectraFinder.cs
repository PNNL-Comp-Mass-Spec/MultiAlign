using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Finds the spectra for a given feature's scan
    /// </summary>
    public class ParentSpectraFinder
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="path"></param>
        /// <param name="mz"></param>
        /// <param name="mzRange"></param>
        /// <returns></returns>
        public static List<XYData> GetParentSpectrum(string path, int scan, double minMz, double maxMz)
        {
            using (ISpectraProvider provider = RawLoaderFactory.CreateFileReader(path))
            {
                if (provider != null)
                {
                    provider.AddDataFile(path, 0);

                    List<XYData> spectrum = null;
                    try
                    {
                        var summary = new ScanSummary();
                        spectrum = provider.GetRawSpectra(scan, 0, 1, out summary);
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
            }
            return null;
        }

        public static List<XYData> GetDaughterSpectrum(string path, int scan)
        {
            using (ISpectraProvider provider = RawLoaderFactory.CreateFileReader(path))
            {
                if (provider != null)
                {
                    provider.AddDataFile(path, 0);

                    List<XYData> spectrum = null;
                    try
                    {
                        var summary = new ScanSummary();
                        spectrum = provider.GetRawSpectra(scan, 0, 2, out summary);
                    }
                    catch
                    {
                        Logger.PrintMessage("Could not load the raw spectra");
                        return null;
                    }


                    return spectrum;
                }
            }
            return null;
        }
    }
}
