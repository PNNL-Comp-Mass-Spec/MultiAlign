using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Parameters
{
    public interface IParameterFileReader
    {
        void ReadParameterFile(string parameterFilePath, ref MultiAlignAnalysis analysis);
    }
}
