using MultiAlignCore.Algorithms.Options;
using System.Windows.Forms.Integration;
using MultiAlignParameterFileEditor;

namespace MultiAlign.ViewModels.Wizard
{
    public class AdvancedOptionsViewModel: ViewModelBase
    {
        public AdvancedOptionsViewModel(MultiAlignAnalysisOptions options)
        {
            var editor = new ParameterFileEditor();
            editor.SetOptions(options, "");

            Editor = new WindowsFormsHost { Child = editor };
        }

        public WindowsFormsHost Editor { get; private set; }
    }
}
