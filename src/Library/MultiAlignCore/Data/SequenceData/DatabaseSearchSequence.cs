#region

using PNNLOmics.Data;

#endregion

namespace MultiAlignCore.Data.SequenceData
{
    /// <summary>
    ///     Database Search Result
    /// </summary>
    public class DatabaseSearchSequence
    {
        public DatabaseSearchSequence()
        {
        }

        public DatabaseSearchSequence(Peptide peptide, int featureId)
        {
            Sequence = peptide.Sequence;
            Scan = peptide.Scan;
            Score = peptide.Score;
            GroupId = peptide.GroupId;
            UmcFeatureId = featureId;
            Id = peptide.Id;
        }

        public string Sequence { get; set; }
        public int Scan { get; set; }
        public double Score { get; set; }
        public int Id { get; set; }
        public int GroupId { get; set; }
        public double Mz { get; set; }
        public double MassMonoisotopic { get; set; }


        /// <summary>
        ///     Gets or sets the parent feature this is associated with.
        /// </summary>
        public int UmcFeatureId { get; set; }

        public override bool Equals(object obj)
        {
            var other = (DatabaseSearchSequence) obj;

            if (other == null)
            {
                return false;
            }
            if (!Id.Equals(other.Id))
            {
                return false;
            }
            if (!GroupId.Equals(other.GroupId))
            {
                return false;
            }
            if (!Sequence.Equals(other.Sequence))
            {
                return false;
            }
            if (!Scan.Equals(other.Scan))
            {
                return false;
            }
            if (!Score.Equals(other.Score))
            {
                return false;
            }
            return Score.Equals(other.Score);
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*23 + Sequence.GetHashCode();
            hash = hash*23 + Scan.GetHashCode();
            hash = hash*23 + Score.GetHashCode();
            hash = hash*23 + Id.GetHashCode();
            hash = hash*23 + GroupId.GetHashCode();
            return hash;
        }
    }
}