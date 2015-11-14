using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.IO.RawData
{
    using MultiAlignCore.Data;

    public class ScanSummaryProviderCache
    {
        private readonly Dictionary<string, IScanSummaryProvider> filePathToSummaryProvider;

        private readonly Dictionary<int, IScanSummaryProvider> datasetToSummaryProvider;

        public ScanSummaryProviderCache()
        {
            this.filePathToSummaryProvider = new Dictionary<string, IScanSummaryProvider>();
            this.datasetToSummaryProvider = new Dictionary<int, IScanSummaryProvider>();
        }

        public virtual IScanSummaryProvider GetScanSummaryProvider(string filePath, int groupId = -1)
        {
            if (!filePathToSummaryProvider.ContainsKey(filePath))
            {
                var scanSummaryProvider = RawLoaderFactory.CreateFileReader(filePath, groupId);
                this.filePathToSummaryProvider.Add(filePath, scanSummaryProvider);

                if (groupId < 0)
                {   // We're adding a new dataset, give it a valid group ID.
                    if (this.datasetToSummaryProvider.Any())
                    {
                        groupId = this.datasetToSummaryProvider.Keys.Max() + 1;
                    }
                }

                if (datasetToSummaryProvider.ContainsKey(groupId))
                {
                    throw new ArgumentException(string.Format("Already contains a scan summary for dataset {0}", groupId));
                }

                this.datasetToSummaryProvider.Add(groupId, scanSummaryProvider);
            }

            return this.filePathToSummaryProvider[filePath];
        }

        public virtual IScanSummaryProvider GetScanSummaryProvider(int groupId)
        {
            if (!datasetToSummaryProvider.ContainsKey(groupId))
            {   // We don't have the scan summary for this dataset yet, try to load it from the DB.
                var scanSummaryProvider = RawLoaderFactory.CreateFileReader(groupId);
                datasetToSummaryProvider.Add(groupId, scanSummaryProvider);
            }

            return this.datasetToSummaryProvider[groupId];
        }
    }
}
