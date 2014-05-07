using System;
using System.Windows.Forms;
using MultiAlign.Data;

namespace MultiAlign.Commands.Viewers
{
    public class LoadExistingAnalysisCommand : BaseCommand
    {
        private readonly OpenFileDialog m_analysisLoadDialog; 
        public event EventHandler<OpenAnalysisArgs> ExistingAnalysisSelected;

        public LoadExistingAnalysisCommand()
            : base(null, AlwaysPass)
        {
            m_analysisLoadDialog = new OpenFileDialog();
        }

        public override void Execute(object parameter)
        {
            var result = m_analysisLoadDialog.ShowDialog();

            if (result != DialogResult.OK) return;
            var filename     = m_analysisLoadDialog.FileName;
            var path         = System.IO.Path.GetDirectoryName(filename); 
            var name         = System.IO.Path.GetFileName(filename);

            var newAnalysis = new RecentAnalysis(path, name);                

            if (ExistingAnalysisSelected != null)
            {
                ExistingAnalysisSelected(this, new OpenAnalysisArgs(newAnalysis));
            }
        }
    }
}
