using UnityEngine;
using System.Collections;

public class BlockColumn : MonoBehaviour {
	[SerializeField]
	private int height;

	public Vector2 position;
	[SerializeField]
	private int _topmost;
	public int topmost { get {return _topmost;} protected set {_topmost = value;}}

	public bool selected = false;
	private BlockController[] myBlocks;
	public PlayerController playerController;

	public void Init(int x, int z, int height, Transform[,,] blocks, int[,,] state, Transform blockPrefab){
		position = new Vector2 (x, z);
		myBlocks =  new BlockController[height + 1];
		Transform block = (Transform) Instantiate(blockPrefab , new Vector3(0, -1, 0), Quaternion.identity);
		block.name = "Base";
		block.SetParent (transform, false);	
		block.GetComponent<BlockController> ().SetCoordinates (x, 0, z);
		block.GetComponent<BlockController> ().SetElement (BlockController.getElementFromId(state[x, 0, z]));
		blocks [x, 0, z] = block;
		myBlocks [0] = block.GetComponent<BlockController>();
		for (int y = 0; y < height-1; y++) {
			block = (Transform) Instantiate(blockPrefab, new Vector3(0, y, 0), Quaternion.identity);
			block.name = "Block" + y;
			block.SetParent (transform, false);	
			block.GetComponent<BlockController> ().Init ();
			block.GetComponent<BlockController> ().SetCoordinates (x, y + 1, z);

			block.GetComponent<BlockController> ().SetElement (BlockController.getElementFromId(state[x, y + 1, z]));

			blocks [x, y + 1, z] = block;
			myBlocks [y + 1] = block.GetComponent<BlockController>();
		}

		for (int y = height - 1; y >= 0; y--) {
			if (myBlocks [y].element != BlockController.Element.AIR) {
				myBlocks [y].SetTopmost (true);
				topmost = y;
				break;
			}
		}
		this.height = height-1;
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
			myBlocks [topmost].Deselect ();
			myBlocks [topmost].SetElement(BlockController.Element.AIR);
			myBlocks [topmost].SetTopmost(false);
			topmost--;
			myBlocks [topmost].SetTopmost (true);
			myBlocks [topmost].Select ();
		}
	}

	public void MoveEarthUp(){
		if (selected && topmost < height && myBlocks[topmost].element == BlockController.Element.EARTH) {
			myBlocks [topmost].Deselect ();
			myBlocks [topmost].SetTopmost(false);
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
			myBlocks [topmost].Deselect ();
			myBlocks [topmost].SetElement(BlockController.Element.AIR);
			topmost--;
			myBlocks [topmost].SetTopmost (true);
			selected = false;
		} else {
			Debug.LogError ("Tried drying block " + height + " of blockColumn "+ position + " which isn't topmost..") ;
		}
	}

	public void MakeTopSand(){
		if (myBlocks [topmost].element == BlockController.Element.EARTH) {
			myBlocks [topmost].SetElement (BlockController.Element.SAND);
		}
	}

	public void BreakTopEarth(){
		if (topmost > 0 && myBlocks [topmost].element == BlockController.Element.EARTH) {
			myBlocks [topmost].Deselect ();
			myBlocks [topmost].SetElement(BlockController.Element.AIR);
			myBlocks [topmost].SetTopmost(false);
			topmost--;
			myBlocks [topmost].SetTopmost (true);
		}
	}

	public BlockController.Element GetBlockElementAt(int y){
		if (y <= height) {
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

	public BlockController.Element GetTopElement(){
		return myBlocks [topmost].element;
	}
}
