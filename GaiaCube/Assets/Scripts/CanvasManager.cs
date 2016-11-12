using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasManager : MonoBehaviour {
	public Canvas canvas;
	public Image elementShower;

    //They are in two different lines so Unity shows them nicely
    [Header("Sprites")]
    public Sprite windSprite;
    public Sprite waterSprite, fireSprite, earthUpSprite, earthDownSprite;

    [Header("GameObjects")]
    public GameObject earthUpButton;
    public GameObject earthDownButton, waterButton, fireButton, windButton;

	public void ShowElement(PlayerAction action){
		switch(action){
		case PlayerAction.WIND:
			elementShower.sprite = windSprite;
			elementShower.enabled = true;
			break;
		case PlayerAction.WATER:
			elementShower.sprite = waterSprite;
			elementShower.enabled = true;
			break;
		case PlayerAction.FIRE:
			elementShower.sprite = fireSprite;
			elementShower.enabled = true;
			break;
		case PlayerAction.EARTH_UP:
			elementShower.sprite = earthUpSprite;
			elementShower.enabled = true;
			break;
		case PlayerAction.EARTH_DOWN:
			elementShower.sprite = earthDownSprite;
			elementShower.enabled = true;
			break;
		case PlayerAction.NONE:
			elementShower.enabled = false;
			break;
		}

	}

    public void showUsableElements(UsableElements elements)
    {
        earthUpButton.SetActive(elements.earth);
        earthDownButton.SetActive(elements.earth);
        waterButton.SetActive(elements.water);
        fireButton.SetActive(elements.fire);
        windButton.SetActive(elements.wind);
    }

	public enum PlayerAction {
		WIND, WATER, FIRE, EARTH_UP, EARTH_DOWN, NONE
	}
}
