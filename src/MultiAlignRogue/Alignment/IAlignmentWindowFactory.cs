namespace MultiAlignRogue.Alignment
{
    using MultiAlignCore.Data.Alignment;

    public interface IAlignmentWindowFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(classAlignmentData alignment);
    }
}
