namespace MultiAlignCore.Algorithms.Alignment.SpectralMatching
{
    public interface IAlignmentFunction
    {
        double AlignNet(double value);
        double AlignMass(double value);
    }
}