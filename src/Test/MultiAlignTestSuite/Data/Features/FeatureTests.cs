
using MultiAlignCore.Data.Features;
using NUnit.Framework;

//using PNNLOmics.Utilities.Importers;

namespace MultiAlignTestSuite.Data.Features
{
    [TestFixture]
    public class FeatureTests
    {
        /// <summary>
        ///  Part of a clustering test to make sure when sending a 
        ///  null list the clustering algorithm fails.
        /// </summary>
        [Test]
        [TestCase(400, 400.1)]
        [TestCase(500, 500.1)]
        [TestCase(600, 600.1)]
        [TestCase(700, 700.1)]
        [TestCase(800, 800.1)]
        [TestCase(900, 900.1)]
        [TestCase(1000, 1000.1)]
        [TestCase(1100, 1100.1)]
        [TestCase(1200, 1200.1)]
        [TestCase(1300, 1300.1)]
        [TestCase(1400, 1400.1)]
        [TestCase(1500, 1500.1)]
        [TestCase(1600, 1600.1)]
        [TestCase(1700, 1700.1)]
        public void MassMassCalculations(double massX, double massY)
        {
            var ppm = FeatureLight.ComputeMassPPMDifference(massX, massY);
            var massYdelta = FeatureLight.ComputeDaDifferenceFromPPM(massX, ppm);
            Assert.AreEqual(massY, massYdelta);
        }
        /// <summary>
        /// Given a mass and ppm calculates the next closest mass
        /// </summary>
        /// <param name="massX">Mass of entity</param>
        /// <param name="ppm">PPM Difference</param>
        /// <param name="epsilon">tolerance for double calculation assertions</param>
        [Test]
        [TestCase(100, 50, .000001)]
        [TestCase(200, 50, .000001)]
        [TestCase(300, 50, .000001)]
        [TestCase(400, 50, .000001)]
        [TestCase(500, 50, .000001)]
        [TestCase(600, 50, .000001)]
        [TestCase(700, 50, .000001)]
        [TestCase(800, 50, .000001)]
        [TestCase(900, 50, .000001)]
        [TestCase(1000, 50, .000001)]
        [TestCase(1100, 50, .000001)]
        [TestCase(1200, 50, .000001)]
        [TestCase(1300, 50, .000001)]
        [TestCase(1400, 50, .000001)]
        [TestCase(1500, 50, .000001)]
        [TestCase(1600, 50, .000001)]
        [TestCase(1700, 50, .000001)]
        [TestCase(1800, 50, .000001)]
        [TestCase(1900, 50, .000001)]
        [TestCase(2000, 50, .000001)]
        public void MassPPMCalculations(double massX, double ppm, double epsilon)
        {
            var massYdelta = FeatureLight.ComputeDaDifferenceFromPPM(massX, ppm);
            var ppmDelta = FeatureLight.ComputeMassPPMDifference(massX, massYdelta);
            //Assert.IsTrue( (ppm - ppmDelta) < epsilon);
			Assert.Less(ppm - ppmDelta, epsilon);
        }
    }
}
