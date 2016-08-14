using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public bool shouldUseLeap = false;

	public Transform cameraTrans;

	[SerializeField]
	private GestureManager GM;

	public bool doSelect = false;
	public bool turnLeft = false;
	public bool turnRight = false;
	public bool goToBirdsEye = false;
	public bool leaveBirdsEye = false;
	public bool doWater = false;
	public bool doFire = false;
	public bool doEarth = false;
	public bool moveEarthDown = false;
	public bool moveEarthUp = false;
	public bool doWind = false;

	public Ray ray_topleft;
	public Ray ray_bottomright;
	// Use this for initialization
	void Start () {
		cameraTrans = GameObject.FindWithTag ("MainCamera").GetComponent<Transform>() ;
	}
	
	// Update is called once per frame
	void Update () {
		setAllAsFalse ();

		if (shouldUseLeap) {
			if (GM.left.isPinching && GM.right.isPinching) {
				print ("GM.left&right.pinching");
				doSelect = true;
				ray_topleft = new Ray (cameraTrans.position, GM.left.pinchPosition - cameraTrans.position);
				ray_bottomright = new Ray (cameraTrans.position, GM.right.pinchPosition - cameraTrans.position);
			} else if (GM.left.doRotate) {
				print ("GM.left.doRotate");
				turnLeft = true;
			} else if (GM.right.doRotate) {
				print ("GM.right.doRotate");
				turnRight = true;
			} else if (GM.right.doRotate) {		
				print ("GM.enterBirdsEye");
				goToBirdsEye = true;
			} else if (GM.right.doRotate) {
				print ("GM.leaveBirdsEye");
				leaveBirdsEye = true;
			} else if (GM.moveEarthDown) {
				print ("GM.moveEarthUp");
				moveEarthDown = true;
			} else if (GM.moveEarthUp) {
				print ("GM.moveEarthUp");
				moveEarthUp = true;
			} else if (GM.doWater) {
				print ("GM.doWater");
				doWater = true;
			} else if (GM.doFire) {
				print ("GM.doFire");
				doFire = true;
			} else if (GM.doWind) {
				print ("GM.doWind");
				doWind = true;
			}
		} else {
			if (Input.GetButtonDown ("Fire1")) {
				//if (!doSelect) {
					doSelect = true;
					ray_topleft = Camera.main.ScreenPointToRay (Input.mousePosition);
					ray_bottomright = ray_topleft;
				//} else {
				//	ray_bottomright = Camera.main.ScreenPointToRay (Input.mousePosition);
				//}
			} else if ( Input.GetButton("Fire1")){
				doSelect = true;
				ray_bottomright = Camera.main.ScreenPointToRay (Input.mousePosition);
			} else if (Input.GetKeyDown ("d")) {
				turnLeft = true;
			} else if (Input.GetKeyDown ("a")) {
				turnRight = true;
			} else if (Input.GetKeyDown ("w")) {
				goToBirdsEye = true;
			} else if (Input.GetKeyDown ("s")) {
				leaveBirdsEye = true;
			} else if (Input.GetKeyDown ("down")) {
				moveEarthDown = true;
			} else if (Input.GetKeyDown ("up")) {
				moveEarthUp = true;
			} else if (Input.GetKeyDown ("1")) {
				doWater = true;
			} else if (Input.GetKeyDown ("2")) {
				doFire = true;
			} else if (Input.GetKeyDown ("3")) {
				doWind = true;
			}
		}
	}

	void setAllAsFalse(){
		doSelect = false;
		turnRight = false;
		turnLeft = false;
		goToBirdsEye = false;
		leaveBirdsEye = false;
		doWater = false;
		doFire = false;
		doEarth = false;
		moveEarthDown = false;
		moveEarthUp = false;
		doWind = false;
	}
}
