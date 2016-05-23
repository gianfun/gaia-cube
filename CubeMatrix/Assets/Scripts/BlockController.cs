using UnityEngine;
using System.Collections;

public class BlockController : BlockHolderController {

	void Start() {
		currentMaterial = normalMaterial;
	}

	void Update() {
		if (Input.GetKeyDown ("down") && selected) {
			Deactivate (true);
			try {
				Transform nextBlock = transform.parent.GetComponent<WorldController> ().GetBlock (x, y - 1, z);
				nextBlock.GetComponent<BlockHolderController> ().SetTopmost (true);
				nextBlock.GetComponent<BlockHolderController> ().Select ();
			} catch (System.IndexOutOfRangeException) {
				return;
			}
		} else if (Input.GetKeyDown ("up") && selected) {
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
	}
}
