using System;

namespace Leap
{
	public struct LeapTransform
	{
		public static readonly LeapTransform Identity = new LeapTransform(Vector.Zero, LeapQuaternion.Identity, Vector.Ones);

		private Vector _translation;

		private Vector _scale;

		private LeapQuaternion _quaternion;

		private bool _quaternionDirty;

		private bool _flip;

		private Vector _flipAxes;

		private Vector _xBasis;

		private Vector _yBasis;

		private Vector _zBasis;

		private Vector _xBasisScaled;

		private Vector _yBasisScaled;

		private Vector _zBasisScaled;

		public Vector xBasis
		{
			get
			{
				return this._xBasis;
			}
			set
			{
				this._xBasis = value;
				this._xBasisScaled = value * this.scale.x;
				this._quaternionDirty = true;
			}
		}

		public Vector yBasis
		{
			get
			{
				return this._yBasis;
			}
			set
			{
				this._yBasis = value;
				this._yBasisScaled = value * this.scale.y;
				this._quaternionDirty = true;
			}
		}

		public Vector zBasis
		{
			get
			{
				return this._zBasis;
			}
			set
			{
				this._zBasis = value;
				this._zBasisScaled = value * this.scale.z;
				this._quaternionDirty = true;
			}
		}

		public Vector translation
		{
			get
			{
				return this._translation;
			}
			set
			{
				this._translation = value;
			}
		}

		public Vector scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this._scale = value;
				this._xBasisScaled = this._xBasis * this.scale.x;
				this._yBasisScaled = this._yBasis * this.scale.y;
				this._zBasisScaled = this._zBasis * this.scale.z;
			}
		}

		public LeapQuaternion rotation
		{
			get
			{
				if (this._quaternionDirty)
				{
					throw new InvalidOperationException("Requesting rotation after Basis vectors have been modified.");
				}
				return this._quaternion;
			}
			set
			{
				this._quaternion = value;
				float magnitudeSquared = value.MagnitudeSquared;
				float num = 2f / magnitudeSquared;
				float num2 = value.x * num;
				float num3 = value.y * num;
				float num4 = value.z * num;
				float num5 = value.w * num2;
				float num6 = value.w * num3;
				float num7 = value.w * num4;
				float num8 = value.x * num2;
				float num9 = value.x * num3;
				float num10 = value.x * num4;
				float num11 = value.y * num3;
				float num12 = value.y * num4;
				float num13 = value.z * num4;
				this._xBasis = new Vector(1f - (num11 + num13), num9 + num7, num10 - num6);
				this._yBasis = new Vector(num9 - num7, 1f - (num8 + num13), num12 + num5);
				this._zBasis = new Vector(num10 + num6, num12 - num5, 1f - (num8 + num11));
				this._xBasisScaled = this._xBasis * this.scale.x;
				this._yBasisScaled = this._yBasis * this.scale.y;
				this._zBasisScaled = this._zBasis * this.scale.z;
				this._quaternionDirty = false;
				this._flip = false;
				this._flipAxes = new Vector(1f, 1f, 1f);
			}
		}

		public LeapTransform(Vector translation, LeapQuaternion rotation)
		{
			this = new LeapTransform(translation, rotation, Vector.Ones);
		}

		public LeapTransform(Vector translation, LeapQuaternion rotation, Vector scale)
		{
			this = default(LeapTransform);
			this._scale = scale;
			this.translation = translation;
			this.rotation = rotation;
		}

		public Vector TransformPoint(Vector point)
		{
			return this._xBasisScaled * point.x + this._yBasisScaled * point.y + this._zBasisScaled * point.z + this.translation;
		}

		public Vector TransformDirection(Vector direction)
		{
			return this._xBasis * direction.x + this._yBasis * direction.y + this._zBasis * direction.z;
		}

		public Vector TransformVelocity(Vector velocity)
		{
			return this._xBasisScaled * velocity.x + this._yBasisScaled * velocity.y + this._zBasisScaled * velocity.z;
		}

		public LeapQuaternion TransformQuaternion(LeapQuaternion rhs)
		{
			if (this._quaternionDirty)
			{
				throw new InvalidOperationException("Calling TransformQuaternion after Basis vectors have been modified.");
			}
			if (this._flip)
			{
				rhs.x *= this._flipAxes.x;
				rhs.y *= this._flipAxes.y;
				rhs.z *= this._flipAxes.z;
			}
			return this._quaternion.Multiply(rhs);
		}

		public void MirrorX()
		{
			this._xBasis = -this._xBasis;
			this._xBasisScaled = -this._xBasisScaled;
			this._flip = true;
			this._flipAxes.y = -this._flipAxes.y;
			this._flipAxes.z = -this._flipAxes.z;
		}

		public void MirrorZ()
		{
			this._zBasis = -this._zBasis;
			this._zBasisScaled = -this._zBasisScaled;
			this._flip = true;
			this._flipAxes.x = -this._flipAxes.x;
			this._flipAxes.y = -this._flipAxes.y;
		}
	}
}
