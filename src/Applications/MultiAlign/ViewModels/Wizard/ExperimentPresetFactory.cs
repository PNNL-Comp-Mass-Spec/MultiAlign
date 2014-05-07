using System.Collections.Generic;
using MultiAlign.ViewModels.Instruments;

namespace MultiAlign.ViewModels.Wizard
{
    public class ExperimentPresetFactory
    {
        public static IEnumerable<ExperimentPresetViewModel> Create()
        {
            var presets = new List<ExperimentPresetViewModel>();
            presets.Add(Create(ExperimentPresets.Peptides));
            presets.Add(Create(ExperimentPresets.LipidsPositive));
            presets.Add(Create(ExperimentPresets.LipidsNegative));
            return presets;
        }

        public static ExperimentPresetViewModel Create(ExperimentPresets preset)
        {
            ExperimentPresetViewModel model = null;

            switch (preset)
            {
                case ExperimentPresets.LipidsPositive:
                    model = new ExperimentPresetViewModel("Small Molecules Positive Mode (Lipids, Metabolites)",
                                    2000,
                                    300,
                                    false,
                                    InstrumentPresetFactory.Create(InstrumentPresets.LtqOrbitrap));
                    break;
                case ExperimentPresets.LipidsNegative:
                    model = new ExperimentPresetViewModel("Small Molecules Negative Mode (Lipids, Metabolites)",
                                    2000,
                                    200,
                                    false,
                                    InstrumentPresetFactory.Create(InstrumentPresets.LtqOrbitrap));
                    break;
                case ExperimentPresets.Peptides:
                    model = new ExperimentPresetViewModel("Bottom Up Proteomics (Peptides)",
                                    10000,
                                    200,
                                    false,
                                    InstrumentPresetFactory.Create(InstrumentPresets.LtqOrbitrap));
                    break;                
            }
            return model;
        }
    }
}
