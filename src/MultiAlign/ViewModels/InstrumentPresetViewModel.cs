using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlign.ViewModels.Wizard
{
    public class InstrumentPresetViewModel
    {
        public InstrumentPresetViewModel(
                                        string name,
                                        bool isIonMobility,
                                        double mass,
                                        double lc,
                                        double mobility,
                                        double fragmentWindow,
                                        double precursorMass)
        {
            Mass                    = mass;
            NETTolerance            = lc;
            DriftTimeTolerance      = mobility;
            FragmentWindowSize      = fragmentWindow;
            PrecursorMassResolution = precursorMass;
            Name                    = name;
            IsIonMobility           = isIonMobility;
        }

        public double Mass { get; private set; }
        public double NETTolerance { get; private set; }
        public double DriftTimeTolerance { get; private set; }
        public double FragmentWindowSize { get; private set; }
        public double PrecursorMassResolution { get; private set; }
        public string Name { get; private set; }
        public bool IsIonMobility { get; private set; }
    }

    public enum InstrumentPresets
    {
        TOF,
        IMS_TOF,
        Velos,
        LTQ_Orbitrap,
    }
}
