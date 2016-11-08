using LeapInternal;
using System;

namespace Leap
{
	public class ClockCorrelator : IDisposable
	{
		private IntPtr _rebaserHandle = IntPtr.Zero;

		private bool _disposed = false;

		public ClockCorrelator()
		{
			eLeapRS eLeapRS = LeapC.CreateClockRebaser(out this._rebaserHandle);
			if (eLeapRS != eLeapRS.eLeapRS_Success)
			{
				throw new Exception(eLeapRS.ToString());
			}
		}

		public void UpdateRebaseEstimate(long applicationClock)
		{
			LeapC.UpdateRebase(this._rebaserHandle, applicationClock, LeapC.GetNow());
		}

		public void UpdateRebaseEstimate(long applicationClock, long leapClock)
		{
			LeapC.UpdateRebase(this._rebaserHandle, applicationClock, leapClock);
		}

		public long ExternalClockToLeapTime(long applicationClock)
		{
			long result;
			LeapC.RebaseClock(this._rebaserHandle, applicationClock, out result);
			return result;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				LeapC.DestroyClockRebaser(this._rebaserHandle);
				this._rebaserHandle = IntPtr.Zero;
				this._disposed = true;
			}
		}

		~ClockCorrelator()
		{
			this.Dispose(false);
		}
	}
}
