using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {
	[SerializeField]
	private PlayerController playerController;

	public Transform baseBlock;
	public Transform waterBlock;
	public Transform basePlane;

	private Transform[,,] blocks;
	private Transform hoveredBlock;

	void Start() {
		CreateBlocks ();
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
		}
	}

	public void SetHovered(Transform block) {
		hoveredBlock = block;
	}

	public Transform GetHovered() {
		return hoveredBlock;
	}

	public int[,] GetTerrain() {
		int[,] terrain = new int[5, 5];
		foreach (Transform block in blocks) {
			BlockHolderController blockController = block.GetComponent<BlockHolderController> ();
			if (blockController.GetTopmost ()) {
				terrain [blockController.x, blockController.z] = blockController.y;
			}
		}
		return terrain;
	}

	public int[,] GetSlice(int height) {
		int[,] slice = new int[5, 5];
		for (int i=0; i < 5; i++) {
			for (int j=0; j < 5; j++) {
				slice [i, j] = blocks [i, height, j].gameObject.activeInHierarchy ? 1 : 0;
			}
		}
		return slice;
	}

	private void ResetSelection() {
		for (int i=0; i < 5; i++) {
			for (int j=0; j < 5; j++) {
				for (int k=0; k < 5; k++) {
					blocks[i, j, k].GetComponent<BlockHolderController>().Deselect();
				}
			}
		}
	}

	public void ResetBlocks() {
		for (int i=0; i < 5; i++) {
			for (int j=0; j < 6; j++) {
				for (int k=0; k < 5; k++) {
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
		blocks = new Transform[5, 6, 5];
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
				}
			}
		}
	}
}
