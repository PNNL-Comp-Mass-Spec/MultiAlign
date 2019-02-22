#region

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

#endregion

namespace MultiAlignCore
{
    public static class ApplicationUtility
    {
        /// <summary>
        /// Calculates the current usage of current processes memory.
        /// </summary>
        /// <returns>Memory usage of current process.</returns>
        public static long GetMemory()
        {
            var process = Process.GetCurrentProcess();
            var memory = process.WorkingSet64;
            memory /= 1024;
            memory /= 1024;
            process.Dispose();
            return memory;
        }

        /// <summary>
        /// Retrieves the assembly information.
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyData()
        {
            // get the version object for this assembly
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName();
            var version = name.Version;

            var versionType = "32-bit";
            if (Environment.Is64BitProcess)
            {
                versionType = "64-bit";
            }

            var data = string.Format("{0} - version: {1} process type: {2}",
                name,
                version,
                versionType);

            return data;
        }

        /// <summary>
        /// Retrieves the assembly information.
        /// </summary>
        /// <returns></returns>
        public static string GetEntryAssemblyData()
        {
            // get the version object for this assembly
            var assembly = Assembly.GetEntryAssembly();
            var name = assembly.GetName();
            var version = name.Version;

            var versionType = "32-bit";
            if (Environment.Is64BitProcess)
            {
                versionType = "64-bit";
            }

            var data = string.Format("{0} - v{1} - {2}",
                name.Name,
                version,
                versionType);

            return data;
        }

        /// <summary>
        /// Retrieves the system data.
        /// </summary>
        /// <returns></returns>
        public static string GetSystemData()
        {
            var operationSystemType = "32-bit OS";
            if (Environment.Is64BitOperatingSystem)
            {
                operationSystemType = "64-bit OS";
            }


            var data =
                string.Format(
                    "OS Version: {0} Processor Count: {1} Operating System Type: {2}  Memory: {3}  Page Size: {4}",
                    Environment.OSVersion,
                    Environment.ProcessorCount,
                    operationSystemType,
                    Environment.WorkingSet,
                    Environment.SystemPageSize);

            return data;
        }

        /// <summary>
        /// Gets the application data folder for the application.
        /// </summary>
        /// <returns>Path to application data, null if path not applicable.</returns>
        public static string GetApplicationDataFolderPath(string applicationName)
        {
            string appDataFolderPath = null;
            appDataFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                applicationName);
            if (!Directory.Exists(appDataFolderPath))
            {
                Directory.CreateDirectory(appDataFolderPath);
            }
            return appDataFolderPath;
        }
    }
}