#region

using System.IO;
using System.Linq;
using InformedProteomics.Backend.MassSpecData;
using MultiAlignCore.Data;

#endregion

namespace MultiAlignCore.IO.RawData
{
    /// <summary>
    ///     Class in charge of creating RAW Data file readers.
    /// </summary>
    public class RawLoaderFactory
    {
        /// <summary>
        ///     Constructs a raw data file reader for reading the instrument (or equivalent) mass spectra.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IScanSummaryProvider CreateFileReader(string name)
        {
            if (name == null)
                return null;

            if (name.ToLower().EndsWith("_scans.csv"))
            {
                // DeconTools Scans file
                return new ScanSummaryProvider();
            }

            if (name.ToLower().Equals("analysis.db3"))
            {
                // Load from the database
                return new ScanSummaryProvider();
            }

            // Just use InformedProteomics...
            if (MassSpecDataReaderFactory.MassSpecDataTypeFilterList.Any(ext => name.ToLower().EndsWith(ext)))
            {
                return new InformedProteomicsReader();
            }

            return null;
        }
    }
}