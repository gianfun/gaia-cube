using LeapInternal;
using System;

namespace Leap
{
	[Serializable]
	public struct LeapQuaternion : IEquatable<LeapQuaternion>
	{
		public float x;

		public float y;

		public float z;

		public float w;

		public static readonly LeapQuaternion Identity = new LeapQuaternion(0f, 0f, 0f, 1f);

		public float Magnitude
		{
			get
			{
				return (float)Math.Sqrt((double)(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w));
			}
		}

		public float MagnitudeSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
			}
		}

		public LeapQuaternion Normalized
		{
			get
			{
				float num = this.MagnitudeSquared;
				LeapQuaternion result;
				if (num <= 1.1920929E-07f)
				{
					result = LeapQuaternion.Identity;
				}
				else
				{
					num = 1f / (float)Math.Sqrt((double)num);
					result = new LeapQuaternion(this.x * num, this.y * num, this.z * num, this.w * num);
				}
				return result;
			}
		}

		public LeapQuaternion(float x, float y, float z, float w)
		{
			this = default(LeapQuaternion);
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public LeapQuaternion(LeapQuaternion quaternion)
		{
			this = default(LeapQuaternion);
			this.x = quaternion.x;
			this.y = quaternion.y;
			this.z = quaternion.z;
			this.w = quaternion.w;
		}

		public LeapQuaternion(LEAP_QUATERNION quaternion)
		{
			this = default(LeapQuaternion);
			this.x = quaternion.x;
			this.y = quaternion.y;
			this.z = quaternion.z;
			this.w = quaternion.w;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"(",
				this.x,
				", ",
				this.y,
				", ",
				this.z,
				", ",
				this.w,
				")"
			});
		}

		public bool Equals(LeapQuaternion v)
		{
			return this.x.NearlyEquals(v.x, 1.1920929E-07f) && this.y.NearlyEquals(v.y, 1.1920929E-07f) && this.z.NearlyEquals(v.z, 1.1920929E-07f) && this.w.NearlyEquals(v.w, 1.1920929E-07f);
		}

		public override bool Equals(object obj)
		{
			return obj is LeapQuaternion && this.Equals((LeapQuaternion)obj);
		}

		public bool IsValid()
		{
			return !float.IsNaN(this.x) && !float.IsInfinity(this.x) && !float.IsNaN(this.y) && !float.IsInfinity(this.y) && !float.IsNaN(this.z) && !float.IsInfinity(this.z) && !float.IsNaN(this.w) && !float.IsInfinity(this.w);
		}

		public LeapQuaternion Multiply(LeapQuaternion rhs)
		{
			return new LeapQuaternion(this.w * rhs.x + this.x * rhs.w + this.y * rhs.z - this.z * rhs.y, this.w * rhs.y + this.y * rhs.w + this.z * rhs.x - this.x * rhs.z, this.w * rhs.z + this.z * rhs.w + this.x * rhs.y - this.y * rhs.x, this.w * rhs.w - this.x * rhs.x - this.y * rhs.y - this.z * rhs.z);
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + this.x.GetHashCode();
			num = num * 23 + this.y.GetHashCode();
			num = num * 23 + this.z.GetHashCode();
			return num * 23 + this.w.GetHashCode();
		}
	}
}
