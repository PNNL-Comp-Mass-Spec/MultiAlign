using System.Collections.Generic;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Encapsulates the data about a dataset.
    /// </summary>
    public class DatasetSummary
    {
        public DatasetSummary()
        {
            this.ScanMetaData = new Dictionary<int, ScanSummary>();
        }

        /// <summary>
        /// Gets or sets the summary data associated with the meta-data.
        /// </summary>
        public Dictionary<int, ScanSummary> ScanMetaData
        {
            get;
            set;
        }
    }
}
