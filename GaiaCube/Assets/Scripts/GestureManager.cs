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
	[SerializeField]
	public GestureDetector gd1;
	[SerializeField]
	public GestureDetector gd2;
	[SerializeField]
	public Camera _camera;

	[SerializeField]
	public Leap.Unity.LeapHandController leapController;

	[SerializeField]
	public GameObject highlighter;


	[SerializeField]
	public Collider plane;

	[SerializeField]
	private UnityEngine.UI.Text infoText;

	private GestureDetector left;
	private GestureDetector right;

	private bool rotatingLeft = false;
	private bool rotatingRight = false;
	private Quaternion startRot;
	private Quaternion endRot;

	private float extrudedLength = 0f;
	private float cameraMovementTime;
	private float currentCameraMovementTime;

	// Use this for initialization
	void Start () {
		cameraMovementTime = 0.7f;
		currentCameraMovementTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 topLeftSelection = Vector3.zero;
		Vector3 bottomRightSelection = Vector3.zero;
		if (gd1.hand.IsLeft) {
			left = gd1;
			right = gd2;
		} else {
			left = gd2;
			right = gd1;
		}

		Vector3 a =  (transform.rotation * left.hand.PalmNormal.ToVector3 ()) .normalized;
		infoText.text =  "Confidence\t\t\t: Left: " 		+ left.hand.Confidence 		+ "  Right: " + right.hand.Confidence 		+ "\n";
		infoText.text += "Extended Fingers \t: Left: " 		+ left.extendedFingers 		+ "  Right: " + right.extendedFingers 		+ "\n";
		infoText.text += "Extended Fingers (raw): Left: " 	+ left.rawExtendedFingers 	+ "  Right: " + right.rawExtendedFingers 	+ "\n";
		infoText.text += "Finger distance\t\t: Left: " 		+ left.fingerDistance  		+ "  Right: " + right.fingerDistance  		+ "\n";
		infoText.text += "Paw      \t\t\t\t\t: Left: " 		+ left.isPaw           		+ "  Right: " + right.isPaw           		+ "\n";
		infoText.text += "Paw position \t\t\t: Left: " 		+ left.pawStartPosition		+ "  Right: " + right.pawStartPosition		+ "\n";
		infoText.text += "Palm position\t\t\t: Left: " 		+ left.hand.PalmPosition.ToVector3()			+ "  Right: " + right.hand.PalmPosition.ToVector3() 			+ "\n";
		infoText.text += "Palm velocity\t\t\t: Left: " 		+ left.palmVelocity			+ "  Right: " + right.palmVelocity 			+ "\n";
		infoText.text += "Palm Normal\t\t\t: Left: " 		+ left.handNormal.normalized			+ "  Right: " + right.handNormal.normalized 			+ "\n";
		infoText.text += "Palm Normal (cntrl)\t\t: Left: " 		+ left.cntrlHandNormal.normalized			+ "  Right: " + right.cntrlHandNormal.normalized 			+ "\n";
		infoText.text += "Palm Roll\t\t\t: Left: " 		+ a		+ "  Right: " + right.hand.PalmNormal.Roll 			+ "\n";
		infoText.text += "Palm Yaw\t\t\t: Left: " 		+ left.hand.PalmNormal.Yaw			+ "  Right: " + right.hand.PalmNormal.Yaw 			+ "\n";
		infoText.text += "Palm Pitch\t\t\t: Left: " 		+ left.hand.PalmNormal.Pitch			+ "  Right: " + right.hand.PalmNormal.Pitch 			+ "\n";

		if (left.isPinching && right.isPinching) {
			topLeftSelection = getPlanePoint (left.pinchPosition);
			bottomRightSelection = getPlanePoint (right.pinchPosition);
			Vector3 tl = topLeftSelection;
			Vector3 br = bottomRightSelection;

			highlighter.transform.position = new Vector3 ((tl.x + br.x) / 2, (tl.y + br.y) / 2, (tl.z + br.z) / 2);
			highlighter.transform.localScale = new Vector3 ((tl.x - br.x) , 1f, (tl.z - br.z) );
			highlighter.transform.rotation = Quaternion.identity;
		}


		if (left.isPaw) {
			if (!left.wasPaw) {
				highlighter.transform.position = new Vector3 (0f, 0f, 0f);
			} else {
				if( Mathf.Abs(left.hand.PalmNormal.ToVector3().normalized.y) > 0.9f){
					extrudedLength += left.palmVelocity.y ;
					if (extrudedLength < 0f) {
						extrudedLength = 0;
					}
				}
				highlighter.transform.localScale = new Vector3 (1f, extrudedLength, 1f);
				highlighter.transform.position = new Vector3 (0f, highlighter.transform.localScale.y/2f, 0f);
				highlighter.transform.rotation = Quaternion.identity;
			}
		} else {
			extrudedLength = 0f;
		}

		if (left.doRotate) {
			rotatingLeft = true;
			startRot = this.transform.rotation;
			this.transform.RotateAround(Vector3.zero, Vector3.up, 90f);
			endRot = this.transform.rotation;
			currentCameraMovementTime = 0f;
			this.transform.rotation = startRot;

			print("boop");

		}
		if (right.doRotate) {
			rotatingRight = true;
			this.transform.RotateAround(Vector3.zero, Vector3.up, -90f);
			print("boop");
		}

		if (rotatingLeft) {
			currentCameraMovementTime += Time.deltaTime;
			Quaternion rot = Quaternion.Slerp (startRot, endRot, currentCameraMovementTime / cameraMovementTime);
			this.transform.rotation = rot;
		}
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