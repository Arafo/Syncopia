﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityStandardAssets.ImageEffects;
using UnityEngine.EventSystems;
using Rewired.UI.ControlMapper;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona la pantalla de resultados que aparece 
/// al terminar una partida y el cambio de todas las 
/// opciones que se pueden realizar en el menú de pausa
/// </summary>
public class PauseManager : MonoBehaviour {

    public Text countdown;

    [Header("[ RESULTADOS ]")]
    public Text positions;
    public Text drivers;
    public Text ships;
    public Text bestTimes;
    public Text totalTimes;
    public Button restart;
    public Button exit;

    [Header("[ GRAFICOS ]")]
    //public Dropdown dropResolution;
    public HorizontallScrollSlider sliderFullScreen;
    public HorizontallScrollSlider sliderResolution;
    public Slider sliderDrawDistance;
    public Text txtDrawDistance;
    public HorizontallScrollSlider sliderFramerate;
    //public Text txtFramerate;
    public HorizontallScrollSlider sliderBloom;
    public HorizontallScrollSlider sliderAA;
    public HorizontallScrollSlider sliderDynamicResolution;

    [Header("[ AUDIO ]")]
    public Slider sliderMasterVolume;
    public Text txtMasterVolume;
    public Slider sliderSFXVolume;
    public Text txtSFXVolume;
    public Slider sliderMusicVolume;
    public Text txtMusicVolume;
    public Slider sliderVoiceVolume;
    public Text txtVoiceVolume;
    public Toggle tglCustomMusic;

    [Header("[ GAMEPLAY ]")]
    public GameObject gameplayWindow;
    public Image gameplayHUDColor;
    public Dropdown dropHudType;
    public Dropdown dropDefaultCamera;
    public Dropdown dropCountdownType;
    public Dropdown dropWeaponAnnouncement;
    public Button tglIntroVoices;
    public Button tglMirror;
    public HorizontallScrollSlider sliderLanguage;

    [Header("[ COLOR HUD ]")]
    public GameObject HUDColorWindow;
    public Image HUDColorVisual;
    public Slider sliderHUDR;
    public Slider sliderHUDG;
    public Slider sliderHUDB;
    public Slider sliderHUDA;
    public Text txtHUDR;
    public Text txtHUDG;
    public Text txtHUDB;
    public Text txtHUDA;

    public MenuAnimationManager animation;

    private Resolution[] resolutions = new Resolution[0];

    private int position = 1;

    public ControlMapper cm;

