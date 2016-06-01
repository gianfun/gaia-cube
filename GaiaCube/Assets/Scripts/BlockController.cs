using UnityEngine;
using System.Collections;

public class BlockController : BlockHolderController {

	void Start() {
		GetComponent<Renderer> ().material = currentMaterial = normalMaterial = earthMaterial;
	}

	void Update() {
		if (playerController.moveEarthDown && selected) {
			Deactivate (true);
			try {
				Transform nextBlock = transform.parent.GetComponent<WorldController> ().GetBlock (x, y - 1, z);
				nextBlock.GetComponent<BlockHolderController> ().SetTopmost (true);
				nextBlock.GetComponent<BlockHolderController> ().Select ();
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

		FloodAdjacent ();
	}

	public void SetElement (Element element) {
		this.element = element;
		switch (element) {
			case Element.EARTH:
				GetComponent<Renderer> ().material = currentMaterial = normalMaterial = earthMaterial;
				break;
			case Element.WATER:
				GetComponent<Renderer> ().material = currentMaterial = normalMaterial = waterMaterial;
				break;
		}
	}

	private void FloodAdjacent() {
		if (this.element == Element.WATER) {
			FloodIfEmpty (x+1, y, z);
			FloodIfEmpty (x-1, y, z);
			FloodIfEmpty (x, y, z+1);
			FloodIfEmpty (x, y, z-1);
			FloodIfEmpty (x, y-1, z);
		}
	}

	private void FloodIfEmpty(int x, int y, int z) {
		try {
			Transform block = transform.parent.GetComponent<WorldController> ().GetBlock (x, y, z);

			if (!block.gameObject.activeInHierarchy) {
				block.gameObject.SetActive(true);
				block.GetComponent<BlockController> ().SetElement (BlockController.Element.WATER);

				Transform blockBelow = transform.parent.GetComponent<WorldController> ().GetBlock (x, y-1, z);
				blockBelow.GetComponent<BlockHolderController>().Deselect();
			}
		} catch (System.IndexOutOfRangeException e) {}
	}
}
