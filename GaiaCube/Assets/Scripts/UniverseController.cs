using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class UniverseController : MonoBehaviour, IConnectionGuru {
    int[,,] clayState, goalState;
    Vector3 dimensions;
    public float cameraSpeed;
    float cameraDuration = 0.8f;
    float cameraTime = 0;

    Quaternion lookAtClayWorld, lookAtGoalWorld;

    Transform clayWorldTrans, goalWorldTrans;
	WorldController goalWorld;
	ClayWorldController clayWorld;
    VRManager vrManager;

    private StateManager sm;

    public PlayerController playerController;
    public CanvasManager canvasManager;
    public GameObject leapController;
    public GameObject WinMessage;
	public bool goBackToMenu;

    private GameObject cameraScroller;

	IEnumerator StartLevel(int level){
		WorldLoader worldLoader = new WorldLoader ();
		yield return worldLoader.LoadLevel(level);
		clayState = worldLoader.getClayState ();
		goalState = worldLoader.getGoalState ();
		dimensions = worldLoader.getDimensions ();

        playerController.setUsableElements(worldLoader.usableElements);
        canvasManager.showUsableElements(worldLoader.usableElements);

        clayWorld.Init ();
		goalWorld.Init ();
		clayWorld.CreateBlocks (clayState, dimensions);
		goalWorld.CreateBlocks (goalState, dimensions);
	}

    void Awake()
    {
        sm = StateManager.getInstance();
        if (!ShouldUseLeap())
        {
            leapController.SetActive(false);
        }
    }

    void Start() {
        vrManager = VRManager.getInstance();
        vrManager.OnToggleVR += (e) => onToggleVR(e);

        cameraScroller = GameObject.FindWithTag("CanvasScroller");

        clayWorldTrans = GameObject.FindWithTag ("ClayWorld").GetComponent<Transform>();
		goalWorldTrans = GameObject.FindWithTag ("GoalWorld").GetComponent<Transform>();
		clayWorld = clayWorldTrans.GetComponent<ClayWorldController>();
		goalWorld = goalWorldTrans.GetComponent<WorldController>();

        lookAtClayWorld = Quaternion.Euler(45, 0, 0);
        lookAtGoalWorld = Quaternion.Euler(45, 90, 0);

        vrManager.toggleVR(sm.shouldUseVR);

        Color skyBlue = new Color(0.2f, 0.3f, 0.4f, 0.7f);
		Color sunYellow = new Color(0.8f, 0.6f, 0.2f, 0.3f);
		RenderSettings.ambientSkyColor = sunYellow;
		RenderSettings.fog = true;
		RenderSettings.fogDensity = 0.05f;
		RenderSettings.fogColor = skyBlue;

		StartCoroutine(StartLevel (sm.currentLevel));
	}

    public void onToggleVR(bool turnOn)
    {
        //active when !turnOn
        cameraScroller.SetActive(!turnOn); //Disable scroller
        //active when turnOn
        Camera.main.GetComponent<GvrHead>().trackRotation = turnOn; //Stop turning with head movement
        if (turnOn)
        {
            GameObject.FindWithTag("Canvas").GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            GameObject.FindWithTag("Canvas").GetComponent<RectTransform>().localPosition = new Vector3(444, -235, 0);
            GameObject.FindWithTag("Canvas").GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 90, 0);
            GameObject.FindWithTag("Canvas").GetComponent<RectTransform>().sizeDelta = new Vector2(888, 470);
            GameObject.FindWithTag("Player").GetComponent<Transform>().localRotation = Quaternion.Euler(0, 90, 0);
            GameObject.FindWithTag("BoundingGrid").GetComponent<Transform>().localPosition = new Vector3(7, 11, 0);
            GameObject.FindWithTag("World").GetComponent<Transform>().localPosition = new Vector3(7, 0, 0);
        }
        else
        {
            cameraTime = 0;
            GameObject.FindWithTag("Player").GetComponent<Transform>().localRotation = Quaternion.Euler(0, 45, 0);
            GameObject.FindWithTag("Canvas").GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Camera.main.GetComponent<Transform>().localRotation = Quaternion.Euler(45, 0, 0);
            GameObject.FindWithTag("BoundingGrid").GetComponent<Transform>().localPosition = new Vector3(0, 11, 0);
            GameObject.FindWithTag("World").GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
        }

        vrManager.toggleReticle(sm.leapMode == StateManager.LeapMode.None);
        GameObject.FindWithTag("EventSystem").GetComponent<OurGazeInputModule>().enabled = (sm.leapMode == StateManager.LeapMode.None);
    }

    public void CheckForWinningCondition()
    {
        bool areEqual = true;
        for(int x = 0; x < dimensions.x && areEqual; x++)
        {
            for (int y = 0; y < dimensions.y && areEqual; y++)
            {
                for (int z = 0; z < dimensions.z -1 && areEqual; z++)
                {
                    if (clayWorld.GetElementAt(x, y, z) != goalWorld.GetElementAt(x, y, z))
                    {
                        areEqual = false;
                        break;
                    }
                }
            }
        }

        if (areEqual)
        {
            print("Finit");
			sm.FinishedLevel (sm.currentLevel);
            StartCoroutine(ShowWin());
        } 
    }

	public void ResetClayWorld(){
		clayWorld.ResetBlocks ();
		clayWorld.CreateBlocks (clayState, dimensions);
		clayWorld.mergeTerrain ();
	}

    private IEnumerator ShowWin()
    {
        WinMessage.SetActive(true);
        yield return new WaitForSeconds(3);
		goBackToMenu = true;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.O))
        {
            vrManager.toggleVR(!sm.shouldUseVR);
            sm.SetVRUsage(!sm.shouldUseVR);

        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            vrManager.toggleReticle(sm.leapMode == StateManager.LeapMode.None);
            sm.IncrementLeapMode();
        }

        if (playerController.lateralMovementDistance != 0f)
        {
            cameraSpeed = Mathf.Clamp(playerController.lateralMovementDistance*2, -1.5f, +1.5f);
        }

        if (cameraSpeed != 0)
        {
            cameraTime += Time.deltaTime * cameraSpeed;
            if (cameraTime < 0f)
            {
                cameraTime = 0f;
            }
            if (cameraTime > cameraDuration)
            {
                cameraTime = cameraDuration;
            }

            Camera.main.transform.localRotation = Quaternion.Slerp(lookAtClayWorld, lookAtGoalWorld, cameraTime / cameraDuration);
        }
    
		if (goBackToMenu) {
			SceneManager.LoadScene("Menu");
		}
    }


    public void MoveCameraLeft(bool start)
    {
        if (start)
        {
            cameraSpeed = -1f;
        }
        else
        {
            cameraSpeed = 0f;
        }
    }

		
    public void MoveCameraRight(bool start)
    {
        if (start)
        {
            cameraSpeed = 1f;
        }
        else
        {
            cameraSpeed = 0f;
        }
    }

    public bool ShouldUseWebLeap()
    {
        return sm.leapMode == StateManager.LeapMode.Web;
    }

    public bool ShouldUseLeap()
    {
        return !(sm.leapMode == StateManager.LeapMode.None);
    }

    public string GetIP()
    {
        return sm.leapIP;
    }

    /*
	void ShowElementAction(CanvasManager.PlayerAction action){
		StartCoroutine(ShowElementActionCoroutine(action));
	}

	IEnumerator ShowElementActionCoroutine(CanvasManager.PlayerAction action){
		
		canvas.ShowElement (action);
		yield return new WaitForSeconds (0.8f);
		canvas.ShowElement (CanvasManager.PlayerAction.NONE);

	}
	*/

}
