using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorldController : MonoBehaviour {
	[SerializeField]
	protected PlayerController playerController;
	public Transform blockColumnPrefab;
	public int level;

	protected WorldJson worldjson;
	public int[,,] state;

	public CanvasManager canvas;
	protected bool recalculateWaterMesh;

	protected Vector3 dimensions = new Vector3 (5, 6, 5);
	public int currentWindDirection = 0;

	public Transform baseBlock;

	protected Transform[,,] blocks;
	protected BlockColumn[,] blockColumns;
	protected Transform hoveredBlock;

	protected GameObject boundingGridPlanes;
	protected GameObject areaSelectorPlane;
	protected MeshRenderer areaSelectorPlaneRenderer;

	protected Vector3[] rotations = { new Vector3 (1, 0, 0), new Vector3 (0, 0, -1), new Vector3 (-1, 0, 0), new Vector3 (0, 0, 1)};
	protected int currentRotation = 0;
	protected bool isRotating = false;
	protected Quaternion targetRotation;
	protected Quaternion initialRotation;
	protected float rotationDuration = 0.7f;
	protected float currentRotationTime;

	public Material waterMaterial;
	protected GameObject allWater;

	public virtual void Init() {
		boundingGridPlanes = GameObject.FindWithTag ("BoundingGrid");
		areaSelectorPlane = GameObject.FindWithTag ("AreaSelector");
		areaSelectorPlaneRenderer = areaSelectorPlane.GetComponent<MeshRenderer> ();

		allWater = GameObject.CreatePrimitive (PrimitiveType.Cube);
		allWater.name = "WaterMesh";
		allWater.transform.SetParent(transform, false);
	}

	void Update () {
		recalculateWaterMesh = false;

		/*
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

	void LateUpdate() {
		if(recalculateWaterMesh){
			mergeTerrain ();
		}
	}


	public void mergeTerrain() {
		
		List<Transform> allWaterBlocks = new List<Transform> ();
		List<Transform> allEarth = new List<Transform> ();
		foreach (Transform block in blocks) {
			BlockController blockCont = block.GetComponent<BlockController> ();
			if (blockCont.element == BlockController.Element.EARTH) {
				allEarth.Add (block);
			} else if (blockCont.element == BlockController.Element.WATER){
				if (!blockCont.selected) {
					allWaterBlocks.Add (block);
					block.GetComponent<Renderer> ().enabled = false;
				} else {
					block.GetComponent<Renderer> ().enabled = true;
				}
			}
		}
		CombineInstance[] combineWater = new CombineInstance[allWaterBlocks.Count];
		for (int i = 0; i < allWaterBlocks.Count; i++) {

			combineWater [i].mesh = allWaterBlocks [i].GetComponent<MeshFilter> ().sharedMesh;
			combineWater [i].transform = allWaterBlocks [i].GetComponent<MeshFilter> ().transform.localToWorldMatrix;
		}
		allWater.GetComponent<MeshFilter>().mesh.CombineMeshes (combineWater);
		allWater.GetComponent<Renderer>().material = waterMaterial;
		allWater.transform.SetParent (gameObject.transform, false);
		allWater.transform.position = Vector3.zero;


	}

	public void CreateBlocks(int[,,] state, Vector3 dimensions) {
		blocks = new Transform[(int)dimensions.x, (int)dimensions.y, (int)dimensions.z];
		blockColumns = new BlockColumn[(int)dimensions.x, (int)dimensions.z];
		int x0 = -(int)((dimensions.x - 1)/2);
		int y0 = -(int)((dimensions.y - 1)/2);
		int z0 = -(int)((dimensions.z - 1)/2);

		for (int x = x0; x - x0 < (int)dimensions.x; x++) {
			for (int z = z0; z - z0 < (int)dimensions.z; z++) {
				Transform blockColumn = (Transform)Instantiate (blockColumnPrefab, new Vector3 (x, -1, z), Quaternion.identity);
				blockColumn.name = "Column_"+ (x - x0) +"_"+ (z - z0);
				blockColumn.SetParent (this.transform, false);
				BlockColumn col = blockColumn.GetComponent<BlockColumn> ();
				col.playerController = playerController;
				col.Init(x - x0, z - z0, (int)dimensions.y, blocks, state, baseBlock);
				blockColumns [x - x0, z - z0] = col;
			}
		}

		recalculateWaterMesh = true;
		mergeTerrain ();
	}

    public BlockController.Element GetElementAt(int x, int y, int z)
    {
        return blockColumns[x, z].GetBlockElementAt(y);
    }

	void ShowElementAction(CanvasManager.PlayerAction action){
		StartCoroutine(ShowElementActionCoroutine(action));
	}

	IEnumerator ShowElementActionCoroutine(CanvasManager.PlayerAction action){
		canvas.ShowElement (action);
		yield return new WaitForSeconds (0.8f);
		canvas.ShowElement (CanvasManager.PlayerAction.NONE);
	}
}
