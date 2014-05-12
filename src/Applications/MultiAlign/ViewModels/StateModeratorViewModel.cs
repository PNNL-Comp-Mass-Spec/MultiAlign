using MultiAlign.Data.States;

namespace MultiAlign.ViewModels
{
    /// <summary>
    ///     Validates whether the user interface can transition from one state to another.
    /// </summary>
    public sealed class StateModeratorViewModel : ViewModelBase
    {
        private AnalysisState m_currentAnalysisState;
        private ViewState m_currentViewState;
        private AnalysisState m_previousAnalysisState;
        private ViewState m_previousViewState;

        public StateModeratorViewModel()
        {
            CurrentViewState = ViewState.HomeView;
            CurrentAnalysisState = AnalysisState.Idle;
        }

        #region View Model Property States

        public ViewState CurrentViewState
        {
            get { return m_currentViewState; }
            set
            {
                if (value == m_currentViewState) return;
                m_currentViewState = value;
                OnPropertyChanged("CurrentViewState");
            }
        }


        public ViewState PreviousViewState
        {
            get { return m_previousViewState; }
            set
            {
                if (value == m_currentViewState) return;
                m_previousViewState = value;
                OnPropertyChanged("PreviousViewState");
            }
        }


        public AnalysisState CurrentAnalysisState
        {
            get { return m_currentAnalysisState; }
            set
            {
                if (value == m_currentAnalysisState) return;
                m_currentAnalysisState = value;
                OnPropertyChanged("CurrentAnalysisState");
            }
        }


        public AnalysisState PreviousAnalysisState
        {
            get { return m_previousAnalysisState; }
            set
            {
                if (value != m_previousAnalysisState)
                {
                    m_previousAnalysisState = value;
                    OnPropertyChanged("PreviousAnalysisState");
                }
            }
        }

        #endregion

        public bool CanPerformNewAnalysis(out string message)
        {
            var canHappen = true;
            message = "";
            switch (CurrentAnalysisState)
            {
                case AnalysisState.Idle:
                    break;
                case AnalysisState.Viewing:
                    break;
                case AnalysisState.Loading:
                    break;
                case AnalysisState.Running:
                    message = "Cannot start a new analysis while one is running.";
                    canHappen = false;
                    break;
                case AnalysisState.Setup:
                    break;
            }
            return canHappen;
        }

        public bool IsAnalysisRunning(out string message)
        {
            var canHappen = false;
            message = "";
            switch (CurrentAnalysisState)
            {
                case AnalysisState.Idle:
                    break;
                case AnalysisState.Viewing:
                    break;
                case AnalysisState.Loading:
                    break;
                case AnalysisState.Running:
                    canHappen = true;
                    break;
                case AnalysisState.Setup:
                    break;
            }
            return canHappen;
        }
    }
}