using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.STACTests
{
    [TestFixture]
    public class STACTests
    {
        //[Test]
        ////TODO: Change the method name here to be capitalized.
        //public void executeSTACOnOrbitrapDataTest1()
        //{
        //    List<UMC> umcList = new List<UMC>();
        //    List<MassTag> massTagList = new List<MassTag>();

        //    loadOrbitrapData(ref umcList, ref massTagList);

        //    Assert.AreEqual(14182, umcList.Count);
        //    Assert.AreEqual(36549, massTagList.Count);

        //    umcList = (from n in umcList where n.ChargeState==3 select n).ToList();
        //    umcList = umcList.Take(10).ToList();

        //    FeatureMatcherParameters fmParams = new FeatureMatcherParameters();
        //    fmParams.ShouldCalculateHistogramFDR = false;
        //    fmParams.ShouldCalculateShiftFDR = false;
        //    fmParams.ShouldCalculateSLiC = false;
        //    fmParams.ShouldCalculateSTAC = false;

        //    PNNLOmics.Algorithms.FeatureMatcher.FeatureMatcher<UMC, MassTag> fm = new PNNLOmics.Algorithms.FeatureMatcher.FeatureMatcher<UMC, MassTag>(umcList, massTagList, fmParams);
        //    fm.MatchFeatures();

        //    //Assert.AreNotEqual(0, fm.MatchList.Count);
        //    Assert.AreEqual(11, fm.MatchList.Count);

        //    //bool useDriftDimension=false;
        //    //bool usePrior = false;
        //    //STACInformation stac = new STACInformation(useDriftDimension);
        //    //stac.PerformSTAC<UMC, MassTag>(fm.MatchList,new FeatureMatcherTolerances(), useDriftDimension, usePrior);

        //}




        //private void loadOrbitrapData(ref List<UMC> umcList, ref List<MassTag> massTagList)
        //{
        //    string umcFilePath = FileReferences.OrbitrapUMCFile1;
        //    string massTagFilePath = FileReferences.OrbitrapMassTagFile;

        //    UMCImporter importer = new UMCImporter(umcFilePath, '\t');
        //    MassTagTextFileImporter mtImporter = new MassTagTextFileImporter(massTagFilePath);
        //    umcList = importer.Import();
        //    massTagList = mtImporter.Import();
        //}


    }
}
