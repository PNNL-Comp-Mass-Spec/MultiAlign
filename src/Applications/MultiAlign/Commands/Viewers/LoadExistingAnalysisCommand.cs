using System;
using System.Windows.Forms;
using System.Windows.Input;
using MultiAlign.Data;
using MultiAlign.Data.States;
using System.Windows.Shapes;

namespace MultiAlign.Commands.Viewers
{
    public class LoadExistingAnalysisCommand : ICommand
    {
        private OpenFileDialog m_analysisLoadDialog; 
        public event EventHandler<OpenAnalysisArgs> ExistingAnalysisSelected;

        public LoadExistingAnalysisCommand()
        {
            m_analysisLoadDialog = new OpenFileDialog();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            string message = "";

            bool canStart  = true; // StateModerator.CanOpenAnalysis(ref message);
            
            if (!canStart)
            {
                ApplicationStatusMediator.Mediator.Status = message;
                // Display a message saying whether we want to cancel or not.                
                return;
            }

            System.Windows.Forms.DialogResult result = m_analysisLoadDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {

                string filename     = m_analysisLoadDialog.FileName;
                string path         = System.IO.Path.GetDirectoryName(filename); 
                string name         = System.IO.Path.GetFileName(filename);

                RecentAnalysis newAnalysis = new RecentAnalysis(path, name);                

                if (ExistingAnalysisSelected != null)
                {
                    ExistingAnalysisSelected(this, new OpenAnalysisArgs(newAnalysis));
                }

            }
        }
    }
}
