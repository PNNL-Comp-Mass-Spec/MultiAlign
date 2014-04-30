using Ookii.Dialogs.Wpf;
using System;
using System.Windows.Input;

namespace MultiAlign.Commands
{
    public sealed class BrowseSaveFileCommand: ICommand
    {

        public event EventHandler CanExecuteChanged;
        private readonly VistaSaveFileDialog                 m_dialog;
        private readonly Action<string> m_action;

        private BrowseSaveFileCommand(string filter)
        {
            m_dialog                = new VistaSaveFileDialog {Filter = filter, AddExtension = true};
        }

        public BrowseSaveFileCommand(Action<string> actionOnExecute, string filter)
            : this (filter)
        {            
            m_action            = actionOnExecute;            
        }

        public bool CanExecute(object parameter)
        {             
            return true;
        }


        public void Execute(object parameter)
        {                
            var result = m_dialog.ShowDialog();

            if (result != true) return;

            if (m_action != null)
            {
                m_action(m_dialog.FileName);
            }
        }
    }
}
