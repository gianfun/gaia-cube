using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour {
    public int currentLevel;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadCurrentLevel()
    {
        SceneManager.LoadScene("Main");
    }
}
