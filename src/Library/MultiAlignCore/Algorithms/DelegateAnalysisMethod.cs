#region

using MultiAlignCore.Data;

#endregion

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    ///     Next analysis method step to run.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public delegate void DelegateAnalysisMethod(AnalysisConfig config);
}