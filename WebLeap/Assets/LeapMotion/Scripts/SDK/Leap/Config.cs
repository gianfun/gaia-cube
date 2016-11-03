using LeapInternal;
using System;
using System.Collections.Generic;

namespace Leap
{
	public class Config
	{
		public enum ValueType
		{
			TYPE_UNKNOWN,
			TYPE_BOOLEAN,
			TYPE_INT32,
			TYPE_FLOAT = 6,
			TYPE_STRING = 8
		}

		private Connection _connection;

		private Dictionary<uint, object> _transactions = new Dictionary<uint, object>();

		public Config(int connectionKey)
		{
			this._connection = Connection.GetConnection(connectionKey);
			Connection expr_25 = this._connection;
			expr_25.LeapConfigChange = (EventHandler<ConfigChangeEventArgs>)Delegate.Combine(expr_25.LeapConfigChange, new EventHandler<ConfigChangeEventArgs>(this.handleConfigChange));
			Connection expr_4C = this._connection;
			expr_4C.LeapConfigResponse = (EventHandler<SetConfigResponseEventArgs>)Delegate.Combine(expr_4C.LeapConfigResponse, new EventHandler<SetConfigResponseEventArgs>(this.handleConfigResponse));
		}

		private void handleConfigChange(object sender, ConfigChangeEventArgs eventArgs)
		{
			object obj;
			if (this._transactions.TryGetValue(eventArgs.RequestId, out obj))
			{
				Action<bool> action = obj as Action<bool>;
				action(eventArgs.Succeeded);
				this._transactions.Remove(eventArgs.RequestId);
			}
		}

		private void handleConfigResponse(object sender, SetConfigResponseEventArgs eventArgs)
		{
			object obj = new object();
			if (this._transactions.TryGetValue(eventArgs.RequestId, out obj))
			{
				Config.ValueType dataType = eventArgs.DataType;
				switch (dataType)
				{
				case Config.ValueType.TYPE_BOOLEAN:
				{
					Action<bool> action = obj as Action<bool>;
					action((int)eventArgs.Value != 0);
					break;
				}
				case Config.ValueType.TYPE_INT32:
				{
					Action<int> action2 = obj as Action<int>;
					action2((int)eventArgs.Value);
					break;
				}
				default:
					switch (dataType)
					{
					case Config.ValueType.TYPE_FLOAT:
					{
						Action<float> action3 = obj as Action<float>;
						action3((float)eventArgs.Value);
						break;
					}
					case Config.ValueType.TYPE_STRING:
					{
						Action<string> action4 = obj as Action<string>;
						action4((string)eventArgs.Value);
						break;
					}
					}
					break;
				}
				this._transactions.Remove(eventArgs.RequestId);
			}
		}

		public bool Get<T>(string key, Action<T> onResult)
		{
			uint configValue = this._connection.GetConfigValue(key);
			bool result;
			if (configValue > 0u)
			{
				this._transactions.Add(configValue, onResult);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool Set<T>(string key, T value, Action<bool> onResult) where T : IConvertible
		{
			uint num = this._connection.SetConfigValue<T>(key, value);
			bool result;
			if (num > 0u)
			{
				this._transactions.Add(num, onResult);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public Config.ValueType Type(string key)
		{
			return Config.ValueType.TYPE_UNKNOWN;
		}

		public bool GetBool(string key)
		{
			return this.Get<bool>(key, delegate(bool value)
			{
			});
		}

		public bool SetBool(string key, bool value)
		{
			return this.Set<bool>(key, value, delegate(bool success)
			{
			});
		}

		public bool GetInt32(string key)
		{
			return this.Get<int>(key, delegate(int value)
			{
			});
		}

		public bool SetInt32(string key, int value)
		{
			return this.Set<int>(key, value, delegate(bool success)
			{
			});
		}

		public bool GetFloat(string key)
		{
			return this.Get<float>(key, delegate(float value)
			{
			});
		}

		public bool SetFloat(string key, float value)
		{
			return this.Set<float>(key, value, delegate(bool success)
			{
			});
		}

		public bool GetString(string key)
		{
			return this.Get<string>(key, delegate(string value)
			{
			});
		}

		public bool SetString(string key, string value)
		{
			return this.Set<string>(key, value, delegate(bool success)
			{
			});
		}

		public bool Save()
		{
			return false;
		}
	}
}
