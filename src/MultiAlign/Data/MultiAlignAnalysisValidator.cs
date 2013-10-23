using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.IO.MTDB;
using MultiAlignCore.Data;
using System.IO;
using MultiAlign.Data.States;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.Data.MassTags;

namespace MultiAlign.Data
{
    public class MultiAlignAnalysisValidator
    {
        /// <summary>
        /// Determines if all of the data is valid yet for the analysis.
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static bool IsStepValid(AnalysisConfig config, AnalysisSetupStep step, ref string errorMessage)
        {
            MultiAlignAnalysis analysis = config.Analysis;
            bool isStepValid            = true;

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
                default:
                    break;
            }

            return isStepValid;
        }

        private static bool ValidateNames(AnalysisConfig config, ref string errorMessage)
        {
            bool isStepValid = true;
            if (config.AnalysisPath == null)
            {
                errorMessage = "An output folder location needs to be supplied.";
                isStepValid = false;
            }
            else
            {
                char [] chars = Path.GetInvalidPathChars();
                string name   = config.AnalysisPath; 
                foreach (char c in chars)
                {
                    if (name.Contains(c))
                    {
                        isStepValid  = false;
                        errorMessage = "The path you provided has invalid characters.";
                        return isStepValid;
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
            
            bool isStepValid = true;

            if (analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB)
            {
                InputDatabase database = analysis.MetaData.Database;
                string databasePath = analysis.MetaData.Database.DatabaseName;

                switch (database.DatabaseFormat)
                {
                    case MassTagDatabaseFormat.None:
                        errorMessage = "No database was selected.";
                        isStepValid = false;
                        break;
                    case MassTagDatabaseFormat.SQL:
                        if (database.DatabaseName == null || database.DatabaseServer == null)
                        {
                            isStepValid = false;
                            errorMessage = "The database or server was not set.";
                        }
                        break;                    
                    case MassTagDatabaseFormat.APE:
                    case MassTagDatabaseFormat.Sqlite:
                        if (databasePath == null)
                        {
                            errorMessage = "No MTDB database file was selected.";
                            isStepValid = false;
                        }
                        else
                        {
                            if (!System.IO.File.Exists(databasePath))
                            {
                                errorMessage = "The database file provided does not exist.";
                                isStepValid = false;
                            }
                        }
                        break;
                    case MassTagDatabaseFormat.MetaSample:
                        errorMessage = "Invalid database type.";
                        isStepValid = false;
                        break;
                    default:
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
