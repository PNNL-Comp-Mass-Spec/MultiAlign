using System;
using System.Collections.Generic;
using System.Text;

using MultiAlign.Alignment;

namespace MultiAlignWin.Data.Alignment
{
    [Serializable()]
    public class clsAlignmentData
    {
        public clsAlignmentFunction alignmentFunction;
        public string aligneeDataset;
        public float[,] heatScores;
        public int minScanBaseline;
        public int maxScanBaseline;
        public float minMTDBNET;
        public float maxMTDBNET;
    }
}
