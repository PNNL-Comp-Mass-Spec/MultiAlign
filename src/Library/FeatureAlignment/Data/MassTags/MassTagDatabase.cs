using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Algorithms.Options;
using FeatureAlignment.Data.MetaData;

namespace FeatureAlignment.Data.MassTags
{
    /// <summary>
    /// Contains all the information for mass tag databases.
    /// </summary>
    public class MassTagDatabase : IDataset
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public MassTagDatabase()
        {
            MassTags    = new List<MassTagLight>();
            AllProteins = new List<Protein>();
            Proteins    = new Dictionary<int, List<Protein>>();
            ProteinsToMassTags = new Dictionary<int, List<MassTagLight>>();
            Name        = "Unknown";
        }


        public MassTagDatabase(MassTagDatabase database, int requiredObservations):
            this()
        {
            Name                    = database.Name;
            var tags = database.MassTags.Where(x => x.ObservationCount >= requiredObservations).ToList();
            AddMassTagsAndProteins(tags, database.Proteins);
        }

        #region Properties

        /// <summary>
        /// Set the name of the mass tag database.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the list of available mass tags.
        /// </summary>
        public List<MassTagLight> MassTags
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the list of proteins that are mapped by mass tag id.
        /// </summary>
        public Dictionary<int, List<Protein>> Proteins
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets a list of all proteins.
        /// </summary>
        public List<Protein> AllProteins
        {
            get;
            set;
        }
        public Dictionary<int, List<MassTagLight>> ProteinsToMassTags
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets whether the database contains drift time or not.
        /// </summary>
        public bool DoesContainDriftTime
        {
            get;
            private set;
        }

        private void DetermineIfContainsDriftTime(List<MassTagLight> masstags)
        {
            DoesContainDriftTime = false;
            var featureTest = masstags.Find(delegate(MassTagLight x)
            {
                return x.DriftTime > 0;
            });

            if (featureTest != null)
            {
                DoesContainDriftTime = true;
            }
        }
        #endregion

        /// <summary>
        /// Loads the mass tags and populates the protein information.
        /// </summary>
        /// <param name="massTags"></param>
        /// <param name="massTagToProteinMap"></param>
        public void AddMassTagsAndProteins(List<MassTagLight>               massTags,
                                            Dictionary<int, List<Protein>>  massTagToProteinMap)
        {

            MassTags.AddRange(massTags);

            var massTagMap = new Dictionary<int, MassTagLight>();

            foreach (var tag in massTags)
            {
                if (massTagMap.ContainsKey(tag.Id) == false)
                {
                    massTagMap.Add(tag.Id, tag);
                }
            }

            ProteinsToMassTags                  = new Dictionary<int, List<MassTagLight>>();
            var proteinMap = new Dictionary<int, Protein>();

            AllProteins = new List<Protein>();


            foreach (var key in massTagToProteinMap.Keys)
            {
                var proteins  = massTagToProteinMap[key];

                if (massTagMap.ContainsKey(key) == false)
                    continue;

                var tag        = massTagMap[key];

                foreach (var p in proteins)
                {
                    if (!proteinMap.ContainsKey(p.RefId))
                    {
                        AllProteins.Add(p);
                        proteinMap.Add(p.RefId, p);
                    }

                    if (!ProteinsToMassTags.ContainsKey(p.ProteinId))
                    {
                        ProteinsToMassTags.Add(p.ProteinId, new List<MassTagLight>());
                    }
                    ProteinsToMassTags[p.ProteinId].Add(tag);
                }
            }

            Proteins                    = massTagToProteinMap;

            DetermineIfContainsDriftTime(massTags);
        }

        public void Filter(MassTagDatabaseOptions options)
        {
            var massTags = MassTags.Where(t => t.Net >= options.MinimumNet)
                .Where(t => t.Net <= options.MaximumNet)
                .Where(t => t.MassMonoisotopic >= options.MinimumMass)
                .Where(t => t.MassMonoisotopic <= options.MaximumMass).ToList();
            this.MassTags.Clear();
            this.AddMassTagsAndProteins(massTags, this.Proteins);
        }
    }
}
