using UnityEngine;
using System.Collections;

public class BlockColumn : MonoBehaviour {
	Transform earthPrefab, waterPrefab, bottomPrefab;
	private int height;

	private int topmost;
	private bool selected = false;
	private BlockHolderController[] myBlocks;
	public PlayerController playerController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (playerController != null && selected) { 
			if (playerController.moveEarthDown && topmost > 0) {
				myBlocks [topmost].Deactivate (true);
				topmost--;
				myBlocks [topmost].SetTopmost(true);
				myBlocks [topmost].Select();

			} else if (playerController.moveEarthUp && topmost < height) {
				myBlocks [topmost].Deactivate (false);
				topmost++;
				myBlocks [topmost].Activate();
				myBlocks [topmost].SetTopmost(true);
				myBlocks [topmost].Select();
			}
		}
	}

	/*
	 		if (playerController.moveEarthDown && selected) {
			Deactivate (true);
			try {
				Transform nextBlock = transform.parent.GetComponent<WorldController> ().GetBlock (x, y - 1, z);
				nextBlock.GetComponent<BlockHolderController> ().SetTopmost (true);
				nextBlock.GetComponent<BlockHolderController> ().Select ();
			} catch (System.IndexOutOfRangeException) {
				return;
			}
		} else if (playerController.moveEarthUp && selected) {
			try {
				Transform nextBlock = transform.parent.GetComponent<WorldController> ().MakeBlock (x, y + 1, z);
				Deactivate (false);
				nextBlock.GetComponent<BlockController> ().SetTopmost (true);
				nextBlock.GetComponent<BlockController> ().Select ();
			} catch (System.Exception e) {
				if (e is System.IndexOutOfRangeException || e is System.NullReferenceException) {
					return;
				}
				throw;
			}
		}
	 */

	public void SetPrefabs(Transform earth, Transform water, Transform bottom){
		earthPrefab = earth;
		waterPrefab = water;
		bottomPrefab = bottom;
	}

	public void Init(int x, int z, int height, Transform[,,] blocks){
		myBlocks =  new BlockHolderController[height + 1];
		Transform block = (Transform) Instantiate(bottomPrefab , new Vector3(0, -1, 0), Quaternion.identity);
		block.SetParent (transform, false);	
		block.GetComponent<BlockHolderController> ().SetCoordinates (x, -1, z);
		blocks [x, 0, z] = block;
		myBlocks [0] = block.GetComponent<BlockHolderController>();
		for (int y = 0; y < height; y++) {
			block = (Transform) Instantiate(earthPrefab, new Vector3(0, y, 0), Quaternion.identity);
			block.SetParent (transform, false);	
			block.GetComponent<BlockHolderController> ().SetCoordinates (x, y + 1, z);
			blocks [x, y + 1, z] = block;
			myBlocks [y + 1] = block.GetComponent<BlockHolderController>();
		}
		block.GetComponent<BlockController> ().SetTopmost (true);
		topmost = height;
		this.height = height;
	}

	public void Select(){
		myBlocks [topmost].Select ();
		selected = true;
	}

	public void Deselect(){
		myBlocks [topmost].Deselect ();
		selected = false;
	}
}
