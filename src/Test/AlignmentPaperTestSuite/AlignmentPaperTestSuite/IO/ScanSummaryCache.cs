#region

using System;
using System.Collections.Generic;
using System.IO;
using PNNLOmics.Data;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment.IO
{
    /// <summary>
    ///     Creates a scan cache summary.
    /// </summary>
    public class ScanSummaryCache
    {
        private const int CONST_HEADER_SIZE = 2;

        public static void WriteCache(string path, Dictionary<int, ScanSummary> cache, string rawPath)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                writer.WriteLine(rawPath);

                writer.WriteLine("Scan\tPrecursor\tMsLevel\tCharge");
                foreach (var scan in cache.Values)
                {
                    writer.WriteLine("{0}\t{1}\t{2}", scan.Scan, scan.PrecursorMz, scan.MsLevel);
                }
            }
        }

        public static Dictionary<int, ScanSummary> ReadCache(string path)
        {
            var cache = new Dictionary<int, ScanSummary>();

            var lines = File.ReadAllLines(path);
            for (var i = CONST_HEADER_SIZE; i < lines.Length; i++)
            {
                var summary = new ScanSummary();
                var line = lines[i];
                line = line.Trim().Replace(" ", "");
                var data = line.Split('\t');

                if (data.Length == 3)
                {
                    summary.Scan = Convert.ToInt32(data[0]);
                    summary.PrecursorMz = Convert.ToDouble(data[1]);
                    summary.MsLevel = Convert.ToInt32(data[2]);

                    cache.Add(summary.Scan, summary);
                }
            }
            return cache;
        }

        public static string ReadPath(string path)
        {
            var lines = File.ReadAllLines(path);
            return lines[0];
        }
    }

    public class DMSFileDownloader
    {
        public void Download(string path, string localPath)
        {
            if (File.Exists(localPath))
                return;


            File.Copy(path, localPath);
        }
    }
}