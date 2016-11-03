using System;

namespace LeapInternal
{
	public enum eLeapEventType
	{
		eLeapEventType_None,
		eLeapEventType_Connection,
		eLeapEventType_ConnectionLost,
		eLeapEventType_Device,
		eLeapEventType_DeviceFailure,
		eLeapEventType_PolicyChange,
		eLeapEventType_Tracking = 256,
		eLeapEventType_ImageRequestError,
		eLeapEventType_ImageComplete,
		eLeapEventType_LogEvent,
		eLeapEventType_DeviceLost,
		eLeapEventType_ConfigResponse,
		eLeapEventType_ConfigChange
	}
}
