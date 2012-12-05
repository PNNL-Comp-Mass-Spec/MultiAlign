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
        /// <summary>
        /// Rules that dictate if a view change can be made from one to another.
        /// </summary>        
        private Dictionary<AnalysisState, Dictionary<ViewState, Action>> m_analRules;

        public StateModerator()
        {
            m_analRules = new Dictionary<AnalysisState, Dictionary<ViewState, Action>>();

            m_analRules.Add(AnalysisState.Idle, new Dictionary<ViewState, Action>());
            m_analRules.Add(AnalysisState.Running, new Dictionary<ViewState, Action>());
            m_analRules.Add(AnalysisState.Setup, new Dictionary<ViewState, Action>());
            m_analRules.Add(AnalysisState.Viewing, new Dictionary<ViewState, Action>());
            
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
        /// Add a transition such that, if we move from one state to another based on a command, that the appropiate
        /// action is executed.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="view"></param>
        /// <param name="a"></param>
        public void AddTransition(AnalysisState state, ViewState view, Action a)
        {
            bool isAnalOk = m_analRules.ContainsKey(state);

            if (!isAnalOk)
            {
                m_analRules.Add(state, new Dictionary<ViewState, Action>());
            }
            
            bool isViewOk = m_analRules[state].ContainsKey(view);

            if (!isViewOk)
            {
                m_analRules[state].Add(view, a);
            }
            else
            {
                throw new Exception("The action is already set for this view");
            }
        }

        /// <summary>
        /// Takes the action specified.
        /// </summary>
        /// <param name="view"></param>
        public void TakeAction(ViewState view)
        {
            if (m_analRules[CurrentAnalysisState].ContainsKey(view))
            {
                PreviousViewState = CurrentViewState;
                CurrentViewState  = view;
                m_analRules[CurrentAnalysisState][view].Invoke();
            }
        }                
    }   
}
