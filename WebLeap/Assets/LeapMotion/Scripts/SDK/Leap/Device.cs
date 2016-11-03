using LeapInternal;
using System;

namespace Leap
{
	public class Device : IEquatable<Device>
	{
		public enum DeviceType
		{
			TYPE_INVALID = -1,
			TYPE_PERIPHERAL = 1,
			TYPE_LAPTOP,
			TYPE_KEYBOARD
		}

		public IntPtr Handle
		{
			get;
			private set;
		}

		public float HorizontalViewAngle
		{
			get;
			private set;
		}

		public float VerticalViewAngle
		{
			get;
			private set;
		}

		public float Range
		{
			get;
			private set;
		}

		public float Baseline
		{
			get;
			private set;
		}

		public bool IsEmbedded
		{
			get;
			private set;
		}

		public bool IsStreaming
		{
			get;
			private set;
		}

		public Device.DeviceType Type
		{
			get
			{
				return Device.DeviceType.TYPE_INVALID;
			}
		}

		public string SerialNumber
		{
			get;
			private set;
		}

		public bool IsSmudged
		{
			get
			{
				return false;
			}
		}

		public bool IsLightingBad
		{
			get
			{
				return false;
			}
		}

		public Device()
		{
		}

		public Device(IntPtr deviceHandle, float horizontalViewAngle, float verticalViewAngle, float range, float baseline, bool isEmbedded, bool isStreaming, string serialNumber)
		{
			this.Handle = deviceHandle;
			this.HorizontalViewAngle = horizontalViewAngle;
			this.VerticalViewAngle = verticalViewAngle;
			this.Range = range;
			this.Baseline = baseline;
			this.IsEmbedded = isEmbedded;
			this.IsStreaming = isStreaming;
			this.SerialNumber = serialNumber;
		}

		public void Update(float horizontalViewAngle, float verticalViewAngle, float range, float baseline, bool isEmbedded, bool isStreaming, string serialNumber)
		{
			this.HorizontalViewAngle = horizontalViewAngle;
			this.VerticalViewAngle = verticalViewAngle;
			this.Range = range;
			this.Baseline = baseline;
			this.IsEmbedded = isEmbedded;
			this.IsStreaming = isStreaming;
			this.SerialNumber = serialNumber;
		}

		public void Update(Device updatedDevice)
		{
			this.HorizontalViewAngle = updatedDevice.HorizontalViewAngle;
			this.VerticalViewAngle = updatedDevice.VerticalViewAngle;
			this.Range = updatedDevice.Range;
			this.Baseline = updatedDevice.Baseline;
			this.IsEmbedded = updatedDevice.IsEmbedded;
			this.IsStreaming = updatedDevice.IsStreaming;
			this.SerialNumber = updatedDevice.SerialNumber;
		}

		public bool SetPaused(bool pause)
		{
			ulong num = 0uL;
			ulong set = 0uL;
			ulong clear = 0uL;
			if (pause)
			{
				set = 1uL;
			}
			else
			{
				clear = 1uL;
			}
			eLeapRS eLeapRS = LeapC.SetDeviceFlags(this.Handle, set, clear, out num);
			return eLeapRS == eLeapRS.eLeapRS_Success;
		}

		public bool Equals(Device other)
		{
			return this.SerialNumber == other.SerialNumber;
		}

		public override string ToString()
		{
			return "Device serial# " + this.SerialNumber;
		}
	}
}
