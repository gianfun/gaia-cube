﻿using System;

namespace Leap.Unity
{
    public static class CopyFromOtherExtensions
    {
        public static Frame CopyFrom(this Frame frame, Frame source)
        {
            frame.Id = source.Id;
            frame.Timestamp = source.Timestamp;
            frame.CurrentFramesPerSecond = source.CurrentFramesPerSecond;
            frame.InteractionBox = source.InteractionBox;
            frame.ResizeHandList(source.Hands.Count);
            int count = frame.Hands.Count;
            while (count-- != 0)
            {
                UnityEngine.Debug.Log("E");
                frame.Hands[count].CopyFrom(source.Hands[count]);
            }
            return frame;
        }

        public static Hand CopyFrom(this Hand hand, Hand source)
        {
            hand.Id = source.Id;
            hand.Confidence = source.Confidence;
            hand.GrabStrength = source.GrabStrength;
            hand.GrabAngle = source.GrabAngle;
            hand.PinchStrength = source.PinchStrength;
            hand.PinchDistance = source.PinchDistance;
            hand.PalmWidth = source.PalmWidth;
            hand.IsLeft = source.IsLeft;
            hand.TimeVisible = source.TimeVisible;
            hand.PalmPosition = source.PalmPosition;
            hand.StabilizedPalmPosition = source.StabilizedPalmPosition;
            hand.PalmVelocity = source.PalmVelocity;
            hand.PalmNormal = source.PalmNormal;
            hand.Direction = source.Direction;
            hand.WristPosition = source.WristPosition;
            UnityEngine.Debug.Log("b - handArm: "  + hand.Arm + "  source.Arm: "+ source.Arm);
            hand.Arm = new Arm();
            hand.Arm.CopyFrom(source.Arm);
            int index = 5;
            while (index-- != 0)
            {
                UnityEngine.Debug.Log("a - bone #" + index);
                hand.Fingers[index].CopyFrom(source.Fingers[index]);
            }
            return hand;
        }

        public static Finger CopyFrom(this Finger finger, Finger source)
        {
            int num = 4;
            while (num-- != 0)
            {
                UnityEngine.Debug.Log("a - finger #" + num);
                finger._bones[num].CopyFrom(source._bones[num]);
            }
            finger.Id = source.Id;
            finger.HandId = source.HandId;
            finger.TimeVisible = source.TimeVisible;
            finger.TipPosition = source.TipPosition;
            finger.TipVelocity = source.TipVelocity;
            finger.Direction = source.Direction;
            finger.StabilizedTipPosition = source.StabilizedTipPosition;
            finger.Width = source.Width;
            finger.Length = source.Length;
            finger.IsExtended = source.IsExtended;
            finger.Type = source.Type;
            return finger;
        }

        public static Bone CopyFrom(this Bone bone, Bone source)
        {
            bone = new Bone();
            bone
                .PrevJoint =
                source.
                PrevJoint;
            bone.NextJoint = source.NextJoint;
            bone.Direction = source.Direction;
            bone.Center = source.Center;
            bone.Length = source.Length;
            bone.Width = source.Width;
            bone.Rotation = source.Rotation;
            bone.Type = source.Type;
            return bone;
        }
    }
}
