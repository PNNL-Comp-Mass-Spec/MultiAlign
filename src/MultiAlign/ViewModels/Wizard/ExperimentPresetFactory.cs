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
            List<ExperimentPresetViewModel> presets = new List<ExperimentPresetViewModel>();
            presets.Add(Create(ExperimentPresets.BottomUpAmt));
            presets.Add(Create(ExperimentPresets.BottomUpLcMs));
            presets.Add(Create(ExperimentPresets.BottomUpLcMsMs));
            presets.Add(Create(ExperimentPresets.BottomUpLcMsMsAmt));
            presets.Add(Create(ExperimentPresets.BottomUpLcImsMs));
            presets.Add(Create(ExperimentPresets.BottomUpLcImsMsAmt));
            presets.Add(Create(ExperimentPresets.LipidsNegativeLcMs));
            presets.Add(Create(ExperimentPresets.LipidsPositiveLcMs));
            presets.Add(Create(ExperimentPresets.LipidsNegativeLcMSMs));
            presets.Add(Create(ExperimentPresets.LipidsPositiveLcMSMs));
            presets.Add(Create(ExperimentPresets.IonMobilityLipids));
            return presets;
        }

        public static ExperimentPresetViewModel Create(ExperimentPresets preset)
        {
            ExperimentPresetViewModel model = null;

            switch (preset)
            {
                case ExperimentPresets.LipidsNegativeLcMs:
                    model = new ExperimentPresetViewModel("Lipidomics Negative Mode LC-MS",
                        4000,
                        200,
                        false,
                        InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;
                case ExperimentPresets.LipidsPositiveLcMs:
                    model = new ExperimentPresetViewModel("Lipidomics Positive Mode LC-MS",
                        4000,
                        300,
                        false,
                        InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;
                case ExperimentPresets.LipidsNegativeLcMSMs:
                    model = new ExperimentPresetViewModel("Lipidomics Negative LC-MS/MS",
                        4000,
                        300,
                        true,
                        InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;
                case ExperimentPresets.LipidsPositiveLcMSMs:
                    model = new ExperimentPresetViewModel("Lipidomics Positive LC-MS/MS",
                        4000,
                        200,
                        true,
                        InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;
                case ExperimentPresets.IonMobilityLipids:
                    model = new ExperimentPresetViewModel("Lipidomics LC-IMS-MS",
                        1000,
                        0,
                        false,
                        InstrumentPresetFactory.Create(InstrumentPresets.TOF));
                    break;
                case ExperimentPresets.BottomUpAmt:
                    model = new ExperimentPresetViewModel("Bottom Up AMT",
                        10000,
                        0,
                        false,
                        InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;
                case ExperimentPresets.BottomUpLcMs:
                    model = new ExperimentPresetViewModel("Bottom Up LC-MS",
                        10000,
                        0,
                        false,
                        InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;
                case ExperimentPresets.BottomUpLcMsMs:
                    model = new ExperimentPresetViewModel("Bottom Up LC-MS/MS",
                        10000,
                        0,
                        true,
                        InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;
                case ExperimentPresets.BottomUpLcMsMsAmt:
                    model = new ExperimentPresetViewModel("Bottom Up LC-MS/MS with AMT",
                        10000,
                        0,
                        true,
                        InstrumentPresetFactory.Create(InstrumentPresets.LTQ_Orbitrap));
                    break;
                case ExperimentPresets.BottomUpLcImsMs:
                    model = new ExperimentPresetViewModel("Bottom Up IMS",
                        10000,
                        0,
                        false,
                        InstrumentPresetFactory.Create(InstrumentPresets.TOF));
                    break;
                case ExperimentPresets.BottomUpLcImsMsAmt:
                    model = new ExperimentPresetViewModel("Bottom Up IMS AMT",
                        10000,
                        0,
                        false,
                        InstrumentPresetFactory.Create(InstrumentPresets.TOF));
                    break;
                default:
                    break;
            }

            return model;
        }
    }
}
