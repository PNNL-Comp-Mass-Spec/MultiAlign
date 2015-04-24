using PNNLOmics.Data.Features;

namespace MultiAlignCore.Data.Features
{
    /// <summary>
    /// Extends the UMCLight class to include some extra fields tracked in a UMC csv file (from VIPER perhaps)
    /// </summary>
    public class UMCExtended : UMCLight
    {
        public UMCExtended()
        {
            ChargeMax = 0;
            ChargeRepresentative = 0;
            MZForCharge = 0;
            DriftTimeUncorrected = 0;
        }    

        public int ChargeMax { get; set; }

        public int ChargeRepresentative { get; set; }

        public double MZForCharge { get; set; }
        
        public double DriftTimeUncorrected { get; set; }
    }
}
