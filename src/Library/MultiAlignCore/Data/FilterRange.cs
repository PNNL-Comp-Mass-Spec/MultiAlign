namespace MultiAlignCore.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class FilterRange
    {
        public FilterRange() : 
            this(0, 10000)
        {
        }

        public FilterRange(double lower, double upper)
        {
            Minimum = lower;
            Maximum = upper;
        }

        public double Minimum { get; set; }
        public double Maximum { get; set; }

        /// <summary>
        /// For NHibernate; set when the class is persisted or read
        /// </summary>
        public int Id { get; set; }     
    }
}