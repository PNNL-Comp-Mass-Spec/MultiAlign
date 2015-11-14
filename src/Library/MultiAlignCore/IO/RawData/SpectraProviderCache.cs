using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.IO.RawData
{
    using MultiAlignCore.Data;

    public class SpectraProviderCache : ScanSummaryProviderCache
    {
        private Dictionary<string, ISpectraProvider> filePathToSpectraProvider;

        private Dictionary<int, ISpectraProvider> datasetToSpectraProvider;

        public SpectraProviderCache()
        {
            this.filePathToSpectraProvider = new Dictionary<string, ISpectraProvider>();
            this.datasetToSpectraProvider = new Dictionary<int, ISpectraProvider>();
        }

        public ISpectraProvider GetSpectraProvider(string filePath, int groupId = -1)
        {
            if (!filePathToSpectraProvider.ContainsKey(filePath))
            {
                var spectraProvider = RawLoaderFactory.CreateFileReader(filePath, groupId) as ISpectraProvider;

                if (spectraProvider == null)
                {
                    return null;
                }

                this.filePathToSpectraProvider.Add(filePath, spectraProvider);

                if (groupId < 0)
                {   // We're adding a new dataset, give it a valid group ID.
                    if (this.datasetToSpectraProvider.Any())
                    {
                        groupId = this.datasetToSpectraProvider.Keys.Max() + 1;
                    }
                }

                if (this.datasetToSpectraProvider.ContainsKey(groupId))
                {
                    throw new ArgumentException(string.Format("Already contains a scan summary for dataset {0}", groupId));
                }

                this.datasetToSpectraProvider.Add(groupId, spectraProvider);
            }

            return this.filePathToSpectraProvider[filePath];
        }

        public ISpectraProvider GetSpectraProvider(int groupId)
        {
            if (!this.datasetToSpectraProvider.ContainsKey(groupId))
            {
                return null;
            }

            return this.datasetToSpectraProvider[groupId];
        }
    }
}
