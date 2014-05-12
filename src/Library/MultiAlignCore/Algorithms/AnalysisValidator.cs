#region

using MultiAlignCore.Data;
using MultiAlignCore.IO;

#endregion

namespace MultiAlignCore.Algorithms
{
    public static class AnalysisValidator
    {
        /// <summary>
        ///     Validates the input options to make sure everything is set.
        /// </summary>
        /// <returns></returns>
        public static AnalysisType ValidateSetup(AnalysisConfig config)
        {
            var analysisType = AnalysisType.Full;

            var isExporting = (config.ExporterNames.CrossTabPath != null);
            isExporting = (isExporting || config.ExporterNames.ClusterScanPath != null);
            isExporting = (isExporting || config.ExporterNames.ClusterMSMSPath != null);
            isExporting = (isExporting || config.ExporterNames.CrossTabAbundance != null);

            // --------------------------------------------------------------------------------
            // Make sure that the analysis name and path are provided first.
            // --------------------------------------------------------------------------------
            if (config.AnalysisName == null)
            {
                Logger.PrintMessage("No analysis database name provided.");
                analysisType = AnalysisType.InvalidParameters;
                return analysisType;
            }

            if (config.AnalysisPath == null)
            {
                Logger.PrintMessage("No analysis path provided.");
                analysisType = AnalysisType.InvalidParameters;
                return analysisType;
            }

            // --------------------------------------------------------------------------------
            // If no input is provided. then they are using the database.
            // --------------------------------------------------------------------------------
            if (config.InputPaths == null)
            {
                Logger.PrintMessage("No input file provided.");
                analysisType = AnalysisType.ExportDataOnly;
            }
            if (config.ParameterFile == null)
            {
                Logger.PrintMessage("No parameter file specified.");
                analysisType = AnalysisType.ExportDataOnly;
            }

            // --------------------------------------------------------------------------------
            // Determine if they are exporting data only or not.
            // --------------------------------------------------------------------------------            
            if (analysisType == AnalysisType.ExportDataOnly)
            {
                // --------------------------------------------------------------------------------
                // Imports the factors into the database.
                // --------------------------------------------------------------------------------            
                if (config.ShouldUseFactors)
                {
                    Logger.PrintMessage("Importing factors only");
                    analysisType = AnalysisType.FactorImporting;
                }
                else if (!isExporting)
                {
                    Logger.PrintMessage("No export file names provided.");
                    analysisType = AnalysisType.InvalidParameters;
                    return analysisType;
                }
            }

            return analysisType;
        }
    }
}