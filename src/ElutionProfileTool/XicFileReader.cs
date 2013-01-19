using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ElutionProfileTool
{
    

    public class XicFileReader
    {

        public List<FitData> ReadFits(string file)
        {
            if (!File.Exists(file))
            {
                return new List<FitData>();
            }
            List<FitData> fits  = new List<FitData>();
            string dirPath      = Path.GetDirectoryName(file);
            string fileName     = Path.GetFileNameWithoutExtension(file);

            try
            {
                string[] data = File.ReadAllLines(file);
                int state = 0;
                FitData fit = null;
                foreach (string line in data)
                {
                    if (line.Length < 3)
                        continue;

                    if (line.Contains("feature"))
                    {
                        if (fit != null)
                        {
                            fits.Add(fit);
                        }
                        fit = new FitData();
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
                    fits.Add(fit);
                }
            }
            catch
            {
            }

           

            return fits;
        }

    }
}
