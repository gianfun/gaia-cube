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
	public GameObject highlighter;


	[SerializeField]
	public Collider plane;


	private GestureDetector left;
	private GestureDetector right;

	// Use this for initialization
	void Start () {
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

		if (left.isPinching && right.isPinching) {
			topLeftSelection = getPlanePoint (left.pinchPosition);
			bottomRightSelection = getPlanePoint (right.pinchPosition);
			Vector3 tl = topLeftSelection;
			Vector3 br = bottomRightSelection;

			highlighter.transform.position = new Vector3 ((tl.x + br.x) / 2, (tl.y + br.y) / 2, (tl.z + br.z) / 2);
			highlighter.transform.localScale = new Vector3 ((tl.x - br.x) , 1f, (tl.z - br.z) );
			highlighter.transform.rotation = Quaternion.identity;
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