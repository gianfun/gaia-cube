using System;

namespace Leap
{
	public class SetConfigResponseEventArgs : LeapEventArgs
	{
		public string ConfigKey
		{
			get;
			set;
		}

		public Config.ValueType DataType
		{
			get;
			set;
		}

		public object Value
		{
			get;
			set;
		}

		public uint RequestId
		{
			get;
			set;
		}

		public SetConfigResponseEventArgs(string config_key, Config.ValueType dataType, object value, uint requestId) : base(LeapEvent.EVENT_CONFIG_RESPONSE)
		{
			this.ConfigKey = config_key;
			this.DataType = dataType;
			this.Value = value;
			this.RequestId = requestId;
		}
	}
}
