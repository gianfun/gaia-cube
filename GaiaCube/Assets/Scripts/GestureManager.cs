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
			

			Vector3 a = (transform.rotation * left.hand.PalmNormal.ToVector3 ()).normalized;

			if (left.hand != null && right.hand != null && false) {
				infoText.text = "Confidence\t\t\t: Left: " + left.hand.Confidence + "  Right: " + right.hand.Confidence + "\n";
				infoText.text += "Extended Fingers \t: Left: " + left.extendedFingers + "  Right: " + right.extendedFingers + "\n";
				infoText.text += "Extended Fingers (raw): Left: " + left.rawExtendedFingers + "  Right: " + right.rawExtendedFingers + "\n";
				infoText.text += "Finger distance\t\t: Left: " + left.fingerDistance + "  Right: " + right.fingerDistance + "\n";
				infoText.text += "Paw      \t\t\t\t\t: Left: " + left.isPaw + "  Right: " + right.isPaw + "\n";
				infoText.text += "Paw position \t\t\t: Left: " + left.pawStartPosition + "  Right: " + right.pawStartPosition + "\n";
				infoText.text += "Palm position\t\t\t: Left: " + left.hand.PalmPosition.ToVector3 () + "  Right: " + right.hand.PalmPosition.ToVector3 () + "\n";
				infoText.text += "Palm velocity\t\t\t: Left: " + left.palmVelocity + "  Right: " + right.palmVelocity + "\n";
				infoText.text += "Palm velocity (raw)\t\t\t: Left: " + left.hand.PalmVelocity.ToVector3() + "  Right: " + right.hand.PalmVelocity.ToVector3() + "\n";
				infoText.text += "Palm Normal\t\t\t: Left: " + left.handNormal.normalized + "  Right: " + right.handNormal.normalized + "\n";
				infoText.text += "Palm Normal (cntrl)\t\t: Left: " + left.cntrlHandNormal.normalized + "  Right: " + right.cntrlHandNormal.normalized + "\n";
				infoText.text += "Palm Roll\t\t\t: Left: " + a + "  Right: " + right.hand.PalmNormal.Roll + "\n";
				infoText.text += "Palm Yaw\t\t\t: Left: " + left.hand.PalmNormal.Yaw + "  Right: " + right.hand.PalmNormal.Yaw + "\n";
				infoText.text += "Palm Pitch\t\t\t: Left: " + left.hand.PalmNormal.Pitch + "  Right: " + right.hand.PalmNormal.Pitch + "\n";
				infoText.text += "Small Finger velocity\t\t\t: Left: " + left.littleFingerVelocity + "  Right: " + right.littleFingerVelocity + "\n";
				infoText.text += "LastTipVelocity Sum\t\t\t: Left: " + left.lastTipVelocitySum + "  Right: " + right.lastTipVelocitySum + "\n";
				infoText.text += "lastPalmVelocity Sum\t\t\t: Left: " + left.lastPalmVelocitySum + "  Right: " + right.lastPalmVelocitySum + "\n";
				infoText.text += "palm - finger velocity\t\t\t: Left: " + (left.lastTipVelocitySum - left.lastPalmVelocitySum) + "  Right: " + (right.lastTipVelocitySum - right.lastPalmVelocitySum) + "\n";
				infoText.text += "palm / finger velocity\t\t\t: Left: " + (left.lastTipVelocitySum / left.lastPalmVelocitySum) + "  Right: " + (right.lastTipVelocitySum / right.lastPalmVelocitySum) + "\n";
			}


			if (left.currentAction != GestureDetector.Action.None) {
				if (left.currentAction == GestureDetector.Action.Fire) {
					doFire = true;
				} else if (left.currentAction == GestureDetector.Action.Water) {
					doWater = true;
				} else if (left.currentAction == GestureDetector.Action.Wind) {
					doWind = true;
				} 
			
			}

			if (left.isPaw) {
				if (left.wasPaw) {
                    extrudedLength += left.palmVelocity.y;
                    //Is palm facing down or up
                    if (left.hand.PalmNormal.ToVector3().normalized.y > 0.8f && extrudedLength > moveEarthYThreshold)
                    {
                        moveEarthUp = true;
                        extrudedLength = 0;
                    }
                    else if (left.hand.PalmNormal.ToVector3().normalized.y < -0.8f && extrudedLength < -moveEarthYThreshold)
                    {
                        moveEarthDown = true;
                        extrudedLength = 0;
                    }
				}
			} else {
				extrudedLength = 0f;
			}




            if (left.isFist && right.isFist)
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