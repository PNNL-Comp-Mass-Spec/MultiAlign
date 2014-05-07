using System;
using System.IO;
using System.Text;

namespace MultiAlignCore.IO
{
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
        private static void OnMessage(string message, long size, DateTime time)
        {
            if (Status != null)
            {
                Status(null, new StatusEventArgs(message, size, time));
            }
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        public static void PrintSpacer()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < 80; i++)
            {
                builder.Append("-");
            }
            PrintMessage(builder.ToString(), false);
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        public static void PrintMessage(string message)
        {
            PrintMessage(message, true);
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        public static void PrintMessage(string message, bool useMemory)
        {
            lock (Synched)
            {
                SynchMessaged(message, useMemory);
            }
        }
        public static void PrintMessageWorker(string message, int size, bool useMemory)
        {
            lock (Synched)
            {
                SynchMessaged(message, useMemory);
            }
        }

        private static readonly object Synched = new object();

        private static void SynchMessaged(string message, bool useMemory)
        {
            var newMessage  = message;
            var size        = ApplicationUtility.GetMemory();
            var time        = DateTime.Now;

            if (LogPath != null)
            {
                if (useMemory)
                    File.AppendAllText(LogPath, time + " - " + size + " MB - " + newMessage + Environment.NewLine);
                else
                    File.AppendAllText(LogPath, time + " - " + newMessage + Environment.NewLine);                                    
            }
            OnMessage(newMessage, size, DateTime.Now);
            Console.WriteLine(newMessage);
        }
        /// <summary>
        /// Prints the version of MA to the log file.
        /// </summary>
        public static void PrintVersion()
        {
            PrintMessage("[Version Info]");
            var assemblyData = ApplicationUtility.GetAssemblyData();
            PrintMessage("\t" + assemblyData);

            var myDomain = AppDomain.CurrentDomain;
            var assembliesLoaded = myDomain.GetAssemblies();

            PrintMessage("\tLoaded Assemblies");
            foreach (var subAssembly in assembliesLoaded)
            {
                var subName = subAssembly.GetName();
                PrintMessage(string.Format("\t\t{0} - version {1}",
                                                                subName,
                                                                subName.Version));
            }

            PrintMessage("[System Information]");
            var systemData = ApplicationUtility.GetSystemData();
            PrintMessage("\t" + systemData);
            PrintMessage("[LogStart]");
        }
    }
}
