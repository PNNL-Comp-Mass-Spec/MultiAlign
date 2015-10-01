
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Data.MassTags
{
    public class MassTagLight: FeatureLight
    {
        //TODO: [gord] agree on this... 

        public int CleavageState
        {
            get;
            set;
        }
        public double MsgfSpecProbMax
        {
            get;
            set;
        }
        public int ModificationCount
        {
            get;
            set;
        }
        public string Modifications
        {
            get;set;
        }

        public string ProteinName
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the peptide sequence. Breaking the model!
        /// </summary>
        public string PeptideSequence
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the peptide sequence. Breaking the model!
        /// </summary>
        public string PeptideSequenceEx
        {
            get;
            set;
        }

        public int ConformationId { get; set; }

        public double NetAverage { get; set; }

        public double NetPredicted { get; set; }

        public double NetStandardDeviation { get; set; }

        public double XCorr { get; set; }

        /// <summary>
        /// Gets or sets the discriminant score.  ???
        /// </summary>
        public double DiscriminantMax { get; set; }

        public double DriftTimePredicted { get; set; }

        /// <summary>
        /// Gets or sets the prior probability value.  This was previously
        /// peptide prophet probability, or EPIC.
        /// </summary>
        public double PriorProbability { get; set; }

        public int ObservationCount { get; set; }

        public int ConformationObservationCount { get; set; }
        public int QualityScore { get; set; }
        public Molecule Molecule { get; set; }
        
    }
}
