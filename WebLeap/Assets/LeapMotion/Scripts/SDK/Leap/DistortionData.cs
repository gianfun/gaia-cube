using LeapInternal;
using System;

namespace Leap
{
	public class DistortionData
	{
		public ulong Version
		{
			get;
			set;
		}

		public float Width
		{
			get;
			set;
		}

		public float Height
		{
			get;
			set;
		}

		public float[] Data
		{
			get;
			set;
		}

		public bool IsValid
		{
			get
			{
				return this.Data != null && this.Width == (float)LeapC.DistortionSize && this.Height == (float)LeapC.DistortionSize && (float)this.Data.Length == this.Width * this.Height * 2f * 2f;
			}
		}

		public DistortionData()
		{
		}

		public DistortionData(ulong version, float width, float height, float[] data)
		{
			this.Version = version;
			this.Width = width;
			this.Height = height;
			this.Data = data;
		}
	}
}
