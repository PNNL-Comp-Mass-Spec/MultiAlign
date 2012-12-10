using System;
using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using PNNLOmics.Algorithms;
using PNNLOmics.Data;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.IO.MTDB
{
    public class MassTagDatabaseLoaderCache: IMtdbLoader, IProgressNotifer
    {

        public event EventHandler<ProgressNotifierArgs> Progress;
        /// <summary>
        /// Updates listeners with status messages.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
            }
        }
        #region Properties
        /// <summary>
        /// Gets or sets the default mass tag database options.
        /// </summary>
        public MassTagDatabaseOptions Options
        {
            get;
            set;
        }
        public IMassTagDAO Provider
        {
            get;
            set;
        }
        #endregion

        #region IMtdbLoader Members

        public Data.MassTags.MassTagDatabase LoadDatabase()
        {
            MassTagDatabase database = new MassTagDatabase();
            database.Name = "";

            UpdateStatus("Loading all of the mass tags.");
            // Get all of the mass tags
            List<MassTagLight> massTags = Provider.FindAll();


            UpdateStatus("Loading all of the tag to protein references.");
            // Then get all of the mass tag to protein maps
            IGenericDAO<MassTagToProteinMap> tagToProteinMapCache = new GenericDAOHibernate<MassTagToProteinMap>();
            List<MassTagToProteinMap> maps = tagToProteinMapCache.FindAll();

            // Then get all of the proteins
            UpdateStatus("Loading all of the protein data.");
            IProteinDAO proteinCache = new MultiAlignCore.IO.Features.Hibernate.ProteinDAO();
            List<Protein> proteins = proteinCache.FindAll();

            UpdateStatus("Indexing the protein data for faster assembly.");
            Dictionary<int, Protein> proteinMap = new Dictionary<int, Protein>();
            foreach (Protein p in proteins)
            {
                if (!proteinMap.ContainsKey(p.ProteinID))
                {
                    proteinMap.Add(p.ProteinID, p);
                }
            }

            Dictionary<int, List<MassTagToProteinMap>> matchedMaps = new Dictionary<int, List<MassTagToProteinMap>>();

            foreach (MassTagToProteinMap singleMap in maps)
            {
                if (!matchedMaps.ContainsKey(singleMap.MassTagId))
                {
                    matchedMaps.Add(singleMap.MassTagId, new List<MassTagToProteinMap>());
                }
                matchedMaps[singleMap.MassTagId].Add(singleMap);
            }

            UpdateStatus("Re-mapping the proteins to the mass tags.");
            Dictionary<int, List<Protein>> massTagProteinMap = new Dictionary<int, List<Protein>>();

            // Then map them.
            foreach (MassTagLight tag in massTags)
            {
                int id = tag.ID;

                if (!massTagProteinMap.ContainsKey(id))
                {
                    massTagProteinMap.Add(id, new List<Protein>());
                }

                List<MassTagToProteinMap> matches = new List<MassTagToProteinMap>();

                if (matchedMaps.ContainsKey(id))
                {
                    matches = matchedMaps[id];
                }

                List<Protein> newProteins = new List<Protein>();
                foreach (MassTagToProteinMap mtMap in matches)
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

            int totalMassTags = database.MassTags.Count;
            UpdateStatus("Loaded " + totalMassTags.ToString() + " mass tags.");

            return database;
        }

        #endregion

        #region IProgressNotifer Members


        #endregion
    }
}
