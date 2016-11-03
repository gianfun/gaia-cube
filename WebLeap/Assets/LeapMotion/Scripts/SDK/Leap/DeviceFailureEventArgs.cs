using System;

namespace Leap
{
	public class DeviceFailureEventArgs : LeapEventArgs
	{
		public uint ErrorCode
		{
			get;
			set;
		}

		public string ErrorMessage
		{
			get;
			set;
		}

		public string DeviceSerialNumber
		{
			get;
			set;
		}

		public DeviceFailureEventArgs(uint code, string message, string serial) : base(LeapEvent.EVENT_DEVICE_FAILURE)
		{
			this.ErrorCode = code;
			this.ErrorMessage = message;
			this.DeviceSerialNumber = serial;
		}
	}
}
