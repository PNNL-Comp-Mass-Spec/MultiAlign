namespace MultiAlignRogue.Alignment
{
    using System.Collections.Generic;

    class AlignmentViewFactory : IAlignmentWindowFactory
    {
        public void CreateNewWindow()
        {

        }

        public void CreateNewWindow(IEnumerable<AlignmentViewModel> alignments)
        {
            AlignmentView window = new AlignmentView
            {
                DataContext = alignments
            };

            window.Show();
        }
    }
}
