using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.IO
{
    public interface IMageSink
    {
        void CommitChanges();        
    }
}
