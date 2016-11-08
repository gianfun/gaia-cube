using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LEAP_IMAGE_FRAME_REQUEST_ERROR_EVENT
	{
		public LEAP_IMAGE_FRAME_REQUEST_TOKEN token;

		public eLeapImageRequestError error;

		public ulong required_buffer_len;

		public LEAP_IMAGE_FRAME_DESCRIPTION description;
	}
}
