using System;

namespace FeatureAlignment.Algorithms
{
    /// <summary>
    /// Interface defining signature for a progress notifier object.
    /// </summary>
    public interface IProgressNotifer
    {
        event EventHandler<ProgressNotifierArgs> Progress;
    }
}
