using PNNLProteomics.Data;

namespace PNNLProteomics.IO
{
    public interface IParameterFileWriter
    {
        void WriteParameterFile(string parameterFilePath, MultiAlignAnalysis analysis);
    }
}
