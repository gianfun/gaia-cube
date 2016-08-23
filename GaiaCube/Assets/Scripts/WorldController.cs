using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorldController : MonoBehaviour {
	[SerializeField]
	private PlayerController playerController;
	public Transform blockColumnPrefab;

	private bool recalculateWaterMesh;

	private Vector3 dimensions = new Vector3 (5, 6, 5);

	public Transform baseBlock;
	public Transform waterBlock;
	public Transform basePlane;

	private Transform[,,] blocks;
	private BlockColumn[,] blockColumns;
	private Transform hoveredBlock;

	private GameObject areaSelectorPlane;
	private MeshRenderer areaSelectorPlaneRenderer;

	public Material waterMaterial;
	private GameObject allWater;

	void Start() {
		areaSelectorPlane = GameObject.FindWithTag ("AreaSelector");
		areaSelectorPlaneRenderer = areaSelectorPlane.GetComponent<MeshRenderer> ();

		CreateBlocks ();

		allWater = GameObject.CreatePrimitive (PrimitiveType.Cube);
		allWater.name = "WaterMesh";
		allWater.transform.SetParent (transform);

		Color skyBlue = new Color(0.2f, 0.3f, 0.4f, 0.7f);
		Color sunYellow = new Color(0.8f, 0.6f, 0.2f, 0.3f);
		RenderSettings.ambientSkyColor = sunYellow;
		RenderSettings.fog = true;
		RenderSettings.fogDensity = 0.05f;
		RenderSettings.fogColor = skyBlue;
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
			foreach (BlockColumn col in GetSelectedColumns()) {
				col.MoveEarthDown ();
				print ("Moved down. Should flood? " + ShouldFlood (col));
				if (ShouldFlood (col)) {
					FillLake (col.GetTopBlock ());
					recalculateWaterMesh = true;
				}

			}
		}
		if (playerController.moveEarthUp) {
			foreach (BlockColumn col in GetSelectedColumns()) {
				col.MoveEarthUp ();
			}
		}

		if (playerController.doWater) {
			FillLake (GetSelectedBlocks());
			recalculateWaterMesh = true;
		}


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
	}

	public List<Vector3> GetCanyon (Vector3 pos) {
		return getAdjacentHoles (new List<Vector3> (), new List<Vector3> { pos }, true);
	}

	public List<Vector3> GetCanyon (List<Vector3> sources) {	
		return getAdjacentHoles (new List<Vector3> (), sources, true);
	}

	public List<Vector3> GetCanyonPlane (List<Vector3> sources) {	
		return getAdjacentHoles (new List<Vector3> (), sources, false);
	}

	private List<Vector3> getAdjacentHoles(List<Vector3> result, List<Vector3> current, bool shouldGoDown) {
		List<Vector3> next = new List<Vector3> ();
		foreach (Vector3 coord in current) {
			if (!GetBlock (coord).gameObject.activeInHierarchy) {
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
					result = getAdjacentHoles (result, next, shouldGoDown);
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
		CreateBlocks ();
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
			print ("Adding " + (block.position + new Vector3 (0, 1, 0)));
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

	public void CreateWaterBlock(int x, int y, int z) {
		Transform block = (Transform)Instantiate (waterBlock, new Vector3 (x - 2, y - 2, z - 2), Quaternion.identity);
		block.SetParent (gameObject.transform, false);
	}

	void CreateBlocks() {
		CreateBlocks2 (5, -2, -2, -2);
	}

	void CreateBlocks(int n, int x0, int y0, int z0) {
		blocks = new Transform[(int)dimensions.x, (int)dimensions.y, (int)dimensions.z];
		for (int x = x0; x - x0 < n; x++) {
			for (int y = y0; y - y0 < n + 1; y++) {
				for (int z = z0; z - z0 < n; z++) {
					Transform blockType;

					if (y == y0) {
						blockType = basePlane;
					} else {
						blockType = baseBlock;
					}

					Transform block = (Transform) Instantiate(blockType, new Vector3 (x, y, z), Quaternion.identity);
					block.SetParent (gameObject.transform, false);
					block.GetComponent<BlockController> ().playerController = this.playerController;
					block.GetComponent<BlockController> ().SetCoordinates (x - x0, y - y0, z - z0);
					blocks [x-x0, y-y0, z-z0] = block;
					if (y - y0 == n) {
						block.GetComponent<BlockController> ().SetTopmost (true);
					}

					if (blockType == baseBlock) {
						//block.Rotate (-90, 0, 0);
					}
				}
			}
		}
	}

	void CreateBlocks2(int n, int x0, int y0, int z0) {
		blocks = new Transform[(int)dimensions.x, (int)dimensions.y, (int)dimensions.z];
		blockColumns = new BlockColumn[(int)dimensions.x, (int)dimensions.z];
		for (int x = x0; x - x0 < n; x++) {
			for (int z = z0; z - z0 < n; z++) {
				Transform blockColumn = (Transform)Instantiate (blockColumnPrefab, new Vector3 (x, -1, z), Quaternion.identity);
				blockColumn.name = "Column_"+ (x - x0) +"_"+ (z - z0);
				blockColumn.SetParent (transform, false);
				BlockColumn col = blockColumn.GetComponent<BlockColumn> ();
				col.playerController = playerController;
				col.SetPrefabs (baseBlock, waterBlock, basePlane);
				col.Init(x - x0, z - z0, n, blocks);
				blockColumns [x - x0, z - z0] = col;
			}
		}
	}
}
