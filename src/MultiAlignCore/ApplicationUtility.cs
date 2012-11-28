using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using MultiAlignEngine;

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
            Process process = Process.GetCurrentProcess();
            long memory = process.WorkingSet64;
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
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName name = assembly.GetName();
            Version version = name.Version;

            string versionType = "32-bit";
            if (System.Environment.Is64BitProcess)
            {
                versionType = "64-bit";
            }

            string data = string.Format("{0} - version: {1} process type: {2}",
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
            Assembly assembly = Assembly.GetEntryAssembly();
            AssemblyName name = assembly.GetName();
            Version version = name.Version;

            string versionType = "32-bit";
            if (System.Environment.Is64BitProcess)
            {
                versionType = "64-bit";
            }

            string data = string.Format("{0} - v{1} - {2}",
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
            string operationSystemType = "32-bit OS";
            if (System.Environment.Is64BitOperatingSystem)
            {
                operationSystemType = "64-bit OS";
            }


            string data = string.Format("OS Version: {0} Processor Count: {1} Operating System Type: {2}  Memory: {3}  Page Size: {4}",
                                System.Environment.OSVersion,
                                System.Environment.ProcessorCount,
                                operationSystemType,
                                System.Environment.WorkingSet,
                                System.Environment.SystemPageSize);

            return data;
        }
    }
}
