using PNNLProteomics.Data;

namespace PNNLProteomics.IO
{
    public interface IParameterFileReader
    {
        void ReadParameterFile(string parameterFilePath, ref MultiAlignAnalysis analysis);
    }
}
