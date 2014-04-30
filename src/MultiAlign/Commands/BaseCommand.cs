using System;
using System.Windows.Input;

namespace MultiAlign.Commands
{
    /// <summary>
    /// Base Command Bridge for easily synching a command back to a view model to reduce the number 
    /// of required commands.
    /// </summary>
    public sealed class BaseCommandBridge: ICommand 
    {
        readonly CommandDelegate    m_delegate;

        public event EventHandler   CanExecuteChanged;
        private readonly Action      m_action;
        private readonly Func<bool>  m_determineExecute;
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
            if (m_determineExecute == null) 
                return m_canExecute;

            var execute = m_determineExecute();

            if (m_canExecute == execute) 
                return m_canExecute;

            m_canExecute = execute;
            if (CanExecuteChanged !=null )
            {
                CanExecuteChanged(this, null);
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
