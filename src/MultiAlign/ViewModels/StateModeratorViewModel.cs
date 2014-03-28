using MultiAlign.ViewModels;

namespace MultiAlign.Data.States
{
    /// <summary>
    /// Delegate that is parameterless but makes a transition
    /// </summary>
    /// <returns></returns>
    public delegate bool UITransition();

    /// <summary>
    /// Validates whether the user interface can transition from one state to another.
    /// </summary>
    public class StateModeratorViewModel : ViewModelBase
    {        
        private AnalysisState   m_currentAnalysisState;
        private AnalysisState   m_previousAnalysisState;
        private ViewState       m_previousViewState;
        private ViewState       m_currentViewState;

        public StateModeratorViewModel()
        {
            CurrentViewState        = ViewState.HomeView;
            CurrentAnalysisState    = AnalysisState.Idle;
        }
        
        #region View Model Property States
        public ViewState CurrentViewState
        {
            get
            {
                return m_currentViewState;
            }
            set
            {
                if (value != m_currentViewState)
                {
                    m_currentViewState = value;
                    OnPropertyChanged("CurrentViewState");
                }
            }
        }


        public ViewState PreviousViewState
        {
            get
            {
                return m_previousViewState;
            }
            set
            {
                if (value != m_currentViewState)
                {
                    m_previousViewState = value;
                    OnPropertyChanged("PreviousViewState");
                }
            }
        }


        public AnalysisState CurrentAnalysisState
        {
            get
            {
                return m_currentAnalysisState;
            }
            set
            {
                if (value != m_currentAnalysisState)
                {
                    m_currentAnalysisState = value;
                    OnPropertyChanged("CurrentAnalysisState");
                }
            }
        }


        public AnalysisState PreviousAnalysisState
        {
            get
            {
                return m_previousAnalysisState;
            }
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

        /// <summary>
        /// Determines if the UI can open an analysis or not.
        /// </summary>
        /// <returns></returns>
        public bool CanOpenAnalysis(ref string message)
        {
            bool canHappen  = true;
            message         = "";
            switch (CurrentAnalysisState)
            {
                case AnalysisState.Idle:
                    break;
                case AnalysisState.Viewing:
                    break;
                case AnalysisState.Loading:
                    break;
                case AnalysisState.Running:                    
                    message = "Cannot open an analysis while one is running.";
                    canHappen = false;
                    break;
                case AnalysisState.Setup:
                    break;
                default:
                    break;
            }
            return canHappen;
        }

        public bool CanPerformNewAnalysis(ref string message)
        {
            bool canHappen = true;
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
                default:
                    break;
            }
            return canHappen;
        }

        public bool IsAnalysisRunning(ref string message)
        {
            bool canHappen  = false;
            message         = "";
            switch (CurrentAnalysisState)
            {
                case AnalysisState.Idle:
                    break;
                case AnalysisState.Viewing:
                    break;
                case AnalysisState.Loading:
                    break;
                case AnalysisState.Running:
                    canHappen   = true;
                    break;
                case AnalysisState.Setup:
                    break;
                default:
                    break;
            }
            return canHappen;
        }    
    }   
}
