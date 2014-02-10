using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MultiAlign.Data;

namespace MultiAlign.Commands
{
    public class BrowseOpenFileCommand: ICommand
    {

        private Ookii.Dialogs.VistaOpenFileDialog m_dialog;
        public event EventHandler<OpenAnalysisArgs> FolderSelected;
        private Action<string> m_action;

        public BrowseOpenFileCommand(string filter)
        {
            m_dialog = new Ookii.Dialogs.VistaOpenFileDialog();
            m_dialog.Filter = filter;
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
            System.Windows.Forms.DialogResult result = m_dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {            
                if (m_action != null)
                {
                    m_action(m_dialog.FileName);
                }
            }
        }
    }
}
