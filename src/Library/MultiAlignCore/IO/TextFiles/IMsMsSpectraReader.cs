using System.Collections.Generic;
using FeatureAlignment.Data;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.TextFiles
{
    public interface IMsMsSpectraReader
    {
        List<MSSpectra> Read(string path);
    }
}
