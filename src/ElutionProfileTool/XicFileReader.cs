using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ElutionProfileTool.Graders;

namespace ElutionProfileTool
{
    
    /// <summary>
    /// Class that reads an XIC file.
    /// </summary>
    public class XicFileReader
    {
        /// <summary>
        /// Reads Xic Data
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public List<XicData> ReadXicData(string file)
        {
            if (!File.Exists(file))
            {
                return new List<XicData>();
            }
            List<XicData> xics = new List<XicData>();
            string dirPath      = Path.GetDirectoryName(file);
            string fileName     = Path.GetFileNameWithoutExtension(file);

            try
            {
                string[] data   = File.ReadAllLines(file);
                int state       = 0;
                XicData fit     = null;
                foreach (string line in data)
                {
                    if (line.Length < 3)
                        continue;

                    if (line.Contains("feature"))
                    {
                        if (fit != null)
                        {
                            xics.Add(fit);
                        }
                        fit = new XicData();
                        fit.Name = line.Replace("feature", "").Replace(",", "");
                        state = 0;
                    }
                    else if (line.Contains("mz"))
                    {
                        state = 1;
                    }
                    else
                    {
                        state = 2;
                    }

                    if (fit == null)
                        continue;

                    switch (state)
                    {
                        case 2:
                            string[] lineData = line.Split(',');
                            fit.X.Add(Convert.ToSingle(lineData[1]));
                            fit.Y.Add(Convert.ToSingle(lineData[2]));
                            break;
                        default:
                            break;
                    }
                }
                if (fit != null)
                {
                    xics.Add(fit);
                }
            }
            catch
            {
            }           
            return xics;
        }

    }
}
