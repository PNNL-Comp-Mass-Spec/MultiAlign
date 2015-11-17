using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.IO.SequenceData
{
    public class IdentificationProviderCache
    {
        /// <summary>
        /// Maps group dataset numbers to identification providers.
        /// </summary>
        private readonly Dictionary<int, IIdentificationProvider> groupToProviderMap;

        /// <summary>
        /// Maps identification file names to identification providers.
        /// </summary>
        private readonly Dictionary<string, IIdentificationProvider> fileNameToProviderMap;

        /// <summary>
        /// The data access object for the analysis database.
        /// </summary>
        private readonly ISequenceToMsnFeatureDAO databaseDao;

        /// <summary>
        /// Initializes an instance of the <see cref="IdentificationProviderCache" /> class.
        /// </summary>
        /// <param name="databaseDao">The data access object for the analysis database.</param>
        public IdentificationProviderCache(ISequenceToMsnFeatureDAO databaseDao = null)
        {
            this.databaseDao = databaseDao;

            this.groupToProviderMap = new Dictionary<int, IIdentificationProvider>();
            this.fileNameToProviderMap = new Dictionary<string, IIdentificationProvider>();
        }

        /// <summary>
        /// Get an identification provider for a given identification file.
        /// </summary>
        /// <param name="fileName">The identification file.</param>
        /// <param name="groupId">The group id for the dataset to get/create the identification file for.</param>
        /// <returns>The correct identification provider.</returns>
        public IIdentificationProvider GetProvider(string fileName, int groupId = -1)
        {
            if (!this.fileNameToProviderMap.ContainsKey(fileName))
            {
                if (!groupToProviderMap.ContainsKey(groupId))
                {
                    groupId = groupId < 0 ? this.groupToProviderMap.Keys.Max() + 1 : groupId;
                    this.groupToProviderMap.Add(groupId, new IdentificationProvider(databaseDao) { GroupId = groupId });
                }

                this.groupToProviderMap[groupId].GetIdentifications(fileName);
                this.fileNameToProviderMap.Add(fileName, this.groupToProviderMap[groupId]);
            }

            return this.fileNameToProviderMap[fileName];
        }

        /// <summary>
        /// Get an identification provider for the given dataset or create an empty one if it doesn't already exist.
        /// </summary>
        /// <param name="groupId">The group identifier of the dataset to create the provider for.</param>
        /// <returns>The identification provider.</returns>
        public IIdentificationProvider GetProvider(int groupId)
        {
            if (!this.groupToProviderMap.ContainsKey(groupId))
            {
                this.groupToProviderMap.Add(groupId, new IdentificationProvider(databaseDao) { GroupId = groupId });
            }

            return this.groupToProviderMap[groupId];
        }
    }
}
