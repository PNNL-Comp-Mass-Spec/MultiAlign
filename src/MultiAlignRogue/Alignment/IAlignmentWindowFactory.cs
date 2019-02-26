namespace MultiAlignRogue.Alignment
{
    using System.Collections.Generic;

    public interface IAlignmentWindowFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(IEnumerable<AlignmentViewModel> alignment);
    }
}
