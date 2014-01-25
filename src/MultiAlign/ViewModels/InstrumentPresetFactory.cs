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
            List<InstrumentPresetViewModel> presets = new List<InstrumentPresetViewModel>();
            presets.Add(Create(InstrumentPresets.IMS_TOF));
            presets.Add(Create(InstrumentPresets.TOF));
            presets.Add(Create(InstrumentPresets.Velos));
            presets.Add(Create(InstrumentPresets.LTQ_Orbitrap));

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
                                                          8,
                                                          .03,
                                                          50,
                                                          .5,
                                                          8);

                    break;
                case InstrumentPresets.IMS_TOF:
                    model = new InstrumentPresetViewModel("IMS TOF",
                                                          true,
                                                          8,
                                                          .03,
                                                          .3,
                                                          .5,
                                                          8);
                    break;
                case InstrumentPresets.Velos:
                    model = new InstrumentPresetViewModel("Velos",
                                                          false,
                                                          8,
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
