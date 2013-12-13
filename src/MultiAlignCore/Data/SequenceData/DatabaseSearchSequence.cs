using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace MultiAlignCore.Data.SequenceData
{
    /// <summary>
    /// Database Search Result 
    /// </summary>
    public class DatabaseSearchSequence
    {

        public DatabaseSearchSequence()
        {
        }

        public DatabaseSearchSequence(Peptide peptide, int featureId)
        {
            Sequence     = peptide.Sequence;
            Scan         = peptide.Scan;
            Score        = peptide.Score;
            GroupId      = peptide.GroupId;               
            UmcFeatureId = featureId;
            Id           = peptide.ID;
        }

        public string   Sequence    { get; set; }
        public int      Scan        { get; set; }
        public double   Score       { get; set; }        
        public int      Id          { get; set; }
        public int GroupId          { get; set; }
        public double Mz { get; set; }
        public double MassMonoisotopic { get; set; }


        /// <summary>
        /// Gets or sets the parent feature this is associated with.
        /// </summary>
        public int UmcFeatureId { get; set; }
        
        public override bool Equals(object obj)
        {
            DatabaseSearchSequence other = (DatabaseSearchSequence)obj;

            if (other == null)
            {
                return false;
            }
            else if (!this.Id.Equals(other.Id))
            {
                return false;
            }
            else if (!this.GroupId.Equals(other.GroupId))
            {
                return false;
            }
            else if (!this.Sequence.Equals(other.Sequence))
            {
                return false;
            }
            else if (!this.Scan.Equals(other.Scan))
            {
                return false;
            }            
            else if (!this.Score.Equals(other.Score))
            {
                return false;
            }                          
            else
            {
                return this.Score.Equals(other.Score);
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + this.Sequence.GetHashCode();
            hash = hash * 23 + this.Scan.GetHashCode();
            hash = hash * 23 + this.Score.GetHashCode();
            hash = hash * 23 + this.Id.GetHashCode();
            hash = hash * 23 + this.GroupId.GetHashCode();            
            return hash;
        }
    }
}
