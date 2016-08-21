using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainController : MonoBehaviour {
	[SerializeField]
	private PlayerController playerController;

	public Transform waterBlock;
	
	void Update () {
		if (playerController.doWater) {
			GameObject world = GameObject.FindGameObjectWithTag ("World");
			Transform hoveredBlock = world.GetComponent<WorldController> ().GetHovered ();
			//FillLake (world, hoveredBlock);

			FillLake (world.GetComponent<WorldController> ());
		}
	}

	private void FillLake (WorldController world){
		List<BlockHolderController> selectedBlocks = world.GetSelectedBlocks();
		if (selectedBlocks.Count == 0) {
			return;
		}

		List<Vector3> blocksToFlood = new List<Vector3> ();
		List<Vector3> floodSource = new List<Vector3> ();

		foreach (BlockHolderController block in selectedBlocks) {
			floodSource.Add (block.position + new Vector3(0, 1, 0));
			print ("Adding " + (block.position + new Vector3 (0, 1, 0)));
		}
		blocksToFlood = world.GetCanyon (floodSource);


		foreach (Vector3 coord in blocksToFlood) {
			AddWaterBlock (coord, world);
		}

		foreach (BlockHolderController block in selectedBlocks) {
			block.Deactivate (false);
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

		foreach (Vector3 coord in worldController.GetCanyon(new Vector3(x, y + 1, z))) {
			AddWaterBlock (coord, worldController, blockController);
		}
//		print (printable);
	}

	private void AddWaterBlock(Vector3 pos, WorldController worldController, BlockHolderController blockController) {
		Transform waterBlock = worldController.MakeWaterBlock ((int)pos.x, (int)pos.y, (int)pos.z);
		blockController.Deactivate (false);
		waterBlock.GetComponent<BlockController> ().SetTopmost (true);
	}

	private void AddWaterBlock(Vector3 pos, WorldController worldController) {
		Transform waterBlock = worldController.MakeWaterBlock ((int)pos.x, (int)pos.y, (int)pos.z);
		waterBlock.GetComponent<BlockController> ().SetTopmost (true);
	}
}
