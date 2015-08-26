
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.Data
{
    public class Molecule
    {
        public Molecule()
        {
            Spectrum = null;
            MassTag  = null;
        }
        public int Id { get; set; }
        public int GroupId { get; set; }
        /// <summary>
        /// Gets or sets the scan the molecule was identified in.
        /// </summary>
        public int Scan
        {
            get;
            set;
        } 
        public string Description
        {
            get;
            set;
        }
        public int ChargeState
        {
            get;
            set;
        }
        public MassTagLight MassTag
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string ChemicalFormula
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the moleculare Weight
        /// </summary>
        public double MassMonoisotopic
        {
            get;
            set;
        }
        public double Mz
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the spectrum that identified the molecule.
        /// </summary>
        public MSSpectra Spectrum
        {
            get;
            set;
        }
    }
}
