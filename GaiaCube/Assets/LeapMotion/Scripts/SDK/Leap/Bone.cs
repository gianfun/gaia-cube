using System;

namespace Leap
{
	[Serializable]
	public class Bone
	{
		public enum BoneType
		{
			TYPE_INVALID = -1,
			TYPE_METACARPAL,
			TYPE_PROXIMAL,
			TYPE_INTERMEDIATE,
			TYPE_DISTAL
		}

		public Vector PrevJoint;

		public Vector NextJoint;

		public Vector Center;

		public Vector Direction;

		public float Length;

		public float Width;

		public Bone.BoneType Type;

		public LeapQuaternion Rotation;

		public LeapTransform Basis
		{
			get
			{
				return new LeapTransform(this.PrevJoint, this.Rotation);
			}
		}

		public Bone()
		{
			this.Type = Bone.BoneType.TYPE_INVALID;
		}

		public Bone(Vector prevJoint, Vector nextJoint, Vector center, Vector direction, float length, float width, Bone.BoneType type, LeapQuaternion rotation)
		{
			this.PrevJoint = prevJoint;
			this.NextJoint = nextJoint;
			this.Center = center;
			this.Direction = direction;
			this.Rotation = rotation;
			this.Length = length;
			this.Width = width;
			this.Type = type;
		}

		public bool Equals(Bone other)
		{
			return this.Center == other.Center && this.Direction == other.Direction && this.Length == other.Length;
		}

		public override string ToString()
		{
			return Enum.GetName(typeof(Bone.BoneType), this.Type) + " bone";
		}
	}
}
