using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour {

	void Update () {
		if (Input.GetKeyDown ("2")) {
			GameObject world = GameObject.FindGameObjectWithTag ("World");
			Transform hoveredBlock = world.GetComponent<WorldController> ().GetHovered ();
			DryOutPoolSlice (world, hoveredBlock);
		}
	}

	private void DryOutPoolSlice (GameObject world, Transform hoveredBlock) {
	
	}
}
