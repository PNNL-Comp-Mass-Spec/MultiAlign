#region

using System.IO;
using System.Linq;
using FeatureAlignment.Data;
using InformedProteomics.Backend.MassSpecData;
using MultiAlignCore.Data;

#endregion

namespace MultiAlignCore.IO.RawData
{
    using MultiAlignCore.IO.Hibernate;

    /// <summary>
    /// Class in charge of creating RAW Data file readers.
    /// </summary>
    public class RawLoaderFactory
    {
        /// <summary>
        /// Constructs a raw data file reader for reading the instrument (or equivalent) mass spectra.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="groupId"></param>
        /// <returns>The cold scan summary provider (uses lazy loading)</returns>
        public static IScanSummaryProvider CreateFileReader(string name, int groupId)
        {
            if (name == null)
                return null;

            if (name.ToLower().EndsWith("_scans.csv"))
            {
                // DeconTools Scans file
                return new ScanSummaryProvider(groupId, name);
            }

            // Just use InformedProteomics...
            if (MassSpecDataReaderFactory.MassSpecDataTypeFilterList.Any(ext => name.ToLower().EndsWith(ext)))
            {
                return new InformedProteomicsReader(groupId, name);
            }

            return null;
        }

        /// <summary>
        /// Construct a raw data file and attempt to fill it from the database.
        /// </summary>
        /// <param name="groupId">The dataset to load scan summaries for.</param>
        /// <returns>The cold scan summary provider (uses lazy loading)</returns>
        public static IScanSummaryProvider CreateFileReader(int groupId)
        {
            return new ScanSummaryProvider(groupId);
        }
    }
}