using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.MassTags;
using PNNLOmics.Data;

namespace MultiAlignCore.Data.MassTags
{
    /// <summary>
    /// Contains all the information for mass tag databases.
    /// </summary>
    public class MassTagDatabase
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
            MassTagLight featureTest = masstags.Find(delegate(MassTagLight x)
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
        /// Loads the mass tags.
        /// </summary>
        /// <param name="massTags"></param>
        public void AddMassTagsAndProteins(List<MassTagLight>               massTags,
                                            Dictionary<int, List<Protein>>  massTagToProteinMap)
        {
            
            MassTags.AddRange(massTags);

            Dictionary<int, MassTagLight> massTagMap = new Dictionary<int, MassTagLight>();

            foreach (MassTagLight tag in massTags)
            {
                if (massTagMap.ContainsKey(tag.ID) == false)
                {
                    massTagMap.Add(tag.ID, tag);
                }
            }
            
            ProteinsToMassTags                  = new Dictionary<int, List<MassTagLight>>();
            Dictionary<int, Protein> proteinMap = new Dictionary<int, Protein>();

            AllProteins = new List<Protein>();


            foreach (int key in massTagToProteinMap.Keys)
            {
                List<Protein> proteins  = massTagToProteinMap[key];

                if (massTagMap.ContainsKey(key) == false)
                    continue;

                MassTagLight tag        = massTagMap[key];

                foreach (Protein p in proteins)
                {
                    if (!proteinMap.ContainsKey(p.RefID))
                    {
                        AllProteins.Add(p);
                        proteinMap.Add(p.RefID, p);                        
                    }

                    if (!ProteinsToMassTags.ContainsKey(p.ProteinID))
                    {
                        ProteinsToMassTags.Add(p.ProteinID, new List<MassTagLight>());
                    }
                    ProteinsToMassTags[p.ProteinID].Add(tag);
                }
            }

            Proteins                    = massTagToProteinMap;

            DetermineIfContainsDriftTime(massTags);
        }
    }
}
