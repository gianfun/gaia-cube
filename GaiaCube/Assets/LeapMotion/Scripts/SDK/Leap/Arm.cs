using System;

namespace Leap
{
	[Serializable]
	public class Arm : Bone
	{
		public Vector ElbowPosition
		{
			get
			{
				return this.PrevJoint;
			}
		}

		public Vector WristPosition
		{
			get
			{
				return this.NextJoint;
			}
		}

		public Arm()
		{
		}

		public Arm(Vector elbow, Vector wrist, Vector center, Vector direction, float length, float width, LeapQuaternion rotation) : base(elbow, wrist, center, direction, length, width, Bone.BoneType.TYPE_METACARPAL, rotation)
		{
		}

		public bool Equals(Arm other)
		{
			return base.Equals(other);
		}

		public override string ToString()
		{
			return "Arm";
		}
	}
}
