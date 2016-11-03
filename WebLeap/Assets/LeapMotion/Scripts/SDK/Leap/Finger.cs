using System;

namespace Leap
{
	[Serializable]
	public class Finger
	{
		public enum FingerType
		{
			TYPE_THUMB,
			TYPE_INDEX,
			TYPE_MIDDLE,
			TYPE_RING,
			TYPE_PINKY,
			TYPE_UNKNOWN = -1
		}

		public Bone[] bones = new Bone[4];

		public Finger.FingerType Type;

		public int Id;

		public int HandId;

		public Vector TipPosition;

		public Vector TipVelocity;

		public Vector Direction;

		public float Width;

		public float Length;

		public bool IsExtended;

		public Vector StabilizedTipPosition;

		public float TimeVisible;

		public Finger()
		{
			this.bones[0] = new Bone();
			this.bones[1] = new Bone();
			this.bones[2] = new Bone();
			this.bones[3] = new Bone();
		}

		public Finger(long frameId, int handId, int fingerId, float timeVisible, Vector tipPosition, Vector tipVelocity, Vector direction, Vector stabilizedTipPosition, float width, float length, bool isExtended, Finger.FingerType type, Bone metacarpal, Bone proximal, Bone intermediate, Bone distal)
		{
			this.Type = type;
			this.bones[0] = metacarpal;
			this.bones[1] = proximal;
			this.bones[2] = intermediate;
			this.bones[3] = distal;
			this.Id = handId * 10 + fingerId;
			this.HandId = handId;
			this.TipPosition = tipPosition;
			this.TipVelocity = tipVelocity;
			this.Direction = direction;
			this.Width = width;
			this.Length = length;
			this.IsExtended = isExtended;
			this.StabilizedTipPosition = stabilizedTipPosition;
			this.TimeVisible = timeVisible;
		}

		public Bone Bone(Bone.BoneType boneIx)
		{
			return this.bones[(int)boneIx];
		}

		public override string ToString()
		{
			return Enum.GetName(typeof(Finger.FingerType), this.Type) + " id:" + this.Id;
		}
	}
}
