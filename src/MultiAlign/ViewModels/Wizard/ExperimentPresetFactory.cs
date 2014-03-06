using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlign.ViewModels.Wizard
{
    public class ExperimentPresetFactory
    {
        public static IEnumerable<ExperimentPresetViewModel> Create()
        {
            var presets = new List<ExperimentPresetViewModel>();            
            presets.Add(Create(ExperimentPresets.Lipids));
            presets.Add(Create(ExperimentPresets.Peptides));            
            return presets;
        }

        public static ExperimentPresetViewModel Create(ExperimentPresets preset)
        {
            ExperimentPresetViewModel model = null;

            switch (preset)
            {
                case ExperimentPresets.Lipids:
                    model = new ExperimentPresetViewModel("Small Molecules (Lipids, Metabolites)",
                                    4000,
                                    200,
                                    false,
                                    InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;
                case ExperimentPresets.Peptides:
                    model = new ExperimentPresetViewModel("Bottom Up Proteomics (Peptides)",
                                    10000,
                                    200,
                                    false,
                                    InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;                
            }
            return model;
        }
    }
}
