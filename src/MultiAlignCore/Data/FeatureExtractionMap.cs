using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.Data
{

    /// <summary>
    /// Class that holds data about the features that were extracted for exporting and inclusion list generation.
    /// </summary>
    public class FeatureExtractionMap
    {
        public int      DatasetID {get;set;}

        public int      MSnDatasetID {get;set;}
        public int      MSnFeatureId{get;set;}
        public double   MSnPrecursorMz{get;set;}
        public int      MSnScan { get; set; }
        public double   MSnRetentionTime { get; set; }

        public int      MSFeatureId{get;set;}
        public double   MSMz {get;set;}
        public int      MSScan { get; set; }
        public int      MSCharge { get; set; }
        public long     MSIntensity {get ; set;}
        
        public int      LCMSFeatureID {get;set;}
        public double   LCMSNet { get; set; }
        public int      LCMSScan { get; set; }
        public double   LCMSMass { get; set; }
        public double   LCMSAbundance {get;set;}
    }
}
