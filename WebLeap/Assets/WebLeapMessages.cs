using UnityEngine;
using System.Collections;
using System;

public class WebLeap
{
    public class smth
    {

    }

    public class FloatArray
    {

    }

    [Serializable]
    public class Hand
    {
        public Vector3[] armBasis;
        public float armWidth;
    }

    [Serializable]
    public class TrackingMsg
    {
        public float currentFrameRate;
        public string devices;
        public string gestures;
        public string hands;
        public int id;
        public string interactionBox;
        public string pointables;
        public string r;
        public float s;
        public string t;
        public long timestamp;
    }
}
