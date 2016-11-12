using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
    public int levels = 10;
    public int itemsPerRow = 4;
    public int rows = 3;

    public GameObject levelButton;
    public RectTransform drawArea;

    public GameObject configOverlay;
    public UnityEngine.UI.InputField configInputText;

    private StateManager sm;
    private VRManager vrManager;

    private GameObject uiCrossVR;
    private GameObject uiTickVR;

    public GameObject[] leapModeGUIImages;

    private GameObject configLeapButton;

    public UnityEngine.UI.Toggle VRToggle;
    void Awake()
    {
        sm = StateManager.getInstance();
        vrManager = VRManager.getInstance();
        vrManager.OnToggleVR += (e) => onToggleVR(e); 
        levels = sm.levelCount;

        CreateLevelButtons();
        Debug.Log("Menu Controller Awake. sm.shouldUseVR: " + sm.shouldUseVR);
    }

    void Start()
    {
        uiCrossVR = GameObject.Find("CrossVR");
        uiTickVR = GameObject.Find("TickVR");
        configLeapButton = GameObject.Find("ConfigLeap");
        configLeapButton.SetActive(sm.leapMode == StateManager.LeapMode.Web);
        vrManager.toggleVR(sm.shouldUseVR);
        HideIPConfig();

        for (int i = 0; i < leapModeGUIImages.Length; i++)
        {
            leapModeGUIImages[i].SetActive(false);
        }
        leapModeGUIImages[(int)sm.leapMode].SetActive(true);
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

    public void onToggleVR(bool turnOn)
    {
        //Save value
        sm.SetVRUsage(turnOn);

        //Display on button
        uiCrossVR.SetActive(!turnOn);
        uiTickVR.SetActive(turnOn);

        Camera.main.transform.localRotation = Quaternion.identity;

        //active when turnOn
        Camera.main.GetComponent<GvrHead>().trackRotation = turnOn; //Stop turning with head movement
    }

    public void onButtonToggleVR(){
        vrManager.toggleVR(!sm.shouldUseVR);
	}

    public void onButtonToggleLeap()
    {
        leapModeGUIImages[(int)sm.leapMode].SetActive(false);
        sm.IncrementLeapMode();
        leapModeGUIImages[(int)sm.leapMode].SetActive(true);
        configLeapButton.SetActive(sm.leapMode == StateManager.LeapMode.Web);
    }

    public void ShowIPConfig()
    {
        configOverlay.SetActive(true);
        configInputText.text = sm.leapIP;
    }

    public void HideIPConfig()
    {
        configOverlay.SetActive(false);
    }

    public void SubmitIPConfig()
    {
        HideIPConfig();
        sm.SetLeapIP(configInputText.text);
    }
}
