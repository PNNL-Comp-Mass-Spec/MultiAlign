using System;
using MultiAlign.Data;
using MultiAlignCore.Algorithms;

namespace MultiAlign.ViewModels.Wizard
{
    public class AnalysisGraphNodeViewModel : ViewModelBase
    {
        private readonly AnalysisGraphNode m_node;

        public AnalysisGraphNodeViewModel(AnalysisGraphNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            // This safely updates the UI thread in case the current analysis is running on a separate thread.
            Action update = () => OnPropertyChanged("IsCurrent");

            m_node = node;
            m_node.StatusChanged += (sender, args) => ThreadSafeDispatcher.Invoke(update);
        }

        public string Name
        {
            get { return m_node.Name; }
        }

        public string Description
        {
            get { return m_node.Description; }
        }

        public bool IsCurrent
        {
            get { return m_node.IsCurrent; }
        }
    }
}