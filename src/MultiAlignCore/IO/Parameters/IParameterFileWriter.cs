using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Parameters
{
    public interface IParameterFileWriter
    {
        void WriteParameterFile(string parameterFilePath, MultiAlignAnalysis analysis);
    }
}
