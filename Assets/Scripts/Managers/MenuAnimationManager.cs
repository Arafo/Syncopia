using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class MenuAnimationManager : MonoBehaviour {

    // Paneles
    public Image startPanelCover;
    public GameObject startPanel;
    public GameObject prevPanel;
    public GameObject fromPanel;
    private List<Text> fromTexts = new List<Text>();
    private List<Image> fromImages = new List<Image>();
    public GameObject toPanel;
    private List<Text> toTexts = new List<Text>();
    private List<Image> toImages = new List<Image>();
    private List<Button> toButtons = new List<Button>();
    private Image toImage;

    public Button previousButton;

    public EventSystem eventSystem;

    // Animacion del panel
    private float animationTime;
    private float interpolatedAnimationTime;
    public bool isAnimating;

    private float coverRevealTimer;
    private bool isRevealing;
    public bool loadingAnimation;
    private bool backPedal;

    string leaveScene;

    public bool ingame;

    /// <summary>
    /// 
    /// </summary>
    void Start() {
        fromPanel = startPanel;

        if (!ingame) {
            startPanelCover.gameObject.SetActive(true);
            isRevealing = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate() {
        if (isRevealing) {
            coverRevealTimer += Time.deltaTime * 2;

            startPanelCover.color = new Color(startPanelCover.color.r, startPanelCover.color.g, startPanelCover.color.b, 1.0f - Mathf.Clamp(coverRevealTimer, 0.0f, 1.0f));

            if (coverRevealTimer > 1)
                isRevealing = false;
        }

        if (isAnimating) {
            // Aumentar el tiempo de la animacion
            animationTime += Time.deltaTime * 4;
            interpolatedAnimationTime = Mathf.Lerp(interpolatedAnimationTime, animationTime, Time.deltaTime * 5);

            // Obtener los multiplicacion de la animacion
            float animationPart1 = Mathf.Clamp(interpolatedAnimationTime, 0.0f, 1.0f);
            float animationPart2 = Mathf.Clamp(1.0f - (interpolatedAnimationTime - 1.0f), 0.0f, 1.0f);
            float animationPart2_Text = Mathf.Clamp((interpolatedAnimationTime - 1.0f), 0.0f, 1.0f);

            // Asignar la posicion del panel de origenset from panel position
            fromPanel.transform.localPosition = new Vector3(animationPart1, 0.0f, 0.0f);

            // Asignar la posicion del panel de destino
            toPanel.transform.localPosition = new Vector3(animationPart2, 0.0f, 0.0f);

            // Asinar el color de los textos
            int i;
            for (i = 0; i < fromTexts.Count; ++i)
                if (fromTexts[i] != null)
                    fromTexts[i].color = new Color(fromTexts[i].color.r, fromTexts[i].color.g, fromTexts[i].color.b, 1.0f - animationPart1);
            for (i = 0; i < toTexts.Count; ++i)
                if (toTexts[i] != null)
                    toTexts[i].color = new Color(toTexts[i].color.r, toTexts[i].color.g, toTexts[i].color.b, animationPart2_Text); ;

            // Asignar los estados a los paneles
            fromPanel.SetActive(interpolatedAnimationTime < 1);
            toPanel.SetActive(interpolatedAnimationTime > 1);

            if (loadingAnimation) {
                if (interpolatedAnimationTime > 1) {
                    if (leaveScene == "QUITGAME") {
                        if (Application.isEditor) {
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#endif
                        }
                        else {
                            Application.Quit();
                        }
                    }
                    else {
                        SceneManager.LoadScene(leaveScene);
                    }
                }
            }

            if (interpolatedAnimationTime > 2) {

                // Seleccionar el ultimo boton seleccionado
                // TODO: Revisar, en algunos paneles no hace falta guardar la ultima posicion
                if (toPanel.GetComponent<BackListener>().lastSelected != null)
                    toPanel.GetComponent<BackListener>().lastSelected.Select();

                if (!backPedal)
                    toPanel.GetComponent<BackListener>().fromPanel = fromPanel;

                prevPanel = fromPanel;
                fromPanel = toPanel;

                isAnimating = false;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="panel"></param>
    public void StartAnimation(GameObject panel) {
        // Asignar el siguiente panel
        toPanel = panel;

        // Asignar las variables de la animacion
        isAnimating = true;
        animationTime = 0;
        interpolatedAnimationTime = 0;

        fromTexts.Clear();
        toTexts.Clear();
        fromImages.Clear();
        toImages.Clear();
        toButtons.Clear();

        // Obtener todos los componentes
        Text[] allChildren = fromPanel.GetComponentsInChildren<Text>();
        fromTexts = new List<Text>(allChildren);
        allChildren = toPanel.GetComponentsInChildren<Text>();
        toTexts = new List<Text>(allChildren);
        Image[] allImages = fromPanel.GetComponentsInChildren<Image>();
        fromImages = new List<Image>(allImages);
        allImages = toPanel.GetComponentsInChildren<Image>();
        toImages = new List<Image>(allImages);
        Button[] allButtons = toPanel.GetComponentsInChildren<Button>();
        toButtons = new List<Button>(allButtons);

        // Sonido de cambio de panel
        ClipManager.CreateClip(Resources.Load("Audio/SFX/Select") as AudioClip, 1.0f, 1.0f);

        // Seleccionar el boton seleccionado en el panel anterior
        fromPanel.GetComponent<BackListener>().lastSelected = eventSystem.currentSelectedGameObject.GetComponent<Button>();

        loadingAnimation = false;
        backPedal = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="panel"></param>
    public void StartAnimationLoadingScreen(GameObject panel) {
        // Asignar el siguiente panel
        toPanel = panel;

        // Asignar las variables de la animacion
        isAnimating = true;
        animationTime = 0;
        interpolatedAnimationTime = 0;

        fromTexts.Clear();
        toTexts.Clear();
        fromImages.Clear();
        toImages.Clear();
        toButtons.Clear();

        // Obtener todos los componentes
        Text[] allChildren = fromPanel.GetComponentsInChildren<Text>();
        fromTexts = new List<Text>(allChildren);
        allChildren = toPanel.GetComponentsInChildren<Text>();
        toTexts = new List<Text>(allChildren);
        Image[] allImages = fromPanel.GetComponentsInChildren<Image>();
        fromImages = new List<Image>(allImages);
        allImages = toPanel.GetComponentsInChildren<Image>();
        toImages = new List<Image>(allImages);
        Button[] allButtons = toPanel.GetComponentsInChildren<Button>();
        toButtons = new List<Button>(allButtons);

        // Sonido de cambio de panel
        ClipManager.CreateClip(Resources.Load("Audio/SFX/Select") as AudioClip, 1.0f, 1.0f);

        // Seleccionar el boton seleccionado en el panel anterior
        fromPanel.GetComponent<BackListener>().lastSelected = eventSystem.currentSelectedGameObject.GetComponent<Button>();

        loadingAnimation = true;
        backPedal = false;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="panel"></param>
    public void StartAnimationBackPedal(GameObject panel) {
        // Asignar el siguiente panel
        toPanel = panel;

        // Asignar las variables de la animacion
        isAnimating = true;
        animationTime = 0;
        interpolatedAnimationTime = 0;

        fromTexts.Clear();
        toTexts.Clear();
        fromImages.Clear();
        toImages.Clear();
        toButtons.Clear();

        // Obtener todos los componentes
        Text[] allChildren = fromPanel.GetComponentsInChildren<Text>();
        fromTexts = new List<Text>(allChildren);
        allChildren = toPanel.GetComponentsInChildren<Text>();
        toTexts = new List<Text>(allChildren);
        Image[] allImages = fromPanel.GetComponentsInChildren<Image>();
        fromImages = new List<Image>(allImages);
        allImages = toPanel.GetComponentsInChildren<Image>();
        toImages = new List<Image>(allImages);
        Button[] allButtons = toPanel.GetComponentsInChildren<Button>();
        toButtons = new List<Button>(allButtons);

        // Sonido de cambio de panel
        ClipManager.CreateClip(Resources.Load("Audio/SFX/Select") as AudioClip, 1.0f, 1.0f);

        // Seleccionar el boton seleccionado en el panel anterior
        fromPanel.GetComponent<BackListener>().lastSelected = eventSystem.currentSelectedGameObject.GetComponent<Button>();

        loadingAnimation = false;
        backPedal = true;

    }

    /// <summary>
    /// 
    /// </summary>
    public void StartAnimationPevious() {
        // Asignar el siguiente panel
        toPanel = prevPanel;

        // Asignar las variables de la animacion
        isAnimating = true;
        animationTime = 0;
        interpolatedAnimationTime = 0;

        fromTexts.Clear();
        toTexts.Clear();
        fromImages.Clear();
        toImages.Clear();
        toButtons.Clear();

        // Obtener todos los componentes
        Text[] allChildren = fromPanel.GetComponentsInChildren<Text>();
        fromTexts = new List<Text>(allChildren);
        allChildren = toPanel.GetComponentsInChildren<Text>();
        toTexts = new List<Text>(allChildren);
        Image[] allImages = fromPanel.GetComponentsInChildren<Image>();
        fromImages = new List<Image>(allImages);
        allImages = toPanel.GetComponentsInChildren<Image>();
        toImages = new List<Image>(allImages);
        Button[] allButtons = toPanel.GetComponentsInChildren<Button>();
        toButtons = new List<Button>(allButtons);

        // Sonido de cambio de panel
        ClipManager.CreateClip(Resources.Load("Audio/SFX/Select") as AudioClip, 1.0f, 1.0f);

        // Seleccionar el boton seleccionado en el panel anterior
        fromPanel.GetComponent<BackListener>().lastSelected = eventSystem.currentSelectedGameObject.GetComponent<Button>();

        loadingAnimation = false;
        backPedal = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public void SetLeaveScene(string name) {
        leaveScene = name;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
