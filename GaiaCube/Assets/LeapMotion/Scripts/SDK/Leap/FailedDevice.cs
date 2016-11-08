using System;

namespace Leap
{
	public class FailedDevice : IEquatable<FailedDevice>
	{
		public enum FailureType
		{
			FAIL_UNKNOWN,
			FAIL_CALIBRATION,
			FAIL_FIRMWARE,
			FAIL_TRANSPORT,
			FAIL_CONTROL,
			FAIL_COUNT
		}

		public string PnpId
		{
			get;
			private set;
		}

		public FailedDevice.FailureType Failure
		{
			get;
			private set;
		}

		public FailedDevice()
		{
			this.Failure = FailedDevice.FailureType.FAIL_UNKNOWN;
			this.PnpId = "0";
		}

		public bool Equals(FailedDevice other)
		{
			return this.PnpId == other.PnpId;
		}
	}
}
