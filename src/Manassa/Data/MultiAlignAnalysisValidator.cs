using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.IO.MTDB;
using MultiAlignCore.Data;

namespace Manassa.Data
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
                    ValidateNames(config, ref errorMessage);
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
                if (!System.IO.Directory.Exists(config.AnalysisPath))
                {
                    errorMessage = "The root folder you specified does not exist or is invalid.";
                    isStepValid = false;
                }
                else if (config.AnalysisName == null)
                {
                    errorMessage = "An analysis name needs to be supplied.";
                    isStepValid = false;
                }
            }
            return isStepValid;
        }

        private static bool ValidateBaseline(MultiAlignAnalysis analysis, ref string errorMessage)
        {
            string databasePath = analysis.Options.MassTagDatabaseOptions.DatabaseFilePath;
            bool isStepValid = true;

            if (analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB)
            {
                switch (analysis.Options.MassTagDatabaseOptions.DatabaseType)
                {
                    case MassTagDatabaseType.None:
                        errorMessage = "No database was selected.";
                        isStepValid = false;
                        break;
                    case MassTagDatabaseType.SQL:
                        if (analysis.Options.MassTagDatabaseOptions.DatabaseName == null || analysis.Options.MassTagDatabaseOptions.Server == null)
                        {
                            isStepValid = false;
                            errorMessage = "The database or server was not set.";
                        }
                        break;
                    case MassTagDatabaseType.ACCESS:
                        errorMessage = "Invalid database type.";
                        isStepValid = false;
                        break;
                    case MassTagDatabaseType.SQLite:
                        if (databasePath == null)
                        {
                            errorMessage = "No SQLite MTDB database was selected.";
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
                    case MassTagDatabaseType.MetaSample:
                        errorMessage = "Invalid database type.";
                        isStepValid = false;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (analysis.MetaData.BaselineDataset == null)
                {
                    errorMessage = "No baseline dataset was selected.";
                    isStepValid = false;
                }
            }

            return isStepValid;            
        }
    }
}
