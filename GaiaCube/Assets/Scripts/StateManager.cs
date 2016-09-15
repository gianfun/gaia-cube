using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour {
    public int currentLevel;

    void Awake()
    {
		if (GameObject.FindGameObjectWithTag ("StateManager") != null) {
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
    }

    public void LoadCurrentLevel()
    {
        SceneManager.LoadScene("Main");
    }
}
