using UnityEngine;
using System.Collections;

public class RainController : MonoBehaviour {

	public Transform waterBlock;
	
	void Update () {
		if (Input.GetKeyDown ("space")) {
			GameObject world = GameObject.FindGameObjectWithTag ("World");
			Transform hoveredBlock = world.GetComponent<WorldController> ().GetHovered ();
			FillLake (world, hoveredBlock);
		}
	}

	private void FillLake (GameObject world, Transform hoveredBlock) {
		int height;
		try {
			height = hoveredBlock.GetComponent<BlockHolderController> ().y + 1;
		} catch (System.NullReferenceException) {
			height = 0;
		}
		Debug.Log ("terrain map for height: " + height);
		int[,] slice;
		slice = world.GetComponent<WorldController> ().GetTerrain ();
		string printable = "";
		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 5; j++) {
				printable += slice [i, j];
			}
			printable += "\n";
		}
		print (printable);
	}
}
