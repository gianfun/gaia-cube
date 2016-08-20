using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorldController : MonoBehaviour {
	[SerializeField]
	private PlayerController playerController;

	private Vector3 dimensions = new Vector3 (5, 6, 5);

	public Transform baseBlock;
	public Transform waterBlock;
	public Transform basePlane;

	private Transform[,,] blocks;
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
		allWater.transform.SetParent (transform);

		Color skyBlue = new Color(0.2f, 0.3f, 0.4f, 0.7f);
		Color sunYellow = new Color(0.8f, 0.6f, 0.2f, 0.3f);
		RenderSettings.ambientSkyColor = sunYellow;
		RenderSettings.fog = true;
		RenderSettings.fogDensity = 0.05f;
		RenderSettings.fogColor = skyBlue;
	}

	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			RaycastHit hit = new RaycastHit();
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (!Physics.Raycast(ray, out hit)) {
				ResetSelection();
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
						for (int y = (int)dimensions.y - 1; y >= 0; y--) {
							BlockHolderController block = GetBlock (x, y, z).GetComponent<BlockHolderController> ();
							if(block != null && block.GetTopmost()){
								block.Select ();
							}
						}
					}
				}
			}
		} else if (!playerController.doSelect && areaSelectorPlaneRenderer.enabled == true) {
			areaSelectorPlaneRenderer.enabled = false;
		};
	}

	void LateUpdate() {
		mergeTerrain ();
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
			BlockHolderController blockController = block.GetComponent<BlockHolderController> ();
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
			if (block.GetComponent<BlockHolderController> ().element == BlockHolderController.Element.EARTH) {
				allEarth.Add (block);
			} else {
				allWaterBlocks.Add (block);
			}
		}
		CombineInstance[] combineWater = new CombineInstance[allWaterBlocks.Count];
		for (int i = 0; i < allWaterBlocks.Count; i++) {
			combineWater [i].mesh = allWaterBlocks [i].GetComponent<MeshFilter> ().sharedMesh;
			combineWater [i].transform = allWaterBlocks [i].GetComponent<MeshFilter> ().transform.localToWorldMatrix;
			allWaterBlocks [i].GetComponent<Renderer> ().enabled = false;
		}
		allWater.GetComponent<MeshFilter>().mesh.CombineMeshes (combineWater);
		allWater.GetComponent<Renderer>().material = waterMaterial;
		allWater.transform.SetParent (gameObject.transform, false);
	}

	public List<int[]> GetCanyon (int x, int y, int z) {
		return getAdjacentHoles (new List<int[]> (), new List<int[]> { new [] { x, y, z } });
	}

	private List<int[]> getAdjacentHoles(List<int[]> result, List<int[]> current) {
		List<int[]> next = new List<int[]> ();
		foreach (int [] coord in current) {
			try {
				if (!GetBlock (coord [0], coord [1], coord [2]).gameObject.activeInHierarchy) {
					bool mustBreak = false;
					foreach (int [] resCoord in result) {
						if (coord.SequenceEqual(resCoord)) {
							mustBreak = true;
							break;
						}
					}
					if (mustBreak) {
						break;
					}
					result.Add(coord);
					next.Add (new [] { coord [0] + 1, coord [1], coord [2] + 1 });
					next.Add (new [] { coord [0] + 1, coord [1], coord [2] - 1 });
					next.Add (new [] { coord [0] - 1, coord [1], coord [2] + 1 });
					next.Add (new [] { coord [0] - 1, coord [1], coord [2] - 1 });
					next.Add (new [] { coord [0], coord [1] - 1, coord [2] });
					result = getAdjacentHoles (result, next);
				}
			} catch (System.IndexOutOfRangeException) {
				continue;
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

	private void ResetSelection() {
		for (int i=0; i < (int)dimensions.x; i++) {
			for (int j=0; j < (int)dimensions.y; j++) {
				for (int k=0; k < (int)dimensions.z; k++) {
					blocks[i, j, k].GetComponent<BlockHolderController>().Deselect();
				}
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
		return blocks [x, y, z];
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

	public void CreateWaterBlock(int x, int y, int z) {
		Transform block = (Transform)Instantiate (waterBlock, new Vector3 (x - 2, y - 2, z - 2), Quaternion.identity);
		block.SetParent (gameObject.transform, false);
	}

	void CreateBlocks() {
		CreateBlocks (5, -2, -2, -2);
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
					block.GetComponent<BlockHolderController> ().playerController = this.playerController;
					block.GetComponent<BlockHolderController> ().SetCoordinates (x - x0, y - y0, z - z0);
					blocks [x-x0, y-y0, z-z0] = block;
					if (y - y0 == n) {
						block.GetComponent<BlockHolderController> ().SetTopmost (true);
					}

					if (blockType == baseBlock) {
						//block.Rotate (-90, 0, 0);
					}
				}
			}
		}
	}
}
