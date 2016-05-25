using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private bool birdsEye;
	private Transform pivot;

	void Start () {
		birdsEye = false;
		pivot = transform.GetChild (0);
	}

	void Update() {
		if (Input.GetKeyDown ("d")) {
			transform.RotateAround (Vector3.zero, Vector3.up, 90);
		} else if (Input.GetKeyDown ("a")) {
			transform.RotateAround (Vector3.zero, Vector3.up, -90);
		} else if (Input.GetKeyDown ("w") && !birdsEye) {
			birdsEye = true;
			transform.RotateAround (Vector3.zero, pivot.up, -45);
		} else if (Input.GetKeyDown ("s") && birdsEye) {
			birdsEye = false;
			transform.RotateAround (Vector3.zero, pivot.up, 45);
		}
	}
}
