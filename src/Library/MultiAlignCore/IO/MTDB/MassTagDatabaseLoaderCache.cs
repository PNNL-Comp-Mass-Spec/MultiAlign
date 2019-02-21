#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Hibernate;
using MultiAlignCore.IO.MassTags;
using MultiAlignCore.IO.Proteins;

#endregion

namespace MultiAlignCore.IO.MTDB
{
    public class MassTagDatabaseLoaderCache : IMtdbLoader, IProgressNotifer
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        ///     Updates listeners with status messages.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            Progress?.Invoke(this, new ProgressNotifierArgs(message));
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the default mass tag database options.
        /// </summary>
        public MassTagDatabaseOptions Options { get; set; }

        public IMassTagDAO Provider { get; set; }

        #endregion

        #region IMtdbLoader Members

        public MassTagDatabase LoadDatabase()
        {
            var database = new MassTagDatabase {Name = ""};

            UpdateStatus("Loading all of the mass tags.");
            // Get all of the mass tags
            var massTags = Provider.FindAll();


            UpdateStatus("Loading all of the tag to protein references.");
            // Then get all of the mass tag to protein maps
            IGenericDAO<MassTagToProteinMap> tagToProteinMapCache = new GenericDAOHibernate<MassTagToProteinMap>();
            var maps = tagToProteinMapCache.FindAll();

            // Then get all of the proteins
            UpdateStatus("Loading all of the protein data.");
            IProteinDAO proteinCache = new ProteinDAO();
            var proteins = proteinCache.FindAll();

            UpdateStatus("Indexing the protein data for faster assembly.");
            var proteinMap = new Dictionary<int, Protein>();
            foreach (var p in proteins)
            {
                if (!proteinMap.ContainsKey(p.ProteinId))
                {
                    proteinMap.Add(p.ProteinId, p);
                }
            }

            var matchedMaps = new Dictionary<int, List<MassTagToProteinMap>>();

            foreach (var singleMap in maps)
            {
                if (!matchedMaps.ContainsKey(singleMap.MassTagId))
                {
                    matchedMaps.Add(singleMap.MassTagId, new List<MassTagToProteinMap>());
                }
                matchedMaps[singleMap.MassTagId].Add(singleMap);
            }

            UpdateStatus("Re-mapping the proteins to the mass tags.");
            var massTagProteinMap = new Dictionary<int, List<Protein>>();

            // Then map them.
            foreach (var tag in massTags)
            {
                var id = tag.Id;

                if (!massTagProteinMap.ContainsKey(id))
                {
                    massTagProteinMap.Add(id, new List<Protein>());
                }

                var matches = new List<MassTagToProteinMap>();

                if (matchedMaps.ContainsKey(id))
                {
                    matches = matchedMaps[id];
                }

                var newProteins = new List<Protein>();
                foreach (var mtMap in matches)
                {
                    if (proteinMap.ContainsKey(mtMap.ProteinId))
                    {
                        newProteins.Add(proteinMap[mtMap.ProteinId]);
                    }
                }
                massTagProteinMap[id].AddRange(newProteins);
            }

            UpdateStatus("Building the in memory mass tag database.");
            database.AddMassTagsAndProteins(massTags, massTagProteinMap);
            database.AllProteins = proteins;

            var totalMassTags = database.MassTags.Count;
            UpdateStatus("Loaded " + totalMassTags + " mass tags.");

            return database;
        }

        #endregion

        #region IProgressNotifer Members

        #endregion
    }
}