using System.Collections.Generic;
using PNNLOmics.Data;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Maps a protein entry in a database to the mass tag data in the AMT.
    /// </summary>
    public class ProteinToMassTags
    {
        /// <summary>
        /// Protein To Mass Tag Constructor.
        /// </summary>
        public ProteinToMassTags()
        {
            RelatedProteins = new List<Protein>();
            MassTags        = new List<MassTagToCluster>();
        }
        /// <summary>
        /// Protein to map.
        /// </summary>
        public Protein Protein
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the list of mass tags that map to this protein.
        /// </summary>
        public List<MassTagToCluster> MassTags
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the related protein list
        /// </summary>
        public List<Protein> RelatedProteins
        {
            get;
            set;
        }
    }
}
