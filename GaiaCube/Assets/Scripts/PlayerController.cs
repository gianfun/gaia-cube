using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public bool shouldUseLeap = false;

	[SerializeField]
	private GestureManager GM;

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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		setAllAsFalse ();

		if (shouldUseLeap) {
			if (GM.left.doRotate) {
				turnLeft = true;
			} else if (GM.right.doRotate) {
				turnRight = true;
			} else if (GM.right.doRotate) {				
				goToBirdsEye = true;
			} else if (GM.right.doRotate) {
				leaveBirdsEye = true;
			} else if (GM.moveEarthDown) {
				moveEarthDown = true;
			} else if (GM.moveEarthUp) {
				moveEarthUp = true;
			} else if (GM.right.doRotate) {
				doWater = true;
			} else if (GM.right.doRotate) {
				doFire = true;
			}
		} else {
			if (Input.GetKeyDown ("d")) {
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
			}
		}
	}

	void setAllAsFalse(){
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
