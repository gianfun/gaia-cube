using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LEAP_IMAGE_FRAME_DESCRIPTION
	{
		public long frame_id;

		public eLeapImageType type;

		public ulong buffer_len;

		public IntPtr pBuffer;
	}
}
