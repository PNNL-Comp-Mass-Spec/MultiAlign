using PNNLOmics.Algorithms;
using System;

namespace MultiAlignCore.Algorithms.Workflow
{
    public abstract class WorkflowBase : IProgressNotifer
    {        
        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        /// Updates listeners with status messages.
        /// </summary>
        /// <param name="message"></param>
        protected void UpdateStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message, 0));
            }
        }

        protected void RegisterProgressNotifier(IProgressNotifer notifier)
        {
            if (notifier != null)
            {
                notifier.Progress += notifier_Progress;
            }
        }
        protected void DeRegisterProgressNotifier(IProgressNotifer notifier)
        {
            if (notifier != null)
            {
                notifier.Progress -= notifier_Progress;
            }
        }

        protected void notifier_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }
    }
}
