using MultiAlignCore.Algorithms.Workflow;
using MultiAlignTestSuite.Algorithms.FeatureFinding;

namespace MultiAlignTestSuite
{
    /// <summary>
    /// Base class to assist resolving test data
    /// </summary>
    public abstract class TestBase : WorkflowBase
    {
        /// <summary>
        /// Absolute base path to main (root) test data directory.
        /// </summary>
        private readonly string m_pathBase;
        /// <summary>
        /// Absolute base path to a new test folder directory.
        /// </summary>
        private readonly string m_outputPathBase;

        protected TestBase()
        {
            TextDelimiter    = ",";
            m_pathBase       = TestPathSingleton.TestDirectory;
            m_outputPathBase = TestPathSingleton.OutputDirectory;
        }
        /// <summary>
        /// Resolves the absolute path for a given relative test data path.
        /// </summary>
        /// <param name="path">Relative path to source.</param>
        /// <returns>Absolute path based on common test data directory path.</returns>
        protected string GetPath(string path)
        {
            return System.IO.Path.Combine(m_pathBase, path);
        }

        protected string GetOutputPath(string path)
        {
            return System.IO.Path.Combine(m_outputPathBase, path);
        }
        protected string TextDelimiter
        {
            get; private set; 
        }        
    }
}
