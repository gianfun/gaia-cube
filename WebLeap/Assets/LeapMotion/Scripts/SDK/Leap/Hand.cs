using System;
using System.Collections.Generic;

namespace Leap
{
	[Serializable]
	public class Hand
	{
		public long FrameId;

		public int Id;

		public List<Finger> Fingers;

		public Vector PalmPosition;

		public Vector PalmVelocity;

		public Vector PalmNormal;

		public Vector Direction;

		public float GrabStrength;

		public float GrabAngle;

		public float PinchStrength;

		public float PinchDistance;

		public float PalmWidth;

		public Vector StabilizedPalmPosition;

		public Vector WristPosition;

		public float TimeVisible;

		public float Confidence;

		public bool IsLeft;

		public Arm Arm;

		public LeapTransform Basis
		{
			get
			{
				return new LeapTransform(this.PalmPosition, this.Rotation);
			}
		}

		public LeapQuaternion Rotation
		{
			get
			{
				return this.Fingers[2].Bone(Bone.BoneType.TYPE_METACARPAL).Rotation;
			}
		}

		public bool IsRight
		{
			get
			{
				return !this.IsLeft;
			}
		}

		public Hand()
		{
			this.Arm = new Arm();
			this.Fingers = new List<Finger>(5);
			this.Fingers.Add(new Finger());
			this.Fingers.Add(new Finger());
			this.Fingers.Add(new Finger());
			this.Fingers.Add(new Finger());
			this.Fingers.Add(new Finger());
		}

		public Hand(long frameID, int id, float confidence, float grabStrength, float grabAngle, float pinchStrength, float pinchDistance, float palmWidth, bool isLeft, float timeVisible, Arm arm, List<Finger> fingers, Vector palmPosition, Vector stabilizedPalmPosition, Vector palmVelocity, Vector palmNormal, Vector direction, Vector wristPosition)
		{
			this.FrameId = frameID;
			this.Id = id;
			this.Confidence = confidence;
			this.GrabStrength = grabStrength;
			this.GrabAngle = grabAngle;
			this.PinchStrength = pinchStrength;
			this.PinchDistance = pinchDistance;
			this.PalmWidth = palmWidth;
			this.IsLeft = isLeft;
			this.TimeVisible = timeVisible;
			this.Arm = arm;
			this.Fingers = fingers;
			this.PalmPosition = palmPosition;
			this.StabilizedPalmPosition = stabilizedPalmPosition;
			this.PalmVelocity = palmVelocity;
			this.PalmNormal = palmNormal;
			this.Direction = direction;
			this.WristPosition = wristPosition;
		}

		public Finger Finger(int id)
		{
			int count = this.Fingers.Count;
			Finger result;
			while (count-- != 0)
			{
				if (this.Fingers[count].Id == id)
				{
					result = this.Fingers[count];
					return result;
				}
			}
			result = null;
			return result;
		}

		public bool Equals(Hand other)
		{
			return this.Id == other.Id && this.FrameId == other.FrameId;
		}

		public override string ToString()
		{
			return string.Format("Hand {0} {1}.", this.Id, this.IsLeft ? "left" : "right");
		}
	}
}
