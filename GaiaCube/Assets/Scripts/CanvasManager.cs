using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasManager : MonoBehaviour {
	public Canvas canvas;
	public Image elementShower;
	public Sprite windSprite, waterSprite, fireSprite, earthUpSprite, earthDownSprite;

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

	public enum PlayerAction {
		WIND, WATER, FIRE, EARTH_UP, EARTH_DOWN, NONE
	}
}
