namespace MultiAlignCore.Data.MassTags
{
    /// <summary>
    /// Class that holds information about a protein.
    /// </summary>
    public class Protein: Molecule
    {

        public Protein()
        {
            RefId = -1;
            Sequence = "";
            CleavageState = CleavageState.NonSpecific;            
        }

        /// <summary>
        /// Gets or sets the protein sequence of amino acids.
        /// </summary>
        public string Sequence
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the reference ID
        /// </summary>
        public int RefId
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the protein ID
        /// </summary>
        public int ProteinId
        {
            get;
            set;
        }
        public string ProteinDescription
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cleavage state of the protein.
        /// </summary>
        public CleavageState CleavageState { get; set; }

        /// <summary>
        /// Gets or sets the position of the ending residue of a cleaved protein.
        /// </summary>
        public int ResidueEndPosition { get; set; }
        /// <summary>
        /// Gets or sets the position of the starting residue of a cleaved protein.
        /// </summary>
        public int ResidueStartPosition { get; set; }
        
    }
}
