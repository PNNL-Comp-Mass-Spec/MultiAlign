using System;

namespace MultiAlignTestSuite.Algorithms.FeatureFinding
{
    /// <summary>
    /// Common locator for test data.
    /// </summary>
    public static class TestPathSingleton
    {
        static TestPathSingleton()
        {
            TestDirectory = @"\\Proto-2\UnitTest_Files\MultiAlignTest\";
            var now         = DateTime.Now;
            OutputDirectory = string.Format(@"\\Proto-2\UnitTest_Files\MultiAlignTest\testResults\{0}_{1}_{2}_{3}_{4}_{5}",
                                                now.Year,
                                                now.Month,
                                                now.Day,
                                                now.Hour,
                                                now.Minute,
                                                now.Second);
        }
        /// <summary>
        /// Maintains a single reference to the root test data path (directory)
        /// </summary>
        public static string TestDirectory
        {
            get; private set;
        }

        public static string OutputDirectory
        {
            get; private set;
        }
    }
}