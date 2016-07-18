namespace MultiAlignCore.Algorithms.Statistics
{
    public class HypothesisTestingFactory
    {
        public static IHypothesisTestingTwoSample CreateTests(HypothesisTests test)
        {
            IHypothesisTestingTwoSample newTest = null;

            switch (test)
            {
                case HypothesisTests.TTest:
                    newTest = new StudentTTest();
                    break;
                case HypothesisTests.MannWhitneyU:
                    newTest = new MannWhitneyTest();
                    break;
                case HypothesisTests.KolmogorovSmirnov:
                    newTest = new KolmogorovSmirnovTest();
                    break;
            }

            return newTest;
        }
    }
}
