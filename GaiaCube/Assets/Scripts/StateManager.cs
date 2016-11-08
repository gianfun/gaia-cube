using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class StateManager : MonoBehaviour {
	public static StateManager instance;

	public const int defaultLevelCount = 10;

	public int currentLevel;
	public int levelCount;
	public bool[] completedLevels;
	public bool[] unlockedLevels;

	public bool shouldUseVR;
	public bool shouldUseLeap;
    public string leapIP;

	public GameState gs;

	public static StateManager getInstance(){
		return instance;
	}

    void Awake()
    {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
			LoadOrCreateNew ();
		} else {
			DestroyImmediate (gameObject);
		}
    }

	public void PlayLevel(int level)
    {
		currentLevel = level;
        SceneManager.LoadScene("Main");
    }

	public void FinishedLevel(int level){
		Debug.Log ("Finished level " + level);
		gs.completedLevels [level - 1] = true;
		if (level < levelCount) {
			gs.unlockedLevels [level] = true;
		}
		Save ();
	}

	private void Save(){
		Debug.Log ("Saving at " + Application.persistentDataPath + "/gameprogess.dat");
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/gameprogess.dat");
		bf.Serialize(file, gs);
		file.Close();
	}

	private void LoadOrCreateNew(){
		if (!Load ()) {
			Debug.Log ("No save file found. Creating new save file.");
			gs = new GameState (defaultLevelCount);
			Save ();
		}

		if (PlayerPrefs.HasKey ("useVR")) {
			shouldUseVR = GetPlayerPrefsBool ("useVR");
		} else {
			shouldUseVR = false;
			SetPlayerPrefsBool ("useVR", shouldUseVR);
		}

		if (PlayerPrefs.HasKey ("useLeap")) {
			shouldUseLeap = GetPlayerPrefsBool ("useLeap");
		} else {
			shouldUseLeap = false;
			SetPlayerPrefsBool ("useLeap", shouldUseLeap);
		}

        if (PlayerPrefs.HasKey("LeapIP"))
        {
            leapIP = PlayerPrefs.GetString("LeapIP");
        }
        else
        {
            leapIP = "192.168.1.53";
            PlayerPrefs.SetString("LeapIP", leapIP);
        }

        levelCount = gs.levels;
		completedLevels = gs.completedLevels;
		unlockedLevels = gs.unlockedLevels;
	}

	public void SetVRUsage(bool newValue){
		SetPlayerPrefsBool ("useVR", newValue);
		shouldUseVR = newValue;
	}

	public void SetLeapUsage(bool newValue){
		SetPlayerPrefsBool ("useLeap", newValue);
		shouldUseLeap = newValue;
	}

    public void SetLeapIP(string newIP)
    {
        PlayerPrefs.SetString("LeapIP", newIP);
        leapIP = newIP;
    }

    private bool Load(){
		Debug.Log ("Loading save file.");
		if(File.Exists(Application.persistentDataPath + "/gameprogess.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gameprogess.dat", FileMode.Open);
			gs = (GameState)bf.Deserialize(file);
			file.Close();
			return true;
		} 
		return false;
	}

	public static void SetPlayerPrefsBool(string name, bool value) {
		PlayerPrefs.SetInt (name, value ? 1 : 0);
	}

	public static bool GetPlayerPrefsBool(string name) {
		return (PlayerPrefs.GetInt (name) == 1);
	}
}

[System.Serializable]
public class GameState{
	public int levels;
	public bool[] completedLevels;
	public bool[] unlockedLevels;

	public GameState(int levels){
		this.levels = levels;
		completedLevels = new bool[levels];
		unlockedLevels = new bool[levels];
		unlockedLevels [0] = true;
	}
}
