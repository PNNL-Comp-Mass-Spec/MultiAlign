using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MultiAlign.Commands
{
    /// <summary>
    /// Base Command Bridge for easily synching a command back to a view model to reduce the number 
    /// of required commands.
    /// </summary>
    public class BaseCommandBridge: ICommand 
    {
        CommandDelegate m_delegate;

        public BaseCommandBridge(CommandDelegate newDelegate )
        {
            m_delegate = newDelegate;
        }

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (m_delegate != null)
            {
                m_delegate(parameter);
            }
        }

        #endregion
    }

    public delegate void CommandDelegate(object parameter);
}
