using System;
using System.IO;
using System.Windows.Forms;
using MultiAlign.Data;

namespace MultiAlign.Commands.Viewers
{
    public class LoadExistingAnalysisCommand : BaseCommand
    {
        private readonly OpenFileDialog m_analysisLoadDialog;

        public LoadExistingAnalysisCommand()
            : base(null, AlwaysPass)
        {
            m_analysisLoadDialog = new OpenFileDialog();
        }

        public event EventHandler<OpenAnalysisArgs> ExistingAnalysisSelected;

        public override void Execute(object parameter)
        {
            var result = m_analysisLoadDialog.ShowDialog();

            if (result != DialogResult.OK) return;
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