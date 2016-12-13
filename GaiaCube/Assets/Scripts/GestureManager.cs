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

	public bool moveEarthUp;
	public bool moveEarthDown;
	public bool doFire;
	public bool doWater;
	public bool doWind;

    bool bothHandsWereFists;
    public bool changedLateralMovement;
    public float lateralMovementDistance = 0;
    private Vector3 initialLeftFistPosition, initialRightFistPosition;


    public float moveEarthYThreshold = 2f;
	public Leap.Unity.LeapHandController leapController;

	[SerializeField]
	private UnityEngine.UI.Text infoText;

	public GestureDetector left;
	public GestureDetector right;

	private float extrudedLength = 0f;

    void Start()
    {
        if (gd1.isLeft)
        {
            left = gd1;
            right = gd2;
        }
        else
        {
            left = gd2;
            right = gd1;
        }
    }
	// Update is called once per frame
	void Update () {
		SetBoolsToFalse ();

        if (gd1.hand != null && gd2.hand != null) {

			if (right.currentAction != GestureDetector.Action.None) {
				if (right.currentAction == GestureDetector.Action.Fire) {
					doFire = true;
				} else if (right.currentAction == GestureDetector.Action.Water) {
					doWater = true;
				} else if (right.currentAction == GestureDetector.Action.Wind) {
					doWind = true;
				} 
			
			}

			if (right.isPaw) {
				if (right.wasPaw) {
                    extrudedLength += right.palmVelocity.y;
                    //Is palm facing down or up
                    if (right.cntrlHandNormal.y > 0.7f && extrudedLength > moveEarthYThreshold)
                    {
                        moveEarthUp = true;
                        extrudedLength = 0;
                    }
                    else if (right.cntrlHandNormal.y < -0.7f && extrudedLength < -moveEarthYThreshold)
                    {
                        moveEarthDown = true;
                        extrudedLength = 0;
                    }
				}
			} else {
				extrudedLength = 0f;
			}

            if (left.isFist && right.isFist && left.cntrlHandNormal.y < -0.3f && right.cntrlHandNormal.y < -0.3f)
            {
                changedLateralMovement = true;
                if (!bothHandsWereFists)
                {
                    initialLeftFistPosition = left.palmPosition;
                    initialRightFistPosition = right.palmPosition;
                }
                else
                {
                    lateralMovementDistance = (left.palmPosition - initialLeftFistPosition).x + (right.palmPosition - initialRightFistPosition).x;
                }
                bothHandsWereFists = true;
            }
            else
            {
                if (bothHandsWereFists)
                {
                    changedLateralMovement = true;
                }
                lateralMovementDistance = 0f;
                bothHandsWereFists = false;
            }
        }
	}

	private void SetBoolsToFalse(){
		moveEarthUp 	= false;
		moveEarthDown 	= false;
		doFire			= false;
		doWater			= false;
		doWind			= false;

        changedLateralMovement = false;
    }


}