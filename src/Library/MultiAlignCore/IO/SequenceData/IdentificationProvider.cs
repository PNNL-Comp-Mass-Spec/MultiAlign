using System.Collections.Generic;
using FeatureAlignment.Data.MassTags;

namespace MultiAlignCore.IO.SequenceData
{
    using MultiAlignCore.Extensions;
    using MultiAlignCore.IO.TextFiles;

    public class IdentificationProvider : IIdentificationProvider
    {
        private readonly Dictionary<int, List<Peptide>> identifications;

        private readonly ISequenceToMsnFeatureDAO databaseDao;

        public IdentificationProvider(ISequenceToMsnFeatureDAO databaseDao = null)
        {
            this.databaseDao = null;
            this.identifications = new Dictionary<int, List<Peptide>>();
        }

        /// <summary>
        /// Gets or sets the group identification number for the dataset this
        /// provider is for.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets the identifications for a specific scan.
        /// </summary>
        /// <param name="scanNum">The scan to get identifications for.</param>
        /// <returns>List of peptide identifications for the scan.</returns>
        public List<Peptide> GetIdentification(int scanNum)
        {
            if (this.identifications.ContainsKey(scanNum))
            {
                // If it's currently cached, just get it from the cache.
                return this.identifications[scanNum];
            }
            else
            {   // Todo: make it look in the database if it isn't in the cache.
                return null;
            }
        }

        /// <summary>
        /// Get all identifications from a particular identifications file.
        /// </summary>
        /// <param name="filePath">The path for the identifications list.</param>
        /// <returns>A dictionary mapping scan numbers to their respective identifications.</returns>
        public Dictionary<int, List<Peptide>> GetIdentifications(string filePath)
        {
            var sequenceProvider = PeptideReaderFactory.CreateReader(filePath);
            var peptides = sequenceProvider.Read(filePath);
            var scanMaps = peptides.CreateScanMaps();
            foreach (var scanMap in scanMaps)
            {
                if (!this.identifications.ContainsKey(scanMap.Key))
                {
                    this.identifications.Add(scanMap.Key, new List<Peptide>());
                }

                this.identifications[scanMap.Key].AddRange(scanMap.Value);
            }

            return scanMaps;
        }

        /// <summary>
        /// Gets all possible identifications for the dataset this provider is for.
        /// </summary>
        /// <returns>A dictionary mapping scan number to identifications.</returns>
        public Dictionary<int, List<Peptide>> GetAllIdentifications()
        {
            return new Dictionary<int, List<Peptide>>(this.identifications);

            // Todo: Alter this method to also get identifications form the database.
        }
    }
}
