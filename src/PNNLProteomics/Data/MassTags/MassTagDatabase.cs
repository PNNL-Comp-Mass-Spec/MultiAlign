﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;   

namespace PNNLProteomics.Data.MassTags
{
    /// <summary>
    /// Contains all the information for mass tag databases.
    /// </summary>
    public class MassTagDatabase
    {
        /// <summary>
        /// Map between proteins and mass tags.
        /// </summary>
        private Dictionary<int, List<Protein>> m_proteinMassTagMap;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MassTagDatabase()
        {
            MassTags    = new List<MassTag>();
            AllProteins = new List<Protein>();
            Proteins    = new Dictionary<int, List<Protein>>();
        }

        #region Properties
        /// <summary>
        /// Gets the list of available mass tags.
        /// </summary>
        public List<MassTag> MassTags
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
            private set;
        }
        #endregion

        /// <summary>
        /// Loads the mass tags.
        /// </summary>
        /// <param name="massTags"></param>
        public void AddMassTagsAndProteins( List<MassTag>                   massTags,
                                            Dictionary<int, List<Protein>>  massTagToProteinMap)
        {            
            MassTags.AddRange(massTags);            
            foreach (int key in massTagToProteinMap.Keys)
            {
                List<Protein> proteins = massTagToProteinMap[key];
                AllProteins.AddRange(proteins);                
            }
            m_proteinMassTagMap = massTagToProteinMap;
        }
    }
}
