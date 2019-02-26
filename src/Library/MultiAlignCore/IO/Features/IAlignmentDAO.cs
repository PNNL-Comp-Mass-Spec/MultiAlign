#region

using FeatureAlignment.Data.Alignment;

#endregion

namespace MultiAlignCore.IO.Features
{
    public interface IAlignmentDAO : IGenericDAO<AlignmentData>
    {
        void ClearAll();
    }
}