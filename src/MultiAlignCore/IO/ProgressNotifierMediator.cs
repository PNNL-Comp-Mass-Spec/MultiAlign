using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Algorithms;

namespace MultiAlignCore.IO
{
    /// <summary>
    /// Class that mediates event messages
    /// </summary>
    public static class ProgressNotifierMediator
    {
        private static Dictionary<IProgressNotifer, EventHandler<ProgressNotifierArgs>> m_notifiers =
                        new Dictionary<IProgressNotifer,EventHandler<ProgressNotifierArgs>>();

        public static void RegisterNotifier(IProgressNotifer notifier, EventHandler<ProgressNotifierArgs> handler)
        {
            if (!m_notifiers.ContainsKey(notifier))
            {
                m_notifiers.Add(notifier, handler);
                notifier.Progress += new EventHandler<ProgressNotifierArgs>(notifier_Progress);
            }
        }

        static void notifier_Progress(object sender, ProgressNotifierArgs e)
        {
            IProgressNotifer notifier = sender as IProgressNotifer;

            if (m_notifiers.ContainsKey(notifier))
            {
                if (m_notifiers[notifier] != null)
                {
                    m_notifiers[notifier](sender, e);
                }
            }
        }
        public static void UnRegisterNotifier(IProgressNotifer notifier)
        {
            try
            {
                notifier.Progress -= notifier_Progress;
                if (m_notifiers.ContainsKey(notifier))
                {
                    m_notifiers.Remove(notifier);
                }
            }
            catch
            {
                // Dont care.
            }
        }
    }
}
