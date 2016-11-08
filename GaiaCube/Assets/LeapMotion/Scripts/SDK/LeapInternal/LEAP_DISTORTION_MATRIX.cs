using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LEAP_DISTORTION_MATRIX
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16384)]
		public float[] matrix_data;
	}
}
