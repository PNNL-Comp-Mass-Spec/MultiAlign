using System;
using System.Collections.Generic;
namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Creates an Xic file from the features provided.
    /// </summary>
    public interface IXicWriter
    {
        void WriteXics(string path, List<UMCLight> features);
    }
}
