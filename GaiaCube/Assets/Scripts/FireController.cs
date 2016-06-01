using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour {
	[SerializeField]
	private PlayerController playerController;

	void Update () {
		if (playerController.doFire) {
			GameObject world = GameObject.FindGameObjectWithTag ("World");
			Transform hoveredBlock = world.GetComponent<WorldController> ().GetHovered ();
			DryOutPoolSlice (world, hoveredBlock);
		}
	}

	private void DryOutPoolSlice (GameObject world, Transform hoveredBlock) {
	
	}
}
