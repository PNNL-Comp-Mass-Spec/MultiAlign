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

        public event EventHandler CanExecuteChanged;
        private Action      m_action;
        private Func<bool>  m_determineExecute;
        bool m_canExecute = true;

        public BaseCommandBridge(CommandDelegate newDelegate)
        {
            m_delegate          = newDelegate;
            m_determineExecute  = null;
        }

        public BaseCommandBridge(Action action, Func<bool> determineExecute)
        {
            m_delegate          = null;
            m_action            = action;
            m_determineExecute  = determineExecute;
        }

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            if (m_determineExecute != null)
            {

                bool execute = m_determineExecute();
                if (m_canExecute != execute)
                {
                    m_canExecute = execute;
                    if (CanExecuteChanged !=null )
                    {
                        CanExecuteChanged(this, null);
                    }
                }
            }
            return m_canExecute;
        }

        public void Execute(object parameter)
        {
            if (m_delegate != null)
            {
                m_delegate(parameter);
            }
            if (m_action != null)
            {
                m_action.Invoke();
            }
        }

        #endregion
    }

    public delegate void CommandDelegate(object parameter);
}
