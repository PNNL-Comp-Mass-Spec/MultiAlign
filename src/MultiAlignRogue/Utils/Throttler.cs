namespace MultiAlignRogue.Utils
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using MultiAlign.Data;

    public class Throttler
    {
        private readonly TimeSpan timeSpan;

        private DateTime startTime;

        private Action actionToRun;

        private bool hasRun;

        public Throttler(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
            if (this.timeSpan <= TimeSpan.FromMilliseconds(50))
            {
                this.timeSpan = TimeSpan.FromMilliseconds(50);
            }

            this.startTime = DateTime.UtcNow;
            this.actionToRun = () => { };
            this.hasRun = true;
        }

        public void Run(Action action)
        {
            this.actionToRun = action;
            this.startTime = DateTime.UtcNow;
            if (this.hasRun)
            {
                this.hasRun = false;
                Task.Run(() => this.Run());
            }
        }

        private void Run()
        {
            while (this.hasRun == false)
            {
                if (DateTime.UtcNow >= this.startTime.Add(this.timeSpan))
                {
                    ThreadSafeDispatcher.Invoke(this.actionToRun);
                    this.hasRun = true;
                }

                Thread.Sleep(50);
            }
        }
    }
}
