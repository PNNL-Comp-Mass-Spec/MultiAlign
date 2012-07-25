using MultiAlignCore.Data.Factors;
using MultiAlignCore.Data.Alignment;

namespace MultiAlignCore.IO.Features
{
	public interface IAlignmentDAO : IGenericDAO<classAlignmentData>
    {
        void ClearAll();
    }
}
