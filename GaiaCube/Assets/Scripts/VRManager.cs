using UnityEngine;
using System.Collections;

public class VRManager : MonoBehaviour {
    public static VRManager instance;

    public GameObject prefabVRViewer;
    public GameObject prefabVRReticle;

    public GvrViewer VR_viewer;
    public OurGazeReticle VR_reticle;

    public static VRManager getInstance()
    {
        if (instance == null)
        {
            instance = new VRManager();
            if (!instance.InitNewInstance())
            {
                instance = null;
            }
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

    private bool InitNewInstance()
    {
        if (prefabVRViewer == null) return false;
        instance = this;
        
        VR_viewer = FindObjectOfType<GvrViewer>();
        if (VR_viewer == null)
        {
            
            GameObject viewer = Instantiate(prefabVRViewer);
            VR_viewer = viewer.GetComponent<GvrViewer>();
        }
        if (VR_reticle == null)
        {
            GameObject reticle = Instantiate(prefabVRReticle);
            reticle.name = "GvrReticle2";
            reticle.transform.SetParent(Camera.main.transform, false);
            VR_reticle = reticle.GetComponent<OurGazeReticle>();
        }
        return true;
    }

    public void toggleVR(bool turnOn)
    {
        Debug.Log("VR Manager. Toggling VR to: " + turnOn);
        VR_viewer.VRModeEnabled = turnOn;
        VR_reticle.GetComponent<MeshRenderer>().enabled = turnOn;
        //VRreticle.enabled = !turnOn;
    }

}
