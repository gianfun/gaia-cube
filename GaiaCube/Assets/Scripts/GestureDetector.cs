using UnityEngine;
using System.Collections;


public class GestureDetector : MonoBehaviour {
	[SerializeField]
	public Leap.Unity.IHandModel _handModel;

	[SerializeField]
	protected Transform _controllerTransform;

	public int handednessFactor = 0;
    public bool isLeft;

	public Leap.Hand hand;
	public bool isPinching 					= false;
	public bool isOpenHand 					= false;
	public bool isFist 	        			= false;
    public bool isPaw 						= false;
	public bool wasPaw 						= false;
	public bool facingInwards 				= false;
	public bool startedMovingForRotation 	= false;
	public bool doRotate  					= false;
	public bool makeItRain  				= false;

	private float[] lastTipVelocities = new float[20];
	private int lastTipVelocityIndex = 0;
	public  float lastTipVelocitySum = 0;

	private float[] lastPalmVelocities = new float[20];
	public  float lastPalmVelocitySum = 0;

	public Vector3 c;
	public Vector3 c2;
	public Vector3 pinchPosition;
	public Vector3 pawStartPosition;
	public Vector3 handNormal;
	public Vector3 cntrlHandNormal;
	public Vector3 palmPosition;
	public Vector3 palmVelocity;
	public Vector3 palmVelocityWithHandedness;
	public Vector3 littleFingerVelocity;
	public int extendedFingers;
	public int rawExtendedFingers;
	public float fingerDistance;
	public float palmwidth;
	public float startedMovingForRotationTime;
	public float startedElementalActionTime;
	public Action currentAction;
    public float middleFingerToPalmDistance;
    public float angleMiddleFingerAndPalmNormal;
    public float angleMiddleFingerProximalAndIntermediate;


	public int pawLeeway = 0;
	public float delayBetweenActions = 1f;

	public enum Action
	{
		None, Fire, Water, Wind
	};

	// Use this for initialization
	void Start () {
	}

	void Awake(){
		hand = _handModel.GetLeapHand ();
        isLeft = (_handModel.Handedness == Leap.Unity.Chirality.Left);
        //Some directions need to change depending if this is the left or right hand.
        handednessFactor = isLeft ? 1 : -1;
	}
	
