using System;

namespace Leap
{
	public class ConfigChangeEventArgs : LeapEventArgs
	{
		public string ConfigKey
		{
			get;
			set;
		}

		public bool Succeeded
		{
			get;
			set;
		}

		public uint RequestId
		{
			get;
			set;
		}

		public ConfigChangeEventArgs(string config_key, bool succeeded, uint requestId) : base(LeapEvent.EVENT_CONFIG_CHANGE)
		{
			this.ConfigKey = config_key;
			this.Succeeded = succeeded;
			this.RequestId = requestId;
		}
	}
}
