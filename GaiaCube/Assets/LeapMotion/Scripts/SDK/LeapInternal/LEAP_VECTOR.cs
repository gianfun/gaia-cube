using Leap;
using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LEAP_VECTOR
	{
		public float x;

		public float y;

		public float z;

		public Vector ToLeapVector()
		{
			return new Vector(this.x, this.y, this.z);
		}

		public LEAP_VECTOR(Vector leap)
		{
			this.x = leap.x;
			this.y = leap.y;
			this.z = leap.z;
		}
	}
}
