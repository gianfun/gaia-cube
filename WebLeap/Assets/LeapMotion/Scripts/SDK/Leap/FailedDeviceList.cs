using System;
using System.Collections.Generic;

namespace Leap
{
	public class FailedDeviceList : List<FailedDevice>
	{
		public bool IsEmpty
		{
			get
			{
				return base.Count == 0;
			}
		}

		public FailedDeviceList Append(FailedDeviceList other)
		{
			base.AddRange(other);
			return this;
		}
	}
}
