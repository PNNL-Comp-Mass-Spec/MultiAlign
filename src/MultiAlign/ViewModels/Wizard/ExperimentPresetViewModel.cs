using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlign.ViewModels.Wizard
{
    public class ExperimentPresetViewModel
    {
        public ExperimentPresetViewModel(
                                        string name,
                                        double massRangeHigh,
                                        double massRangeLow,       
                                        bool hasMsMs,                                 
                                        InstrumentPresetViewModel preset)
        {
            MassRangeHigh       = massRangeHigh;
            MassRangeLow        = massRangeLow;
            Name                = name;
            InstrumentPreset    = preset;
            HasMsMs             = hasMsMs;
        }
        public bool HasMsMs { get; private set; }
        public double MassRangeHigh { get; private set; }
        public double MassRangeLow { get; private set; }        
        public string Name { get; private set; }        
        public InstrumentPresetViewModel InstrumentPreset
        { get; private set; }
    }

    public enum ExperimentPresets
    {
        LipidsNegativeLcMs,
        LipidsPositiveLcMs,
        LipidsNegativeLcMSMs,
        LipidsPositiveLcMSMs,        
        IonMobilityLipids,
        BottomUpAmt,
        BottomUpLcMs,
        BottomUpLcMsMs,
        BottomUpLcMsMsAmt,
        BottomUpLcImsMs,
        BottomUpLcImsMsAmt
    }
}
