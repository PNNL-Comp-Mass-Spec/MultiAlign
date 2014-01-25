using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MultiAlign.Data;

namespace MultiAlign.Commands
{
    public class BrowseFolderCommand: ICommand
    {
        
        private System.Windows.Forms.FolderBrowserDialog m_folderBrowser;
        public event EventHandler<OpenAnalysisArgs> FolderSelected;

        public BrowseFolderCommand()
        {
            m_folderBrowser = new System.Windows.Forms.FolderBrowserDialog();            
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            string message = "";
            
            System.Windows.Forms.DialogResult result = m_folderBrowser.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {

                string path = m_folderBrowser.SelectedPath;
                string name = "";

                RecentAnalysis newAnalysis = new RecentAnalysis(path, name);
                if (FolderSelected != null)
                {
                    FolderSelected(this, new OpenAnalysisArgs(newAnalysis));
                }
            }
        }
    }
}
