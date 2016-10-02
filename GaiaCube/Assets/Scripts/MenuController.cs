using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
    public int levels = 10;
    public int itemsPerRow = 4;
    public int rows = 3;

    public GameObject levelButton;
    public RectTransform drawArea;
	private StateManager sm;
	// Use this for initialization
	void Start () {
		sm = StateManager.getInstance ();
		levels = sm.levelCount;

        CreateLevelButtons();

		if (sm.shouldUseVR) {
			//drawArea.GetComponent<Canvas> ();
			GvrViewer.Instance.VRModeEnabled = true;
		}
    }

    void CreateLevelButtons()
    {
        float w = drawArea.rect.width;
        float h = drawArea.rect.height;
        float holderWidth = w / itemsPerRow;
        float holderHeight = h / rows;
        float itemWidth = w / (itemsPerRow + 1);
        float itemHeight = h / (rows + 1);
        float xStart = (holderWidth - itemWidth) / 2;
        float yStart = -(holderHeight - itemHeight) / 2;
        int lvl;
        RectTransform obj;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < itemsPerRow; col++)
            {
                lvl = col + itemsPerRow * row + 1;
                if (lvl <= levels) { 
                    obj = ((GameObject)Instantiate(levelButton, new Vector3(xStart + col * holderWidth, yStart - row * holderHeight, 0), Quaternion.identity)).GetComponent<RectTransform>();
                    obj.SetParent(drawArea, false);
                    obj.sizeDelta = new Vector2(itemWidth, itemHeight); //new Rect(col * itemWidth, -row * itemHeight, itemWidth, itemHeight);
					obj.GetComponent<LevelButton>().Init(lvl, sm.unlockedLevels [lvl - 1]);
                }
            }
        }
    }

	public void toggleVRMode(bool isOff){
		sm.SetVRUsage (!isOff);
		FindObjectOfType<GvrReticle> ().gameObject.SetActive(!isOff);
		GvrViewer.Instance.VRModeEnabled = !isOff;
	}
}
