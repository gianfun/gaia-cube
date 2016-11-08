using System;
using System.Collections.Generic;

namespace Leap
{
	public class DeviceList : List<Device>
	{
		public Device ActiveDevice
		{
			get
			{
				Device result;
				if (base.Count == 1)
				{
					result = base[0];
				}
				else
				{
					for (int i = 0; i < base.Count; i++)
					{
						if (base[i].IsStreaming)
						{
							result = base[i];
							return result;
						}
					}
					result = new Device();
				}
				return result;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return base.Count == 0;
			}
		}

		public Device FindDeviceByHandle(IntPtr deviceHandle)
		{
			Device result;
			for (int i = 0; i < base.Count; i++)
			{
				if (base[i].Handle == deviceHandle)
				{
					result = base[i];
					return result;
				}
			}
			result = null;
			return result;
		}

		public void AddOrUpdate(Device device)
		{
			Device device2 = this.FindDeviceByHandle(device.Handle);
			if (device2 != null)
			{
				device2.Update(device);
			}
			else
			{
				base.Add(device);
			}
		}
	}
}
