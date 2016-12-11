using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour {
    public enum Tutorials { NONE, SELECT, EARTH, WATER, WIND };

    public GameObject continueButton;

    private Animator animator;
    public Tutorials currentTut;

    private RectTransform myTrans;

    Vector3 pos;
    Quaternion rot;
    Vector3 scale;
    Vector2 size;

    // Use this for initialization
    void Awake() {
        animator = GetComponent<Animator>();
        myTrans = GetComponent<RectTransform>();
        //continueButton = GetComponentInChildren<UnityEngine.UI.Button>().gameObject;
    }

    public void SetPosition(Vector3 pos, Quaternion rot, Vector3 scale, Vector2 size)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
        this.size = size;

        GoToSavedPosition();
    }

    public void GoToSavedPosition()
    {
        myTrans.localPosition = pos;
        myTrans.localRotation = rot;
        myTrans.localScale = scale;
        myTrans.sizeDelta = size;
    }

    public void ShowButton()
    {
        continueButton.SetActive(true);
    }

    public void HideButton()
    {
        continueButton.SetActive(false);
    }

    public void showTutorial(Tutorials tut) {
        Debug.Log("Show tutorial " + tut);
        currentTut = tut;
		switch (tut) {
		    case Tutorials.SELECT:
			    animator.SetTrigger ("select");
			    break;
		    case Tutorials.EARTH:
                animator.SetTrigger ("earth");
			    break;
		    case Tutorials.WATER:
                animator.SetTrigger ("water");
			    break;
		    case Tutorials.WIND:
                animator.SetTrigger ("wind");
			    break;
		}
	}
	

}
