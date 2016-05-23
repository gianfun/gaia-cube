using UnityEngine;
using System.Collections;

public class BlockHolderController : MonoBehaviour {

	public Material normalMaterial;
	public Material selectedMaterial;

	protected Material currentMaterial;

	protected bool topmost = false;
	protected bool selected = false;
	public int x { get; protected set; }
	public int y { get; protected set; }
	public int z { get; protected set; }

	void Start() {
		currentMaterial = normalMaterial;
		transform.Translate (0, .5f, 0);
	}

	void Update() {
		if (Input.GetKeyDown ("up") && selected) {
			try {
				Transform nextBlock = transform.parent.GetComponent<WorldController> ().MakeBlock (x, y + 1, z);
				Deactivate (false);
				nextBlock.GetComponent<BlockController> ().SetTopmost (true);
				nextBlock.GetComponent<BlockController> ().Select ();
			} catch (System.IndexOutOfRangeException) {
				return;
			}
		}
	}

	public void Select() {
		if (topmost) {
			if (selected) {
				Deselect ();
			} else {
				selected = true;
				GetComponent<Renderer> ().material = selectedMaterial;
				currentMaterial = selectedMaterial;
			}
		}
	}

	public void Deselect() {
		selected = false;
		GetComponent<Renderer> ().material = normalMaterial;
		currentMaterial = normalMaterial;
	}

	protected void Deactivate(bool hide) {
		if (hide) {
			gameObject.SetActive (false);
		}
		Deselect ();
		topmost = false;
	}

	public bool GetTopmost() {
		return this.topmost;
	}

	public void SetTopmost(bool topmost) {
		this.topmost = topmost;
	}

	public void SetCoordinates(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	void OnMouseEnter() {
		transform.parent.GetComponent<WorldController> ().SetHovered (transform);
		if (topmost) {
			Color newColor = new Color (
				currentMaterial.color.r + 0.1f,
				currentMaterial.color.g + 0.1f,
				currentMaterial.color.b + 0.1f
			);
			GetComponent<Renderer> ().material.color = newColor;
		}
	}

	void OnMouseExit() {
		if (transform.parent.GetComponent<WorldController> ().GetHovered () == transform) {
			transform.parent.GetComponent<WorldController> ().SetHovered (null);
		}
		Color newColor = new Color (
			currentMaterial.color.r,
			currentMaterial.color.g,
			currentMaterial.color.b
		);
		GetComponent<Renderer> ().material.color = newColor;
	}

	void OnMouseUpAsButton() {
		Select ();
	}
}
