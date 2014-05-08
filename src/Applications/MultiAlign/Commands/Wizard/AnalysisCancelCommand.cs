using MultiAlign.ViewModels.Wizard;

namespace MultiAlign.Commands.Wizard
{
    public class AnalysisCancelCommand : BaseCommand
    {
        private readonly AnalysisSetupViewModel m_model;

        public AnalysisCancelCommand(AnalysisSetupViewModel model)
            : base(null, AlwaysPass)
        {
            m_model = model;
        }

        public override void Execute(object parameter)
        {
            m_model.Cancel();
        }
    }
}