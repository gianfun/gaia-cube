using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ClayWorldController : WorldController {
    /*
	[SerializeField]
	private PlayerController playerController;
	public Transform blockColumnPrefab;
	public int level;

	private Transform clayWorld;
	private Transform goalWorld; 

	private WorldJson worldjson;
	public int[,,] state;

	public CanvasManager canvas;
	private bool recalculateWaterMesh;

	private Vector3 dimensions = new Vector3 (5, 6, 5);
	public int currentWindDirection = 0;

	public Transform baseBlock;

	private Transform[,,] blocks;
	private BlockColumn[,] blockColumns;
	private Transform hoveredBlock;

	private GameObject boundingGridPlanes;
	private GameObject areaSelectorPlane;
	private MeshRenderer areaSelectorPlaneRenderer;

	private Vector3[] rotations = { new Vector3 (1, 0, 0), new Vector3 (0, 0, -1), new Vector3 (-1, 0, 0), new Vector3 (0, 0, 1)};
	private int currentRotation = 0;
	private bool isRotating = false;
	private Quaternion targetRotation;
	private Quaternion initialRotation;
	private float rotationDuration = 0.7f;
	private float currentRotationTime;

	public Material waterMaterial;
	private GameObject allWater;
	*/
    private UniverseController universe;

    override public void Init() {
        base.Init();
        universe = GameObject.FindGameObjectWithTag("UniverseController").GetComponent<UniverseController>();
	}

	void Update () {
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
            universe.CheckForWinningCondition();
        }
		if (playerController.moveEarthUp) {
			ShowElementAction (CanvasManager.PlayerAction.EARTH_UP);
			foreach (BlockColumn col in GetSelectedColumns()) {
				col.MoveEarthUp ();
			}
            universe.CheckForWinningCondition();
        }

        if (playerController.doWater) {
			ShowElementAction (CanvasManager.PlayerAction.WATER);
			FillLake (GetSelectedBlocks());
			recalculateWaterMesh = true;
            universe.CheckForWinningCondition();
        }

		if (playerController.doFire) {
			ShowElementAction (CanvasManager.PlayerAction.FIRE);
			DryOutWater (GetSelectedBlocks());
			recalculateWaterMesh = true;
            universe.CheckForWinningCondition();
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
            universe.CheckForWinningCondition();
        }

		if (playerController.turnRight && !isRotating) {
			isRotating = true;
			currentRotation = (currentRotation - 1 + 4) % 4;
			targetRotation =  Quaternion.FromToRotation(rotations[0], rotations[currentRotation]);
			initialRotation = this.transform.rotation;
			currentRotationTime = 0f;
			boundingGridPlanes.SetActive (false);
		}

		if (playerController.turnLeft && !isRotating) {
			print("Rotate left");
			isRotating = true;
			currentRotation = (currentRotation + 1) % 4;
			targetRotation =  Quaternion.FromToRotation(rotations[0], rotations[currentRotation]);
			print ("From " + rotations[0] + " to " + rotations[(currentRotation )]);
			initialRotation = this.transform.rotation;

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
			this.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, rotationPercent);
	//goalWorld.rotation = Quaternion.Slerp(initialRotation, targetRotation, rotationPercent);
			//this.transform.RotateAround (Vector3.zero, Vector3.up, rotationAngle * rotationPercent);
			//Quaternion rot = Quaternion.Slerp (startRotation, endRotation, currentCameraMovementTime / cameraMovementDuration);
			//this.transform.rotation = rot;

		}

		MoveSelectorPlane ();
	}

	void LateUpdate() {
		if(recalculateWaterMesh){
			mergeTerrain ();
		}
	}

	public void SetHovered(Transform block) {
		hoveredBlock = block;
	}

	public Transform GetHovered() {
		return hoveredBlock;
	}

	public int[,] GetTerrain() {
		int[,] terrain = new int[(int)dimensions.x, (int)dimensions.z];
		foreach (Transform block in blocks) {
			BlockController blockController = block.GetComponent<BlockController> ();
			if (blockController.GetTopmost ()) {
				terrain [blockController.x, blockController.z] = blockController.y;
			}
		}
		return terrain;
	}

	public List<Vector3> GetCanyon (Vector3 pos) {
		return getAdjacentHoles (new List<Vector3> (), new List<Vector3> { pos }, true, BlockController.Element.AIR);
	}

	public List<Vector3> GetCanyon (List<Vector3> sources) {	
		return getAdjacentHoles (new List<Vector3> (), sources, true, BlockController.Element.AIR);
	}

	public List<Vector3> GetCanyonPlane (List<Vector3> sources) {	
		return getAdjacentHoles (new List<Vector3> (), sources, false, BlockController.Element.AIR);
	}

	public List<Vector3> GetCanyonPlane (List<Vector3> sources, BlockController.Element element) {	
		return getAdjacentHoles (new List<Vector3> (), sources, false, element);
	}

	private List<Vector3> getAdjacentHoles(List<Vector3> result, List<Vector3> current, bool shouldGoDown) {
		return getAdjacentHoles (result, current, shouldGoDown, BlockController.Element.AIR);
	}

	private List<Vector3> getAdjacentHoles(List<Vector3> result, List<Vector3> current, bool shouldGoDown, BlockController.Element element) {
		List<Vector3> next = new List<Vector3> ();
		foreach (Vector3 coord in current) {
			if (GetBlock (coord).GetComponent<BlockController>().element == element) {
				if(!result.Contains(coord)){
					result.Add(coord);
					if (coord.x < dimensions.x - 1) {
						next.Add (new Vector3 (coord.x + 1, coord.y, coord.z));
					}
					if (coord.x > 0) {
						next.Add (new Vector3 (coord.x - 1, coord.y, coord.z));
					}
					if (coord.z < dimensions.z - 1) {
						next.Add(new Vector3(coord.x, coord.y	 , coord.z + 1	));
					}
					if (coord.z > 0) {
						next.Add(new Vector3(coord.x, coord.y	 , coord.z - 1	));
					}
					if (shouldGoDown && coord.y > 0) {
						next.Add (new Vector3 (coord.x, coord.y - 1, coord.z));
					}
					result = getAdjacentHoles (result, next, shouldGoDown, element);
				}
			}
		}
		return result;
	}

	public int[,] GetSlice(int height) {
		int[,] slice = new int[(int)dimensions.x, (int)dimensions.z];
		for (int i=0; i < (int)dimensions.x; i++) {
			for (int j=0; j < (int)dimensions.z; j++) {
				slice [i, j] = blocks [i, height, j].gameObject.activeInHierarchy ? 1 : 0;
			}
		}
		return slice;
	}


	public List<BlockColumn> GetSelectedColumns() {
		List<BlockColumn> selectedColumns = new List<BlockColumn>();
		for (int i=0; i < (int)dimensions.x; i++) {
			for (int k=0; k < (int)dimensions.z; k++) {
				if (blockColumns[i, k].selected) {
					selectedColumns.Add (blockColumns[i, k]);
				}
			}
		}

		return selectedColumns;
	}

	public List<BlockController> GetSelectedBlocks() {
		List<BlockController> selectedBlocks = new List<BlockController>();
		foreach (Transform block in blocks) {
			BlockController blockController = block.GetComponent<BlockController> ();
			if (blockController.selected) {
				selectedBlocks.Add (blockController);
			}
		}
		return selectedBlocks;
	}

	private void MoveSelectorPlane (){
		int top = 0;
		for (int i=0; i < (int)dimensions.x; i++) {
			for (int k=0; k < (int)dimensions.z; k++) {
				if (blockColumns[i, k].topmost > top) {
					top = blockColumns [i, k].topmost;
				}
			}
		}

		Vector3 pos = areaSelectorPlane.transform.position;
		if ((int)(pos.y - 0.52f + 2) != top) {
			areaSelectorPlane.transform.position = new Vector3 (pos.x, top-2+0.52f, pos.z);
		}
	}

	private void ResetSelection() {
		for (int i=0; i < (int)dimensions.x; i++) {
			for (int k=0; k < (int)dimensions.z; k++) {
				blockColumns[i, k].Deselect();
			}
		}
	}

	public void ResetBlocks() {
		for (int i=0; i < (int)dimensions.x; i++) {
			for (int j=0; j < (int)dimensions.y; j++) {
				for (int k=0; k < (int)dimensions.z; k++) {
					Destroy (blocks [i, j, k].gameObject);
				}
			}
		}
		//CreateBlocks ();
	}

	public Transform GetBlock(int x, int y, int z) {
		return GetBlock (new Vector3 (x, y, z));
	}

	public Transform GetBlock(Vector3 pos) {
		return blocks [(int)pos.x, (int)pos.y, (int)pos.z];
	}

	public Transform MakeBlock(int x, int y, int z) {
		blocks [x, y, z].gameObject.SetActive(true);
		blocks [x, y, z].GetComponent<BlockController> ().SetElement (BlockController.Element.EARTH);
		return blocks [x, y, z];
	}

	public Transform MakeWaterBlock(int x, int y, int z) {
		blocks [x, y, z].gameObject.SetActive(true);
		blocks [x, y, z].GetComponent<BlockController> ().SetElement (BlockController.Element.WATER);
		return blocks [x, y, z];
	}

	public void FillWaterColumn(Vector3 pos) {
		blockColumns [(int)pos.x, (int)pos.z].FillWaterColumn ((int)pos.y);
	}

	public void FillLake (BlockController selectedBlock){
		FillLake (new List<BlockController> { selectedBlock });
	}

	public void FillLake (List<BlockController> selectedBlocks){
		if (selectedBlocks.Count == 0) {
			return;
		}

		List<Vector3> blocksToFlood = new List<Vector3> ();
		List<Vector3> floodSource = new List<Vector3> ();

		foreach (BlockController block in selectedBlocks) {
			floodSource.Add (block.position + new Vector3(0, 1, 0)); // + (0,1,0) to get block on top.
			//print ("Adding " + (block.position + new Vector3 (0, 1, 0)));
		}
		blocksToFlood = GetCanyonPlane (floodSource);


		foreach (Vector3 coord in blocksToFlood) {
			FillWaterColumn (coord);
		}

		foreach (BlockController block in selectedBlocks) {
			block.Deactivate (false);
		}
	}

	public bool ShouldFlood(BlockColumn col){
		int x = (int)col.position.x;
		int y = col.topmost + 1;
		int z = (int)col.position.y;

		if (x > 0) {
			if(blockColumns [x - 1, z].GetBlockElementAt (y) == BlockController.Element.WATER){
				return true;
			}
		}
		if (x < dimensions.x - 1) {
			if(blockColumns [x + 1, z].GetBlockElementAt (y) == BlockController.Element.WATER){
				return true;
			}
		}
		if (z > 0) {
			if(blockColumns [x, z - 1].GetBlockElementAt (y) == BlockController.Element.WATER){
				return true;
			}
		}
		if (z > dimensions.z - 1) {
			if(blockColumns [x, z + 1].GetBlockElementAt (y) == BlockController.Element.WATER){
				return true;
			}
		}
		return false;
	}


	public void DryOutWater(List<BlockController> selectedBlocks){
		if (selectedBlocks.Count == 0) {
			return;
		}

		List<Vector3> blocksToDry = new List<Vector3> ();
		List<Vector3> dryOutSource = new List<Vector3> ();

		foreach (BlockController block in selectedBlocks) {
			dryOutSource.Add (block.position);
		}
		blocksToDry = GetCanyonPlane (dryOutSource, BlockController.Element.WATER);

		foreach (Vector3 coord in blocksToDry) {
			blockColumns[(int)coord.x, (int)coord.z].DryBlock((int)coord.y);
		}
	}

	public void ApplyWind (int windDirection){
		//Warning: Do not read this function, you will get sad.
		// In a vain attempt to not have a thousand fors inside a million ifs, everything got ugly.
		// This will look for a water block at height N at (i, j), followed by a earth block at height N + 1 at (i+1, j), and then 
		//   (optionally) followed by earth blocks at height N at positions (i+2, j-1);(i+2, j);(i+2, j+1). These blocks
		//	 will become sand if they exist. (If they are not earth or are not at height N, nothing happens to them)
		// The positions i and j are defined based on rotation of the world. At any of the 4 given rotations, the world is seen
		// at an angle of 45, forming a V. The vector which goes to the top right is 'i', whilst the one that goes
		// to the top left is 'j'.

		//Declare auxiliary vars.
		int x, z; //Loop vars
		int initX, initZ, stepX, stepZ, finalX, finalZ; //Loop start, end and step.
		int loopCheckMultiplierX, loopCheckMultiplierZ; //If loop is decrementing, these are -1 (to flip the comparison from '<' to '>'
		bool mightHaveLeft, mightHaveRight; //Bools so we don't try to access OutOfBounds stuff
		BlockColumn waterCol, stoneCol; //Receives the water blockColumn and the stone blockColumn
		BlockColumn leftSand, middleSand, rightSand; //Receive, depending on borders, the blocks which will become sand.

		if (windDirection == 0) {
			//Two incrementing loops. Perfect.
			initX = 0;
			stepX = 1;
			finalX = (int)dimensions.x - 2;
			loopCheckMultiplierX = 1;

			initZ = 0;
			stepZ = 1;
			finalZ = (int)dimensions.z;
			loopCheckMultiplierZ = 1;
		} else if (windDirection == 1) {
			//Increments in X and decrements in Z (but X depends on Z and Z on X, because this is rotated 90 deg)
			initX = 0;
			stepX = 1;
			finalX = (int)dimensions.z - 2;
			loopCheckMultiplierX = 1;

			initZ = (int)dimensions.x - 1;
			stepZ = -1;
			finalZ = -1;
			loopCheckMultiplierZ = -1;
		} else if (windDirection == 2) {
			//Increments in X and decrements in Z
			initX = (int)dimensions.x - 1;
			stepX = -1;
			finalX = 1;
			loopCheckMultiplierX = -1;

			initZ = 0;
			stepZ = 1;
			finalZ = (int)dimensions.z;
			loopCheckMultiplierZ = 1;
		} else { //if (windDirection == 3) {
			initX =(int)dimensions.z - 1;
			stepX = -1;
			finalX = 1;
			loopCheckMultiplierX = -1;

			initZ = 0;
			stepZ = 1;
			finalZ = (int)dimensions.x;
			loopCheckMultiplierZ = 1;
		}

		for (x = initX; x * loopCheckMultiplierX < finalX * loopCheckMultiplierX; x += stepX) {
			for (z = initZ; z * loopCheckMultiplierZ < finalZ * loopCheckMultiplierZ; z += stepZ) {
				if (windDirection == 0) {
					waterCol = blockColumns [x, z];
					stoneCol = blockColumns [x + 1, z];

					mightHaveLeft = (z < dimensions.z - 1);
					mightHaveRight = (z > 0);

					if (mightHaveLeft) {
						leftSand = blockColumns [x + 2, z + 1];
					} else {
						leftSand = null;
					}
					middleSand = blockColumns [x + 2, z];
					if (mightHaveRight) {
						rightSand = blockColumns [x + 2, z - 1];
					}else {
						rightSand = null;
					}	
				} else if (windDirection == 1) {
					waterCol = blockColumns [z, x];
					stoneCol = blockColumns [z, x + 1];

					mightHaveLeft = (z > 0);
					mightHaveRight = (z < dimensions.z - 1);

					if (mightHaveLeft) {
						leftSand = blockColumns [z - 1, x + 2];
					} else {
						leftSand = null;
					}
					middleSand = blockColumns [z, x + 2];
					if (mightHaveRight) {
						rightSand = blockColumns [z + 1, x + 2];
					}else {
						rightSand = null;
					}	

				} else if (windDirection == 2) {
					waterCol = blockColumns [x, z];
					stoneCol = blockColumns [x - 1, z];

					mightHaveLeft = (z > 0);
					mightHaveRight = (z < dimensions.z - 1);

					if (mightHaveLeft) {
						leftSand = blockColumns [x - 2, z - 1];
					} else {
						leftSand = null;
					}
					middleSand = blockColumns [x - 2, z];
					if (mightHaveRight) {
						rightSand = blockColumns [x - 2, z + 1];
					}else {
						rightSand = null;
					}	
				} else { //if (windDirection == 3) {

					waterCol = blockColumns [z, x];
					stoneCol = blockColumns [z, x - 1];

					mightHaveLeft = (z < dimensions.z - 1);
					mightHaveRight = (z > 0);

					if (mightHaveLeft) {
						leftSand = blockColumns [z + 1, x - 2];
					} else {
						leftSand = null;
					}
					middleSand = blockColumns [z, x - 2];
					if (mightHaveRight) {
						rightSand = blockColumns [z - 1, x - 2];
					}else {
						rightSand = null;
					}	
				}


				/*
				print (x+","+z+" Activation? "+ (waterCol.topmost + 1 == stoneCol.topmost) + " " 
					+(waterCol.GetTopElement () == BlockController.Element.WATER) + " "
					+(stoneCol.GetTopElement () == BlockController.Element.EARTH) + " "
				);
				*/

				if (waterCol.topmost + 1 == stoneCol.topmost
					&&	waterCol.GetTopElement () == BlockController.Element.WATER
					&& stoneCol.GetTopElement () == BlockController.Element.EARTH) {
					if (mightHaveLeft
						&& 	leftSand.topmost == waterCol.topmost 
						&& 	leftSand.GetTopElement() == BlockController.Element.EARTH ) {
						leftSand.MakeTopSand ();
					}

					if (	middleSand.topmost == waterCol.topmost 
						&& 	middleSand.GetTopElement() == BlockController.Element.EARTH ) {
						middleSand.MakeTopSand ();
					}

					if (mightHaveRight
						&& 	rightSand.topmost == waterCol.topmost 
						&& 	rightSand.GetTopElement() == BlockController.Element.EARTH ) {
						rightSand.MakeTopSand ();
					}

					stoneCol.BreakTopEarth();
				}

			}
		}
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