	// Update is called once per frame
	void Update () {
		hand = _handModel.GetLeapHand ();
		if (hand != null || !_handModel.IsTracked) {
			wasPaw = isPaw;
			doRotate = false;
			isPinching = false;
			isOpenHand = false;
			isFist = false;
			isPaw = false;
			extendedFingers = 0;
			rawExtendedFingers = 0;
			fingerDistance = 0f;

			//Vector3 pos = hand.PalmPosition.ToVector3(); 	
			pinchPosition = Vector3.zero;
			handNormal = hand.PalmNormal.ToVector3 ();  //Normal vector from hand palm (in world coords)
			c = _controllerTransform.rotation.eulerAngles;
			c2 = (_controllerTransform.rotation * handNormal).normalized; //new Vector3(Mathf.Sin(c.y/360f*Mathf.PI),Mathf.Cos(c.y/360f*Mathf.PI), Mathf.Tan(c.y/360f*Mathf.PI));
			Quaternion inv = Quaternion.Inverse(_controllerTransform.rotation);
			cntrlHandNormal = (inv * handNormal).normalized; // Normal vector in camera coords
			//float p = _controller.transform.rotation.eulerAngles.y;
			//print(_controller.transform.rotation.eulerAngles.y); 
			//print ((_controller.transform.rotation.eulerAngles.y).GetType() );
			//print(_controller.transform.rotation.eulerAngles.y == p);
			//print((p - 315));
			//if (_controller.transform.rotation.eulerAngles.y == 135 || _controller.transform.rotation.eulerAngles.y == 315) {
			//	cntrlHandNormal *= -1;
			//}
			littleFingerVelocity = inv * hand.Fingers[4].TipVelocity.ToVector3();
			lastTipVelocitySum += littleFingerVelocity.sqrMagnitude;
			lastTipVelocitySum -= lastTipVelocities [lastTipVelocityIndex % 20];
			lastTipVelocities [lastTipVelocityIndex % 20] = littleFingerVelocity.sqrMagnitude;

            palmVelocity = inv * hand.PalmVelocity.ToVector3() ; // Palm velocity vector
			lastPalmVelocitySum -= lastPalmVelocities [lastTipVelocityIndex % 20];
			lastPalmVelocitySum += palmVelocity.sqrMagnitude;
			lastPalmVelocities [lastTipVelocityIndex % 20] = palmVelocity.sqrMagnitude;

			lastTipVelocityIndex++;

			palmwidth = hand.PalmWidth	; // Palm width vector
			//Positive if going to 'center' (if left hand going right or vice versa)
			palmVelocityWithHandedness = new Vector3(palmVelocity.x, palmVelocity.y, palmVelocity.z*handednessFactor);
			palmPosition = inv * hand.PalmPosition.ToVector3 ();	
			var fingers = hand.Fingers;

			//*-- Pinch Check --*//
			//If we are pinching, calculate pinch position.
			if (hand.PinchDistance < 30f && cntrlHandNormal.y < 0 && Mathf.Abs(cntrlHandNormal.x) > 0.7f) {
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

                if (finger.Type == Leap.Finger.FingerType.TYPE_MIDDLE)
                {
                    middleFingerToPalmDistance = Vector3.Distance(finger.TipPosition.ToVector3(), palmPosition);
                    angleMiddleFingerAndPalmNormal = Vector3.Dot(finger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Direction.ToVector3(), handNormal);
                    angleMiddleFingerProximalAndIntermediate = Vector3.Dot(finger.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Direction.ToVector3(), finger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Direction.ToVector3());
                    if (angleMiddleFingerAndPalmNormal > 0.9f && angleMiddleFingerProximalAndIntermediate < 0.4)
                    {
                        isFist = true;
                    }
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
			if (isOpenHand && fingerDistance < 0.025f) {
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
					print (angleFromZ);
				if (40f < angleFromZ){//	 && angleFromZ < 50f) {
					doRotate = true;
					startedMovingForRotation = false;
					//Dont reset startedMovingForRotationTime so that we can have a cooldown for that.
					print ("Action");
				}

			}

			if (Time.time - startedElementalActionTime > delayBetweenActions) {
				if (extendedFingers == 0 && lastTipVelocitySum / lastPalmVelocitySum > 5f && lastPalmVelocitySum < 1f) {
					if (handNormal.normalized.y < -0.9f) { //Is facing down
						currentAction = Action.Water;
						startedElementalActionTime = Time.time;
					} else if (handNormal.normalized.y > 0.9f) { //Is facing up
						currentAction = Action.Fire;
						startedElementalActionTime = Time.time;
					} else if (facingInwards) { //Is facing inwards
						currentAction = Action.Wind;
						startedElementalActionTime = Time.time;
					} else { //Something else
						currentAction = Action.None;
					}
				} else { //Something else
					currentAction = Action.None;
				}
			} 

				
		} else {
			wasPaw = isPaw;
			doRotate = false;
			isPinching = false;
			isOpenHand = false;
            isFist = false;
            isPaw = false;
			makeItRain  = false;
			extendedFingers = 0;
			rawExtendedFingers = 0;
			fingerDistance = 0f;

		}
	}

    void OnDisable()
    {
        wasPaw = isPaw;
        doRotate = false;
        isPinching = false;
        isOpenHand = false;
        isFist = false;
        isPaw = false;
        makeItRain = false;
        extendedFingers = 0;
        rawExtendedFingers = 0;
        fingerDistance = 0f;
    }
}

