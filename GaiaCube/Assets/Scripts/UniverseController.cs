using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class UniverseController : MonoBehaviour {
    int[,,] clayState, goalState;
    Vector3 dimensions;
    public bool moveCameraLeft { get; set; }
    public bool moveCameraRight { get; set; }
    float cameraDuration = 1.5f;
    float cameraTime = 0;

    Quaternion lookAtClayWorld, lookAtGoalWorld;

    Transform clayWorldTrans, goalWorldTrans;
	WorldController goalWorld;
	ClayWorldController clayWorld;

    private StateManager sm;

    public GameObject WinMessage;

	void StartLevel(int level){
		WorldLoader worldLoader = new WorldLoader (level);
		clayState = worldLoader.getClayState ();
		goalState = worldLoader.getGoalState ();
		dimensions = worldLoader.getDimensions ();

		clayWorld.Init ();
		goalWorld.Init ();
		clayWorld.CreateBlocks (clayState, dimensions);
		goalWorld.CreateBlocks (goalState, dimensions);
	}

	void Start() {
        sm = GameObject.FindGameObjectWithTag("StateManager").GetComponent<StateManager>();

		clayWorldTrans = GameObject.FindWithTag ("ClayWorld").GetComponent<Transform>();
		goalWorldTrans = GameObject.FindWithTag ("GoalWorld").GetComponent<Transform>();
		clayWorld = clayWorldTrans.GetComponent<ClayWorldController>();
		goalWorld = goalWorldTrans.GetComponent<WorldController>();

        lookAtClayWorld = Quaternion.Euler(45, 0, 0);
        lookAtGoalWorld = Quaternion.Euler(45, 90, 0);

        

        Color skyBlue = new Color(0.2f, 0.3f, 0.4f, 0.7f);
		Color sunYellow = new Color(0.8f, 0.6f, 0.2f, 0.3f);
		RenderSettings.ambientSkyColor = sunYellow;
		RenderSettings.fog = true;
		RenderSettings.fogDensity = 0.05f;
		RenderSettings.fogColor = skyBlue;

		StartLevel (sm.currentLevel);
	}

    public void CheckForWinningCondition()
    {
        bool areEqual = true;
        for(int x = 0; x < dimensions.x && areEqual; x++)
        {
            for (int y = 0; y < dimensions.y && areEqual; y++)
            {
                for (int z = 0; z < dimensions.z && areEqual; z++)
                {
                    if (clayWorld.GetElementAt(x, y, z) != goalWorld.GetElementAt(x, y, z))
                    {
                        areEqual = false;
                        break;
                    }
                }
            }
        }

        if (areEqual)
        {
            print("Finit");
            StartCoroutine(ShowWin());
        } 
    }

    private IEnumerator ShowWin()
    {
        WinMessage.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Menu");
    }

    void Update()
    {
        if (moveCameraLeft || moveCameraRight)
        {
            if (moveCameraLeft)
            {
                cameraTime -= Time.deltaTime;
                if (cameraTime < 0f)
                {
                    cameraTime = 0f;
                }
            }
            else
            {
                cameraTime += Time.deltaTime;
                if (cameraTime > cameraDuration)
                {
                    cameraTime = cameraDuration;
                }
            }
            Camera.main.transform.localRotation = Quaternion.Slerp(lookAtClayWorld, lookAtGoalWorld, cameraTime / cameraDuration);
        }
    
        /*
		recalculateWaterMesh = false;

		if (Input.GetButtonDown("Fire1")) {
			RaycastHit hit = new RaycastHit();
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (!Physics.Raycast(ray, out hit)) {
				ResetSelection();
				recalculateWaterMesh = true;
			}
		} else if (playerController.doSelect) {
			if (!areaSelectorPlaneRenderer.enabled) {
				areaSelectorPlaneRenderer.enabled = true;
			}
			//print ("World Controller: DoSelect. Layer: " + LayerMask.NameToLayer ("SelectorPlane"));
			RaycastHit hit;
			int selectLeft, selectRight, selectTop, selectBottom;
			Vector3 normalizedPointTL = new Vector3 (-1, -1, -1);
			Vector3 normalizedPointBR = normalizedPointTL;
			if (Physics.Raycast (playerController.ray_topleft, out hit, 100f, 1 << LayerMask.NameToLayer ("SelectorPlane"))) {
				normalizedPointTL = transform.InverseTransformPoint (hit.point) + new Vector3(2.5f, -3.6f, 2.5f);
				normalizedPointTL = new Vector3 ((int)normalizedPointTL.x, (int)normalizedPointTL.y, (int)normalizedPointTL.z);
			} 
			if (Physics.Raycast (playerController.ray_bottomright, out hit, 100f, 1 << LayerMask.NameToLayer ("SelectorPlane"))) {
				normalizedPointBR = transform.InverseTransformPoint (hit.point) + new Vector3(2.5f, -3.6f, 2.5f);
				normalizedPointBR = new Vector3 ((int)normalizedPointBR.x, (int)normalizedPointBR.y, (int)normalizedPointBR.z);
				//print ("Hit plane on " + normalizedPointBR); 

			} else {
			}

			if (normalizedPointBR != new Vector3 (-1, -1, -1) && normalizedPointTL != new Vector3 (-1, -1, -1)) {
				if (normalizedPointTL.x < normalizedPointBR.x) {
					selectLeft = (int) normalizedPointTL.x;
					selectRight = (int) normalizedPointBR.x;
				} else {
					selectLeft = (int) normalizedPointBR.x;
					selectRight = (int) normalizedPointTL.x;
				}

				if (normalizedPointTL.z < normalizedPointBR.z) {
					selectBottom = (int) normalizedPointTL.z;
					selectTop = (int) normalizedPointBR.z;
				} else {
					selectBottom = (int) normalizedPointBR.z;
					selectTop = (int) normalizedPointTL.z;
				}

				ResetSelection ();

				for(int x = selectLeft; x <= selectRight; x++){
					for(int z = selectBottom; z <= selectTop; z++){
						blockColumns [x, z].GetComponent<BlockColumn> ().Select ();

					}
				}
				recalculateWaterMesh = true;
			}
		} else if (!playerController.doSelect && areaSelectorPlaneRenderer.enabled == true) {
			areaSelectorPlaneRenderer.enabled = false;
		};

		if (playerController.moveEarthDown) {
			ShowElementAction (CanvasManager.PlayerAction.EARTH_DOWN);

			foreach (BlockColumn col in GetSelectedColumns()) {
				col.MoveEarthDown ();
				//print ("Moved down. Should flood? " + ShouldFlood (col));
				if (ShouldFlood (col)) {
					FillLake (col.GetTopBlock ());
					recalculateWaterMesh = true;
				}

			}
		}
		if (playerController.moveEarthUp) {
			ShowElementAction (CanvasManager.PlayerAction.EARTH_UP);
			foreach (BlockColumn col in GetSelectedColumns()) {
				col.MoveEarthUp ();
			}
		}

		if (playerController.doWater) {
			ShowElementAction (CanvasManager.PlayerAction.WATER);
			FillLake (GetSelectedBlocks());
			recalculateWaterMesh = true;
		}

		if (playerController.doFire) {
			ShowElementAction (CanvasManager.PlayerAction.FIRE);
			DryOutWater (GetSelectedBlocks());
			recalculateWaterMesh = true;
		}

		if (playerController.turnRight) {
			currentWindDirection = (currentWindDirection + 3) % 4; // == cWD - 1 but without underflow
		}

		if (playerController.turnLeft) {
			currentWindDirection = (currentWindDirection + 1) % 4;
		} 
		if (playerController.doWind ) {
			ShowElementAction (CanvasManager.PlayerAction.WIND);
			ApplyWind (currentWindDirection);
			ResetSelection ();
			recalculateWaterMesh = true;
		}

		if (playerController.turnRight && !isRotating) {
			print("Rotate right");
			isRotating = true;
			//startRotation = this.transform.rotation;
			//startPosition = this.transform.position;

			//Quaternion. AngleAxis (90, this.transform.up);

			currentRotation = (currentRotation - 1 + 4) % 4;
			targetRotation =  Quaternion.FromToRotation(rotations[0], rotations[currentRotation]);
			print ("From " + rotations[0] + " to " + rotations[(currentRotation )]);
			initialRotation = clayWorld.rotation;
			currentRotationTime = 0f;
			boundingGridPlanes.SetActive (false);

		}

		if (playerController.turnLeft && !isRotating) {
			print("Rotate left");
			isRotating = true;
			//startRotation = this.transform.rotation;
			//startPosition = this.transform.position;

			//Quaternion. AngleAxis (90, this.transform.up);

			currentRotation = (currentRotation + 1) % 4;
			targetRotation =  Quaternion.FromToRotation(rotations[0], rotations[currentRotation]);
			print ("From " + rotations[0] + " to " + rotations[(currentRotation )]);
			initialRotation = clayWorld.rotation;

			currentRotationTime = 0f;
			boundingGridPlanes.SetActive (false);

		}

		if (isRotating) {
			currentRotationTime += Time.deltaTime;
			float rotationPercent = currentRotationTime / rotationDuration;
			if(rotationPercent >= 1.0f){
				isRotating = false;
				rotationPercent = 1.0f; //So our rotation turns exactly 'rotationAngle' degrees
				boundingGridPlanes.SetActive (true);
			}
			//this.transform.position = startPosition;
			//this.transform.rotation = startRotation;
			clayWorld.rotation = Quaternion.Slerp(initialRotation, targetRotation, rotationPercent);
			goalWorld.rotation = Quaternion.Slerp(initialRotation, targetRotation, rotationPercent);
			//this.transform.RotateAround (Vector3.zero, Vector3.up, rotationAngle * rotationPercent);
			//Quaternion rot = Quaternion.Slerp (startRotation, endRotation, currentCameraMovementTime / cameraMovementDuration);
			//this.transform.rotation = rot;

		}

		MoveSelectorPlane ();
		*/
    }

    /*
	void ShowElementAction(CanvasManager.PlayerAction action){
		StartCoroutine(ShowElementActionCoroutine(action));
	}

	IEnumerator ShowElementActionCoroutine(CanvasManager.PlayerAction action){
		
		canvas.ShowElement (action);
		yield return new WaitForSeconds (0.8f);
		canvas.ShowElement (CanvasManager.PlayerAction.NONE);

	}
	*/

}
