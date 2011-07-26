using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Algorithms.FeatureMatcher.MSnLinker;

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    /// Class that creates MSnLinker objects.
    /// </summary>
    public static class MSnLinkerFactory
    {
        public static IMSnLinker CreateLinker(MSnLinkerType create)
        {
            IMSnLinker linker = null;
            switch (create)
            {
                case MSnLinkerType.BoxMethod:
                    linker = new BoxMSnLinker();
                    break;
            }

            return linker;
        }
    }

}
