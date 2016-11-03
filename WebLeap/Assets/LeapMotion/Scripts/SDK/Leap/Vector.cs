using System;

namespace Leap
{
	[Serializable]
	public struct Vector : IEquatable<Vector>
	{
		public float x;

		public float y;

		public float z;

		public static readonly Vector Zero = new Vector(0f, 0f, 0f);

		public static readonly Vector Ones = new Vector(1f, 1f, 1f);

		public static readonly Vector XAxis = new Vector(1f, 0f, 0f);

		public static readonly Vector YAxis = new Vector(0f, 1f, 0f);

		public static readonly Vector ZAxis = new Vector(0f, 0f, 1f);

		public static readonly Vector Forward = new Vector(0f, 0f, -1f);

		public static readonly Vector Backward = new Vector(0f, 0f, 1f);

		public static readonly Vector Left = new Vector(-1f, 0f, 0f);

		public static readonly Vector Right = new Vector(1f, 0f, 0f);

		public static readonly Vector Up = new Vector(0f, 1f, 0f);

		public static readonly Vector Down = new Vector(0f, -1f, 0f);

		public float this[uint index]
		{
			get
			{
				float result;
				if (index == 0u)
				{
					result = this.x;
				}
				else if (index == 1u)
				{
					result = this.y;
				}
				else if (index == 2u)
				{
					result = this.z;
				}
				else
				{
					result = 0f;
				}
				return result;
			}
			set
			{
				if (index == 0u)
				{
					this.x = value;
				}
				if (index == 1u)
				{
					this.y = value;
				}
				if (index == 2u)
				{
					this.z = value;
				}
			}
		}

		public float Magnitude
		{
			get
			{
				return (float)Math.Sqrt((double)(this.x * this.x + this.y * this.y + this.z * this.z));
			}
		}

		public float MagnitudeSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		public float Pitch
		{
			get
			{
				return (float)Math.Atan2((double)this.y, (double)(-(double)this.z));
			}
		}

		public float Roll
		{
			get
			{
				return (float)Math.Atan2((double)this.x, (double)(-(double)this.y));
			}
		}

		public float Yaw
		{
			get
			{
				return (float)Math.Atan2((double)this.x, (double)(-(double)this.z));
			}
		}

		public Vector Normalized
		{
			get
			{
				float num = this.MagnitudeSquared;
				Vector result;
				if (num <= 1.1920929E-07f)
				{
					result = Vector.Zero;
				}
				else
				{
					num = 1f / (float)Math.Sqrt((double)num);
					result = new Vector(this.x * num, this.y * num, this.z * num);
				}
				return result;
			}
		}

		public static Vector operator +(Vector v1, Vector v2)
		{
			return v1._operator_add(v2);
		}

		public static Vector operator -(Vector v1, Vector v2)
		{
			return v1._operator_sub(v2);
		}

		public static Vector operator *(Vector v1, float scalar)
		{
			return v1._operator_mul(scalar);
		}

		public static Vector operator *(float scalar, Vector v1)
		{
			return v1._operator_mul(scalar);
		}

		public static Vector operator /(Vector v1, float scalar)
		{
			return v1._operator_div(scalar);
		}

		public static Vector operator -(Vector v1)
		{
			return v1._operator_sub();
		}

		public static bool operator ==(Vector v1, Vector v2)
		{
			return v1.Equals(v2);
		}

		public static bool operator !=(Vector v1, Vector v2)
		{
			return !v1.Equals(v2);
		}

		public float[] ToFloatArray()
		{
			return new float[]
			{
				this.x,
				this.y,
				this.z
			};
		}

		public Vector(float x, float y, float z)
		{
			this = default(Vector);
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector(Vector vector)
		{
			this = default(Vector);
			this.x = vector.x;
			this.y = vector.y;
			this.z = vector.z;
		}

		public float DistanceTo(Vector other)
		{
			return (float)Math.Sqrt((double)((this.x - other.x) * (this.x - other.x) + (this.y - other.y) * (this.y - other.y) + (this.z - other.z) * (this.z - other.z)));
		}

		public float AngleTo(Vector other)
		{
			float num = this.MagnitudeSquared * other.MagnitudeSquared;
			float result;
			if (num <= 1.1920929E-07f)
			{
				result = 0f;
			}
			else
			{
				float num2 = this.Dot(other) / (float)Math.Sqrt((double)num);
				if (num2 >= 1f)
				{
					result = 0f;
				}
				else if (num2 <= -1f)
				{
					result = 3.14159274f;
				}
				else
				{
					result = (float)Math.Acos((double)num2);
				}
			}
			return result;
		}

		public float Dot(Vector other)
		{
			return this.x * other.x + this.y * other.y + this.z * other.z;
		}

		public Vector Cross(Vector other)
		{
			return new Vector(this.y * other.z - this.z * other.y, this.z * other.x - this.x * other.z, this.x * other.y - this.y * other.x);
		}

		private Vector _operator_sub()
		{
			return new Vector(-this.x, -this.y, -this.z);
		}

		private Vector _operator_add(Vector other)
		{
			return new Vector(this.x + other.x, this.y + other.y, this.z + other.z);
		}

		private Vector _operator_sub(Vector other)
		{
			return new Vector(this.x - other.x, this.y - other.y, this.z - other.z);
		}

		private Vector _operator_mul(float scalar)
		{
			return new Vector(this.x * scalar, this.y * scalar, this.z * scalar);
		}

		private Vector _operator_div(float scalar)
		{
			return new Vector(this.x / scalar, this.y / scalar, this.z / scalar);
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
				")"
			});
		}

		public bool Equals(Vector v)
		{
			return this.x.NearlyEquals(v.x, 1.1920929E-07f) && this.y.NearlyEquals(v.y, 1.1920929E-07f) && this.z.NearlyEquals(v.z, 1.1920929E-07f);
		}

		public override bool Equals(object obj)
		{
			return obj is Vector && this.Equals((Vector)obj);
		}

		public bool IsValid()
		{
			return !float.IsNaN(this.x) && !float.IsInfinity(this.x) && !float.IsNaN(this.y) && !float.IsInfinity(this.y) && !float.IsNaN(this.z) && !float.IsInfinity(this.z);
		}

		private float _operator_get(uint index)
		{
			float result;
			if (index == 0u)
			{
				result = this.x;
			}
			else if (index == 1u)
			{
				result = this.y;
			}
			else if (index == 2u)
			{
				result = this.z;
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		public static Vector Lerp(Vector a, Vector b, float t)
		{
			return new Vector(a.x + t * (b.x - a.x), a.y + t * (b.y - a.y), a.z + t * (b.z - a.z));
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + this.x.GetHashCode();
			num = num * 23 + this.y.GetHashCode();
			return num * 23 + this.z.GetHashCode();
		}
	}
}
