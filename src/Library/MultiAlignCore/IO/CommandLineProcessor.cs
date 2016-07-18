#region

using System;
using MultiAlignCore.Data;
using MultiAlignCore.IO.InputFiles;

#endregion

namespace MultiAlignCore.IO
{
    public class CommandLineProcessor
    {
        /// <summary>
        ///     Processes the command line arguments.
        /// </summary>
        /// <param name="args"></param>
        public static void ProcessCommandLineArguments(string[] args, AnalysisConfig config)
        {
            var jobID = -1;
            var worked = false;
            config.options = CommandLineParser.ProcessArgs(args, 0);
            foreach (var option in config.options.Keys)
            {
                try
                {
                    var values = config.options[option];
                    switch (option)
                    {
                        case "-job":
                            worked = int.TryParse(values[0], out jobID);
                            if (worked)
                            {
                                config.JobID = jobID;
                            }
                            else
                            {
                                Logger.PrintMessage(string.Format("The job Id {0} was not understood", values[0]));
                                return;
                            }
                            break;
                        case "-charge":
                            config.ChargeState = Convert.ToInt32(values[0]);
                            config.ShouldClusterOnlyCharge = true;
                            break;
                            //--------------------------------------------------------------------
                            //  Path and name
                            //--------------------------------------------------------------------
                        case "-path":
                            config.AnalysisPath = values[0];
                            break;
                        case "-name":
                            config.AnalysisName = values[0];
                            break;
                            //--------------------------------------------------------------------
                            //  Files and parameters
                            //--------------------------------------------------------------------
                        case "-files":
                            config.InputPaths = values[0];
                            break;
                        case "-params":
                            config.ParameterFile = values[0];
                            break;
                        case "-usefactors":
                            config.ShouldUseFactors = true;
                            break;
                            //--------------------------------------------------------------------
                            //  Log, HTML names
                            //--------------------------------------------------------------------
                        case "-log":
                            config.logPath = values[0];
                            break;
                        case "-html":
                            config.HtmlPathName = values[0];
                            break;
                            //--------------------------------------------------------------------
                            //  Print helps
                            //--------------------------------------------------------------------
                        case "-h":
                            config.showHelp = true;
                            break;
                        case "-help":
                            config.showHelp = true;
                            break;
                            //--------------------------------------------------------------------
                            //  Feature databases
                            //--------------------------------------------------------------------
                        case "-useexistingdatabase":
                            config.ShouldUseExistingDatabase = true;
                            break;
                        case "-buildfeaturedatabase":
                            config.ShouldCreateFeatureDatabaseOnly = true;
                            break;
                            //--------------------------------------------------------------------
                            //  Data exporting
                            //--------------------------------------------------------------------
                        case "-exportmsms":
                            config.ExporterNames.ClusterMSMSPath = values[0];
                            config.ShouldExportMSMS = true;
                            break;
                        case "-exportsics":
                            config.ShouldExportSICs = true;
                            break;
                        case "-exportcrosstab":
                            config.ExporterNames.CrossTabPath = values[0];
                            break;
                        case "-exportabundances":
                            config.ExporterNames.CrossTabAbundance = values[0];
                            break;
                            //--------------------------------------------------------------------
                            // Exporting
                            //--------------------------------------------------------------------
                        case "-export":
                            config.ExporterNames.ClusterScanPath = values[0];
                            break;
                        case "-noplots":
                            config.ShouldCreateChargeStatePlots = false;
                            config.ShouldCreatePlots = false;
                            break;
                        default:
                            Logger.PrintMessage("One option was not understood: " + option);
                            break;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    Logger.PrintMessage(string.Format("You did not provide enough information for the option {0}",
                        option));
                    return;
                }
            }
        }
    }
}