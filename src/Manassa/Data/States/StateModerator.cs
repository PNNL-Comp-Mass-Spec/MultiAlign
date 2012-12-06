using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Manassa.Data
{
    /// <summary>
    /// Delegate that is parameterless but makes a transition
    /// </summary>
    /// <returns></returns>
    public delegate bool UITransition();

    /// <summary>
    /// Validates whether the user interface can transition from one state to another.
    /// </summary>
    public class StateModerator : DependencyObject
    {

        public StateModerator()
        {
        }
        
        #region Dependency Properties for View and Analysis States
        public ViewState CurrentViewState
        {
            get { return (ViewState)GetValue(CurrentViewStateProperty); }
            set { SetValue(CurrentViewStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentViewState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentViewStateProperty =
            DependencyProperty.Register("CurrentViewState", typeof(ViewState), typeof(StateModerator), new UIPropertyMetadata(ViewState.HomeView));

        public ViewState PreviousViewState
        {
            get { return (ViewState)GetValue(PreviousViewStateProperty); }
            set { SetValue(PreviousViewStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousViewState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousViewStateProperty =
            DependencyProperty.Register("PreviousViewState", typeof(ViewState), typeof(StateModerator), new UIPropertyMetadata(ViewState.HomeView));


        public AnalysisState CurrentAnalysisState
        {
            get { return (AnalysisState)GetValue(CurrentAnalysisStateProperty); }
            set { SetValue(CurrentAnalysisStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnalysisState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentAnalysisStateProperty =
            DependencyProperty.Register("CurrentAnalysisState", typeof(AnalysisState), typeof(StateModerator), new UIPropertyMetadata(AnalysisState.Idle));


        public AnalysisState PreviousAnalysisState
        {
            get { return (AnalysisState)GetValue(PreviousAnalysisStateProperty); }
            set { SetValue(PreviousAnalysisStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousAnalysisState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousAnalysisStateProperty =
            DependencyProperty.Register("PreviousAnalysisState", typeof(AnalysisState), typeof(StateModerator), new UIPropertyMetadata(AnalysisState.Idle));
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
