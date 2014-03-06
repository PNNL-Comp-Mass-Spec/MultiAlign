using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlign.ViewModels.Wizard;

namespace MultiAlign.ViewModels
{
    public class InstrumentPresetFactory
    {
        public static IEnumerable<InstrumentPresetViewModel> Create()
        {
            var presets = new List<InstrumentPresetViewModel>
            {
                Create(InstrumentPresets.TOF),
                Create(InstrumentPresets.Velos),
                Create(InstrumentPresets.LTQ_Orbitrap)
            };

            return presets;
        }
        public static InstrumentPresetViewModel Create(InstrumentPresets preset)
        {
            InstrumentPresetViewModel model = null;
            switch (preset)
            {                
                case InstrumentPresets.TOF:
                    model = new InstrumentPresetViewModel("TOF",
                                                          false,
                                                          12,
                                                          .03,
                                                          50,
                                                          .5,
                                                          8);

                    break;
                case InstrumentPresets.Velos:
                    model = new InstrumentPresetViewModel("Velos",
                                                          false,
                                                          6,
                                                          .03,
                                                          50,
                                                          .5,
                                                          8);
                    break;
                case InstrumentPresets.LTQ_Orbitrap:
                    model = new InstrumentPresetViewModel("LTQ Orbitrap",
                                                          false,
                                                          8,
                                                          .03,
                                                          50,
                                                          .5,
                                                          8);
                    break;
                default:
                    break;
            }

            return model;
        }
    }
}
