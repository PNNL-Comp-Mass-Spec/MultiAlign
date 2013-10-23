using System.Collections.Generic;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlignCore.IO.MTDB
{
    public interface IDatabaseServerLoader
    {
        ICollection<InputDatabase> LoadDatabases();
    }
}
