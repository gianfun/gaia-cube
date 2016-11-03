using System;

namespace Leap
{
	public class DistortionEventArgs : LeapEventArgs
	{
		public DistortionData distortion
		{
			get;
			set;
		}

		public DistortionEventArgs(DistortionData distortion) : base(LeapEvent.EVENT_DISTORTION_CHANGE)
		{
			this.distortion = distortion;
		}
	}
}
