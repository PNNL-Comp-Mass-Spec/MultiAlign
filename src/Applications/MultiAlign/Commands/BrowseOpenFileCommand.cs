using System;

namespace MultiAlign.Commands
{
    public sealed class BrowseOpenFileCommand: BaseCommand
    {

        private readonly Ookii.Dialogs.VistaOpenFileDialog m_dialog;
        private readonly Action<string> m_browseAction;

        private BrowseOpenFileCommand(string filter)
            : base(null, AlwaysPass)
        {
            m_dialog = new Ookii.Dialogs.VistaOpenFileDialog {Filter = filter};
        }

        public BrowseOpenFileCommand(Action<string> actionOnExecute, string filter)
            : this (filter)
        {
            m_browseAction = actionOnExecute;
        }
        public override void Execute(object parameter)
        {                
            var result = m_dialog.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK) 
                return;

            if (m_browseAction != null)
            {
                m_browseAction(m_dialog.FileName);
            }
        }
    }
}
