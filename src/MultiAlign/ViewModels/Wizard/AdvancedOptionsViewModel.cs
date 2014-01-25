using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Integration;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels.Wizard
{
    public class AdvancedOptionsViewModel: ViewModelBase
    {

        MultiAlignParameterFileEditor.ParameterFileEditor m_editor;

        public AdvancedOptionsViewModel(AnalysisOptions options)
        {
            m_editor = new MultiAlignParameterFileEditor.ParameterFileEditor();
            m_editor.SetOptions(options, "");

            Editor = new WindowsFormsHost() { Child = m_editor };
        }

        public WindowsFormsHost Editor { get; private set; }
    }
}
