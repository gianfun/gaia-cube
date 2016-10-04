using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	[SerializeField]
	private PlayerController playerController;

	private bool birdsEye;
	private Transform pivot;
	private Transform cameraTrans;

	private bool isRotating = true;

	void Start () {
		birdsEye = false;
		cameraTrans = GameObject.FindWithTag ("MainCamera").GetComponent<Transform>();
		pivot = cameraTrans.GetChild (0);
	}

	void Update() {
		if (playerController.goToBirdsEye && !birdsEye && !isRotating) {
			birdsEye = true;
			cameraTrans.RotateAround (Vector3.zero, pivot.up, -45);
		} else if (playerController.leaveBirdsEye && birdsEye && !isRotating) {
			birdsEye = false;
			cameraTrans.RotateAround (Vector3.zero, pivot.up, 45);
		}
	}
}
