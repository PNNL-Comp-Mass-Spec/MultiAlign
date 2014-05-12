using System;
using System.Windows.Forms;
using Ookii.Dialogs;

namespace MultiAlign.Commands
{
    public sealed class BrowseOpenFileCommand : BaseCommand
    {
        private readonly Action<string> m_browseAction;
        private readonly VistaOpenFileDialog m_dialog;

        private BrowseOpenFileCommand(string filter)
            : base(null, AlwaysPass)
        {
            m_dialog = new VistaOpenFileDialog {Filter = filter};
        }

        public BrowseOpenFileCommand(Action<string> actionOnExecute, string filter)
            : this(filter)
        {
            m_browseAction = actionOnExecute;
        }

        public override void Execute(object parameter)
        {
            var result = m_dialog.ShowDialog();

            if (result != DialogResult.OK)
                return;

            if (m_browseAction != null)
            {
                m_browseAction(m_dialog.FileName);
            }
        }
    }
}