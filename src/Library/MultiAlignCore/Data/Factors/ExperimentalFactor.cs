#region

using System;

#endregion

namespace MultiAlignCore.Data.Factors
{
    [Serializable]
    public class ExperimentalFactor
    {
        public int FactorID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }


        public override bool Equals(object obj)
        {
            var map = obj as ExperimentalFactor;
            if (map == null)
                return false;

            return map.Value == Value && map.FactorID == FactorID && map.Value == Value;
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash*23 + FactorID.GetHashCode();
            hash = hash*23 + Name.GetHashCode();
            hash = hash*23 + Value.GetHashCode();

            return hash;
        }
    }
}