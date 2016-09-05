using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour {
	public PlayerController playerController;

	public Material earthMaterial;
	public Material earthTopMaterial;
	public Material waterMaterial;
	public Material sandMaterial;
	public Material selectedMaterial;
	[SerializeField]
	protected Material normalMaterial;
	[SerializeField]
	protected Material currentMaterial;

	[SerializeField]
	protected bool topmost = false;

	public bool selected { get; protected set; }

	public Element element = Element.EARTH;

	public int x { get; protected set; }
	public int y { get; protected set; }
	public int z { get; protected set; }
	public Vector3 position {
		get	{ 
			return new Vector3 (x, y, z); 
		} 
		set{
			x = (int)value.x; y = (int)value.y; z = (int)value.z;
		}
	}

	public void Init() {
		selected = false;
	}

	public void ToggleSelect() {
		if (topmost) {
			if (selected) {
				Deselect ();
			} else {
				Select ();
			}
		}
	}

	public void Select() {
		if (!selected) {
			selected = true;

			GetComponent<Renderer> ().material = selectedMaterial;
			currentMaterial = selectedMaterial;
		}
	}

	public void Deselect() {
		if (selected) {
			selected = false;

			GetComponent<Renderer> ().material = normalMaterial;
			currentMaterial = normalMaterial;
		}
	}

	public void Deactivate(bool hide) {
		if (hide) {
			gameObject.SetActive (false);
		}
		Deselect ();
		topmost = false;
	}

	public void Activate() {
		gameObject.SetActive (true);
	}

	public bool GetTopmost() {
		return this.topmost;
	}

	public void SetTopmost(bool topmost) {
		this.topmost = topmost;
		if (topmost) {
			//TODO: Show grass
		} 
	}

	public void SetElement (Element element) {
		this.element = element;
		GetComponent<Renderer> ().enabled = true;
		switch (element) {
		case Element.EARTH:
			GetComponent<Renderer> ().material = currentMaterial = normalMaterial = earthMaterial;
			break;
		case Element.WATER:
			GetComponent<Renderer> ().material = currentMaterial = normalMaterial = waterMaterial;
			break;
		case Element.AIR:
			gameObject.SetActive (false);
			//GetComponent<Renderer> ().material = currentMaterial = normalMaterial = waterMaterial;
			break;
		case Element.SAND:
			GetComponent<Renderer> ().material = currentMaterial = normalMaterial = sandMaterial;
			break;
		case Element.BASE:
			this.element = Element.EARTH;
			GetComponent<Renderer> ().material = currentMaterial = normalMaterial = earthMaterial;
			transform.localScale = new Vector3(1f, 0.1f, 1f);
			transform.localPosition = new Vector3(0f, -0.55f, 0f);
			break;
		}
	}

	public void SetCoordinates(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	/*
	void OnMouseEnter() {
		if (element != Element.EARTH)
			return;
		transform.parent.parent.GetComponent<WorldController> ().SetHovered (transform);
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
		if (element != Element.EARTH)
			return;
		if (transform.parent.parent.GetComponent<WorldController> ().GetHovered () == transform) {
			transform.parent.parent.GetComponent<WorldController> ().SetHovered (null);
		}
		Color newColor = new Color (
			currentMaterial.color.r,
			currentMaterial.color.g,
			currentMaterial.color.b
		);
		GetComponent<Renderer> ().material.color = newColor;
	}

	void OnMouseUpAsButton() {
		if (element != Element.EARTH)
			return;
		ToggleSelect ();
	}
	*/

	public static Element getElementFromId(int id){
		switch (id) {
		case 0:
			return Element.AIR;
		case 1:
			return Element.EARTH;
		case 2:
			return Element.WATER;
		case 3:
			return Element.SAND;
		case 4:
			return Element.BASE;
		}
		return Element.INVALID;
	}

	public enum Element {
		AIR, EARTH, WATER, SAND, BASE, INVALID
	}
}
