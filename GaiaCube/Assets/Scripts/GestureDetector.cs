using UnityEngine;
using System.Collections;


public class GestureDetector : MonoBehaviour {
	[SerializeField]
	public Leap.Unity.IHandModel _handModel;

	[SerializeField]
	protected GameObject _cube;

	[SerializeField]
	protected Camera _camera;

	[SerializeField]
	public GameObject _plane;

	public Leap.Hand hand;
	public bool isPinching = false;
	public bool isOpenHand = false;
	public bool isPaw = false;
	public bool wasPaw = false;

	public Vector3 pinchPosition;
	public Vector3 pawStartPosition;
	public Vector3 palmVelocity;
	public int extendedFingers;
	public int rawExtendedFingers;
	public float fingerDistance;

	public int pawLeeway = 0;


	private Collider plane;
	// Use this for initialization
	void Start () {
		hand = _handModel.GetLeapHand ();
		plane = _plane.GetComponent<Collider>();

		//plane = (Plane)_plane;
	}
	
	// Update is called once per frame
	void Update () {
		hand = _handModel.GetLeapHand ();
		if (hand != null || !_handModel.IsTracked) {
			wasPaw = isPaw;

			isPinching = false;
			isOpenHand = false;
			isPaw = false;
			extendedFingers = 0;
			rawExtendedFingers = 0;
			fingerDistance = 0f;

			//Vector3 pos = hand.PalmPosition.ToVector3(); 
			pinchPosition = Vector3.zero;
			Vector3 handNormal = hand.PalmNormal.ToVector3();
			var fingers = hand.Fingers;

			if (hand.PinchDistance < 30f) {
				for (int i = 0; i < fingers.Count; i++) {
					var finger = fingers [i];
					if (finger.Type == Leap.Finger.FingerType.TYPE_INDEX ||
					    finger.Type == Leap.Finger.FingerType.TYPE_THUMB) {
						Leap.Vector v = finger.Bone (Leap.Bone.BoneType.TYPE_DISTAL).NextJoint;
						pinchPosition += new Vector3(v.x, v.y, v.z);
					}
				}
				pinchPosition /= 2.0f;
				isPinching = true;
			}

			fingerDistance = 0f;
			for (int i = 0; i < fingers.Count; i++) {
				var finger = fingers [i];
				Vector3 fDir = finger.Bone (Leap.Bone.BoneType.TYPE_DISTAL).Direction.ToVector3();
				if (Mathf.Abs( Vector3.Dot(fDir, handNormal)) < 0.50f && finger.IsExtended) {
					extendedFingers += 1;
				}
				if (finger.IsExtended) {
					rawExtendedFingers += 1;
				}

				if (finger.Type == Leap.Finger.FingerType.TYPE_MIDDLE ||
					finger.Type == Leap.Finger.FingerType.TYPE_RING ||
					finger.Type == Leap.Finger.FingerType.TYPE_PINKY) {
					fingerDistance += Vector3.Distance (finger.TipPosition.ToVector3(), fingers [i - 1].TipPosition.ToVector3());
				}
			}
			fingerDistance /= 3;



			Debug.DrawLine(hand.PalmPosition.ToVector3 (), hand.PalmPosition.ToVector3 () + hand.PalmNormal.Normalized.ToVector3 (), Color.red);

			//print ("Finger dist: " + fingerDistance);
			//print ("We have " + extendedFingers + " extended fingers!");
			if (rawExtendedFingers == 5) { //extendedFingers needs work.
				isOpenHand = true;
			}
			if (isOpenHand && fingerDistance < 0.021f) {
				pawLeeway = 0;
				isPaw = true;
			//	print ("Paw!");
			} else {
				if (pawLeeway > 10) {
					pawLeeway = -1;
					isPaw = false;
					wasPaw = false;
				}
				pawLeeway += 1;
			}
		
			palmVelocity = hand.PalmVelocity.ToVector3 ();
			
			if (!wasPaw && isPaw) { //Just became paw!
				pawStartPosition = hand.PalmPosition.ToVector3();
				print("Just became paw. Start pos: " + pawStartPosition);
			}
		} else {
			isPinching = false;
			isOpenHand = false;
			isPaw = false;
			extendedFingers = 0;
			fingerDistance = 0f;
		}
	}
}

