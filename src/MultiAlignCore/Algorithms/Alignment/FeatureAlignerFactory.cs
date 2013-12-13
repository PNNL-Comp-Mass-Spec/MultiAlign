using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.Algorithms.Alignment
{
    public class FeatureAlignerFactory
    {
        public static IFeatureAligner Create(FeatureAlignmentType type )
        {
            IFeatureAligner aligner = null;

            switch (type)
            {
                case FeatureAlignmentType.LCMSWarp:
                    aligner = new LCMSWarpFeatureAligner();
                    break;
                case FeatureAlignmentType.DirectImsInfusion:
                    aligner = new DummyAlignment();
                    break;
                default:
                    break;
            }

            return aligner;
        }
    }

    public enum FeatureAlignmentType
    {
        LCMSWarp,
        DirectImsInfusion
    }
}
