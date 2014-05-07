using System.IO;
using System.Linq;
using MultiAlign.Data.States;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;

namespace MultiAlign.Data
{
    public class MultiAlignAnalysisValidator
    {
        /// <summary>
        /// Determines if all of the data is valid yet for the analysis.
        /// </summary>        
        /// <returns></returns>
        public static bool IsStepValid(AnalysisConfig config, AnalysisSetupStep step, ref string errorMessage)
        {
            var analysis = config.Analysis;
            var isStepValid            = true;

            switch (step)
            {
                case AnalysisSetupStep.DatasetSelection:
                    if (analysis.MetaData.Datasets.Count < 1)
                    {
                        errorMessage = "Select datasets before continuing.";
                        isStepValid  = false;
                    }
                    break;
                case AnalysisSetupStep.BaselineSelection:
                    isStepValid = ValidateBaseline(analysis, ref errorMessage);
                    break;
                case AnalysisSetupStep.OptionsSelection:
                    break;
                case AnalysisSetupStep.Naming:
                    isStepValid = ValidateNames(config, ref errorMessage);
                    break;
                case AnalysisSetupStep.Started:
                    if (analysis.AnalysisType != MultiAlignCore.Algorithms.AnalysisType.Full)
                    {
                        isStepValid = false;
                    }
                    break;
            }

            return isStepValid;
        }

        private static bool ValidateNames(AnalysisConfig config, ref string errorMessage)
        {
            var isStepValid = true;
            if (config.AnalysisPath == null)
            {
                errorMessage = "An output folder location needs to be supplied.";
                isStepValid = false;
            }
            else
            {
                var chars  = Path.GetInvalidPathChars();
                var name   = config.AnalysisPath; 
                foreach (var c in chars)
                {
                    if (name.Contains(c))
                    {                        
                        errorMessage = "The path you provided has invalid characters.";
                        return false;
                    }
                }
                

                //if (!System.IO.Directory.Exists(config.AnalysisPath))
                //{
                //    errorMessage = "The root folder you specified does not exist or is invalid.";
                //    isStepValid = false;
                //}
                if (config.AnalysisName == null)
                {
                    errorMessage = "An analysis name needs to be supplied.";
                    isStepValid = false;
                }
            }
            return isStepValid;
        }

        private static bool ValidateBaseline(MultiAlignAnalysis analysis, ref string errorMessage)
        {
            
            var isStepValid = true;

            if (analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB)
            {
                var database = analysis.MetaData.Database;
                var databasePath = analysis.MetaData.Database.DatabaseName;

                switch (database.DatabaseFormat)
                {
                    case MassTagDatabaseFormat.None:
                        errorMessage = "No database was selected.";
                        isStepValid = false;
                        break;
                    case MassTagDatabaseFormat.MassTagSystemSql:
                        if (database.DatabaseName == null || database.DatabaseServer == null)
                        {
                            isStepValid = false;
                            errorMessage = "The database or server was not set.";
                        }
                        break; 
                    case MassTagDatabaseFormat.SkipAlignment:                    
                    case MassTagDatabaseFormat.Sqlite:
                        databasePath = analysis.MetaData.Database.LocalPath;
                        if (databasePath == null)
                        {
                            errorMessage = "No MTDB database file was selected.";
                            isStepValid = false;
                        }
                        else
                        {
                            if (!File.Exists(databasePath))
                            {
                                errorMessage = "The database file provided does not exist.";
                                isStepValid = false;
                            }
                        }
                        break;
                    case MassTagDatabaseFormat.DelimitedTextFile:
                        errorMessage = "Invalid database type.";
                        isStepValid = false;
                        break;                    
                }
            }
            else
            {
                if (analysis.MetaData.Datasets.Count < 2)
                {
                    isStepValid = false;
                    errorMessage = "You must align a single dataset to a database.";
                }
                else if (analysis.MetaData.BaselineDataset == null)
                {
                    errorMessage = "No baseline dataset was selected.";
                    isStepValid = false;
                }
            }
            return isStepValid;            
        }
    }
}
