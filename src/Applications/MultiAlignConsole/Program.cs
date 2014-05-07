using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Reports;
using PNNLOmics.Annotations;
using System;
using System.Runtime.InteropServices;

namespace MultiAlignConsole
{


    /// <summary>
    /// Main application.
    /// </summary>
    [UsedImplicitly]
    class Program
    {

        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="dwMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        private static readonly IAnalysisReportGenerator  m_reportCreator; 
        private static readonly AnalysisConfig            m_config;

        /// <summary>
        /// Default constructor.
        /// </summary>
        static Program()
        {
            m_config        = new AnalysisConfig();
            m_reportCreator = new AnalysisReportGenerator();
            m_reportCreator.Config = m_config;
        }        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static int Main(string[] args)
        {
            var handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);

            try
            {
                CommandLineProcessor.ProcessCommandLineArguments(args, m_config);

                var controller = new AnalysisController();
                var result = controller.StartMultiAlign(m_config, m_reportCreator);
                if (result != 0)
                {
                    Logger.PrintMessage("");
                    Logger.PrintMessage("ANALYSIS FAILED");
                }
                else
                {
                    Logger.PrintMessage("");
                    Logger.PrintMessage("ANALYSIS SUCCESS");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.PrintMessage("Unhandled Error: " + ex.Message);
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.PrintMessage("Inner Exception: " + innerEx.Message);
                    innerEx = innerEx.InnerException;
                }
                Logger.PrintMessage("Stack: " + ex.StackTrace);
                Logger.PrintMessage("");
                Logger.PrintMessage("ANALYSIS FAILED");
                return 1;
            }
        }
    }
}