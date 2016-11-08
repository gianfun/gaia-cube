using Leap;
using System;
using System.Collections.Generic;

namespace LeapInternal
{
	public class DistortionDictionary : Dictionary<ulong, DistortionData>
	{
		private ulong _currentMatrix = 0uL;

		private bool _distortionChange = false;

		private object locker = new object();

		public ulong CurrentMatrix
		{
			get
			{
				ulong currentMatrix;
				lock (this.locker)
				{
					currentMatrix = this._currentMatrix;
				}
				return currentMatrix;
			}
			set
			{
				lock (this.locker)
				{
					this._currentMatrix = value;
				}
			}
		}

		public bool DistortionChange
		{
			get
			{
				bool distortionChange;
				lock (this.locker)
				{
					distortionChange = this._distortionChange;
				}
				return distortionChange;
			}
			set
			{
				lock (this.locker)
				{
					this._distortionChange = value;
				}
			}
		}

		public DistortionData GetMatrix(ulong version)
		{
			DistortionData result;
			lock (this.locker)
			{
				DistortionData distortionData;
				base.TryGetValue(version, out distortionData);
				result = distortionData;
			}
			return result;
		}

		public bool VersionExists(ulong version)
		{
			bool result;
			lock (this.locker)
			{
				result = base.ContainsKey(version);
			}
			return result;
		}
	}
}
