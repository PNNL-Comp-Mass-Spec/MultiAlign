namespace MultiAlignCore.Algorithms
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
            Lower = lower;
            Upper = upper;
        }

        public double Lower { get; set; }
        public double Upper { get; set; }        
    }
}