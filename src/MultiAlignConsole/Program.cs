using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MultiAlignConsole.Drawing;
using MultiAlignConsole.IO;
using MultiAlignCore;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.MTDB;
using MultiAlignCore.IO.Parameters;
using PNNLOmics.Data.Features;

namespace MultiAlignConsole
{


    /// <summary>
    /// Main application.
    /// </summary>
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

        private static AnalysisReportGenerator  m_reportCreator; 
        private static AnalysisConfig           m_config;

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
            IntPtr handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);

            try
            {
                CommandLineProcessor.ProcessCommandLineArguments(args, m_config);

                AnalysisController controller = new AnalysisController();

                int result = controller.StartMultiAlign(m_config, m_reportCreator);
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
                Exception innerEx = ex.InnerException;
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
