namespace MultiAlign.Data
{
    /// <summary>
    /// Class that holds information about a recent analysis.
    /// </summary>
    public class RecentAnalysis
    {
        public RecentAnalysis(string path, string name)
        {
            Path = path;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the path of the analysis.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the name of the analysis.
        /// </summary>
        public string Name { get; set; }
    }
}