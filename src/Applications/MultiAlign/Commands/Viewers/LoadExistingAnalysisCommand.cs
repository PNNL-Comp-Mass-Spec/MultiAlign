using System;
using System.IO;
using Microsoft.Win32;
using MultiAlign.Data;

namespace MultiAlign.Commands.Viewers
{
    public class LoadExistingAnalysisCommand : BaseCommand
    {
        private readonly OpenFileDialog m_analysisLoadDialog;

        public LoadExistingAnalysisCommand()
            : base(null, AlwaysPass)
        {
            const string fileTypeFilter = "Input Files (*.db3)| *.db3| All Files (*.*)|*.*";
            m_analysisLoadDialog = new OpenFileDialog {Filter = fileTypeFilter};
        }

        public event EventHandler<OpenAnalysisArgs> ExistingAnalysisSelected;

        public override void Execute(object parameter)
        {
            var result = m_analysisLoadDialog.ShowDialog();

            if (result == null || !result.Value)
                return;

            var filename = m_analysisLoadDialog.FileName;
            var path = Path.GetDirectoryName(filename);
            var name = Path.GetFileName(filename);

            var newAnalysis = new RecentAnalysis(path, name);

            if (ExistingAnalysisSelected != null)
            {
                ExistingAnalysisSelected(this, new OpenAnalysisArgs(newAnalysis));
            }
        }
    }
}