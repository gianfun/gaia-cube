using System;

namespace LeapInternal
{
	public enum eLeapRS : uint
	{
		eLeapRS_Success,
		eLeapRS_UnknownError = 3791716352u,
		eLeapRS_InvalidArgument,
		eLeapRS_InsufficientResources,
		eLeapRS_InsufficientBuffer,
		eLeapRS_Timeout,
		eLeapRS_NotConnected,
		eLeapRS_HandshakeIncomplete,
		eLeapRS_BufferSizeOverflow,
		eLeapRS_ProtocolError,
		eLeapRS_InvalidClientID,
		eLeapRS_UnexpectedClosed,
		eLeapRS_CannotCancelImageFrameRequest,
		eLeapRS_NotAvailable = 3875602434u,
		eLeapRS_NotStreaming = 3875602436u,
		eLeapRS_CannotOpenDevice
	}
}
