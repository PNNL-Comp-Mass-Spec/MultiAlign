using MultiAlignEngine;
using MultiAlignCore.IO.Parameters;

using MultiAlignCore.Data;

namespace MultiAlignCore.IO.MTDB
{
    /// <summary>
    /// Class in progress.  Options for loading data to and from the MTDB.
    /// </summary>
    public class MassTagDatabaseOptions
    {

        public MassTagDatabaseOptions()
        {            
				MinimumObservationCountFilter   = 0;
				ConfirmedTags		            = false;
                UseDriftTime                    = false;
				MinimumXCorr			        = 0.0F; 
				MinimumPMTScore		            = 1; 
				NETValType			            = 0; //-- 0 to use GANET values, 1 to use PNET values
				MinimumDiscriminant		        = 0;
				PeptideProphetVal	            = 0.5; 
				Server				            = "albert"; 
				DatabaseName			        = "";
                DatabaseFilePath                = "";
				UserID				            = "mtuser"; 
				Password				        = "mt4fun"; 
				ExperimentExclusionFilter	    = ""; 
				ExperimentFilter			    = "";
				DatabaseType				    = MassTagDatabaseType.None;
        }

        [ParameterFileAttribute("ConfirmedTags", "MassTagDBOptions")]
        public bool ConfirmedTags { get; set; }
        [ParameterFileAttribute("UseDriftTime", "MassTagDBOptions")]
        public bool UseDriftTime { get; set; }        
        [DataSummaryAttribute("Minimum X-Correlation")]
        [ParameterFileAttribute("MinimumXCorr","MassTagDBOptions")]
        public double MinimumXCorr
        {
            get;
            set;
        }

        [DataSummaryAttribute("Minimum MS-MS Observations")]
        [ParameterFileAttribute("MinimumObservationCountFilter","MassTagDBOptions")]
        public int MinimumObservationCountFilter
        {
            get;
            set;
        }
				
        [DataSummaryAttribute("Minimum PMT Score")]
        [ParameterFileAttribute("MinimumPMTScore","MassTagDBOptions")]
        public double MinimumPMTScore 
        {
            get;
            set;
        }
				
        /// <summary>
        ///  0 to use GANET values, 1 to use PNET values    
        /// </summary>
        [DataSummaryAttribute("NET Value Type")]
        [ParameterFileAttribute("NETValType","MassTagDBOptions")]
        public int NETValType 
        {
            get;
            set;
        }
				
        [DataSummaryAttribute("Minimum Discriminant")]				
        [ParameterFileAttribute("MinimumDiscriminant","MassTagDBOptions")]
        public double MinimumDiscriminant
        {
            get;
            set;
        }
				
        [DataSummaryAttribute("Prophet Value")]
        [ParameterFileAttribute("PeptideProphetVal","MassTagDBOptions")]
        public double PeptideProphetVal
        {
            get;
            set;
        }
			
				
        [DataSummaryAttribute("Experiment Filter")]
        [ParameterFileAttribute("ExperimentFilter","MassTagDBOptions")]
        public string ExperimentFilter
        {
            get;
            set;
        }

        [DataSummaryAttribute("Experiment Exlusion Filter")]
        [ParameterFileAttribute("ExperimentExclusionFilter","MassTagDBOptions")]
        public string ExperimentExclusionFilter
        {
            get;
            set;
        }
								
        [DataSummaryAttribute("Database Type")]
        [ParameterFileAttribute("DatabaseType","DatabaseType")]
        public MassTagDatabaseType DatabaseType
        {
            get;
            set;
        }
				
        [DataSummaryAttribute("Database File Path")]
        public string DatabaseFilePath
        {
            get;
            set;
        }

        [DataSummaryAttribute("Database Name")]
        public string DatabaseName
        {
            get;
            set;
        }

        [DataSummaryAttribute("Server Name")]
        public string Server
        {
            get;
            set;
        }
				
        public string UserID
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }
    }    
}
