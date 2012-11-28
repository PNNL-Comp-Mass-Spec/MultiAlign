using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace MultiAlignCore.IO
{
    public class StatusEventArgs: EventArgs
    {
        public StatusEventArgs(string message)
        {
            Message = message;
        }
        public string Message
        {
            get;
            private set;
        }
    }
    /// <summary>
    /// Class that handles logging messages to the console and log files.
    /// </summary>
    public static class Logger
    {
        public static event EventHandler<StatusEventArgs> Status;        

        /// <summary>
        /// File to log information to.
        /// </summary>
        public static string LogPath
        {
            get;
            set;
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        public static void PrintMessage(string message)
        {
            PrintMessage(message, true);
        }
        private static void OnMessage(string message)
        {
            if (Status != null)
            {
                Status(null, new StatusEventArgs(message));
            }
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        public static void PrintSpacer()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (int i = 0; i < 80; i++)
            {
                builder.Append("-");
            }
            PrintMessage(builder.ToString(), false);
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        public static void PrintMessage(string message, bool useMemory)
        {
            string newMessage = message;
            if (useMemory)
            {
                newMessage = DateTime.Now.ToString() + " - " + ApplicationUtility.GetMemory().ToString() + " MB - " + newMessage;
            }
            if (Logger.LogPath != null)
            {
                File.AppendAllText(Logger.LogPath, newMessage + Environment.NewLine);
            }
            OnMessage(newMessage);
            Console.WriteLine(newMessage);
        }
        /// <summary>
        /// Prints the version of MA to the log file.
        /// </summary>
        public static void PrintVersion()
        {
            PrintMessage("[Version Info]");
            string assemblyData = ApplicationUtility.GetAssemblyData();
            PrintMessage("\t" + assemblyData);

            AppDomain myDomain = AppDomain.CurrentDomain;
            Assembly[] assembliesLoaded = myDomain.GetAssemblies();

            PrintMessage("\tLoaded Assemblies");
            foreach (Assembly subAssembly in assembliesLoaded)
            {
                AssemblyName subName = subAssembly.GetName();
                PrintMessage(string.Format("\t\t{0} - version {1}",
                                                                subName,
                                                                subName.Version));
            }

            PrintMessage("[System Information]");
            string systemData = ApplicationUtility.GetSystemData();
            PrintMessage("\t" + systemData);
            PrintMessage("[LogStart]");
        }
    }
}
