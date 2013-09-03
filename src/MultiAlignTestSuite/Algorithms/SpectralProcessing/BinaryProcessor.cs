using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PNNLOmics.Data;
using MultiAlignCore.IO.Features;
using PNNLOmics.Algorithms.SpectralProcessing;

using MultiAlignCustomControls.Charting;
using System.IO;
using System.Drawing;

namespace MultiAlignTestSuite.Algorithms.SpectralProcessing
{
    [TestFixture]
    public class BinaryProcessor
    {
        private Dictionary<string, int> m_pathMap;

        public BinaryProcessor()
        {
            m_pathMap = new Dictionary<string, int>();
        }


        private MSSpectra GetSpectrum(string path, int scan)
        {

            ISpectraProvider reader = new ThermoRawDataFileReader();
            reader.AddDataFile(path, 0);
            return GetSpectrum(reader, scan, 0);
        }

        private MSSpectra GetSpectrum(ISpectraProvider reader, int scan, int group, double mzTolerance = .5)
        {
            List<XYData> peaks  = reader.GetRawSpectra(scan, group, 2);
            MSSpectra spectrum  = new MSSpectra();
            spectrum.Peaks      = peaks;
            return spectrum;
        }

        private void FitSpectrum(MSSpectra spectrum)
        {
            spectrum.Peaks.Add(new XYData(400, 0));
            spectrum.Peaks.Add(new XYData(2000, 0));
        }


        /// <summary>
        /// Creates binary spectra and and compares them (visually)
        /// </summary>
        /// <param name="pathX"></param>
        /// <param name="pathY"></param>
        /// <param name="scanX"></param>
        /// <param name="scanY"></param>
        /// <param name="threshold">Top Percent of spectra</param>
        [Test(Description = "Compares two spectra against each other.")]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",                    
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 
                    7582,
                    7413,
                    .4
                    )]        
        public void TestBinarySpectra(string pathX, string pathY, int scanX, int scanY, double threshold)
        {
            Console.WriteLine("Test: {0}\tcompared to\t{1}", pathX, pathY);
            MSSpectra spectrumX = GetSpectrum(pathX, scanX);
            MSSpectra spectrumY = GetSpectrum(pathY, scanY);
            
            string dirX         = Path.GetDirectoryName(pathX);
            string dirY         = Path.GetDirectoryName(pathY);

            string nameX        = Path.GetFileNameWithoutExtension(pathX);
            string nameY        = Path.GetFileNameWithoutExtension(pathY);

            ISpectraFilter filterer = new TopPercentSpectralFilter();
            MSSpectra filteredX     = filterer.Threshold(spectrumX, threshold);
            MSSpectra filteredY     = filterer.Threshold(spectrumY, threshold);

            ISpectralNormalizer binary  = new BinarySpectraNormalizer();
            MSSpectra binaryX           = binary.Normalize(filteredX);
            MSSpectra binaryY           = binary.Normalize(filteredY);



            List<XYData> invertedBinaryX = new List<XYData>();
            binaryX.Peaks.ForEach(x => invertedBinaryX.Add(new XYData(x.X, x.Y * -1)));



            List<XYData> invertedBinaryY = new List<XYData>();
            binaryY.Peaks.ForEach(x => invertedBinaryY.Add(new XYData(x.X, x.Y * -1)));

            for (int i = 0; i < binaryX.Peaks.Count; i++)
            {
                binaryX.Peaks[i].Y = 1;
            }

            FitSpectrum(binaryX);
            FitSpectrum(binaryY);
            FitSpectrum(filteredX);
            FitSpectrum(filteredY);

            string extension = string.Format("-{0:0.00}.png", threshold);

            // Create plots for spectra X
            SpectraChart chart = new SpectraChart();
            //chart.Width  = 1280;
            //chart.Height = 1024;
            //chart.AddSpectra(filteredX.Peaks, "Filtered X", Color.DarkRed);
            //chart.AddSpectra(invertedBinaryX, "Binary X",   Color.Red);
            //chart.XAxisLabel = "m/z";
            //chart.LegendVisible = false;
            //chart.TitleVisible  = false;
            //chart.AutoViewPort();
            //Image image = chart.ToBitmap();
            //image.Save(Path.Combine(dirX, nameX + extension));
            
            //// Create plots for spectra Y 
            //chart        = new SpectraChart();
            //chart.Width  = 1280;
            //chart.Height = 1024;
            //chart.AddSpectra(filteredY.Peaks, "Filtered Y", Color.DarkGreen);
            //chart.AddSpectra(invertedBinaryY, "Binary Y",   Color.Lime);
            //chart.XAxisLabel = "m/z";
            //chart.LegendVisible = false;
            //chart.TitleVisible  = false;
            //chart.AutoViewPort();

            //image = chart.ToBitmap();
            //image.Save(Path.Combine(dirY, nameY + extension));

            // Compare binary charts
            chart = new SpectraChart();
            chart.Width  = 1280;
            chart.Height = 1024;
            chart.AddSpectra(binaryX.Peaks,   "Binary X", Color.Red);
            chart.AddSpectra(invertedBinaryY, "Binary Y", Color.Lime);
            chart.XAxisLabel    = "m/z";            
            chart.LegendVisible = false;
            chart.TitleVisible  = false;
            chart.AutoViewPort();
            Image image = chart.ToBitmap();
            image.Save(Path.Combine(dirY, "comparisonX-Y" + extension));


            BinaryDotProduct comparison = new BinaryDotProduct(threshold);            
            MSSpectra xx    = new MSSpectra();
            MSSpectra yy    = new MSSpectra();
            xx.Peaks        = XYData.Bin(binaryX.Peaks, 0, 2000, .1);
            yy.Peaks        = XYData.Bin(binaryY.Peaks, 0, 2000, .1);

            double value =  comparison.CompareSpectra(xx, yy);


            using (TextWriter writer = File.CreateText((Path.Combine(dirY, "comparisonX-Y-tons.csv"))))
            {
                writer.WriteLine("value = {0}", value);
                writer.WriteLine();
                for(int i = 0; i < xx.Peaks.Count; i++)
                {
                    XYData peakX = xx.Peaks[i];
                    XYData peakY = yy.Peaks[i];

                    if (peakX.Y <= 0 && peakY.Y <= 0)
                    {
                        continue;
                    }
                    writer.WriteLine("{0},{1},{2},{3}",
                                                    peakX.X,                         
                                                    peakX.Y, 
                                                    peakY.Y * -1, 
                                                    peakX.Y * peakY.Y);
                }
                writer.WriteLine();                
            }
        }
    }
}
