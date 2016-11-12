using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {


	// Use this for initialization
	void Start () {
		Animator animator = GetComponent<Animator>();

		StateManager sm = StateManager.getInstance();
		switch (sm.currentLevel) {
		case 1:
			animator.SetTrigger ("select");
			break;
		case 2:
			animator.SetTrigger ("earth");
			break;
		case 3:
			animator.SetTrigger ("water");
			break;
		case 4:
			animator.SetTrigger ("wind");
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
