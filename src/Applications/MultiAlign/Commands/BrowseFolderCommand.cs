using System;
using System.Windows.Forms;
using Ookii.Dialogs;

namespace MultiAlign.Commands
{
    public sealed class BrowseFolderCommand : BaseCommand
    {
        private readonly VistaFolderBrowserDialog m_folderBrowser;
        private readonly Action<string> m_saveAction;

        public BrowseFolderCommand(Action<string> actionOnExecute)
            : base(null, AlwaysPass)
        {
            m_folderBrowser = new VistaFolderBrowserDialog {ShowNewFolderButton = true};
            m_saveAction = actionOnExecute;
        }


        public override void Execute(object parameter)
        {
            DialogResult result = m_folderBrowser.ShowDialog();

            if (result != DialogResult.OK)
                return;
            if (m_saveAction != null)
                m_saveAction(m_folderBrowser.SelectedPath);
        }
    }
}