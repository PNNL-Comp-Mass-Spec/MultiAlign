using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Utilities;

namespace MultiAlignCore
{
    /// <summary>
    /// Provides status messaegs.
    /// </summary>
    public interface IStatusProvider
    {
        event MessageEventHandler Status;
    }
}
