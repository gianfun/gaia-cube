using UnityEngine;
using System.Collections;

public class TutorialPanelController : MonoBehaviour {
    public enum Tutorials { NONE, SELECT, EARTH, WATER, WIND };

    [Header("Self References")]
    public RectTransform panelTrans;
    public GameObject continueButton;
    public UnityEngine.UI.Text txt;
    public UnityEngine.UI.Text shadow;

    [Header("Vars")]
    public Tutorials currentTut;
    public Canvas canvas;

    private Animator animator;
    private RenderTexture renderTexture;
    private UnityEngine.UI.RawImage rawImage;

    Vector3 pos;
    Quaternion rot;
    Vector3 scale;
    Vector2 size;

    // Use this for initialization
    void Awake() {
        animator = GetComponentInChildren<Animator>();
        canvas = panelTrans.GetComponent<Canvas>();
        renderTexture = new RenderTexture(448, 224, 24);
        GetComponentInChildren<Camera>().targetTexture = renderTexture;
        GetComponentInChildren<UnityEngine.UI.RawImage>().texture = renderTexture;
        //continueButton = GetComponentInChildren<UnityEngine.UI.Button>().gameObject;
    }

    public void SetPosition(Vector3 pos, Quaternion rot, Vector3 scale, Vector2 size)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
        this.size = size;

        GoToSavedPosition();
    }

    public void GoToSavedPosition()
    {
        panelTrans.position = pos;
        panelTrans.rotation = rot;
        panelTrans.localScale = scale;
        panelTrans.sizeDelta = size;
    }

    public void ShowButton()
    {
        continueButton.SetActive(true);
    }

    public void HideButton()
    {
        continueButton.SetActive(false);
    }

    public void showTutorial(Tutorials tut)
    {
        Debug.Log("Show tutorial " + tut);
        currentTut = tut;
        switch (tut)
        {
            case Tutorials.SELECT:
                animator.SetTrigger("select");
                setText("Seleção de Blocos");
                break;
            case Tutorials.EARTH:
                animator.SetTrigger("earth");
                setText("Subir/Descer Terreno");
                break;
            case Tutorials.WATER:
                animator.SetTrigger("water");
                setText("Adicionar/Remover Água");
                break;
            case Tutorials.WIND:
                animator.SetTrigger("wind");
                setText("Criar Vento");
                break;
        }
    }

    private void setText(string newText)
    {
        txt.text = newText;
        shadow.text = newText;
    }


}
