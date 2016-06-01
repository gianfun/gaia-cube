using UnityEngine;
using System.Collections;

public class RainController : MonoBehaviour {
	[SerializeField]
	private PlayerController playerController;

	public Transform waterBlock;
	
	void Update () {
		if (playerController.doWater) {
			GameObject world = GameObject.FindGameObjectWithTag ("World");
			Transform hoveredBlock = world.GetComponent<WorldController> ().GetHovered ();
			FillLake (world, hoveredBlock);
		}
	}

	private void FillLake (GameObject world, Transform hoveredBlock) {
		if (hoveredBlock == null) {
			return;
		}

		BlockHolderController blockController = hoveredBlock.GetComponent<BlockHolderController> ();
		WorldController worldController = world.GetComponent<WorldController> ();

		int x;
		int y;
		int z;

		try {
			x = blockController.x;
			y = blockController.y;
			z = blockController.z;
		} catch (System.NullReferenceException) {
			print ("WHOOPSIES");
			return;
		}

		foreach (int [] coord in worldController.GetCanyon(x, y + 1, z)) {
			AddWaterBlock (coord [0], coord [1], coord [2], worldController, blockController);
		}
//		print (printable);
	}

	private void AddWaterBlock(int x, int y, int z, WorldController worldController, BlockHolderController blockController) {
		Transform waterBlock = worldController.MakeWaterBlock (x, y, z);
		blockController.Deactivate (false);
		waterBlock.GetComponent<BlockController> ().SetTopmost (true);
	}
}
