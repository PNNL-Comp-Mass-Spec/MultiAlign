//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using PNNLOmics.Algorithms.PeakDetection;
//using PNNLOmics.Algorithms.SpectralProcessing;
//using PNNLOmics.Data;
//using PNNLOmics.Data.Peaks;
//using TestSpectra;

//namespace PNNLOmics.UnitTests.AlgorithmTests.PeakDetectorTests
//{
//    [TestFixture]
//    public class PeakDetectorTests
//    {
//        /// <summary>
//        /// This tests only the XYData loader and the peak detection
//        /// </summary>
//        [Test]
//        public void PeakDetectorV3_DiscoverPeaks_only_no_ThresholdingTest_AOnly()
//        {
//            float[] xvals = null;
//            float[] yvals = null;

//            loadTestScanData(ref xvals, ref yvals);
//            Assert.That(xvals != null);
//            Assert.AreEqual(122032, xvals.Length);

//            var testXyData = convertXYDataToOMICSXYData(xvals, yvals);
//            var newPeakCentroider = new PeakCentroider();
//            var centroidedPeakList = newPeakCentroider.DiscoverPeaks(testXyData);

//            displayPeakData(centroidedPeakList);

//            Assert.That(centroidedPeakList.Count > 0);
//            Assert.AreEqual(15255, centroidedPeakList.Count);
//        }

//        /// <summary>
//        /// this tests the peak detection and the thresholding
//        /// </summary>
//        [Test]
//        public void PeakDetectorV3_DiscoverPeaks_Then_Threshold_A_and_B()
//        {
//            float[] xvals = null;
//            float[] yvals = null;

//            loadTestScanData(ref xvals, ref yvals);
//            Assert.That(xvals != null);
//            Assert.AreEqual(122032, xvals.Length);
//            Console.WriteLine("Passed Load" + Environment.NewLine);

//            var testXyData          = convertXYDataToOMICSXYData(xvals, yvals);
//            var newPeakCentroider   = new PeakCentroider();
//            var centroidedPeakList  = newPeakCentroider.DiscoverPeaks(testXyData);

//            Assert.AreEqual(15255, centroidedPeakList.Count);
//            Console.WriteLine("Passed Peak Detection" + Environment.NewLine);

//            var newPeakThresholder = new PeakThresholder();
//            //newPeakThresholder.Parameters.SignalToShoulderCuttoff = 3f;//3 sigma
//            newPeakThresholder.Parameters.SignalToShoulderCuttoff = 4f;//4 sigma// this is very nice
//            var thresholdedData = newPeakThresholder.ApplyThreshold(centroidedPeakList);

//            Console.WriteLine("Non-thresholded Candidate Peaks detected = " + centroidedPeakList.Count);

//            Assert.That(thresholdedData.Count > 0);
//            //Assert.AreEqual(thresholdedData.Count, 53);
//            Assert.AreEqual(thresholdedData.Count, 414);

//            displayPeakData(thresholdedData);
//            Console.WriteLine();
//            Console.WriteLine("Thresholded Peaks detected = " + thresholdedData.Count);
//        }

//        [Test]
//        public void PeakDetectorTest()
//        {
//            float[] xvals = null;
//            float[] yvals = null;

//            loadTestScanData(ref xvals, ref yvals);
//            Assert.That(xvals != null);
//            Assert.AreEqual(122032, xvals.Length);

//            var testXYData = convertXYDataToOMICSXYData(xvals, yvals);
//            var dataInput = new Collection<XYData>(testXYData);

//            var newPeakDetector = new KronewitterPeakDetector();
//            var finalPeakList = newPeakDetector.DetectPeaks(dataInput);

//            Console.WriteLine("We found " + finalPeakList.Count + " Peaks.");
//            //Assert.AreEqual(finalPeakList.Count, 53);
//            Assert.AreEqual(finalPeakList.Count, 3134);
//        }

//        [Test]
//        public void SmoothTest()
//        {
//            float[] xvals = null;
//            float[] yvals = null;

//            loadTestScanData(ref xvals, ref yvals);
//            Assert.That(xvals != null);
//            Assert.AreEqual(122032, xvals.Length);

//            var testXYData = convertXYDataToOMICSXYData(xvals, yvals);
//            var dataInput = new List<XYData>(testXYData);

//            var numberOfSmootherPoints = 9;
//            var polynomialOrder = 2;
//            var allowNegativeValues = true;//faster
//            var smoother = new SavitzkyGolaySmoother(numberOfSmootherPoints, polynomialOrder, allowNegativeValues);

//            var smoothedValues = smoother.Smooth(dataInput);

//            Console.WriteLine("We found " + smoothedValues.Count + " Peaks.");
//            Assert.AreEqual(122032, smoothedValues.Count);

//            Assert.AreEqual(17006.553739209197d, smoothedValues[4000].Y);//this is the correct number
//            Console.WriteLine("The Intensity is " + smoothedValues[4000].Y + " and passes the test.");

//            Assert.AreEqual(77942.695035173179d, smoothedValues[3].Y);//this is the correct number
//            Console.WriteLine("The Intensity is " + smoothedValues[3].Y + " and passes the test.");

//            Assert.AreEqual(164.94386929598679d, smoothedValues[smoothedValues.Count - 3].Y);//this is the correct number
//            Console.WriteLine("The Intensity is " + smoothedValues[smoothedValues.Count - 3].Y + " and passes the test.");
//        }


//        #region private functions

//        private void displayPeakData(List<ProcessedPeak> centroidedPeakList)
//        {
//            var sb = new StringBuilder();
//            sb.Append("m/z" + '\t' + "Height" + '\t' + "Width");
//            foreach (var item in centroidedPeakList)
//            {
//                sb.Append(item.XValue);
//                sb.Append('\t');
//                sb.Append(item.Height);
//                sb.Append('\t');

//                sb.Append(item.Width);
//                sb.Append(Environment.NewLine);
//            }
//            Console.WriteLine(sb.ToString());
//        }

//        private List<XYData> convertXYDataToOMICSXYData(float[] xvals, float[] yvals)
//        {
//            var xydataList = new List<XYData>();
//            for (var i = 0; i < xvals.Length; i++)
//            {
//                var xydatapoint = new XYData(xvals[i], yvals[i]);
//                xydataList.Add(xydatapoint);
//            }
//            return xydataList;
//        }

//        private void loadTestScanData(ref float[] xvals, ref float[] yvals)
//        {
//            double[] tempXVals = null;
//            double[] tempYVals = null;

//            loadTestScanData(ref tempXVals, ref tempYVals);

//            xvals = tempXVals.Select(i => (float)i).ToArray();
//            yvals = tempYVals.Select(i => (float)i).ToArray();
//        }

//        private void loadTestScanData(ref double[] xvals, ref double[] yvals)
//        {
//            var newSpectra = new HardCodedSpectraDouble();
//            newSpectra.GenerateSpectraDouble();
//            Console.WriteLine(newSpectra.XValues.Length);
//            xvals = newSpectra.XValues;
//            yvals = newSpectra.YValues;
//        }

//        #endregion
//    }
//}


