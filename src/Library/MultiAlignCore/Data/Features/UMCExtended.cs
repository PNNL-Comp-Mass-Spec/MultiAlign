
namespace MultiAlignCore.Data.Features
{
    /// <summary>
    /// Extends the UMCLight class to include some extra fields tracked in a UMC csv file
    /// </summary>
    /// <remarks>
    /// This is used by STACConsole when processing VIPER results
    /// </remarks>
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
