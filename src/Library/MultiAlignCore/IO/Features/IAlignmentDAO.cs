#region

using MultiAlignCore.Data.Alignment;

#endregion

namespace MultiAlignCore.IO.Features
{
    public interface IAlignmentDAO : IGenericDAO<AlignmentData>
    {
        void ClearAll();
    }
}