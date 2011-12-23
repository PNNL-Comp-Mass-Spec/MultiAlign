using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.Data.SequenceData
{
    /// <summary>
    /// Database Search Result 
    /// </summary>
    public class DatabaseSearchSequence
    {

        public string   Sequence { get; set; }
        public int      Scan { get; set; }
        public string   Reference { get; set; }
        public double   Score {get;set;}
        public double   DeltaScore {get;set;}
        public double   DeltaScore2 {get;set;}
        public int      Rank {get;set;}
        public int      Charge {get;set;}
        public double   Mass{get;set;}
        public int      TrypticEnds {get;set;}
        public int ID        { get; set; }
        public int GroupID   { get; set; }

        public override bool Equals(object obj)
        {
            DatabaseSearchSequence other = (DatabaseSearchSequence)obj;

            if (other == null)
            {
                return false;
            }
            else if (!this.ID.Equals(other.ID))
            {
                return false;
            }
            else if (!this.GroupID.Equals(other.ID))
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
            else if (!this.DeltaScore.Equals(other.DeltaScore))
            {
                return false;
            }
            else if (!this.DeltaScore2.Equals(other.DeltaScore2))
            {
                return false;
            }
            else if (!this.Mass.Equals(other.Mass))
            {
                return false;
            }
            else if (!this.Charge.Equals(other.Charge))
            {
                return false;
            }
            else if (!this.Rank.Equals(other.Rank))
            {
                return false;
            }
            else if (!this.Reference.Equals(other.Reference))
            {
                return false;
            }
            else if (!this.Score.Equals(other.Score))
            {
                return false;
            }
            else if (!this.TrypticEnds.Equals(other.TrypticEnds))
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
            hash = hash * 23 + this.Reference.GetHashCode();
            hash = hash * 23 + this.Score.GetHashCode();
            hash = hash * 23 + this.DeltaScore.GetHashCode();
            hash = hash * 23 + this.DeltaScore2.GetHashCode();
            hash = hash * 23 + this.Rank.GetHashCode();
            hash = hash * 23 + this.Charge.GetHashCode();
            hash = hash * 23 + this.Mass.GetHashCode();
            hash = hash * 23 + this.TrypticEnds.GetHashCode();
            hash = hash * 23 + this.ID.GetHashCode();
            hash = hash * 23 + this.GroupID.GetHashCode();                                     
            return hash;
        }
    }
}
