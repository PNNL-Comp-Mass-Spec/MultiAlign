using System;
using System.Collections.Generic;
using FeatureAlignment.Algorithms.Alignment;
using FeatureAlignment.Algorithms.Alignment.LcmsWarp;
using FeatureAlignment.Data.Features;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.Alignment
{
    [TestFixture]
    public class LCMS_LCAlignment
    {
        [Test]
        public void GlycoAlignment()
        {
            var multiplier = 100000;//for converting doubles to ints

            //1.  pull in test data
            List<double> referenceTimes;
            List<double> experimentalTimes;
            ExampleTimesDB02(out experimentalTimes, out referenceTimes);

            //2.  convert to acceptable format
            List<UMCLight> referenceTimesAsUmc;
            List<UMCLight> experimentalTimesAsUmc;
            ConvertToUMCLight(referenceTimes, experimentalTimes, out experimentalTimesAsUmc, out referenceTimesAsUmc, multiplier);

            //3.  set up processor and options
            var options = new LcmsWarpAlignmentOptions
            {
                MassTolerance = 0.5,                // masses are exact so this does not matter in this test
                NumTimeSections = 12,               // default is 100
                StoreAlignmentFunction = true,
                NetBinSize = 0.001,                 // default is 0.001; //this does not do much
                UsePromiscuousPoints = false,       // this does not do much but is likely needed when matching to a dense AMT tag databaase
                AlignmentAlgorithmType = FeatureAlignmentType.LCMS_WARP,
                ContractionFactor = 1,              // setting this to 1 helped
                AlignType = LcmsWarpAlignmentType.NET_WARP
            };
            var processor = new LcmsWarpAlignmentProcessor(options);

            //4.  Set references with processor setter
            processor.SetReferenceDatasetFeatures(referenceTimesAsUmc);
            processor.SetAligneeDatasetFeatures(experimentalTimesAsUmc);

            //5.  perform alignment
            processor.PerformAlignmentToMsFeatures();

            //6.  apply alignment to data
            processor.ApplyNetMassFunctionToAligneeDatasetFeatures(experimentalTimesAsUmc);

            //7.  test alignment
            Console.WriteLine("index" + "," + "ReferenceNet" + "," + "ExperimentalNet" + "," + "AlignedNet");
            double sum = 0;
            for (int i = 0; i < experimentalTimes.Count; i++)
            {
                double referenceNet = referenceTimes[i];
                double experimentalNet = experimentalTimes[i];
                double alignedNet = experimentalTimesAsUmc[i].NetAligned / multiplier;//replace with calculated Net

                Console.WriteLine(i + "," + referenceNet + "," + experimentalNet + "," + alignedNet);

                sum += (alignedNet - referenceNet)*(alignedNet - referenceNet);
            }

            Console.WriteLine(" The sum of the squares score is " + Math.Round(sum,2));
            
            // Old value before September 2016
            // Assert.AreEqual(2.0126026729399675, sum);
            
            // New value
            Assert.AreEqual(0.15686173611926441, sum);
        }

        private static void ConvertToUMCLight(List<double> referenceTimes, List<double> experimentalTimes, out List<UMCLight> experimentalTimesAsUMC, out List<UMCLight> referenceTimesAsUMC, int multiplier)
        {
            referenceTimesAsUMC = new List<UMCLight>();
            experimentalTimesAsUMC = new List<UMCLight>();

            for (int i = 0; i < experimentalTimes.Count; i++)
            {
                UMCLight referencePoint = new UMCLight();
                UMCLight experimentalPoint = new UMCLight();

                referencePoint.Net = Math.Round(referenceTimes[i] * multiplier);
                //experimentalPoint.Net = experimentalTimes[i]*multiplier;


                //referencePoint.Scan = Convert.ToInt32(referenceTimes[i]*multiplier);
                //scan needs to be populated
                experimentalPoint.Scan = Convert.ToInt32(Math.Round(experimentalTimes[i] * multiplier, 0));
                experimentalPoint.ScanStart = Convert.ToInt32(Math.Round(experimentalTimes[i] * multiplier, 0));
                experimentalPoint.ScanEnd = Convert.ToInt32(Math.Round(experimentalTimes[i] * multiplier, 0));

                referencePoint.Mz = 1000 + i;
                experimentalPoint.Mz = 1000 + i;

                //this is needed
                referencePoint.MassMonoisotopic = 1000 + i;
                experimentalPoint.MassMonoisotopic = 1000 + i;

                referenceTimesAsUMC.Add(referencePoint);
                experimentalTimesAsUMC.Add(experimentalPoint);
            }
        }

        private static void ExampleTimesDB02(out List<double> experimentalTimes, out List<double> referenceTimes)
        {
            experimentalTimes = new List<double>();
            referenceTimes = new List<double>();
            referenceTimes.Add(0.59678225); experimentalTimes.Add(0.57193475);
            referenceTimes.Add(0.59678225); experimentalTimes.Add(0.57193475);
            referenceTimes.Add(0.587572175925926); experimentalTimes.Add(0.56411899537037);
            referenceTimes.Add(0.587572175925926); experimentalTimes.Add(0.56411899537037);
            referenceTimes.Add(0.582922912037037); experimentalTimes.Add(0.574027208333333);
            referenceTimes.Add(0.582922912037037); experimentalTimes.Add(0.549039351851852);
            referenceTimes.Add(0.578383236111111); experimentalTimes.Add(0.555170949074074);
            referenceTimes.Add(0.565160833333333); experimentalTimes.Add(0.529214712962963);
            referenceTimes.Add(0.562653486111111); experimentalTimes.Add(0.5945595);
            referenceTimes.Add(0.541268444444444); experimentalTimes.Add(0.523082125);
            referenceTimes.Add(0.540748606481482); experimentalTimes.Add(0.522474675925926);
            referenceTimes.Add(0.536829726851852); experimentalTimes.Add(0.515745685185185);
            referenceTimes.Add(0.534449712962963); experimentalTimes.Add(0.496004125);
            referenceTimes.Add(0.524368541666667); experimentalTimes.Add(0.550369921296297);
            referenceTimes.Add(0.524368541666667); experimentalTimes.Add(0.550369921296297);
            referenceTimes.Add(0.521498861111111); experimentalTimes.Add(0.480176347222223);
            referenceTimes.Add(0.516576097222223); experimentalTimes.Add(0.5159995);
            referenceTimes.Add(0.513626791666667); experimentalTimes.Add(0.496823518518518);
            referenceTimes.Add(0.511560847222223); experimentalTimes.Add(0.500737916666667);
            referenceTimes.Add(0.510325078703703); experimentalTimes.Add(0.528510708333333);
            referenceTimes.Add(0.507936398148148); experimentalTimes.Add(0.494298083333333);
            referenceTimes.Add(0.506911740740741); experimentalTimes.Add(0.488802666666667);
            referenceTimes.Add(0.506911740740741); experimentalTimes.Add(0.495103208333333);
            referenceTimes.Add(0.50599887962963); experimentalTimes.Add(0.452463916666667);
            referenceTimes.Add(0.492358180555556); experimentalTimes.Add(0.507228694444444);
            referenceTimes.Add(0.492358180555556); experimentalTimes.Add(0.507228694444444);
            referenceTimes.Add(0.490434342592593); experimentalTimes.Add(0.481984421296297);
            referenceTimes.Add(0.485717087962963); experimentalTimes.Add(0.485901569444444);
            referenceTimes.Add(0.482623087962962); experimentalTimes.Add(0.484140453703703);
            referenceTimes.Add(0.481976152777778); experimentalTimes.Add(0.405033194444444);
            referenceTimes.Add(0.480717638888889); experimentalTimes.Add(0.488802666666667);
            referenceTimes.Add(0.480717638888889); experimentalTimes.Add(0.488802666666667);
            referenceTimes.Add(0.48052475); experimentalTimes.Add(0.478577041666667);
            referenceTimes.Add(0.476060300925926); experimentalTimes.Add(0.468308106481482);
            referenceTimes.Add(0.472158078703704); experimentalTimes.Add(0.482203041666667);
            referenceTimes.Add(0.470236652777777); experimentalTimes.Add(0.459643597222223);
            referenceTimes.Add(0.448177587962963); experimentalTimes.Add(0.439915773148148);
            referenceTimes.Add(0.447478902777777); experimentalTimes.Add(0.438658555555556);
            referenceTimes.Add(0.434003763888889); experimentalTimes.Add(0.365319527777777);
            referenceTimes.Add(0.433283435185185); experimentalTimes.Add(0.4230855);
            referenceTimes.Add(0.423215546296297); experimentalTimes.Add(0.424826083333333);
            referenceTimes.Add(0.422813222222223); experimentalTimes.Add(0.417291185185185);
            referenceTimes.Add(0.414961583333333); experimentalTimes.Add(0.410331625);
            referenceTimes.Add(0.414416916666667); experimentalTimes.Add(0.410064930555556);
            referenceTimes.Add(0.413510541666667); experimentalTimes.Add(0.44315912037037);
            referenceTimes.Add(0.410480319444444); experimentalTimes.Add(0.403399138888889);
            referenceTimes.Add(0.406959347222222); experimentalTimes.Add(0.409008041666667);
            referenceTimes.Add(0.40332400462963); experimentalTimes.Add(0.404200152777777);
            referenceTimes.Add(0.393576); experimentalTimes.Add(0.390494717592593);
            referenceTimes.Add(0.384698976851852); experimentalTimes.Add(0.379935699074074);
            referenceTimes.Add(0.382914287037037); experimentalTimes.Add(0.377269902777778);
            referenceTimes.Add(0.379019222222223); experimentalTimes.Add(0.375195819444444);
            referenceTimes.Add(0.371914523148148); experimentalTimes.Add(0.371154819444444);
            referenceTimes.Add(0.368733486111111); experimentalTimes.Add(0.377547291666667);
            referenceTimes.Add(0.344332476851852); experimentalTimes.Add(0.334647333333333);
            referenceTimes.Add(0.336418263888889); experimentalTimes.Add(0.337376962962963);
            referenceTimes.Add(0.335104115740741); experimentalTimes.Add(0.376368833333333);
            referenceTimes.Add(0.331696226851852); experimentalTimes.Add(0.33159837037037);
            referenceTimes.Add(0.330559342592593); experimentalTimes.Add(0.330705375);
            referenceTimes.Add(0.329436138888889); experimentalTimes.Add(0.329376180555556);
            referenceTimes.Add(0.326360819444444); experimentalTimes.Add(0.323877680555556);
            referenceTimes.Add(0.326360819444444); experimentalTimes.Add(0.327481638888889);
            referenceTimes.Add(0.325822212962963); experimentalTimes.Add(0.232088097222222);
            referenceTimes.Add(0.32555400462963); experimentalTimes.Add(0.117427532407408);
            referenceTimes.Add(0.324626800925926); experimentalTimes.Add(0.319103615740741);
            referenceTimes.Add(0.314172652777778); experimentalTimes.Add(0.314977291666667);
            referenceTimes.Add(0.314369444444444); experimentalTimes.Add(0.337255041666667);
            referenceTimes.Add(0.312403050925926); experimentalTimes.Add(0.341961824074074);
            referenceTimes.Add(0.309384490740741); experimentalTimes.Add(0.310606569444444);
            referenceTimes.Add(0.308070046296297); experimentalTimes.Add(0.308532731481482);
            referenceTimes.Add(0.307035175925926); experimentalTimes.Add(0.314798319444444);
            referenceTimes.Add(0.301942717592593); experimentalTimes.Add(0.301737976851852);
            referenceTimes.Add(0.298721287037037); experimentalTimes.Add(0.299652847222222);
            referenceTimes.Add(0.291840694444444); experimentalTimes.Add(0.230620319444444);
            referenceTimes.Add(0.286201185185185); experimentalTimes.Add(0.289244324074074);
            referenceTimes.Add(0.282022458333333); experimentalTimes.Add(0.283343);
            referenceTimes.Add(0.274171972222223); experimentalTimes.Add(0.218942138888889);
            referenceTimes.Add(0.273568416666667); experimentalTimes.Add(0.270847986111111);
            referenceTimes.Add(0.273658898148148); experimentalTimes.Add(0.278667907407407);
            referenceTimes.Add(0.272455958333333); experimentalTimes.Add(0.253622074074074);
            referenceTimes.Add(0.272363263888889); experimentalTimes.Add(0.275680430555556);
            referenceTimes.Add(0.264558189814815); experimentalTimes.Add(0.270425277777777);
            referenceTimes.Add(0.254103916666667); experimentalTimes.Add(0.259639134259259);
            referenceTimes.Add(0.246883671296297); experimentalTimes.Add(0.250141787037037);
            referenceTimes.Add(0.243115342592592); experimentalTimes.Add(0.248133819444444);
            referenceTimes.Add(0.239939523148148); experimentalTimes.Add(0.244559652777778);
            referenceTimes.Add(0.239853587962963); experimentalTimes.Add(0.244388893518518);
            referenceTimes.Add(0.238968208333333); experimentalTimes.Add(0.245665958333333);
            referenceTimes.Add(0.238677527777778); experimentalTimes.Add(0.221731375);
            referenceTimes.Add(0.238316814814815); experimentalTimes.Add(0.245924666666667);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.238526486111111);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.238355333333333);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.238355333333333);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.238270541666667);
            referenceTimes.Add(0.234279435185185); experimentalTimes.Add(0.238270541666667);
            referenceTimes.Add(0.233838111111111); experimentalTimes.Add(0.238440125);
            referenceTimes.Add(0.233282166666667); experimentalTimes.Add(0.238440125);
            referenceTimes.Add(0.230461027777777); experimentalTimes.Add(0.319209236111111);
            referenceTimes.Add(0.230461027777777); experimentalTimes.Add(0.238785337962963);
            referenceTimes.Add(0.230025930555556); experimentalTimes.Add(0.234182662037037);
            referenceTimes.Add(0.209980361111111); experimentalTimes.Add(0.218746472222223);
            referenceTimes.Add(0.205638819444444); experimentalTimes.Add(0.215820847222223);
            referenceTimes.Add(0.18528350462963); experimentalTimes.Add(0.197160541666667);
            referenceTimes.Add(0.160785194444444); experimentalTimes.Add(0.173556986111111);
            referenceTimes.Add(0.147514833333333); experimentalTimes.Add(0.265894055555556);
            referenceTimes.Add(0.146135847222222); experimentalTimes.Add(0.233300472222223);
            referenceTimes.Add(0.146329439814815); experimentalTimes.Add(0.159405268518518);
            referenceTimes.Add(0.103025791666667); experimentalTimes.Add(0.270591777777778);


        }

        private static void ExampleTimesDB09(out List<double> experimentalTimes, out List<double> referenceTimes)
        {
            experimentalTimes = new List<double>();
            referenceTimes = new List<double>();
            referenceTimes.Add(0.597636236111111); experimentalTimes.Add(0.604530111111111);
            referenceTimes.Add(0.59678225); experimentalTimes.Add(0.568825597222222);
            referenceTimes.Add(0.59678225); experimentalTimes.Add(0.568825597222222);
            referenceTimes.Add(0.587572175925926); experimentalTimes.Add(0.600409569444444);
            referenceTimes.Add(0.587572175925926); experimentalTimes.Add(0.600409569444444);
            referenceTimes.Add(0.582922912037037); experimentalTimes.Add(0.553956138888889);
            referenceTimes.Add(0.582922912037037); experimentalTimes.Add(0.553956138888889);
            referenceTimes.Add(0.582473055555556); experimentalTimes.Add(0.553778620370371);
            referenceTimes.Add(0.578383236111111); experimentalTimes.Add(0.562604740740741);
            referenceTimes.Add(0.565160833333333); experimentalTimes.Add(0.543455097222223);
            referenceTimes.Add(0.562653486111111); experimentalTimes.Add(0.600059615740741);
            referenceTimes.Add(0.540748606481482); experimentalTimes.Add(0.486345583333333);
            referenceTimes.Add(0.536829726851852); experimentalTimes.Add(0.531969347222222);
            referenceTimes.Add(0.534449712962963); experimentalTimes.Add(0.559214055555556);
            referenceTimes.Add(0.533727666666667); experimentalTimes.Add(0.522660541666667);
            referenceTimes.Add(0.532274611111111); experimentalTimes.Add(0.491062388888889);
            referenceTimes.Add(0.530040111111111); experimentalTimes.Add(0.522924013888889);
            referenceTimes.Add(0.530040111111111); experimentalTimes.Add(0.522924013888889);
            referenceTimes.Add(0.524368541666667); experimentalTimes.Add(0.555256490740741);
            referenceTimes.Add(0.524368541666667); experimentalTimes.Add(0.555256490740741);
            referenceTimes.Add(0.522399333333333); experimentalTimes.Add(0.519797388888889);
            referenceTimes.Add(0.511560847222223); experimentalTimes.Add(0.501836513888889);
            referenceTimes.Add(0.511175601851852); experimentalTimes.Add(0.459703189814815);
            referenceTimes.Add(0.507936398148148); experimentalTimes.Add(0.500109370370371);
            referenceTimes.Add(0.5071455); experimentalTimes.Add(0.502536375);
            referenceTimes.Add(0.50599887962963); experimentalTimes.Add(0.45592199537037);
            referenceTimes.Add(0.50280200462963); experimentalTimes.Add(0.451952717592593);
            referenceTimes.Add(0.492358180555556); experimentalTimes.Add(0.478852842592593);
            referenceTimes.Add(0.492358180555556); experimentalTimes.Add(0.487260231481482);
            referenceTimes.Add(0.492358180555556); experimentalTimes.Add(0.487260231481482);
            referenceTimes.Add(0.482623087962962); experimentalTimes.Add(0.480547458333333);
            referenceTimes.Add(0.481976152777778); experimentalTimes.Add(0.477399009259259);
            referenceTimes.Add(0.480717638888889); experimentalTimes.Add(0.477399009259259);
            referenceTimes.Add(0.476060300925926); experimentalTimes.Add(0.473418310185185);
            referenceTimes.Add(0.470236652777777); experimentalTimes.Add(0.467106865740741);
            referenceTimes.Add(0.464607407407407); experimentalTimes.Add(0.459000143518518);
            referenceTimes.Add(0.461820875); experimentalTimes.Add(0.468515287037037);
            referenceTimes.Add(0.448177587962963); experimentalTimes.Add(0.431503064814815);
            referenceTimes.Add(0.447478902777777); experimentalTimes.Add(0.444403842592593);
            referenceTimes.Add(0.434003763888889); experimentalTimes.Add(0.372923537037037);
            referenceTimes.Add(0.433283435185185); experimentalTimes.Add(0.430912115740741);
            referenceTimes.Add(0.423215546296297); experimentalTimes.Add(0.421395027777778);
            referenceTimes.Add(0.422813222222223); experimentalTimes.Add(0.42225887037037);
            referenceTimes.Add(0.414961583333333); experimentalTimes.Add(0.411330069444444);
            referenceTimes.Add(0.414416916666667); experimentalTimes.Add(0.411245115740741);
            referenceTimes.Add(0.413510541666667); experimentalTimes.Add(0.443997194444444);
            referenceTimes.Add(0.413151337962963); experimentalTimes.Add(0.40993325462963);
            referenceTimes.Add(0.410480319444444); experimentalTimes.Add(0.409141083333333);
            referenceTimes.Add(0.406959347222222); experimentalTimes.Add(0.387943152777778);
            referenceTimes.Add(0.40332400462963); experimentalTimes.Add(0.400914722222223);
            referenceTimes.Add(0.396567810185185); experimentalTimes.Add(0.394448819444444);
            referenceTimes.Add(0.382914287037037); experimentalTimes.Add(0.386745958333333);
            referenceTimes.Add(0.379019222222223); experimentalTimes.Add(0.382300875);
            referenceTimes.Add(0.376127305555556); experimentalTimes.Add(0.434959157407407);
            referenceTimes.Add(0.375147407407407); experimentalTimes.Add(0.379918388888889);
            referenceTimes.Add(0.371914523148148); experimentalTimes.Add(0.372038851851852);
            referenceTimes.Add(0.370115125); experimentalTimes.Add(0.376073134259259);
            referenceTimes.Add(0.368733486111111); experimentalTimes.Add(0.375561111111111);
            referenceTimes.Add(0.345897958333333); experimentalTimes.Add(0.348599625);
            referenceTimes.Add(0.344332476851852); experimentalTimes.Add(0.347839615740741);
            referenceTimes.Add(0.335925648148148); experimentalTimes.Add(0.339090416666667);
            referenceTimes.Add(0.335104115740741); experimentalTimes.Add(0.337939194444444);
            referenceTimes.Add(0.331696226851852); experimentalTimes.Add(0.335727694444444);
            referenceTimes.Add(0.330559342592593); experimentalTimes.Add(0.334229689814815);
            referenceTimes.Add(0.329436138888889); experimentalTimes.Add(0.332224462962963);
            referenceTimes.Add(0.326360819444444); experimentalTimes.Add(0.327463134259259);
            referenceTimes.Add(0.326360819444444); experimentalTimes.Add(0.327463134259259);
            referenceTimes.Add(0.325822212962963); experimentalTimes.Add(0.228486888888889);
            referenceTimes.Add(0.32555400462963); experimentalTimes.Add(0.207072231481482);
            referenceTimes.Add(0.324626800925926); experimentalTimes.Add(0.327922587962963);
            referenceTimes.Add(0.317040662037037); experimentalTimes.Add(0.320433319444444);
            referenceTimes.Add(0.314172652777778); experimentalTimes.Add(0.317779518518518);
            referenceTimes.Add(0.314369444444444); experimentalTimes.Add(0.339090416666667);
            referenceTimes.Add(0.312403050925926); experimentalTimes.Add(0.346615152777778);
            referenceTimes.Add(0.312233337962963); experimentalTimes.Add(0.315877069444444);
            referenceTimes.Add(0.309384490740741); experimentalTimes.Add(0.298279152777777);
            referenceTimes.Add(0.308070046296297); experimentalTimes.Add(0.313584583333333);
            referenceTimes.Add(0.307035175925926); experimentalTimes.Add(0.317779518518518);
            referenceTimes.Add(0.301942717592593); experimentalTimes.Add(0.305432106481482);
            referenceTimes.Add(0.298721287037037); experimentalTimes.Add(0.302559527777778);
            referenceTimes.Add(0.286201185185185); experimentalTimes.Add(0.289481546296297);
            referenceTimes.Add(0.282022458333333); experimentalTimes.Add(0.286212847222222);
            referenceTimes.Add(0.274845125); experimentalTimes.Add(0.279160574074074);
            referenceTimes.Add(0.274171972222223); experimentalTimes.Add(0.277855217592593);
            referenceTimes.Add(0.273568416666667); experimentalTimes.Add(0.276292597222222);
            referenceTimes.Add(0.273658898148148); experimentalTimes.Add(0.276206694444444);
            referenceTimes.Add(0.272455958333333); experimentalTimes.Add(0.249926611111111);
            referenceTimes.Add(0.272363263888889); experimentalTimes.Add(0.25589725);
            referenceTimes.Add(0.264558189814815); experimentalTimes.Add(0.267931486111111);
            referenceTimes.Add(0.254017055555556); experimentalTimes.Add(0.257335157407408);
            referenceTimes.Add(0.254103916666667); experimentalTimes.Add(0.257612439814815);
            referenceTimes.Add(0.246883671296297); experimentalTimes.Add(0.251118277777778);
            referenceTimes.Add(0.243115342592592); experimentalTimes.Add(0.248673027777778);
            referenceTimes.Add(0.239939523148148); experimentalTimes.Add(0.290373916666667);
            referenceTimes.Add(0.239853587962963); experimentalTimes.Add(0.249517527777778);
            referenceTimes.Add(0.238968208333333); experimentalTimes.Add(0.242422287037037);
            referenceTimes.Add(0.238677527777778); experimentalTimes.Add(0.216367189814815);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.23856875);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.23865725);
            referenceTimes.Add(0.233838111111111); experimentalTimes.Add(0.237792680555556);
            referenceTimes.Add(0.233282166666667); experimentalTimes.Add(0.216284643518518);
            referenceTimes.Add(0.230808773148148); experimentalTimes.Add(0.235571361111111);
            referenceTimes.Add(0.230461027777777); experimentalTimes.Add(0.235846722222223);
            referenceTimes.Add(0.229941638888889); experimentalTimes.Add(0.234481888888889);
            referenceTimes.Add(0.230025930555556); experimentalTimes.Add(0.234393976851852);
            referenceTimes.Add(0.227252976851852); experimentalTimes.Add(0.339611916666667);
            referenceTimes.Add(0.213242027777778); experimentalTimes.Add(0.216202097222222);
            referenceTimes.Add(0.209980361111111); experimentalTimes.Add(0.123754037037037);
            referenceTimes.Add(0.205638819444444); experimentalTimes.Add(0.209567398148148);
            referenceTimes.Add(0.18528350462963); experimentalTimes.Add(0.190166444444444);
            referenceTimes.Add(0.18528350462963); experimentalTimes.Add(0.124895893518518);
            referenceTimes.Add(0.160785194444444); experimentalTimes.Add(0.161402671296297);
            referenceTimes.Add(0.147514833333333); experimentalTimes.Add(0.18764725);
            referenceTimes.Add(0.146135847222222); experimentalTimes.Add(0.229847916666667);
            referenceTimes.Add(0.146329439814815); experimentalTimes.Add(0.139145736111111);
            referenceTimes.Add(0.1160105); experimentalTimes.Add(0.117611097222222);
            referenceTimes.Add(0.103025791666667); experimentalTimes.Add(0.268013842592593);


        }

        private static void ExampleTimesDB12(out List<double> experimentalTimes, out List<double> referenceTimes)
        {
            experimentalTimes = new List<double>();
            referenceTimes = new List<double>();
            referenceTimes.Add(0.597636236111111); experimentalTimes.Add(0.663321032407408);
            referenceTimes.Add(0.59678225); experimentalTimes.Add(0.608498402777778);
            referenceTimes.Add(0.59678225); experimentalTimes.Add(0.608498402777778);
            referenceTimes.Add(0.587572175925926); experimentalTimes.Add(0.599915097222222);
            referenceTimes.Add(0.582922912037037); experimentalTimes.Add(0.56941725);
            referenceTimes.Add(0.582922912037037); experimentalTimes.Add(0.56941725);
            referenceTimes.Add(0.565160833333333); experimentalTimes.Add(0.554531416666667);
            referenceTimes.Add(0.540748606481482); experimentalTimes.Add(0.499627);
            referenceTimes.Add(0.533727666666667); experimentalTimes.Add(0.54690425);
            referenceTimes.Add(0.533727666666667); experimentalTimes.Add(0.54690425);
            referenceTimes.Add(0.532274611111111); experimentalTimes.Add(0.542313194444444);
            referenceTimes.Add(0.532274611111111); experimentalTimes.Add(0.542313194444444);
            referenceTimes.Add(0.530040111111111); experimentalTimes.Add(0.536685958333333);
            referenceTimes.Add(0.530040111111111); experimentalTimes.Add(0.536685958333333);
            referenceTimes.Add(0.524368541666667); experimentalTimes.Add(0.531978805555556);
            referenceTimes.Add(0.524368541666667); experimentalTimes.Add(0.531978805555556);
            referenceTimes.Add(0.522399333333333); experimentalTimes.Add(0.528415365740741);
            referenceTimes.Add(0.521498861111111); experimentalTimes.Add(0.500280328703703);
            referenceTimes.Add(0.514312296296297); experimentalTimes.Add(0.523921430555556);
            referenceTimes.Add(0.514312296296297); experimentalTimes.Add(0.523921430555556);
            referenceTimes.Add(0.513626791666667); experimentalTimes.Add(0.526944444444444);
            referenceTimes.Add(0.511560847222223); experimentalTimes.Add(0.519076203703703);
            referenceTimes.Add(0.511175601851852); experimentalTimes.Add(0.468954625);
            referenceTimes.Add(0.507936398148148); experimentalTimes.Add(0.514771222222222);
            referenceTimes.Add(0.506911740740741); experimentalTimes.Add(0.514077847222222);
            referenceTimes.Add(0.506911740740741); experimentalTimes.Add(0.514077847222222);
            referenceTimes.Add(0.50599887962963); experimentalTimes.Add(0.465713125);
            referenceTimes.Add(0.492358180555556); experimentalTimes.Add(0.485336625);
            referenceTimes.Add(0.482623087962962); experimentalTimes.Add(0.454864291666667);
            referenceTimes.Add(0.481976152777778); experimentalTimes.Add(0.487725597222222);
            referenceTimes.Add(0.480717638888889); experimentalTimes.Add(0.484549444444444);
            referenceTimes.Add(0.480717638888889); experimentalTimes.Add(0.484549444444444);
            referenceTimes.Add(0.476060300925926); experimentalTimes.Add(0.487725597222222);
            referenceTimes.Add(0.472158078703704); experimentalTimes.Add(0.555036601851852);
            referenceTimes.Add(0.470236652777777); experimentalTimes.Add(0.476885560185185);
            referenceTimes.Add(0.464607407407407); experimentalTimes.Add(0.471892384259259);
            referenceTimes.Add(0.464607407407407); experimentalTimes.Add(0.459658226851852);
            referenceTimes.Add(0.451553518518518); experimentalTimes.Add(0.500912458333333);
            referenceTimes.Add(0.447478902777777); experimentalTimes.Add(0.453286208333333);
            referenceTimes.Add(0.445812375); experimentalTimes.Add(0.450913055555556);
            referenceTimes.Add(0.434003763888889); experimentalTimes.Add(0.434921638888889);
            referenceTimes.Add(0.433283435185185); experimentalTimes.Add(0.440888486111111);
            referenceTimes.Add(0.423215546296297); experimentalTimes.Add(0.426184777777777);
            referenceTimes.Add(0.422813222222223); experimentalTimes.Add(0.429272069444444);
            referenceTimes.Add(0.416682569444444); experimentalTimes.Add(0.440888486111111);
            referenceTimes.Add(0.414961583333333); experimentalTimes.Add(0.42271475);
            referenceTimes.Add(0.414416916666667); experimentalTimes.Add(0.422024125);
            referenceTimes.Add(0.413510541666667); experimentalTimes.Add(0.418894763888889);
            referenceTimes.Add(0.413151337962963); experimentalTimes.Add(0.424139777777778);
            referenceTimes.Add(0.410480319444444); experimentalTimes.Add(0.409617273148148);
            referenceTimes.Add(0.406959347222222); experimentalTimes.Add(0.476078902777777);
            referenceTimes.Add(0.404507273148148); experimentalTimes.Add(0.412667736111111);
            referenceTimes.Add(0.40332400462963); experimentalTimes.Add(0.412517365740741);
            referenceTimes.Add(0.397255791666667); experimentalTimes.Add(0.38378237037037);
            referenceTimes.Add(0.396684314814815); experimentalTimes.Add(0.400037898148148);
            referenceTimes.Add(0.396567810185185); experimentalTimes.Add(0.401443527777777);
            referenceTimes.Add(0.393576); experimentalTimes.Add(0.399197532407407);
            referenceTimes.Add(0.382914287037037); experimentalTimes.Add(0.390973893518518);
            referenceTimes.Add(0.379019222222223); experimentalTimes.Add(0.385564027777778);
            referenceTimes.Add(0.375147407407407); experimentalTimes.Add(0.381965611111111);
            referenceTimes.Add(0.371914523148148); experimentalTimes.Add(0.377038634259259);
            referenceTimes.Add(0.370115125); experimentalTimes.Add(0.3777245);
            referenceTimes.Add(0.368733486111111); experimentalTimes.Add(0.377854319444444);
            referenceTimes.Add(0.353105791666667); experimentalTimes.Add(0.358438416666667);
            referenceTimes.Add(0.348586518518518); experimentalTimes.Add(0.354800611111111);
            referenceTimes.Add(0.34817775); experimentalTimes.Add(0.353959263888889);
            referenceTimes.Add(0.344332476851852); experimentalTimes.Add(0.341361837962963);
            referenceTimes.Add(0.335925648148148); experimentalTimes.Add(0.341474180555556);
            referenceTimes.Add(0.335104115740741); experimentalTimes.Add(0.341361837962963);
            referenceTimes.Add(0.331696226851852); experimentalTimes.Add(0.337225722222223);
            referenceTimes.Add(0.3293345); experimentalTimes.Add(0.335308069444444);
            referenceTimes.Add(0.329232861111111); experimentalTimes.Add(0.334622601851852);
            referenceTimes.Add(0.328091777777778); experimentalTimes.Add(0.335308069444444);
            referenceTimes.Add(0.32737637962963); experimentalTimes.Add(0.332687486111111);
            referenceTimes.Add(0.326360819444444); experimentalTimes.Add(0.331028013888889);
            referenceTimes.Add(0.326360819444444); experimentalTimes.Add(0.330672958333333);
            referenceTimes.Add(0.325822212962963); experimentalTimes.Add(0.245977694444444);
            referenceTimes.Add(0.32555400462963); experimentalTimes.Add(0.234149347222223);
            referenceTimes.Add(0.324626800925926); experimentalTimes.Add(0.399107763888889);
            referenceTimes.Add(0.317040662037037); experimentalTimes.Add(0.33547600462963);
            referenceTimes.Add(0.314172652777778); experimentalTimes.Add(0.321020027777777);
            referenceTimes.Add(0.314369444444444); experimentalTimes.Add(0.321117222222223);
            referenceTimes.Add(0.312403050925926); experimentalTimes.Add(0.348947833333333);
            referenceTimes.Add(0.309384490740741); experimentalTimes.Add(0.316939833333333);
            referenceTimes.Add(0.307035175925926); experimentalTimes.Add(0.321117222222223);
            referenceTimes.Add(0.304361902777777); experimentalTimes.Add(0.350828638888889);
            referenceTimes.Add(0.301942717592593); experimentalTimes.Add(0.309093268518518);
            referenceTimes.Add(0.298721287037037); experimentalTimes.Add(0.305574199074074);
            referenceTimes.Add(0.292886333333333); experimentalTimes.Add(0.301040555555556);
            referenceTimes.Add(0.286201185185185); experimentalTimes.Add(0.291923740740741);
            referenceTimes.Add(0.282022458333333); experimentalTimes.Add(0.290304555555556);
            referenceTimes.Add(0.27859875); experimentalTimes.Add(0.250265986111111);
            referenceTimes.Add(0.274845125); experimentalTimes.Add(0.284198458333333);
            referenceTimes.Add(0.273568416666667); experimentalTimes.Add(0.271032060185185);
            referenceTimes.Add(0.273658898148148); experimentalTimes.Add(0.279981162037037);
            referenceTimes.Add(0.272363263888889); experimentalTimes.Add(0.258889888888889);
            referenceTimes.Add(0.264558189814815); experimentalTimes.Add(0.270930319444444);
            referenceTimes.Add(0.259335958333333); experimentalTimes.Add(0.265921763888889);
            referenceTimes.Add(0.254103916666667); experimentalTimes.Add(0.260588287037037);
            referenceTimes.Add(0.246883671296297); experimentalTimes.Add(0.254057462962963);
            referenceTimes.Add(0.243115342592592); experimentalTimes.Add(0.249919083333333);
            referenceTimes.Add(0.239939523148148); experimentalTimes.Add(0.240620763888889);
            referenceTimes.Add(0.239853587962963); experimentalTimes.Add(0.247266902777778);
            referenceTimes.Add(0.238968208333333); experimentalTimes.Add(0.24579537037037);
            referenceTimes.Add(0.238677527777778); experimentalTimes.Add(0.221383699074074);
            referenceTimes.Add(0.238316814814815); experimentalTimes.Add(0.245977694444444);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.24180087037037);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.24180087037037);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.24180087037037);
            referenceTimes.Add(0.234453972222223); experimentalTimes.Add(0.241712032407408);
            referenceTimes.Add(0.233282166666667); experimentalTimes.Add(0.220882180555556);
            referenceTimes.Add(0.230808773148148); experimentalTimes.Add(0.238542194444444);
            referenceTimes.Add(0.230461027777777); experimentalTimes.Add(0.241623194444444);
            referenceTimes.Add(0.230461027777777); experimentalTimes.Add(0.243933814814815);
            referenceTimes.Add(0.229941638888889); experimentalTimes.Add(0.237525111111111);
            referenceTimes.Add(0.230025930555556); experimentalTimes.Add(0.237608740740741);
            referenceTimes.Add(0.227252976851852); experimentalTimes.Add(0.233867430555556);
            referenceTimes.Add(0.209980361111111); experimentalTimes.Add(0.218574291666667);
            referenceTimes.Add(0.205638819444444); experimentalTimes.Add(0.215441578703703);
            referenceTimes.Add(0.18528350462963); experimentalTimes.Add(0.198003782407408);
            referenceTimes.Add(0.160785194444444); experimentalTimes.Add(0.174658625);
            referenceTimes.Add(0.147514833333333); experimentalTimes.Add(0.161055087962963);
            referenceTimes.Add(0.146135847222222); experimentalTimes.Add(0.233961402777778);
            referenceTimes.Add(0.146329439814815); experimentalTimes.Add(0.159930074074074);
            referenceTimes.Add(0.1160105); experimentalTimes.Add(0.127390092592592);
            referenceTimes.Add(0.103025791666667); experimentalTimes.Add(0.259811277777778);


        }
    }
}
