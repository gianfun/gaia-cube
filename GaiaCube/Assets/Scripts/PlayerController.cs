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
	public bool doWind = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		setAllAsFalse ();

		if (shouldUseLeap) {
			//if(GM.
		} else {
			if (Input.GetKeyDown ("d")) {
				turnLeft = true;
			} else if (Input.GetKeyDown ("a")) {
				turnRight = true;
			} else if (Input.GetKeyDown ("w")) {
				goToBirdsEye = true;
			} else if (Input.GetKeyDown ("s")) {
				leaveBirdsEye = true;
			} else if (Input.GetKeyDown ("1")) {
				doWater = true;
			} else if (Input.GetKeyDown ("2")) {
				doFire = true;
			}
		}
	}

	void setAllAsFalse(){
		turnLeft = false;
		turnRight = false;
		goToBirdsEye = false;
		leaveBirdsEye = false;
	}
}
