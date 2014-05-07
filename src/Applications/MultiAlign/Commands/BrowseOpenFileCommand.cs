using System;
using System.Windows.Input;

namespace MultiAlign.Commands
{
    public sealed class BrowseOpenFileCommand: ICommand
    {

        private readonly Ookii.Dialogs.VistaOpenFileDialog m_dialog;
        private readonly Action<string> m_action;



        private BrowseOpenFileCommand(string filter)
        {
            m_dialog = new Ookii.Dialogs.VistaOpenFileDialog {Filter = filter};
        }

        public BrowseOpenFileCommand(Action<string> actionOnExecute, string filter)
            : this (filter)
        {
            
            m_action = actionOnExecute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {                
            var result = m_dialog.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK) 
                return;

            if (m_action != null)
            {
                m_action(m_dialog.FileName);
            }
        }
    }
}
