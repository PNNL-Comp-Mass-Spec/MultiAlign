using System;
using System.Windows.Input;

namespace MultiAlign.Commands
{
    public class BaseCommand : ICommand
    {        
        public event EventHandler CanExecuteChanged;
        private readonly Action m_action;
        private readonly Func<object, bool> m_executeCheckFunc;


        public BaseCommand(Action actionOnExecute)
            : this(actionOnExecute, AlwaysPass)
        {
            
        }
        public BaseCommand(Action actionOnExecute, Func<object, bool> executeFunc)          
        {            
            m_action           = actionOnExecute;
            m_executeCheckFunc = executeFunc;
        }

        public void InvokeCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public virtual bool CanExecute(object parameter)
        {             
            return m_executeCheckFunc(parameter);
        }

        public virtual void Execute(object parameter)
        {                            
            if (m_action != null)
            {
                m_action();
            }
        }

        public static bool AlwaysPass(object parameter)
        {
            return true;
        }
    }
}
