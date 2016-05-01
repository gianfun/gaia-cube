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

	public Vector3 pinchPosition;
	public int extendedFingers;
	public float fingerDistance;

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
			isPinching = false;
			isOpenHand = false;
			isPaw = false;
			extendedFingers = 0;
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
				if (Vector3.Dot(fDir, handNormal) < 0.40f && finger.IsExtended) {
					extendedFingers += 1;
				}

				if (finger.Type == Leap.Finger.FingerType.TYPE_MIDDLE ||
					finger.Type == Leap.Finger.FingerType.TYPE_RING ||
					finger.Type == Leap.Finger.FingerType.TYPE_PINKY) {
					fingerDistance += Vector3.Distance (finger.TipPosition.ToVector3(), fingers [i - 1].TipPosition.ToVector3());
				}
			}
			fingerDistance /= 3;

			print ("Finger dist: " + fingerDistance);
			print ("We have " + extendedFingers + " extended fingers!");
			if (extendedFingers == 5) {
				isOpenHand = true;
			}
			if (isOpenHand && fingerDistance < 0.021f) {
				isPaw = true;
				print ("Paw!");
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

