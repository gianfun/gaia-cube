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
	protected GameObject _controller;

	[SerializeField]
	public GameObject _plane;

	public int handednessFactor = 0;

	public Leap.Hand hand;
	public bool isPinching = false;
	public bool isOpenHand = false;
	public bool isPaw = false;
	public bool wasPaw = false;
	public bool facingInwards = false;
	public bool startedMovingForRotation = false;
	public bool doRotate  = false;

	public Vector3 c;
	public Vector3 c2;
	public Vector3 pinchPosition;
	public Vector3 pawStartPosition;
	public Vector3 handNormal;
	public Vector3 cntrlHandNormal;
	public Vector3 palmPosition;
	public Vector3 palmVelocity;
	public Vector3 palmVelocityWithHandedness;
	public int extendedFingers;
	public int rawExtendedFingers;
	public float fingerDistance;
	public float palmwidth;
	public float startedMovingForRotationTime;

	public int pawLeeway = 0;


	private Collider plane;
	// Use this for initialization
	void Start () {
		hand = _handModel.GetLeapHand ();
		plane = _plane.GetComponent<Collider>();

		//Some directions need to change depending if this is the left or right hand.
		handednessFactor = (hand.IsLeft) ? 1 : -1;
	}
	
	// Update is called once per frame
	void Update () {
		hand = _handModel.GetLeapHand ();
		if (hand != null || !_handModel.IsTracked) {
			wasPaw = isPaw;
			doRotate = false;
			isPinching = false;
			isOpenHand = false;
			isPaw = false;
			extendedFingers = 0;
			rawExtendedFingers = 0;
			fingerDistance = 0f;

			//Vector3 pos = hand.PalmPosition.ToVector3(); 	
			pinchPosition = Vector3.zero;
			handNormal = hand.PalmNormal.ToVector3 ();  //Normal vector from hand palm (in world coords)
			c = _controller.transform.rotation.eulerAngles;
			c2 = (_controller.transform.rotation * handNormal).normalized; //new Vector3(Mathf.Sin(c.y/360f*Mathf.PI),Mathf.Cos(c.y/360f*Mathf.PI), Mathf.Tan(c.y/360f*Mathf.PI));
			Quaternion inv = Quaternion.Inverse(_controller.transform.rotation);
			cntrlHandNormal = (inv * handNormal).normalized; // Normal vector in camera coords
			//float p = _controller.transform.rotation.eulerAngles.y;
			//print(_controller.transform.rotation.eulerAngles.y); 
			//print ((_controller.transform.rotation.eulerAngles.y).GetType() );
			//print(_controller.transform.rotation.eulerAngles.y == p);
			//print((p - 315));
			//if (_controller.transform.rotation.eulerAngles.y == 135 || _controller.transform.rotation.eulerAngles.y == 315) {
			//	cntrlHandNormal *= -1;
			//}

			palmVelocity = inv * (hand.PalmVelocity.ToVector3() - _controller.transform.position); // Palm velocity vector
			palmwidth = hand.PalmWidth	; // Palm velocity vector
			//Positive if going to 'center' (if left hand going right or vice versa)
			palmVelocityWithHandedness = new Vector3(palmVelocity.x, palmVelocity.y, palmVelocity.z*handednessFactor);
			palmPosition = hand.PalmPosition.ToVector3 ();	
			var fingers = hand.Fingers;

			//*-- Pinch Check --*//
			//If we are pinching, calculate pinch position.
			if (hand.PinchDistance < 30f) {
				for (int i = 0; i < fingers.Count; i++) {
					var finger = fingers [i];
					//Get position of thumb and index
					if (finger.Type == Leap.Finger.FingerType.TYPE_INDEX ||
					    finger.Type == Leap.Finger.FingerType.TYPE_THUMB) {
						Leap.Vector v = finger.Bone (Leap.Bone.BoneType.TYPE_DISTAL).NextJoint;
						//Sum position of the tip of the fingers
						pinchPosition += new Vector3 (v.x, v.y, v.z);
					}
				}
				//Divide by two to get mean position 
				pinchPosition /= 2.0f;
				//Set boolean
				isPinching = true;
			}

			//*-- Paw Check --*//
			//Calculate mean distance between pairs of fingers.
			fingerDistance = 0f;
			for (int i = 0; i < fingers.Count; i++) {
				var finger = fingers [i];
				Vector3 fDir = finger.Bone (Leap.Bone.BoneType.TYPE_DISTAL).Direction.ToVector3();

				//see if angle between finger and palm normal is larger than a certain value
				if (Mathf.Abs( Vector3.Dot(fDir, handNormal)) < 0.50f && finger.IsExtended) {
					extendedFingers += 1;
				}
				//uses just the isExtended
				if (finger.IsExtended) {
					rawExtendedFingers += 1;
				}
				//For the last 3 fingers, check distance to previous finger
				if (finger.Type == Leap.Finger.FingerType.TYPE_MIDDLE ||
					finger.Type == Leap.Finger.FingerType.TYPE_RING ||
					finger.Type == Leap.Finger.FingerType.TYPE_PINKY) {
					fingerDistance += Vector3.Distance (finger.TipPosition.ToVector3(), fingers [i - 1].TipPosition.ToVector3());
				}
			}
			fingerDistance /= 3;

			//Draw debug line
			Debug.DrawLine(hand.PalmPosition.ToVector3 (), hand.PalmPosition.ToVector3 () + handNormal, Color.red);

			//Check if hand is open
			if (rawExtendedFingers == 5) { //extendedFingers needs work, so working with isExtended
				isOpenHand = true;
			}
			//Check if is currently paw
			if (isOpenHand && fingerDistance < 0.021f) {
				pawLeeway = 0;
				isPaw = true;
			} else { //If not currently paw, wait a bit to set not-paw (smoothing to obtain better results)
				if (pawLeeway > 10) {
					pawLeeway = -1;
					isPaw = false;
					wasPaw = false;
				}
				pawLeeway += 1;
			}

			//Check if just became paw
			if (!wasPaw && isPaw) { //Just became paw!
				pawStartPosition = hand.PalmPosition.ToVector3();
			}

			if (cntrlHandNormal.x * handednessFactor > 0.95f) {
				facingInwards = true;
			} else {
				facingInwards = false;
			}

			if (isOpenHand && facingInwards) {
				
				if ((palmVelocityWithHandedness.x * handednessFactor > 0.25) && ((Time.time - startedMovingForRotationTime) > 0.5f) ) {	
					startedMovingForRotation = true;
					startedMovingForRotationTime = Time.time;	
				}
			}


			if (startedMovingForRotation && (Time.time - startedMovingForRotationTime) < 0.5f) {
				float angleFromZ = Mathf.Atan2 (-cntrlHandNormal.z, cntrlHandNormal.x * handednessFactor )*(180/Mathf.PI);
				//print (angleFromZ);
				if (40f < angleFromZ){//	 && angleFromZ < 50f) {
					doRotate = true;
					startedMovingForRotation = false;
					//Dont reset startedMovingForRotationTime so that we can have a cooldown for that.
					print ("Action");
				}

			}
				
		} else {
			wasPaw = isPaw;
			doRotate = false;
			isPinching = false;
			isOpenHand = false;
			isPaw = false;
			extendedFingers = 0;
			rawExtendedFingers = 0;
			fingerDistance = 0f;
		}
	}
}

