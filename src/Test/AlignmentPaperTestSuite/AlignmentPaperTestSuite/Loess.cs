using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PNNLOmics.Algorithms.Regression;

namespace AlignmentPaperTestSuite
{
    [TestFixture]
    public class Loess
    {
        [Test]
        [TestCase(@"M:\errors.txt",
            @"M:\errors-fit-05.txt",
            .05,
            5
            )]
        [TestCase(@"M:\errors.txt",
            @"M:\errors-fit-02.txt",
            .02,
            5
            )]
        [TestCase(@"M:\errors.txt",
            @"M:\errors-fit-20.txt",
            .2,
            5
            )]
        [TestCase(@"M:\errors.txt",
            @"M:\errors-fit-30.txt",
            .3,
            15
            )]
        [TestCase(@"M:\errors.txt",
            @"M:\errors-fit-10.txt",
            .1,
            5
            )]
        [TestCase(@"M:\errors.txt",
            @"M:\errors-fit-90.txt",
            .9,
            5
            )]
        public void TestAlignmentfunction(
            string path,
            string outputPath,
            double bandwidth,
            int robustnessIterators
            )
        {
            var data = File.ReadAllLines(path);

            var x = new List<double>();
            var y = new List<double>();

            for (var i = 1; i < data.Length; i++)
            {
                var columns = data[i].Split('\t');
                if (columns.Count() < 4)
                    continue;

                x.Add(Convert.ToDouble(columns[0]));
                y.Add(Convert.ToDouble(columns[2]));
            }
            using (var writer = File.CreateText(outputPath))
            {
                var loess = new LoessInterpolator(bandwidth, robustnessIterators);
                loess.MaxDistance = bandwidth;
                loess.Smooth(x, y, FitFunctionFactory.Create(FitFunctionTypes.Cubic));
                writer.WriteLine("NET\tNETY\tAligned\tNET\tERRORY\tERROR-Aligned");
                for (var i = 0; i < y.Count; i++)
                {
                    var value = loess.Predict(y[i]);
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", x[i], y[i], value, x[i], y[i] - x[i], value - x[i]);
                }
            }
        }
    }
}
