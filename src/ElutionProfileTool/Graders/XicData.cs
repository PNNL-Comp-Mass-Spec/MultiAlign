using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElutionProfileTool.Graders
{
    public class XicData
    {
        public XicData()
        {
            X = new List<float>();
            Y = new List<float>();
        }
        public int Charge { get; set; }
        public double Mz { get; set; }
        public int Scan { get; set; }
        public string Name { get; set; }
        public List<float> X { get; set; }
        public List<float> Y { get; set; }
    }    
}
