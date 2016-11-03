using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LEAP_IMAGE_COMPLETE_EVENT
	{
		public LEAP_IMAGE_FRAME_REQUEST_TOKEN token;

		public LEAP_FRAME_HEADER info;

		public IntPtr properties;

		public ulong matrix_version;

		public IntPtr calib;

		public IntPtr distortionMatrix;

		public IntPtr pfnData;

		public ulong data_written;
	}
}
