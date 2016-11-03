using System;

namespace Leap
{
	public struct InteractionBox
	{
		public Vector Center;

		public Vector Size;

		public float Width
		{
			get
			{
				return this.Size.x;
			}
		}

		public float Height
		{
			get
			{
				return this.Size.y;
			}
		}

		public float Depth
		{
			get
			{
				return this.Size.z;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.Size != Vector.Zero && !float.IsNaN(this.Size.x) && !float.IsNaN(this.Size.y) && !float.IsNaN(this.Size.z);
			}
		}

		public InteractionBox(Vector center, Vector size)
		{
			this.Size = size;
			this.Center = center;
		}

		public Vector NormalizePoint(Vector position, bool clamp = true)
		{
			Vector result;
			if (!this.IsValid)
			{
				result = Vector.Zero;
			}
			else
			{
				float num = (position.x - this.Center.x + this.Size.x / 2f) / this.Size.x;
				float num2 = (position.y - this.Center.y + this.Size.y / 2f) / this.Size.y;
				float num3 = (position.z - this.Center.z + this.Size.z / 2f) / this.Size.z;
				if (clamp)
				{
					num = Math.Min(1f, Math.Max(0f, num));
					num2 = Math.Min(1f, Math.Max(0f, num2));
					num3 = Math.Min(1f, Math.Max(0f, num3));
				}
				result = new Vector(num, num2, num3);
			}
			return result;
		}

		public Vector DenormalizePoint(Vector normalizedPosition)
		{
			Vector result;
			if (!this.IsValid)
			{
				result = Vector.Zero;
			}
			else
			{
				float x = normalizedPosition.x * this.Size.x + (this.Center.x - this.Size.x / 2f);
				float y = normalizedPosition.y * this.Size.y + (this.Center.y - this.Size.y / 2f);
				float z = normalizedPosition.z * this.Size.z + (this.Center.z - this.Size.z / 2f);
				result = new Vector(x, y, z);
			}
			return result;
		}

		public bool Equals(InteractionBox other)
		{
			return this.IsValid && other.IsValid && this.Center == other.Center && this.Size == other.Size;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"InteractionBox Center: ",
				this.Center,
				", Size: ",
				this.Size
			});
		}
	}
}
