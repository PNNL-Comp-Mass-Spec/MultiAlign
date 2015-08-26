using System.Collections.Generic;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.TextFiles
{
    public interface IMsMsSpectraReader
    {
        List<MSSpectra> Read(string path);
    }
}
