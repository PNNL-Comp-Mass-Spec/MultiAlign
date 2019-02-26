using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Algorithms.Regression;
using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Algorithms.Alignment;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.Alignment
{
    [TestFixture]
    public class DriftTimeAlignmentTest
    {
        [Test]
        public void TestDriftTimeAlignment()
        {
            List<UMCLight> observedUmcList = null;
            List<UMCLight> targetUmcList = null;

            CreateObservedAndTargetUmcLists(ref observedUmcList, ref targetUmcList);

            DriftTimeAlignment<UMCLight, UMCLight>.AlignObservedEnumerable(observedUmcList, observedUmcList,
                targetUmcList, 20, 0.03);

            Assert.AreEqual(Math.Round(observedUmcList[0].DriftTimeAligned, 2), 17.10);
            Assert.AreEqual(Math.Round(observedUmcList[1].DriftTimeAligned, 2), 28.67);
            Assert.AreEqual(Math.Round(observedUmcList[2].DriftTimeAligned, 2), 19.31);
            Assert.AreEqual(Math.Round(observedUmcList[3].DriftTimeAligned, 2), 18.80);
            Assert.AreEqual(Math.Round(observedUmcList[4].DriftTimeAligned, 2), 20.43);
            Assert.AreEqual(Math.Round(observedUmcList[5].DriftTimeAligned, 2), 21.56);
            Assert.AreEqual(Math.Round(observedUmcList[6].DriftTimeAligned, 2), 22.67);
            Assert.AreEqual(Math.Round(observedUmcList[7].DriftTimeAligned, 2), 23.41);
            Assert.AreEqual(Math.Round(observedUmcList[8].DriftTimeAligned, 2), 18.10);
            Assert.AreEqual(Math.Round(observedUmcList[9].DriftTimeAligned, 2), 24.86);
        }

        [Test]
        public void TestLinearEquationCalculation()
        {
            List<UMCLight> observedUmcList = null;
            List<UMCLight> targetUmcList = null;

            CreateObservedAndTargetUmcLists(ref observedUmcList, ref targetUmcList);

            var xyDataList =
                observedUmcList.Select((t, i) => new XYData(t.DriftTime, targetUmcList[i].DriftTime)).ToList();

            var regression = new LinearRegressionModel();
            var linearEquation = regression.CalculateRegression(xyDataList);
            Assert.AreEqual(Math.Round(linearEquation.Slope, 4), 0.7142);
            Assert.AreEqual(Math.Round(linearEquation.Intercept, 4), 1.1324);
        }

        private void CreateObservedAndTargetUmcLists(ref List<UMCLight> observedUmcList,
            ref List<UMCLight> targetUmcList)
        {
            //if (observedUmcList == null) throw new ArgumentNullException("observedUmcList");
            observedUmcList = new List<UMCLight>();
            targetUmcList = new List<UMCLight>();

            var observedFeature1 = new UMCLight
            {
                MassMonoisotopic = 771.47578,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 22.35554f
            };
            observedUmcList.Add(observedFeature1);

            var targetFeature1 = new UMCLight
            {
                MassMonoisotopic = 771.47313,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 16.99548f
            };
            targetUmcList.Add(targetFeature1);

            var observedFeature2 = new UMCLight();
            observedFeature2.MassMonoisotopic = 783.40238;
            observedFeature2.Net = 0.5;
            observedFeature2.ChargeState = 1;
            observedFeature2.DriftTime = 38.56024f;
            observedUmcList.Add(observedFeature2);

            var targetFeature2 = new UMCLight
            {
                MassMonoisotopic = 783.40651,
                Net = 0.5,
                ChargeState = 1,
                DriftTime = 28.64959f
            };
            targetUmcList.Add(targetFeature2);

            var observedFeature3 = new UMCLight
            {
                MassMonoisotopic = 1045.5403,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 25.4562f
            };
            observedUmcList.Add(observedFeature3);

            var targetFeature3 = new UMCLight
            {
                MassMonoisotopic = 1045.53546,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 19.34219f
            };
            targetUmcList.Add(targetFeature3);

            var observedFeature4 = new UMCLight
            {
                MassMonoisotopic = 1059.56535,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 24.7409f
            };
            observedUmcList.Add(observedFeature4);

            var targetFeature4 = new UMCLight
            {
                MassMonoisotopic = 1059.56132,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 18.79057f
            };
            targetUmcList.Add(targetFeature4);

            var observedFeature5 = new UMCLight
            {
                MassMonoisotopic = 1227.72843,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 27.01977f
            };
            observedUmcList.Add(observedFeature5);

            var targetFeature5 = new UMCLight
            {
                MassMonoisotopic = 1227.72107,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 20.46228f
            };
            targetUmcList.Add(targetFeature5);

            var observedFeature6 = new UMCLight
            {
                MassMonoisotopic = 1346.72985,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 28.60684f
            };
            observedUmcList.Add(observedFeature6);

            var targetFeature6 = new UMCLight
            {
                MassMonoisotopic = 1346.72875,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 21.55198f
            };
            targetUmcList.Add(targetFeature6);

            var observedFeature7 = new UMCLight
            {
                MassMonoisotopic = 1453.89352,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 30.15363f
            };
            observedUmcList.Add(observedFeature7);

            var targetFeature7 = new UMCLight
            {
                MassMonoisotopic = 1453.89305,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 22.72478f
            };
            targetUmcList.Add(targetFeature7);

            var observedFeature8 = new UMCLight
            {
                MassMonoisotopic = 1524.94014,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 31.19778f
            };
            observedUmcList.Add(observedFeature8);

            var targetFeature8 = new UMCLight
            {
                MassMonoisotopic = 1524.92889,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 23.4855f
            };
            targetUmcList.Add(targetFeature8);

            var observedFeature9 = new UMCLight
            {
                MassMonoisotopic = 1621.98666,
                Net = 0.5,
                ChargeState = 3,
                DriftTime = 23.75201f
            };
            observedUmcList.Add(observedFeature9);

            var targetFeature9 = new UMCLight
            {
                MassMonoisotopic = 1621.98151,
                Net = 0.5,
                ChargeState = 3,
                DriftTime = 18.13624f
            };
            targetUmcList.Add(targetFeature9);

            var observedFeature10 = new UMCLight
            {
                MassMonoisotopic = 1757.92318,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 33.22705f
            };
            observedUmcList.Add(observedFeature10);

            var targetFeature10 = new UMCLight
            {
                MassMonoisotopic = 1757.91498,
                Net = 0.5,
                ChargeState = 2,
                DriftTime = 24.77506f
            };
            targetUmcList.Add(targetFeature10);
        }
    }
}