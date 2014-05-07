using MultiAlign.Data;
using System;
using System.Windows.Input;

namespace MultiAlign.Commands
{
    public sealed class BrowseFolderCommand: ICommand
    {

        private readonly Ookii.Dialogs.VistaFolderBrowserDialog m_folderBrowser;
        private readonly Action<string> m_action;

        public  event EventHandler CanExecuteChanged;
        public  event EventHandler<OpenAnalysisArgs> FolderSelected;

        public BrowseFolderCommand()
        {
            m_folderBrowser = new Ookii.Dialogs.VistaFolderBrowserDialog {ShowNewFolderButton = true};
        }

        public BrowseFolderCommand(Action<string> actionOnExecute)
        {
            m_folderBrowser = new Ookii.Dialogs.VistaFolderBrowserDialog {ShowNewFolderButton = true};

            m_action = actionOnExecute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var result = m_folderBrowser.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK) 
                return;

            var path          = m_folderBrowser.SelectedPath;
            const string name = "";

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
