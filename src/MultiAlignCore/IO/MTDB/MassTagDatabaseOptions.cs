using MultiAlignEngine;
using MultiAlignCore.IO.Parameters;
using System.ComponentModel;
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
                OnlyLoadTagsWithDriftTime       = false;
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



        [Description("Experiment Filter")]
        [Category("Experiments")]
        [ParameterFileAttribute("ExperimentFilter", "MassTagDatabase")]
        public string ExperimentFilter
        {
            get;
            set;
        }

        [Description("Experiment Exlusion Filter")]
        [Category("Experiments")]
        [ParameterFileAttribute("ExperimentExclusionFilter", "MassTagDatabase")]
        public string ExperimentExclusionFilter
        {
            get;
            set;
        }

        [ParameterFileAttribute("OnlyLoadTagsWithDriftTime", "MassTagDatabase")]
        [Description("If True, only mass tags with drift time > 0 will be loaded.")]
        [Category("Ion Mobility")]
        public bool OnlyLoadTagsWithDriftTime { get; set; }     
   
        [Description("Minimum X-Correlation")]
        [Category("Scores")]
        [ParameterFileAttribute("MinimumXCorr","MassTagDatabase")]
        public double MinimumXCorr
        {
            get;
            set;
        }

        [Description("Minimum MS-MS Observations")]
        [Category("Scores")]
        [ParameterFileAttribute("MinimumObservationCountFilter","MassTagDatabase")]
        public int MinimumObservationCountFilter
        {
            get;
            set;
        }

        [Description("Minimum PMT Score")]
        [Category("Scores")]
        [ParameterFileAttribute("MinimumPMTScore","MassTagDatabase")]
        public double MinimumPMTScore 
        {
            get;
            set;
        }

        [Description("Minimum Discriminant")]
        [Category("Scores")]
        [ParameterFileAttribute("MinimumDiscriminant", "MassTagDatabase")]
        public double MinimumDiscriminant
        {
            get;
            set;
        }

        [Description("Prophet Value")]
        [Category("Scores")]
        [ParameterFileAttribute("PeptideProphetVal", "MassTagDatabase")]
        public double PeptideProphetVal
        {
            get;
            set;
        }

        [ParameterFileAttribute("ConfirmedTags", "MassTagDatabase")]
        [Description("Determines if only those tags that were confirmed are loaded.")]
        [Category("Tags")]
        public bool ConfirmedTags { get; set; }

        /// <summary>
        ///  0 to use GANET values, 1 to use PNET values    
        /// </summary>
        [Description("How NET will be used.  0 for global average NET (recommended), 1 for predicted NET")]
        [Category("Tags")]
        [ParameterFileAttribute("NETValType","MassTagDatabase")]
        public int NETValType 
        {
            get;
            set;
        }


        [Description("Database Type")]
        [Browsable(false)]
        public MassTagDatabaseType DatabaseType
        {
            get;
            set;
        }

        [Description("Database File Path")]
        [Browsable(false)]
        public string DatabaseFilePath
        {
            get;
            set;
        }

        [Description("Database Name")]
        [Browsable(false)]
        public string DatabaseName
        {
            get;
            set;
        }

        [Description("Server Name")]
        [Browsable(false)]
        public string Server
        {
            get;
            set;
        }

        [Browsable(false)]				
        public string UserID
        {
            get;
            set;
        }
        [Browsable(false)]
        public string Password
        {
            get;
            set;
        }
    }    
}
