using Leap;
using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LEAP_QUATERNION
	{
		public float x;

		public float y;

		public float z;

		public float w;

		public LeapQuaternion ToLeapQuaternion()
		{
			return new LeapQuaternion(this.x, this.y, this.z, this.w);
		}

		public LEAP_QUATERNION(LeapQuaternion q)
		{
			this.x = q.x;
			this.y = q.y;
			this.z = q.z;
			this.w = q.w;
		}
	}
}
