using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LEAP_IMAGE_FRAME_REQUEST_TOKEN
	{
		public uint requestID;
	}
}
