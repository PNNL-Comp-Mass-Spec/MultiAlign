using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignTestSuite.Papers.Alignment.IO
{

    /// <summary>
    /// Factory for creating figure writers
    /// </summary>
    public static class AlignmentAnalysisWriterFactory
    {
        public static ISpectralAnalysisWriter Create(AlignmentFigureType type)
        {
            ISpectralAnalysisWriter writer = null;
            switch (type)
            {
                case AlignmentFigureType.Figure1:
                    writer = new SpectralWriterFigureOne();
                    break;
                case AlignmentFigureType.Figure2:
                    writer = new SpectralWriterFigureTwo();
                    break;
                default:
                    break;
            }
            return writer;
        }
    }
}
