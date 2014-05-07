using MultiAlign.ViewModels.Wizard;

namespace MultiAlign.Commands.Wizard
{
    public sealed class AnalysisBackCommand: BaseCommand
    {
        private readonly AnalysisSetupViewModel m_model;

        public AnalysisBackCommand(AnalysisSetupViewModel model)
            : base(null, AlwaysPass)
        {
            m_model = model;
        }
        public override void Execute(object parameter)
        {
            m_model.MoveBack();
        }
    }
}
