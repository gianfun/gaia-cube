using System;

namespace LeapInternal
{
	public enum eLeapDeviceStatus : uint
	{
		eLeapDeviceStatus_Streaming = 1u,
		eLeapDeviceStatus_Paused,
		eLeapDeviceStatus_UnknownFailure = 3892379648u,
		eLeapDeviceStatus_BadCalibration,
		eLeapDeviceStatus_BadFirmware,
		eLeapDeviceStatus_BadTransport,
		eLeapDeviceStatus_BadControl
	}
}
