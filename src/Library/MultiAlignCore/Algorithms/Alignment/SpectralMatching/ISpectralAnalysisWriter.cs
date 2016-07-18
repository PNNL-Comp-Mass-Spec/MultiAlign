namespace MultiAlignCore.Algorithms.Alignment.SpectralMatching
{
    public interface ISpectralAnalysisWriter
    {
        void Write(SpectralAnalysis analysis);
        void WriteLine(string value);
        void Close();
    }
}
