using System;
using System.IO;
using System.Collections.Generic;

using MultiAlignCore.IO;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;

namespace MultiAlignCore.IO
{
    public class AnalysisAlignmentDataWriter: IMultiAlignAnalysisWriter
    {
        #region IAnalysisWriter Members

        public void WriteAnalysis(string path, MultiAlignAnalysis analysis) 
        {            
            using (TextWriter writer = File.CreateText(path))
            {
                writer.WriteLine("Filename, Mass Mean, Mass Std, NET Mean, NET Std");
                for(int i = 0; i < analysis.AlignmentData.Count; i++)
                {
                    classAlignmentData data = analysis.AlignmentData[i];
                    writer.Write("{0},", Path.GetFileNameWithoutExtension(analysis.MetaData.Datasets[i].Features.Path));
                    if (data == null)
                    {
                        writer.WriteLine("Baseline dataset");
                    }
                    else
                    {
                        writer.WriteLine("{0},{1},{2},{3}",
                            data.MassMean,
                            data.MassStandardDeviation,
                            data.NETMean,
                            data.NETStandardDeviation);                            
                    }
                }
            }
        }

        #endregion
    }
}
