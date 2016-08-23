using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainController : MonoBehaviour {
	[SerializeField]
	private PlayerController playerController;
	WorldController worldController;
	public Transform waterBlock;

	void Start(){
		worldController = GameObject.FindGameObjectWithTag ("World").GetComponent<WorldController> ();
	}

	void Update () {
		
	}



	/*
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
	*/

	/*
	private void AddWaterBlock(Vector3 pos, WorldController worldController, BlockHolderController blockController) {
		Transform waterBlock = worldController.MakeWaterBlock ((int)pos.x, (int)pos.y, (int)pos.z);
		blockController.Deactivate (false);
		waterBlock.GetComponent<BlockController> ().SetTopmost (true);
	}
	*/

	private void FillWaterColumn(Vector3 pos, WorldController worldController) {
		worldController.FillWaterColumn (pos);
	}
}
