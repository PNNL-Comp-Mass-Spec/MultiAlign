using System;
using Ookii.Dialogs.Wpf;

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
            var result = m_folderBrowser.ShowDialog();

            if (result == null || !result.Value)
                return;
            if (m_saveAction != null)
                m_saveAction(m_folderBrowser.SelectedPath);
        }
    }
}