using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	[SerializeField]
	private PlayerController playerController;

	private bool birdsEye;
	private Transform pivot;
	private Transform cameraTrans;

	private GameObject boundingGrid;

	private bool isRotating = false;
	private float rotationAngle;
	private float currentCameraMovementTime;
	private float cameraMovementDuration = 0.7f;
	private Quaternion startRotation;
	private Vector3 startPosition;

	void Start () {
		birdsEye = false;
		cameraTrans = GameObject.FindWithTag ("MainCamera").GetComponent<Transform>();
		boundingGrid = GameObject.FindWithTag ("BoundingGrid");
		pivot = cameraTrans.GetChild (0);
	}

	void Update() {
		/*
		if (playerController.turnRight && !isRotating) {
			isRotating = true;
			startRotation = this.transform.rotation;
			startPosition = this.transform.position;
			rotationAngle = 90f;
			currentCameraMovementTime = 0f;
			boundingGrid.SetActive (false);
		} else if (playerController.turnLeft && !isRotating) {
			isRotating = true;
			startRotation = this.transform.rotation;
			startPosition = this.transform.position;
			rotationAngle = -90f;
			currentCameraMovementTime = 0f;
			boundingGrid.SetActive (false);
		} else */if (playerController.goToBirdsEye && !birdsEye && !isRotating) {
			birdsEye = true;
			cameraTrans.RotateAround (Vector3.zero, pivot.up, -45);
		} else if (playerController.leaveBirdsEye && birdsEye && !isRotating) {
			birdsEye = false;
			cameraTrans.RotateAround (Vector3.zero, pivot.up, 45);
		}

		if (isRotating) {
			currentCameraMovementTime += Time.deltaTime;
			float rotationPercent = currentCameraMovementTime / cameraMovementDuration;
			if(rotationPercent >= 1.0f){
				isRotating = false;
				rotationPercent = 1.0f; //So our rotation turns exactly 'rotationAngle' degrees
				boundingGrid.SetActive (true);
			}
			this.transform.position = startPosition;
			this.transform.rotation = startRotation;
			this.transform.RotateAround (Vector3.zero, Vector3.up, rotationAngle * rotationPercent);
			//Quaternion rot = Quaternion.Slerp (startRotation, endRotation, currentCameraMovementTime / cameraMovementDuration);
			//this.transform.rotation = rot;
			
		}
	}
}
