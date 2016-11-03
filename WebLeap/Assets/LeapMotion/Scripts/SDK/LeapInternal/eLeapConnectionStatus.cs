using System;

namespace LeapInternal
{
	public enum eLeapConnectionStatus : uint
	{
		eLeapConnectionStatus_NotConnected,
		eLeapConnectionStatus_Connected,
		eLeapConnectionStatus_HandshakeIncomplete,
		eLeapConnectionStatus_NotRunning = 3875733508u
	}
}
