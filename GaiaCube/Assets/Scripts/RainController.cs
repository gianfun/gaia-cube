using UnityEngine;
using System.Collections;

public class RainController : MonoBehaviour {

	public Transform waterBlock;
	
	void Update () {
		if (Input.GetKeyDown ("1")) {
			GameObject world = GameObject.FindGameObjectWithTag ("World");
			Transform hoveredBlock = world.GetComponent<WorldController> ().GetHovered ();
			FillLake (world, hoveredBlock);
		}
	}

	private void FillLake (GameObject world, Transform hoveredBlock) {
		if (hoveredBlock == null) {
			return;
		}
		int height;
		BlockController blockController = hoveredBlock.GetComponent<BlockController> ();
		WorldController worldController = world.GetComponent<WorldController> ();
		try {
			height = blockController.y;
		} catch (System.NullReferenceException) {
			height = 0;
		}
		Debug.Log ("terrain map for height: " + height);
		int[,] terrain;
		terrain = worldController.GetTerrain ();
		string printable = "";
		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 5; j++) {
				printable += terrain [i, j];
				if (terrain [i, j] <= height) { // TODO: Actually check for CONNECTED pools, not just any block under the height line
					AddWaterBlock (i, height, j, worldController, blockController);
				}
			}
			printable += "\n";
		}
		print (printable);
	}

	private void AddWaterBlock(int x, int y, int z, WorldController worldController, BlockController blockController) {
		Transform waterBlock = worldController.MakeWaterBlock (x, y + 1, z);
		blockController.Deactivate (false);
		waterBlock.GetComponent<BlockController> ().SetTopmost (true);
	}
}
