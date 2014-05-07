using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using System.IO;
using MultiAlignCore.IO.MTDB;

namespace MultiAlignTestSuite.Papers.Alignment.IO
{
    /// <summary>
    /// Creates a scan cache summary.
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
                    writer.WriteLine("{0}\t{1}\t{2}", scan.Scan, scan.PrecursorMZ, scan.MsLevel);
                }
            }
        }
        public static Dictionary<int, ScanSummary> ReadCache(string path)
        {
            Dictionary<int, ScanSummary> cache = new Dictionary<int, ScanSummary>();

            string[] lines = File.ReadAllLines(path);
            for (int i = CONST_HEADER_SIZE; i < lines.Length; i++)
            {
                ScanSummary summary = new ScanSummary();
                string line         = lines[i];
                line                = line.Trim().Replace(" ", "");
                string[] data       = line.Split('\t');
                
                if (data.Length == 3)
                {
                    summary.Scan        = Convert.ToInt32(data[0]);
                    summary.PrecursorMZ = Convert.ToDouble(data[1]);
                    summary.MsLevel     = Convert.ToInt32(data[2]);

                    cache.Add(summary.Scan, summary);
                }
            }
            return cache;
        }
        public static string ReadPath(string path)
        {            
            string[] lines = File.ReadAllLines(path);
            return lines[0];
        }
    }

    public class DMSFileDownloader
    {
        public void Download(string path, string localPath)
        {
            if (File.Exists(localPath))
                return;

            try
            {
                File.Copy(path, localPath);
            }
            catch
            {

            }
        }

        public void Download(IEnumerable<string> paths, string localPath)
        {
            MageMSGFFinderLoader x = new MageMSGFFinderLoader();
            
            foreach (string path in paths)
            {
                List<string> msgfPaths = x.LoadFiles(path);    

                if (msgfPaths != null && msgfPaths.Count > 0)
                    Download(msgfPaths[0], localPath);
            }
        }
    }
}
