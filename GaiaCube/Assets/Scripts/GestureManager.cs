using UnityEngine;
using System.Collections;

public static class UnityVectorExtension {

	/**
    * Converts a Leap Vector object to a UnityEngine Vector3 object.
    *
    * Does not convert to the Unity left-handed coordinate system or scale
    * the coordinates from millimeters to meters.
    */
	public static Vector3 ToVector3(this Leap.Vector vector) {
		return new Vector3(vector.x, vector.y, vector.z);
	}
}

public class GestureManager : MonoBehaviour {
	public GestureDetector gd1;
	public GestureDetector gd2;
	public Camera _camera;

	public bool moveEarthUp;
	public bool moveEarthDown;

	public float moveEarthYThreshold = 2f;
	public Leap.Unity.LeapHandController leapController;

	public Collider plane;

	[SerializeField]
	private UnityEngine.UI.Text infoText;

	public GestureDetector left;
	public GestureDetector right;

	private float extrudedLength = 0f;

	// Update is called once per frame
	void Update () {
		Vector3 topLeftSelection = Vector3.zero;
		Vector3 bottomRightSelection = Vector3.zero;

		SetBoolsToFalse ();

		if (gd1.hand != null && gd2.hand != null) {
			if (gd1.hand.IsLeft) {
				left = gd1;
				right = gd2;
			} else {
				left = gd2;
				right = gd1;
			}

			Vector3 a = (transform.rotation * left.hand.PalmNormal.ToVector3 ()).normalized;

			if (false && left.hand != null && right.hand != null) {
				infoText.text = "Confidence\t\t\t: Left: " + left.hand.Confidence + "  Right: " + right.hand.Confidence + "\n";
				infoText.text += "Extended Fingers \t: Left: " + left.extendedFingers + "  Right: " + right.extendedFingers + "\n";
				infoText.text += "Extended Fingers (raw): Left: " + left.rawExtendedFingers + "  Right: " + right.rawExtendedFingers + "\n";
				infoText.text += "Finger distance\t\t: Left: " + left.fingerDistance + "  Right: " + right.fingerDistance + "\n";
				infoText.text += "Paw      \t\t\t\t\t: Left: " + left.isPaw + "  Right: " + right.isPaw + "\n";
				infoText.text += "Paw position \t\t\t: Left: " + left.pawStartPosition + "  Right: " + right.pawStartPosition + "\n";
				infoText.text += "Palm position\t\t\t: Left: " + left.hand.PalmPosition.ToVector3 () + "  Right: " + right.hand.PalmPosition.ToVector3 () + "\n";
				infoText.text += "Palm velocity\t\t\t: Left: " + left.palmVelocity + "  Right: " + right.palmVelocity + "\n";
				infoText.text += "Palm velocity (raw)\t\t\t: Left: " + left.hand.PalmVelocity.ToVector3() + "  Right: " + right.hand.PalmVelocity.ToVector3() + "\n";
				infoText.text += "Palm Normal\t\t\t: Left: " + left.handNormal.normalized + "  Right: " + right.handNormal.normalized + "\n";
				infoText.text += "Palm Normal (cntrl)\t\t: Left: " + left.cntrlHandNormal.normalized + "  Right: " + right.cntrlHandNormal.normalized + "\n";
				infoText.text += "Palm Roll\t\t\t: Left: " + a + "  Right: " + right.hand.PalmNormal.Roll + "\n";
				infoText.text += "Palm Yaw\t\t\t: Left: " + left.hand.PalmNormal.Yaw + "  Right: " + right.hand.PalmNormal.Yaw + "\n";
				infoText.text += "Palm Pitch\t\t\t: Left: " + left.hand.PalmNormal.Pitch + "  Right: " + right.hand.PalmNormal.Pitch + "\n";
				infoText.text += "Small Finger velocity\t\t\t: Left: " + left.littleFingerVelocity + "  Right: " + right.littleFingerVelocity + "\n";
				infoText.text += "LastTipVelocity Sum\t\t\t: Left: " + left.lastTipVelocitySum + "  Right: " + right.lastTipVelocitySum + "\n";
				infoText.text += "lastPalmVelocity Sum\t\t\t: Left: " + left.lastPalmVelocitySum + "  Right: " + right.lastPalmVelocitySum + "\n";
				infoText.text += "palm - finger velocity\t\t\t: Left: " + (left.lastTipVelocitySum - left.lastPalmVelocitySum) + "  Right: " + (right.lastTipVelocitySum - right.lastPalmVelocitySum) + "\n";
			}

			if (left.isPinching && right.isPinching) {
				topLeftSelection = getPlanePoint (left.pinchPosition);
				bottomRightSelection = getPlanePoint (right.pinchPosition);
				Vector3 tl = topLeftSelection;
				Vector3 br = bottomRightSelection;

			}


			if (left.isPaw) {
				if (left.wasPaw) {
					//Is palm facing down or up
					if (Mathf.Abs (left.hand.PalmNormal.ToVector3 ().normalized.y) > 0.9f) {
						extrudedLength += left.palmVelocity.y;
						//print (extrudedLength);
						if (extrudedLength > moveEarthYThreshold) {
							
							moveEarthUp = true;
							extrudedLength = 0;
						} else if (extrudedLength < -moveEarthYThreshold) {
							moveEarthDown = true;
							extrudedLength = 0;
						}
					}
				}
			} else {
				extrudedLength = 0f;
			}
		}
	}

	private void SetBoolsToFalse(){
		moveEarthUp = false;
		moveEarthDown = false;
	}

	public Vector3 getPlanePoint(Vector3 pinchPoint){
		Vector3 camPos = _camera.transform.position; 
		Ray ray = new Ray (camPos, pinchPoint - camPos);
		RaycastHit rayHit;
		if (plane.Raycast (ray, out rayHit, 100f)) {
//			print ("Hit: " + rayHit.point);
			Vector3 hitPoint = rayHit.point;
			return new Vector3 (hitPoint.x, hitPoint.y, hitPoint.z);
		} else {
			return Vector3.zero;
		}
	}
}