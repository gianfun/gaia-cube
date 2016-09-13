using UnityEngine;
using System.Collections;

public class LevelButton : MonoBehaviour {
    private int level;
    private StateManager sm;
    public UnityEngine.UI.Text txt;

    public void Init(int level)
    {
        this.level = level;
        txt.text = "" + level;
        sm = GameObject.FindGameObjectWithTag("StateManager").GetComponent<StateManager>();
    }

    public void Click()
    {
        sm.currentLevel = level;
        sm.LoadCurrentLevel();
    }
}
