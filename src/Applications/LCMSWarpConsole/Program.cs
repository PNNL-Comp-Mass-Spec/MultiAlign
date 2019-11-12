
namespace LCMSWarpConsole
{
    class Program
    {

        static void Main(string[] args)
        {


            string optionsFilePath;
            string lcmsFeaturesFilePath;
            string aligneePMTsFilePath;

            const bool NET_MASS_WARP = true;

            if (NET_MASS_WARP)
            {
                optionsFilePath = @"..\..\Data\QC_Shew_TEDDY_01_500ng_HCD_RR-end_12JUL16_NETAndMassAlignment\MassMatchCom_Options.txt";
                lcmsFeaturesFilePath = @"..\..\Data\QC_Shew_TEDDY_01_500ng_HCD_RR-end_12JUL16_NETAndMassAlignment\MassMatchCom_LCMSFeaturesFile.txt";
                aligneePMTsFilePath = @"..\..\Data\QC_Shew_TEDDY_01_500ng_HCD_RR-end_12JUL16_NETAndMassAlignment\MassMatchCom_PMTs.txt";

                optionsFilePath = @"..\..\Data\QC_Shew_16-01_2_26July16_Pippin_16-05-01_NETAndMassAlignment\MassMatchCom_Options.txt";
                lcmsFeaturesFilePath = @"..\..\Data\QC_Shew_16-01_2_26July16_Pippin_16-05-01_NETAndMassAlignment\MassMatchCom_LCMSFeaturesFile.txt";
                aligneePMTsFilePath = @"..\..\Data\QC_Shew_16-01_2_26July16_Pippin_16-05-01_NETAndMassAlignment\MassMatchCom_PMTs.txt";

            }
            else
            {
                optionsFilePath = @"..\..\Data\QC_Shew_TEDDY_01_500ng_HCD_RR-end_12JUL16_NETAlignment\MassMatchCom_Options.txt";
                lcmsFeaturesFilePath = @"..\..\Data\QC_Shew_TEDDY_01_500ng_HCD_RR-end_12JUL16_NETAlignment\MassMatchCom_LCMSFeaturesFile.txt";
                aligneePMTsFilePath = @"..\..\Data\QC_Shew_TEDDY_01_500ng_HCD_RR-end_12JUL16_NETAlignment\MassMatchCom_PMTs.txt";

                optionsFilePath = @"..\..\Data\QC_Shew_16-01_2_26July16_Pippin_16-05-01_NETAlignment\MassMatchCom_Options.txt";
                lcmsFeaturesFilePath = @"..\..\Data\QC_Shew_16-01_2_26July16_Pippin_16-05-01_NETAlignment\MassMatchCom_LCMSFeaturesFile.txt";
                aligneePMTsFilePath = @"..\..\Data\QC_Shew_16-01_2_26July16_Pippin_16-05-01_NETAlignment\MassMatchCom_PMTs.txt";

            }

            var processor = new clsLCMSWarpRunner();
            processor.AlignLCMSFeaturesToPMTs(optionsFilePath, lcmsFeaturesFilePath, aligneePMTsFilePath);
        }

    }
}
