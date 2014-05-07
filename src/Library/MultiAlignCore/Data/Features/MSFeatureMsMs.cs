namespace MultiAlignCore.Data.Features
{
    public class MSFeatureMsMs
    {

        public int FeatureID{get;set;}                     
        public int FeatureGroupID{get;set;}                
        public double PrecursorMZ{get;set;}            
        public double Mz{get;set;}                     
        public int FeatureScan{get;set;}                   
        public int MsMsScan{get;set;}                   
        public double MassMonoisotopicAligned{get;set;}
        public int ChargeState{get;set;}            
        public string Sequence{get;set;}
        public string PeptideSequence { get; set; }        
    }
}
