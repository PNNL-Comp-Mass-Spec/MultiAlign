using MultiAlign.Data;
using Ookii.Dialogs.Wpf;
using System;
using System.Windows.Input;

namespace MultiAlign.Commands
{
    public class BrowseSaveFileCommand: ICommand
    {

        public event EventHandler CanExecuteChanged;
        private VistaSaveFileDialog                 m_dialog;
        private Action<string> m_action;

        public BrowseSaveFileCommand(string filter)
        {
            m_dialog                = new VistaSaveFileDialog();
            m_dialog.Filter         = filter;
            m_dialog.AddExtension   = true;
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
            bool? result = m_dialog.ShowDialog();

            if (result == true)
            {            
                if (m_action != null)
                {
                    m_action(m_dialog.FileName);
                }
            }
        }
    }
}
