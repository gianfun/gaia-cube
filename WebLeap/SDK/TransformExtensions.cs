using System;

namespace Leap.Unity
{
    public static class TransformExtensions
    {
        public static Frame Transform(this Frame frame, LeapTransform transform)
        {
            int count = frame.Hands.Count;
            while (count-- != 0)
            {
                frame.Hands[count].Transform(transform);
            }
            return frame;
        }

        public static Frame TransformedCopy(this Frame frame, LeapTransform transform)
        {
            return new Frame().CopyFrom(frame).Transform(transform);
        }

        public static Hand Transform(this Hand hand, LeapTransform transform)
        {
            hand.PalmPosition = transform.TransformPoint(hand.PalmPosition);
            hand.StabilizedPalmPosition = transform.TransformPoint(hand.StabilizedPalmPosition);
            hand.PalmVelocity = transform.TransformVelocity(hand.PalmVelocity);
            hand.PalmNormal = transform.TransformDirection(hand.PalmNormal);
            hand.Direction = transform.TransformDirection(hand.Direction);
            hand.WristPosition = transform.TransformPoint(hand.WristPosition);
            hand.PalmWidth *= Math.Abs(transform.scale.x);
            hand.Arm.Transform(transform);
            int index = 5;
            while (index-- != 0)
            {
                hand.Fingers[index].Transform(transform);
            }
            return hand;
        }

        public static Hand TransformedCopy(this Hand hand, LeapTransform transform)
        {
            UnityEngine.Debug.Log("D");
            return new Hand().CopyFrom(hand).Transform(transform);
        }

        public static Finger Transform(this Finger finger, LeapTransform transform)
        {
            Bone bone = finger._bones[3];
            bone.NextJoint = transform.TransformPoint(bone.NextJoint);
            finger.TipPosition = bone.NextJoint;
            int num = 3;
            while (num-- != 0)
            {
                Bone bone2 = finger._bones[num];
                bone2.NextJoint = (bone.PrevJoint = transform.TransformPoint(bone2.NextJoint));
                bone.TransformGivenJoints(transform);
                bone = bone2;
            }
            bone.PrevJoint = transform.TransformPoint(bone.PrevJoint);
            bone.TransformGivenJoints(transform);
            finger.TipVelocity = transform.TransformVelocity(finger.TipVelocity);
            finger.Direction = finger._bones[2].Direction;
            finger.StabilizedTipPosition = transform.TransformPoint(finger.StabilizedTipPosition);
            finger.Width *= Math.Abs(transform.scale.x);
            finger.Length *= Math.Abs(transform.scale.z);
            return finger;
        }

        public static Finger TransformedCopy(this Finger finger, LeapTransform transform)
        {
            return new Finger().CopyFrom(finger).Transform(transform);
        }

        public static Bone Transform(this Bone bone, LeapTransform transform)
        {
            bone.PrevJoint = transform.TransformPoint(bone.PrevJoint);
            bone.NextJoint = transform.TransformPoint(bone.NextJoint);
            bone.TransformGivenJoints(transform);
            return bone;
        }

        internal static void TransformGivenJoints(this Bone bone, LeapTransform transform)
        {
            bone.Length *= Math.Abs(transform.scale.z);
            bone.Center = (bone.PrevJoint + bone.NextJoint) / 2f;
            if (bone.Length < 1.401298E-45f)
            {
                bone.Direction = Vector.Zero;
            }
            else
            {
                bone.Direction = (bone.NextJoint - bone.PrevJoint) / bone.Length;
            }
            bone.Width *= Math.Abs(transform.scale.x);
            bone.Rotation = transform.TransformQuaternion(bone.Rotation);
        }

        public static Bone TransformedCopy(this Bone bone, LeapTransform transform)
        {
            UnityEngine.Debug.Log("c");
            Bone b = new Bone();
            return b.CopyFrom(bone).Transform(transform);
        }
    }
}
