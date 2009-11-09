using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace PNNLControls.Diagnostics
{
	/// <summary>
	/// Class from CodeProject on how to use a high performance counter for a timer object.
	/// </summary>
	public class HiPerfTimer
	{
		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceCounter(
			out long lpPerformanceCount);

		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(
			out long lpFrequency);

		private long startTime, stopTime;
		private long freq;

		// Constructor
		public HiPerfTimer()
		{
			startTime = 0;
			stopTime  = 0;

			if (QueryPerformanceFrequency(out freq) == false)
			{
				// high-performance counter not supported
				throw new System.Exception("High Performance counter not supported.");
			}
		}

		// Start the timer
		public void Start()
		{
			// lets do the waiting threads there work
			Thread.Sleep(0);

			QueryPerformanceCounter(out startTime);
		}

		// Stop the timer
		public void Stop()
		{
			QueryPerformanceCounter(out stopTime);
		}

		// Returns the duration of the timer (in seconds)
		public double Duration
		{
			get
			{
				return (double)(stopTime - startTime) / (double) freq;
			}
		}
	}
}
