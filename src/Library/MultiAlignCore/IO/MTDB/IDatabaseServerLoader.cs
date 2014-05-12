#region

using System.Collections.Generic;
using MultiAlignCore.IO.InputFiles;

#endregion

namespace MultiAlignCore.IO.MTDB
{
    public interface IDatabaseServerLoader
    {
        ICollection<InputDatabase> LoadDatabases();
    }
}