    private int score;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (gameplayWindow != null && gameplayWindow.activeSelf) {
            txtHUDR.text = Math.Round(sliderHUDR.value, 2).ToString();
            txtHUDG.text = Math.Round(sliderHUDG.value, 2).ToString();
            txtHUDB.text = Math.Round(sliderHUDB.value, 2).ToString();
            txtHUDA.text = Math.Round(sliderHUDA.value, 2).ToString();
            gameplayHUDColor.color = new Color(sliderHUDR.value, sliderHUDG.value, sliderHUDB.value, sliderHUDA.value);
        }
    }

    /// <summary>
    /// Muestra los resultados
    /// </summary>
    public void Results() {
        
        //int position = 1;

        ResetResults();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(restart.gameObject);

        // Blur de fondo
        RaceSettings.ships[0].cam.GetComponent<Blur>().enabled = true;

        if (RaceSettings.gamemode == Enumerations.E_GAMEMODE.ARCADE) {
            List<ShipReferer> shipList = RaceSettings.ships.OrderBy(o => o.finalPosition).ToList();
            for (int i = 0; i < shipList.Count; i++) {
                ShipReferer ship = shipList[i];
                //if (ship.finalPosition == position) {
                    if (!ship.isAI) {
                        positions.text += "\n<color=yellow>" + position + "</color>";
                        position++;
                        ships.text += "\n<color=yellow>" + ship.name + "</color>";
                        drivers.text += "\n<color=yellow>" + ship.name + "</color>";
                        bestTimes.text += "\n<color=yellow>" + ToTime(ship.bestLap) + "</color>";
                        totalTimes.text += "\n<color=yellow>" + ToTime(ship.totalTime) + "</color>";
                    }
                    else {
                        if (ship.finalPosition != int.MaxValue) {
                            positions.text += "\n" + position;
                            position++;
                            ships.text += "\n" + ship.name;
                            drivers.text += "\n" + ship.name;
                            bestTimes.text += "\n" + ToTime(ship.bestLap);
                            totalTimes.text += "\n" + ToTime(ship.totalTime);
                        }
                    }

                    //if (position <= RaceSettings.ships.Count)
                        //i = -1;
                //}
            }
        }
        else if (RaceSettings.gamemode == Enumerations.E_GAMEMODE.TIMETRIAL) {
            positions.text = "LAP";
            drivers.text = "SECTOR 1";
            ships.text = "SECTOR 2";
            bestTimes.text = "TIME";
            totalTimes.text = "";

            ShipReferer ship = RaceSettings.ships[0];
            for (int i = 0; i < ship.laps.Length; i++) {
                if (ship.laps[i] == ship.bestLap) {
                    positions.text += "\n<color=yellow>" + (i + 1) + "</color>";
                    drivers.text += "\n<color=yellow>" + ToTime(ship.sector1[i]) + "</color>";
                    ships.text += "\n<color=yellow>" + ToTime(ship.sector2[i]) + "</color>";
                    bestTimes.text += "\n<color=yellow>" + ToTime(ship.laps[i]) + "</color>";
                }
                else {
                    positions.text += "\n" + (i + 1);
                    drivers.text += "\n" + ToTime(ship.sector1[i]);
                    ships.text += "\n" + ToTime(ship.sector2[i]);
                    bestTimes.text += "\n" + ToTime(ship.laps[i]);
                }
            }
        }
    }

    /// <summary>
    /// Cuando una nave de la IA acaba la carrera se muestran
    /// sus datos
    /// </summary>
    /// <param name="ship"></param>
    public void AIFinished(ShipReferer ship) {
        if (RaceSettings.ships[0].finished && ship.isAI) {
            positions.text += "\n" + position;
            position++;
            ships.text += "\n" + ship.name;
            drivers.text += "\n" + ship.name;
            bestTimes.text += "\n" + ToTime(ship.bestLap);
            totalTimes.text += "\n" + ToTime(ship.totalTime);
        }
    }

    /// <summary>
    /// Continuar la partida pausada
    /// </summary>
    public void Continue() {
        RaceSettings.raceManager.Pause();
    }

    /// <summary>
    /// Reiniciar la partida
    /// </summary>
    public void Restart() {
        GameSettings.PauseToggle(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Reinicia el menu de pausa poniendo el primer panel en pantalla
    /// </summary>
    public void ResetMenu() {       
        transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(false);
        animation.StartAnimationNoSound(transform.GetChild(0).transform.GetChild(0).gameObject);
    }

    /// <summary>
    /// Salir de una partida
    /// </summary>
    public void Quit() {
        Time.timeScale = 1.0f;
        GameSettings.isPaused = false;
        if (ServerSettings.isNetworked) {
            //LobbyManager.s_Singleton.ServerReturnToLobby();
            LobbyManager.s_Singleton.StopHost();
            //LobbyManager.s_Singleton.StopClient();
            ServerSettings.players.Clear();
            //LobbyManager.s_Singleton.mainMenuPanel.gameObject.SetActive(true);
            SceneManager.LoadScene("Online");
            LobbyManager.s_Singleton.menuManager.StartAnimation(LobbyManager.s_Singleton.mainMenuPanel.gameObject);
        }
        else
            SceneManager.LoadScene("Menu");

    }

    public void InitParcialResults() {
        List<ShipReferer> aux = RaceSettings.ships;
        aux = aux.OrderBy(lst => lst.currentPosition).ToList();

        positions.text = "POS.";
        ships.text = "DRIVER";
        drivers.text = "SHIP";
        bestTimes.text = "BEST";
        totalTimes.text = "TOTAL";

        for (int i = 0; i < aux.Count; i++) {
            ShipReferer ship = aux[i];
            if (!ship.isAI) {
                positions.text += "\n<color=yellow>" + ship.currentPosition + "</color>";
                ships.text += "\n<color=yellow>PLACEHOLDER" + ship.currentPosition + "</color>";
                drivers.text += "\n<color=yellow>PLACEHOLDER" + ship.currentPosition + "</color>";
                bestTimes.text += "\n<color=yellow>" + ToTime(ship.bestLap) + "</color>";
                totalTimes.text += "\n<color=yellow>" + ToTime(ship.totalTime) + "</color>";
            }
            else {
                positions.text += "\n" + ship.currentPosition;
                ships.text += "\nPLACEHOLDER" + ship.currentPosition;
                drivers.text += "\nPLACEHOLDER" + ship.currentPosition;
                bestTimes.text += "\n" + ToTime(ship.bestLap);
                totalTimes.text += "\n" + ToTime(ship.totalTime);
            }         
        }
    }

    /// <summary>
    /// Reinicia el texto de la pantalla de resultados
    /// </summary>
    public void ResetResults() {
        positions.text = "POS.";
        ships.text = "DRIVER";
        drivers.text = "SHIP";
        bestTimes.text = "BEST";
        totalTimes.text = "TOTAL";
    }

    /// <summary>
    /// Inicia el menú de opcines gráficas
    /// </summary>
    public void InitGraphicsMenu() {
        GameOptions.LoadGameSettings();

        GetResolutions();
        SetResolutionDropDown();

        score = GameSettings.AutoQualitySettings();
        if (score <= 24000) {
            if (sliderAA.hideContent == null)
                sliderAA.hideContent = new List<string>();
            sliderAA.hideContent.Add(Enumerations.E_AA.MSAAx8.ToDescription());
            sliderAA.hideContent.Add(Enumerations.E_AA.MSAAx8TAA.ToDescription());
            sliderAA.hideContent.Add(Enumerations.E_AA.TAAx32.ToDescription());
            sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx8.ToDescription());
        }

        if (score <= 12000) {
            if (sliderAA.hideContent == null)
                sliderAA.hideContent = new List<string>();
            sliderAA.hideContent.Add(Enumerations.E_AA.MSAAx4.ToDescription());
            sliderAA.hideContent.Add(Enumerations.E_AA.MSAAx4TAA.ToDescription());
            sliderAA.hideContent.Add(Enumerations.E_AA.TAASharpening.ToDescription());
            sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx4.ToDescription());
        }

        if (GameSettings.GS_FRAMECAP == -1)
            sliderFramerate.firstIndex = 0;
        else if (GameSettings.GS_FRAMECAP == 30)
            sliderFramerate.firstIndex = 1;
        else if (GameSettings.GS_FRAMECAP == 60)
            sliderFramerate.firstIndex = 2;
        else
            sliderFramerate.firstIndex = 0;

        sliderFullScreen.firstIndex = GameSettings.GS_FULLSCREEN == true ? 1 : 0;
        sliderFullScreen.index = GameSettings.GS_FULLSCREEN == true ? 1 : 0;

        sliderBloom.firstIndex = GameSettings.GS_BLOOM;
        sliderBloom.index = GameSettings.GS_BLOOM;
        sliderDynamicResolution.firstIndex = GameSettings.GS_DYNAMICRESOLUTION;
        sliderAA.firstIndex = GameSettings.GS_AA;
        // Con resolucion dinamica no se permite MSAA
        if (GameSettings.GS_DYNAMICRESOLUTION == 1) {
            if (sliderAA.hideContent == null)
                sliderAA.hideContent = new List<string>();
            if (!sliderAA.hideContent.Contains(Enumerations.E_AA.SSAAx2.ToDescription())) sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx2.ToDescription());
            if (!sliderAA.hideContent.Contains(Enumerations.E_AA.SSAAx4.ToDescription())) sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx4.ToDescription());
            if (!sliderAA.hideContent.Contains(Enumerations.E_AA.SSAAx8.ToDescription())) sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx8.ToDescription());

            if (GameSettings.GS_AA == (int)Enumerations.E_AA.SSAAx2 ||
                GameSettings.GS_AA == (int)Enumerations.E_AA.SSAAx4 ||
                GameSettings.GS_AA == (int)Enumerations.E_AA.SSAAx8)
                sliderAA.firstIndex = (int)Enumerations.E_AA.OFF;
        }

        // Importante que este valor se cambie el ultimo porque luego llama
        // aL metodo UpdateDrawDistanceValue() que actualiza las opciones
        sliderDrawDistance.value = GameSettings.GS_DRAWDISTANCE;
    }

    /// <summary>
    /// Inicia el menú de opcines de volumen
    /// </summary>
    public void InitAudioMenu() {
        GameOptions.LoadGameSettings();

        sliderMasterVolume.value = AudioSettings.VOLUME_MAIN;
        sliderSFXVolume.value = AudioSettings.VOLUME_SFX;
        sliderMusicVolume.value = AudioSettings.VOLUME_MUSIC;
        sliderVoiceVolume.value = AudioSettings.VOLUME_VOICES;
        //tglCustomMusic.isOn = AudioSettings.customMusicEnabled;
    }

    /// <summary>
    /// Inicia el menú de opcines de gameplay
    /// </summary>
    public void InitGameplayMenu() {
        GameOptions.LoadGameSettings();

        //dropDefaultCamera.value = GameSettings.G_DEFAULTCAMERA;
        //dropCountdownType.value = GameSettings.G_COUNTDOWNTYPE;

        //tglIntroVoices.SetState(GameSettings.G_TRACKINTROVOICES);
        //tglMirror.SetState(GameSettings.G_MIRROR);

        sliderLanguage.firstIndex = (int)GameSettings.G_LANGUAGE;

        //sliderHUDR.value = GameSettings.G_CUSTOMHUDCOLOR.r;
        //sliderHUDG.value = GameSettings.G_CUSTOMHUDCOLOR.g;
        //sliderHUDB.value = GameSettings.G_CUSTOMHUDCOLOR.b;
        //sliderHUDA.value = GameSettings.G_CUSTOMHUDCOLOR.a;
    }

    /// <summary>
    /// Actualiza el menú de opcines gráficas
    /// </summary>
    public void UpdateGraphicsSettings() {
        if (resolutions.Length > 0) {
            GameOptions.LoadGameSettings();

            //SetResolutionIndex();
            GameSettings.GS_RESOLUTION.x = Mathf.RoundToInt(resolutions[sliderResolution.index].width);
            GameSettings.GS_RESOLUTION.y = Mathf.RoundToInt(resolutions[sliderResolution.index].height);

            if (sliderFramerate.listContent[sliderFramerate.index] == "OFF")
                GameSettings.GS_FRAMECAP = Mathf.RoundToInt(-1);
            else if (sliderFramerate.listContent[sliderFramerate.index] == "30")
                GameSettings.GS_FRAMECAP = Mathf.RoundToInt(30);
            else if (sliderFramerate.listContent[sliderFramerate.index] == "60")
                GameSettings.GS_FRAMECAP = Mathf.RoundToInt(60);
            GameSettings.CapFPS(GameSettings.GS_FRAMECAP);

            GameSettings.GS_FULLSCREEN = sliderFullScreen.index == 0 ? false : true;
            GameSettings.GS_DRAWDISTANCE = Mathf.RoundToInt(sliderDrawDistance.value);
            GameSettings.GS_BLOOM = sliderBloom.index;
            GameSettings.GS_AA = sliderAA.index;
            GameSettings.GS_DYNAMICRESOLUTION = sliderDynamicResolution.index;

            // Con resolucion dinamica no se permite MSAA
            if (GameSettings.GS_DYNAMICRESOLUTION == 1) {
                if (sliderAA.hideContent == null)
                    sliderAA.hideContent = new List<string>();
                if (!sliderAA.hideContent.Contains(Enumerations.E_AA.SSAAx2.ToDescription())) sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx2.ToDescription());
                if (!sliderAA.hideContent.Contains(Enumerations.E_AA.SSAAx4.ToDescription())) sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx4.ToDescription());
                if (!sliderAA.hideContent.Contains(Enumerations.E_AA.SSAAx8.ToDescription())) sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx8.ToDescription());

                if (GameSettings.GS_AA == (int)Enumerations.E_AA.SSAAx2 ||
                    GameSettings.GS_AA == (int)Enumerations.E_AA.SSAAx4 ||
                    GameSettings.GS_AA == (int)Enumerations.E_AA.SSAAx8)
                    // Se desactiva el AA
                    while (sliderAA.index != 0)
                        sliderAA.setNextText();
            }
            else {
                sliderAA.hideContent.Clear();
                if (score <= 24000) {
                    if (sliderAA.hideContent == null)
                        sliderAA.hideContent = new List<string>();
                    sliderAA.hideContent.Add(Enumerations.E_AA.MSAAx8.ToDescription());
                    sliderAA.hideContent.Add(Enumerations.E_AA.MSAAx8TAA.ToDescription());
                    sliderAA.hideContent.Add(Enumerations.E_AA.TAAx32.ToDescription());
                    sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx8.ToDescription());
                }

                if (score <= 12000) {
                    if (sliderAA.hideContent == null)
                        sliderAA.hideContent = new List<string>();
                    sliderAA.hideContent.Add(Enumerations.E_AA.MSAAx4.ToDescription());
                    sliderAA.hideContent.Add(Enumerations.E_AA.MSAAx4TAA.ToDescription());
                    sliderAA.hideContent.Add(Enumerations.E_AA.TAASharpening.ToDescription());
                    sliderAA.hideContent.Add(Enumerations.E_AA.SSAAx4.ToDescription());
                }
            }


            Screen.SetResolution(Mathf.RoundToInt(GameSettings.GS_RESOLUTION.x), Mathf.RoundToInt(GameSettings.GS_RESOLUTION.y), GameSettings.GS_FULLSCREEN);

            GameOptions.SaveGameSettings();
        }
    }

    /// <summary>
    /// Actualiza el menú de opcines de volumen
    /// </summary>
    public void UpdateAudioSettings() {
        GameOptions.LoadGameSettings();

        AudioSettings.VOLUME_MAIN = sliderMasterVolume.value;
        AudioSettings.VOLUME_SFX = sliderSFXVolume.value;
        AudioSettings.VOLUME_MUSIC = sliderMusicVolume.value;
        AudioSettings.VOLUME_VOICES = sliderVoiceVolume.value;
        //AudioSettings.customMusicEnabled = tglCustomMusic.isOn;

        GameOptions.SaveGameSettings();
    }

    /// <summary>
    /// Actualiza el menú de opcines de gameplay
    /// </summary>
    public void UpdateGameplaySettings() {
        GameOptions.LoadGameSettings();

        // dropdowns
        //GameSettings.G_DEFAULTCAMERA = dropDefaultCamera.value;
        //GameSettings.G_COUNTDOWNTYPE = dropCountdownType.value;

        // toggles
        //GameSettings.G_TRACKINTROVOICES = tglIntroVoices.toggled;
        //GameSettings.G_MIRROR = tglMirror.;

        // Idioma
        // Se suma 1 para tener en cuenta que este metodo se ejecuta antes que la actualizacion
        // del HorizontallScrollSlider. Esto se hace para que primero se cambie el idioma y luego
        // se actualize el texto
        GameSettings.G_LANGUAGE = (Enumerations.E_LANGUAGE)((sliderLanguage.index + 1) % Enum.GetNames(typeof(Enumerations.E_LANGUAGE)).Length);
        LanguageSingleton.InstanceLanguage(true);

        // sliders
        //GameSettings.G_CUSTOMHUDCOLOR.r = sliderHUDR.value;
        //GameSettings.G_CUSTOMHUDCOLOR.g = sliderHUDG.value;
        //GameSettings.G_CUSTOMHUDCOLOR.b = sliderHUDB.value;
        //GameSettings.G_CUSTOMHUDCOLOR.a = sliderHUDA.value;

        GameOptions.SaveGameSettings();
    }

    /// <summary>
    /// Obtiene las resoluciones disponibles de la pantlla 
    /// </summary>
    public void GetResolutions() {
        resolutions = Screen.resolutions;
        List<string> od = new List<string>();

        for (int i = 0; i < resolutions.Length; ++i) {
            od.Add(string.Format("{0}x{1}", resolutions[i].width, resolutions[i].height));
        }

        sliderResolution.listContent = od;
    }

    /// <summary>
    /// Inicia el valor de la resolución actual
    /// </summary>
    public void SetResolutionDropDown() {
        int val = 0;
        for (int i = 0; i < resolutions.Length; ++i) {
            int width = Mathf.RoundToInt(GameSettings.GS_RESOLUTION.x);
            int height = Mathf.RoundToInt(GameSettings.GS_RESOLUTION.y);

            if (resolutions[i].width == width && resolutions[i].height == height)
                val = i;
        }

        sliderResolution.firstIndex = val;
        sliderResolution.index = val;
    }

    /// <summary>
    /// Inicia el indice de la resolución actual
    /// </summary>
    public void SetResolutionIndex() {
        int val = 0;
        for (int i = 0; i < resolutions.Length; ++i) {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                val = i;
        }

        sliderResolution.index = val;
    }

    /// <summary>
    /// Actualiza el limitador de framerate
    /// </summary>
    public void UpdateFramerate() {
        if (sliderFramerate.listContent[sliderFramerate.index] == "OFF")
            GameSettings.GS_FRAMECAP = Mathf.RoundToInt(-1);
        else if (sliderFramerate.listContent[sliderFramerate.index] == "30")
            GameSettings.GS_FRAMECAP = Mathf.RoundToInt(30);
        else if (sliderFramerate.listContent[sliderFramerate.index] == "60")
            GameSettings.GS_FRAMECAP = Mathf.RoundToInt(60);
        //GameSettings.GS_FRAMECAP = Mathf.RoundToInt(sliderFramerate.value);
        //txtFramerate.text = GameSettings.GS_FRAMECAP.ToString();

        UpdateGraphicsSettings();
    }

    /// <summary>
    /// Actualiza el limitador de distancia de dibujado
    /// </summary>
    public void UpdateDrawDistanceValue() {
        GameSettings.GS_DRAWDISTANCE = Mathf.RoundToInt(sliderDrawDistance.value);
        txtDrawDistance.text = GameSettings.GS_DRAWDISTANCE.ToString();

        UpdateGraphicsSettings();
    }

    /// <summary>
    /// Actualiza el texto de todas las opciones de volumen
    /// </summary>
    public void UpdateVolumes() {
        txtMasterVolume.text = System.Math.Round(sliderMasterVolume.value, 2).ToString();
        txtSFXVolume.text = System.Math.Round(sliderSFXVolume.value, 2).ToString();
        txtMusicVolume.text = System.Math.Round(sliderMusicVolume.value, 2).ToString();
        txtVoiceVolume.text = System.Math.Round(sliderVoiceVolume.value, 2).ToString();

        UpdateAudioSettings();
    }

    /// <summary>
    /// Controla el lenguaje a cargar
    /// </summary>
    public void ControlMapperLanguage() {
        switch(GameSettings.G_LANGUAGE) {
            case Enumerations.E_LANGUAGE.ENGLISH:
                cm.language = Resources.Load("UI/DefaultEnglish") as LanguageData;
                break;
            case Enumerations.E_LANGUAGE.SPANISH:
                cm.language = Resources.Load("UI/DefaultSpanish") as LanguageData;
                break;
            //default:

        }
    }

    /// <summary>
    /// Transforma un float a texto en formato 00:00.000
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private string ToTime(float time) {
        string newString = (Mathf.Floor(time / 60)).ToString("00") + ":" +
                    (Mathf.Floor(time) % 60).ToString("00") + "." +
                        Mathf.Floor((time * 1000) % 1000).ToString("000");
        return newString;
    }
}
