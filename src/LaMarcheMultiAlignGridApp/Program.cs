using System;
using System.IO;
using System.Text;
using MultiAlignCore.Algorithms.FeatureMatcher;
using System.Collections.Generic;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace LaMarcheMultiAlignGridApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Read a csv file, put the data into a new UMCLight for each one
            string[] csvFileText = File.ReadAllLines(args[1]);

            var csvDataList = new List<UMCLight>{Capacity = csvFileText.Length};

            foreach (var line in csvFileText)
            {
                string[] parsedLine = line.Split(',');

                var umcDataMember = new UMCLight();
                //put the data from the parsed line into the umcDataMember in the appropriate fashion

                csvDataList.Add(umcDataMember);

            }

            //Create clusters from the data read in from the file

            UMCClusterLight cluster = null;

            List<UMCClusterLight> filteredClusters = new List<UMCClusterLight>();

            if (!Filter(cluster))
            {
                //Save the cluster
                filteredClusters.Add(cluster);
            }


            //Read a mtdb file using MACore or sqliteDb
            string databasePath = "C:\\UnitTestFolder\\MSGFPlus\\blah.db"; //Either read from console, or entered at program execution
                                      // Console.ReadLine(databasePath) or databasePath = args[2]
            MassTagDatabase database = ReadDatabase(databasePath);

            STACAdapter<UMCClusterLight> stacAdapter = new STACAdapter<UMCClusterLight>();

            var matchList = stacAdapter.PerformPeakMatching(filteredClusters, database);
            string writePath = null;
            // As with databasePath, could either be read from console, or entered at program execution
            // Console.ReadLine(writePath) or, if(args.Length >= 4){ writePath = args[3]; }
            // If writePath isn't entered, then it writes to a default file, defined inside the WriteData method
            WriteData(matchList, writePath);
        }

        private static bool Filter(UMCClusterLight cluster) // might be different cluster object
        {
            //Perform filtering as necesary, returns true if the cluster does not pass the filter
            return true;
        }

        private static MassTagDatabase ReadDatabase(string databasePath)
        {
            MassTagDatabase database = null;

            //Perform the reading from the binary file, saving data into database
            byte[] fileBytes = File.ReadAllBytes(databasePath);

            StringBuilder sb = new StringBuilder();

            foreach (var b in fileBytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            //Now it's a string of data

            return database;
        }

        private static void WriteData(List<FeatureMatchLight<UMCClusterLight, MassTagLight>> matchList,
                                      string writePath)
        {
            if (writePath == null)
            {
                writePath = "C:\\DataGoesHere.csv";
            }

            // Open the file, or create it if it didn't exist, for write access
            using(StreamWriter writeFile = new StreamWriter(writePath))
            {
                foreach (var match in matchList)
                {
                    //Write the data into the file!
                    writeFile.Write(match.Observed.DriftTime);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.MassMonoisotopicAligned);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.NETAligned);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.DatasetMemberCount);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.Features.Count);
                    writeFile.Write('\n');
                }
            }
        }
    }
}
