namespace MultiAlignCore.Data.Alignment
{
    public class AlignmentDataPoint
    {
        public int GroupId { get; set; }
        public int ScanNumber { get; set; }
        public double RetentionTime { get; set; }
        public double NET { get; set; }
        public double AlignedNET { get; set; }

        /// <summary>
        /// If this and obj should be treated as equal (by id)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as AlignmentDataPoint;
            if (other == null)
                return false;
            if (this.GroupId != other.GroupId)
                return false;
            if (this.ScanNumber != other.ScanNumber)
                return false;

            return true;
        }

        /// <summary>
        ///     Calculates a hash code for the data point
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + GroupId.GetHashCode();
            hash = hash * 23 + ScanNumber.GetHashCode();
            return hash;
        }
    }
}
