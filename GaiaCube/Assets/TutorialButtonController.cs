using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButtonController : MonoBehaviour {
    public UniverseController universe;

    public void Start()
    {
        universe = GameObject.FindWithTag("UniverseController").GetComponent<UniverseController>();
    }

	public void OnClick() {
        universe.TutorialOnClick();
	}
}
