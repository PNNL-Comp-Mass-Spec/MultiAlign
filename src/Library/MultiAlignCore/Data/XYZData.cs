
namespace MultiAlignCore.Data
{

    public class XYZData : XYData
    {
        public XYZData(double x, double y, double z) :
            base(x, y)
        {
            Z = z;
        }
        public double Z
        {
            get;
            set;
        }
    }
}
