#region

using PNNLOmics.Algorithms.Alignment.SpectralMatching;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment.IO
{
    /// <summary>
    ///     Factory for creating figure writers
    /// </summary>
    public static class AlignmentAnalysisWriterFactory
    {
        public static ISpectralAnalysisWriter Create(AlignmentFigureType type, string name)
        {
            ISpectralAnalysisWriter writer = null;
            switch (type)
            {
                case AlignmentFigureType.Figure1:
                    writer = new SpectralWriterFigureOne(name, BasePath);
                    break;
                case AlignmentFigureType.Figure2:
                    writer = new SpectralWriterFigureTwo(name, BasePath);
                    break;
                case AlignmentFigureType.Figure3:
                    writer = new SpectralWriterFigureThree(name, BasePath);
                    break;
            }
            return writer;
        }

        /// <summary>
        ///     Gets or sets the base path for the data.
        /// </summary>
        public static string BasePath { get; set; }
    }
}