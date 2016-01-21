namespace MultiAlignRogue.Alignment
{
    using System.Collections.Generic;

    using MultiAlignCore.Data.Alignment;

    public interface IAlignmentWindowFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(IEnumerable<AlignmentViewModel> alignment);
    }
}
