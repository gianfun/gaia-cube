using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LEAP_DEVICE_REF
	{
		public IntPtr handle;

		public uint id;

		public LEAP_DEVICE_REF(IntPtr handle, uint id)
		{
			this.handle = handle;
			this.id = id;
		}
	}
}
