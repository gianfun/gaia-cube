using UnityEngine;
using System.Collections;

public class LevelButton : MonoBehaviour {
    private int level;
    private StateManager sm;
    public UnityEngine.UI.Text txt;

	public void Init(int level, bool isUnlocked)
    {
        this.level = level;
        txt.text = "" + level;
		sm = StateManager.getInstance();
		if (!isUnlocked) {
			GetComponent<UnityEngine.UI.Button> ().interactable = false;
		}
    }

    public void Click()
    {
        sm.PlayLevel(level);
    }
}
