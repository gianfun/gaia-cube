using System;
using System.Reflection;

namespace LeapInternal
{
	public static class Logger
	{
		public static void Log(object message)
		{
		}

		public static void LogStruct(object thisObject, string title = "")
		{
			try
			{
				if (!thisObject.GetType().IsValueType)
				{
					Logger.Log(title + " ---- Trying to log non-struct with struct logger");
				}
				else
				{
					Logger.Log(title + " ---- " + thisObject.GetType().ToString());
					FieldInfo[] fields = thisObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
					FieldInfo[] array = fields;
					for (int i = 0; i < array.Length; i++)
					{
						FieldInfo fieldInfo = array[i];
						string str = fieldInfo.GetValue(thisObject).ToString();
						Logger.Log(" -------- Name: " + fieldInfo.Name + ", Value = " + str);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.Message);
			}
		}
	}
}
