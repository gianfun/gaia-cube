using UnityEngine;
using System.Collections;

public class VRManager : MonoBehaviour {
    public static VRManager instance;

    public GameObject prefabVRViewer;
    public GameObject prefabVRReticle;

    public GvrViewer VRviewer;
    public OurGazeReticle VRreticle;

    public static VRManager getInstance()
    {
        if (instance == null)
        {
            instance = new VRManager();
            instance.InitNewInstance();
        }
        return instance;
    }

    void Awake()
    {
        if (instance == null)
        {
            InitNewInstance();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void InitNewInstance()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        VRviewer = FindObjectOfType<GvrViewer>();
        if (VRviewer == null)
        {
            GameObject viewer = Instantiate(prefabVRViewer);
            VRviewer = viewer.GetComponent<GvrViewer>();
        }
        if (VRreticle == null)
        {
            GameObject reticle = Instantiate(prefabVRReticle);
            reticle.name = "GvrReticle2";
            reticle.transform.SetParent(Camera.main.transform);
            VRreticle = reticle.GetComponent<OurGazeReticle>();
        }
    }

    public void toggleVR(bool turnOn)
    {
        Debug.Log("VR Manager. Toggling VR to: " + turnOn);
        VRviewer.VRModeEnabled = turnOn;
        VRreticle.GetComponent<MeshRenderer>().enabled = turnOn;
        //VRreticle.enabled = !turnOn;
    }

}
