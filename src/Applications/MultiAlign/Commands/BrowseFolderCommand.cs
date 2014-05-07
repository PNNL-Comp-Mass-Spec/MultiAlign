using System;

namespace MultiAlign.Commands
{
    public sealed class BrowseFolderCommand: BaseCommand
    {

        private readonly Ookii.Dialogs.VistaFolderBrowserDialog m_folderBrowser;        
        private readonly Action<string> m_saveAction;
        
        public BrowseFolderCommand(Action<string> actionOnExecute)
            : base(null, AlwaysPass)
        {
            m_folderBrowser = new Ookii.Dialogs.VistaFolderBrowserDialog {ShowNewFolderButton = true};
            m_saveAction = actionOnExecute;
        }


        public override void Execute(object parameter)
        {
            var result = m_folderBrowser.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK) 
                return;            
            if (m_saveAction != null)
                m_saveAction(m_folderBrowser.SelectedPath);            
        }
    }
}
