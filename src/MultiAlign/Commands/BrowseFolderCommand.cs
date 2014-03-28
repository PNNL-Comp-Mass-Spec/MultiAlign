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
        
        private Ookii.Dialogs.VistaFolderBrowserDialog m_folderBrowser;
        public event EventHandler<OpenAnalysisArgs> FolderSelected;
        private Action<string> m_action;

        public BrowseFolderCommand()
        {
            m_folderBrowser = new Ookii.Dialogs.VistaFolderBrowserDialog();
            m_folderBrowser.ShowNewFolderButton = true;
        }

        public BrowseFolderCommand(Action<string> actionOnExecute)
        {
            m_folderBrowser = new Ookii.Dialogs.VistaFolderBrowserDialog();
            m_folderBrowser.ShowNewFolderButton = true;

            m_action = actionOnExecute;
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

                var newAnalysis = new RecentAnalysis(path, name);
                if (FolderSelected != null)
                {
                    FolderSelected(this, new OpenAnalysisArgs(newAnalysis));
                }

                if (m_action != null)
                {
                    m_action(path);

                }
            }
        }
    }
}
