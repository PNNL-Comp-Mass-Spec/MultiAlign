using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.IO.Parameters;
using MultiAlignCore.Algorithms.FeatureFinding;
using System.ComponentModel;

namespace MultiAlignCore.Algorithms.Features
{    
    public class FeatureConsolidatorOptions
    {
        public FeatureConsolidatorOptions()
        {
            AbundanceType = AbundanceReportingType.Sum;
        }
        /// <summary>
        /// 
        /// </summary>
        [ParameterFileAttribute("AbundanceType", "FeatureConsolidator")]
        [Description("Determines how the abundance from features of the same dataset in a cluster will be reported.  Either by summing their abundance, or by taking the maximum")]
        [Category("Abundance")]
        public AbundanceReportingType AbundanceType
        {
            get;
            set;
        }
    }
}
