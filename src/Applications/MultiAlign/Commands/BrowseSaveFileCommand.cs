using System;
using Ookii.Dialogs.Wpf;

namespace MultiAlign.Commands
{
    public sealed class BrowseSaveFileCommand : BaseCommand
    {
        private readonly VistaSaveFileDialog m_dialog;
        private readonly Action<string> m_saveAction;

        public BrowseSaveFileCommand(Action<string> actionOnExecute, Func<object, bool> executeFunc, string filter)
            : base(null, executeFunc)
        {
            m_saveAction = actionOnExecute;
            m_dialog = new VistaSaveFileDialog {Filter = filter, AddExtension = true};
        }

        public override void Execute(object parameter)
        {
            bool? result = m_dialog.ShowDialog();
            if (result != true) return;

            if (m_saveAction != null)
            {
                m_saveAction(m_dialog.FileName);
            }

            base.Execute(parameter);
        }
    }
}