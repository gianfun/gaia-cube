using UnityEngine;
using System.Collections;

public class BlockColumn : MonoBehaviour {
	Transform earthPrefab, waterPrefab, bottomPrefab;
	private int height;

	public Vector2 position;

	public int topmost { get; protected set;}
	public bool selected = false;
	private BlockController[] myBlocks;
	public PlayerController playerController;


	/*
	 		if (playerController.moveEarthDown && selected) {
			Deactivate (true);
			try {
				Transform nextBlock = transform.parent.GetComponent<WorldController> ().GetBlock (x, y - 1, z);
				nextBlock.GetComponent<BlockController> ().SetTopmost (true);
				nextBlock.GetComponent<BlockController> ().Select ();
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
		position = new Vector2 (x, z);
		myBlocks =  new BlockController[height + 1];
		Transform block = (Transform) Instantiate(bottomPrefab , new Vector3(0, -1, 0), Quaternion.identity);
		block.name = "Base";
		block.SetParent (transform, false);	
		block.GetComponent<BlockController> ().SetCoordinates (x, -1, z);
		blocks [x, 0, z] = block;
		myBlocks [0] = block.GetComponent<BlockController>();
		for (int y = 0; y < height; y++) {
			block = (Transform) Instantiate(earthPrefab, new Vector3(0, y, 0), Quaternion.identity);
			block.name = "Block" + y;
			block.SetParent (transform, false);	
			block.GetComponent<BlockController> ().SetCoordinates (x, y + 1, z);
			block.GetComponent<BlockController> ().SetElement (BlockController.Element.EARTH);

			blocks [x, y + 1, z] = block;
			myBlocks [y + 1] = block.GetComponent<BlockController>();
		}
		block.GetComponent<BlockController> ().SetTopmost (true);
		topmost = height;
		this.height = height + 1;
	}

	public void Select(){
		myBlocks [topmost].Select ();
		selected = true;
	}

	public void Deselect(){
		myBlocks [topmost].Deselect ();
		selected = false;
	}

	public void MoveEarthDown(){
		if (selected && topmost > 0 && myBlocks [topmost].element == BlockController.Element.EARTH) {
			myBlocks [topmost].Deactivate (true);
			myBlocks [topmost].SetElement(BlockController.Element.AIR);
			topmost--;
			myBlocks [topmost].SetTopmost (true);
			myBlocks [topmost].Select ();
		}
	}

	public void MoveEarthUp(){
		if (selected && topmost < height && myBlocks[topmost].element == BlockController.Element.EARTH) {
			myBlocks [topmost].Deactivate (false);
			topmost++;
			myBlocks [topmost].Activate();
			myBlocks [topmost].SetTopmost(true);
			myBlocks [topmost].SetElement(BlockController.Element.EARTH);
			myBlocks [topmost].Select();
		}
	}

	public void FillWaterColumn(int height){
		if (topmost + 1 <= height) {
			//print ("FillWaterColumn -> Topmost: " + topmost + " height: " + height);
			for (int i = topmost + 1; i <= height; i++) {
				myBlocks [i].SetElement (BlockController.Element.WATER);
				myBlocks [i].Activate ();
			}
			myBlocks [topmost].SetTopmost (false);
			myBlocks [topmost].Deselect();
			topmost = height;
			myBlocks [topmost].SetTopmost (true);
			selected = false;
		}
	}

	public void FloodTop(){
		FillWaterColumn (topmost + 1);
	}

	public void DryBlock(int height){
		if (topmost == height) {
			myBlocks [topmost].SetTopmost (false);
			myBlocks [topmost].Deactivate (true);
			myBlocks [topmost].SetElement(BlockController.Element.AIR);
			topmost--;
			myBlocks [topmost].SetTopmost (true);
			selected = false;
		} else {
			Debug.LogError ("Tried drying block " + height + " of blockColumn "+ position + " which isn't topmost..") ;
		}
	}

	public BlockController.Element GetBlockElementAt(int y){
		if (y < height) {
			return myBlocks [y].element;
		} else {
			return BlockController.Element.INVALID;
		}
	}

	public BlockController GetTopBlock(){
		if (topmost < height) {
			return myBlocks [topmost];
		}
		return null;
	}
}
