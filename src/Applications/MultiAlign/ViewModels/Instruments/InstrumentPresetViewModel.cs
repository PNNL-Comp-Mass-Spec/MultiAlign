
namespace MultiAlign.ViewModels.Instruments
{
    public class InstrumentPresetViewModel
    {
        public InstrumentPresetViewModel(
                                        string name,
                                        double mass,
                                        double lc,
                                        double mobility,
                                        double fragmentWindow,
                                        double precursorMass)
        {
            Mass                    = mass;
            NetTolerance            = lc;
            DriftTimeTolerance      = mobility;
            FragmentWindowSize      = fragmentWindow;
            PrecursorMassResolution = precursorMass;
            Name                    = name;
        }

        public double Mass { get; private set; }
        public double NetTolerance { get; private set; }
        public double DriftTimeTolerance { get; private set; }
        public double FragmentWindowSize { get; private set; }
        public double PrecursorMassResolution { get; private set; }
        public string Name { get; private set; }
        public bool IsIonMobility { get; private set; }
    }
}
