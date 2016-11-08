using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public bool shouldUseLeap = false;

	public Transform cameraTrans;

	[SerializeField]
	private GestureManager GM;

	public bool somethingTriggered = false;

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

    private const float TRIGGER_COOLDOWN = 1f;
    private float lastTriggerTime;

	public Ray ray_topleft;
	public Ray ray_bottomright;
	// Use this for initialization
	void Start () {
		cameraTrans = GameObject.FindWithTag ("MainCamera").GetComponent<Transform>() ;
	}

    // Update is called once per frame
    void Update() {
        if (!somethingTriggered) {
            setAllAsFalse();
        } else
        {
            lastTriggerTime = Time.time;
        }
		somethingTriggered = false;
        if(lastTriggerTime + TRIGGER_COOLDOWN > Time.time)
        {
            print("boop");
            //Don't do anything... Wait cooldown
            return;
        }

		if (shouldUseLeap) {
            lastTriggerTime = Time.time;
            if (GM.left.isPinching && GM.right.isPinching) {
				doSelect = true;
				ray_topleft = new Ray (cameraTrans.position, GM.left.pinchPosition - cameraTrans.position);
				ray_bottomright = new Ray (cameraTrans.position, GM.right.pinchPosition - cameraTrans.position);
                lastTriggerTime = 0;
            } else if (GM.left.doRotate) {
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
			} else if (GM.doWater) {
				doWater = true;
			} else if (GM.doFire) {
				doFire = true;
			} else if (GM.doWind) {
				doWind = true;
			} else
            {
                lastTriggerTime = 0;
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


	public void TriggerEarthDown(){
		print ("TriggerEarthDown");
		moveEarthDown = true;
		somethingTriggered = true;
	}

	public void TriggerEarthUp(){
		print ("TriggerEarthUp");
		moveEarthUp = true;
		somethingTriggered = true;
	}

	public void TriggerRotateLeft(){
		print ("TriggerRotateLeft");
		turnRight = true;
		somethingTriggered = true;
	}

	public void TriggerRotateRight(){
		print ("TriggerRotateRight");
		turnLeft = true;
		somethingTriggered = true;
	}

	public void TriggerWater(){
		print ("TriggerWater");
		somethingTriggered = true;
		doWater = true;
	}

	public void TriggerFire(){
		print ("TriggerFire");
		somethingTriggered = true;
		doFire = true;
	}

	public void TriggerWind(){
		print ("TriggerWind");
		somethingTriggered = true;
		doWind = true;
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
