using System.Collections.Generic;

namespace MultiAlignCore.Data.MassTags
{
    /// <summary>
    /// This class encapsulates peptide level information.
    /// </summary>
    public class Peptide: Molecule
    {
        public Peptide()
        {
           ProteinList = new List<Protein>();
           Sequence = "";
        }


        public List<Protein> ProteinList
        {
            get;
            set;
        }        
        public string Sequence
        {
            get; 
            set;
        }
        public double Score
        {
            get;
            set;
        }
       
        public string ExtendedSequence
        {
            get;
            set;
        }
        public int CleavageState
        {
            get;
            set;            
        }
        public double QualityScore
        {
            get;
            set;
        }
        public double NET
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the false discovery rate of this peptide
        /// </summary>
        public double Fdr { get; set; }

        public override string ToString()
        {
            return string.Format("{0}   Score: {1}  Mz: {2}  Scan: {3}", Sequence, Score, Mz, Scan);
        }
    }
}
