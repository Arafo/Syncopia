using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;



public class PauseManager : MonoBehaviour {

    [Header("[ RESULTADOS ]")]
    public Text positions;
    public Text drivers;
    public Text ships;
    public Text bestTimes;
    public Text totalTimes;

    [Header("[ GRAFICOS ]")]
    public Dropdown dropResolution;
    public Slider sliderDrawDistance;
    public Text txtDrawDistance;
    public Slider sliderFramerate;
    public Text txtFramerate;
    public Toggle tglBloom;
    public Toggle tglFXAA;

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

    private Resolution[] resolutions = new Resolution[0];

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

    public void Results() {
        
        int position = 1;

        ResetResults();

        for (int i = 0; i < RaceSettings.ships.Count; i++) {
            ShipReferer ship = RaceSettings.ships[i];
            if (ship.finalPosition == position) {
                if (!ship.isAI) {
                    positions.text += "\n<color=yellow>" + ship.finalPosition + "</color>";
                    ships.text += "\n<color=yellow>PLACEHOLDER" + ship.finalPosition + "</color>";
                    drivers.text += "\n<color=yellow>PLACEHOLDER" + ship.finalPosition + "</color>";
                    bestTimes.text += "\n<color=yellow>" + ToTime(ship.bestLap) + "</color>";
                    totalTimes.text += "\n<color=yellow>" + ToTime(ship.totalTime) + "</color>";
                }
                else {
                    positions.text += "\n" + ship.finalPosition;
                    ships.text += "\nPLACEHOLDER" + ship.finalPosition;
                    drivers.text += "\nPLACEHOLDER" + ship.finalPosition;
                    bestTimes.text += "\n" + ToTime(ship.bestLap);
                    totalTimes.text += "\n" + ToTime(ship.totalTime);
                }
                position++;

                if (position <= RaceSettings.ships.Count)
                    i = -1;
            }
        }
    }

    public void Continue() {
        RaceSettings.raceManager.Pause();
    }

    public void Restart() {
        GameSettings.PauseToggle(false);
        SceneManager.LoadScene("Test");
    }

    public void Quit() {
        Time.timeScale = 1.0f;
        GameSettings.isPaused = false;
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

    public void ResetResults() {
        positions.text = "POS.";
        ships.text = "DRIVER";
        drivers.text = "SHIP";
        bestTimes.text = "BEST";
        totalTimes.text = "TOTAL";
    }

    public void InitGraphicsMenu() {
        GameOptions.LoadGameSettings();

        GetResolutions();
        SetResolutionDropDown();

        sliderFramerate.value = GameSettings.GS_FRAMECAP;
        sliderDrawDistance.value = GameSettings.GS_DRAWDISTANCE;
        tglBloom.isOn = GameSettings.GS_BLOOM;
        tglFXAA.isOn = GameSettings.GS_FXAA;
    }

    public void InitAudioMenu() {
        GameOptions.LoadGameSettings();

        sliderMasterVolume.value = AudioSettings.VOLUME_MAIN;
        sliderSFXVolume.value = AudioSettings.VOLUME_SFX;
        sliderMusicVolume.value = AudioSettings.VOLUME_MUSIC;
        sliderVoiceVolume.value = AudioSettings.VOLUME_VOICES;
        tglCustomMusic.isOn = AudioSettings.customMusicEnabled;
    }

    public void InitGameplayMenu() {
        GameOptions.LoadGameSettings();

        dropDefaultCamera.value = GameSettings.G_DEFAULTCAMERA;
        dropCountdownType.value = GameSettings.G_COUNTDOWNTYPE;

        //tglIntroVoices.SetState(GameSettings.G_TRACKINTROVOICES);
        //tglMirror.SetState(GameSettings.G_MIRROR);

        sliderHUDR.value = GameSettings.G_CUSTOMHUDCOLOR.r;
        sliderHUDG.value = GameSettings.G_CUSTOMHUDCOLOR.g;
        sliderHUDB.value = GameSettings.G_CUSTOMHUDCOLOR.b;
        sliderHUDA.value = GameSettings.G_CUSTOMHUDCOLOR.a;
    }

    public void UpdateGraphicsSettings() {
        if (resolutions.Length > 0) {
            GameOptions.LoadGameSettings();

            GameSettings.GS_RESOLUTION.x = Mathf.RoundToInt(resolutions[dropResolution.value].width);
            GameSettings.GS_RESOLUTION.y = Mathf.RoundToInt(resolutions[dropResolution.value].height);
            GameSettings.GS_FRAMECAP = Mathf.RoundToInt(sliderFramerate.value);
            GameSettings.CapFPS(GameSettings.GS_FRAMECAP);
            GameSettings.GS_DRAWDISTANCE = Mathf.RoundToInt(sliderDrawDistance.value);
            GameSettings.GS_BLOOM = tglBloom.isOn;
            GameSettings.GS_FXAA = tglFXAA.isOn;

            Screen.SetResolution(Mathf.RoundToInt(GameSettings.GS_RESOLUTION.x), Mathf.RoundToInt(GameSettings.GS_RESOLUTION.y), GameSettings.GS_FULLSCREEN);

            GameOptions.SaveGameSettings();
        }
    }

    public void UpdateAudioSettings() {
        GameOptions.LoadGameSettings();

        AudioSettings.VOLUME_MAIN = sliderMasterVolume.value;
        AudioSettings.VOLUME_SFX = sliderSFXVolume.value;
        AudioSettings.VOLUME_MUSIC = sliderMusicVolume.value;
        AudioSettings.VOLUME_VOICES = sliderVoiceVolume.value;
        AudioSettings.customMusicEnabled = tglCustomMusic.isOn;

        GameOptions.SaveGameSettings();
    }

    public void UpdateGameplaySettings() {
        GameOptions.LoadGameSettings();

        // dropdowns
        GameSettings.G_DEFAULTCAMERA = dropDefaultCamera.value;
        GameSettings.G_COUNTDOWNTYPE = dropCountdownType.value;

        // toggles
        //GameSettings.G_TRACKINTROVOICES = tglIntroVoices.toggled;
        //GameSettings.G_MIRROR = tglMirror.;

        // sliders
        GameSettings.G_CUSTOMHUDCOLOR.r = sliderHUDR.value;
        GameSettings.G_CUSTOMHUDCOLOR.g = sliderHUDG.value;
        GameSettings.G_CUSTOMHUDCOLOR.b = sliderHUDB.value;
        GameSettings.G_CUSTOMHUDCOLOR.a = sliderHUDA.value;

        GameOptions.SaveGameSettings();
    }

    public void GetResolutions() {
        resolutions = Screen.resolutions;
        List<Dropdown.OptionData> od = new List<Dropdown.OptionData>();

        for (int i = 0; i < resolutions.Length; ++i)
            od.Add(new Dropdown.OptionData(string.Format("{0}x{1}", resolutions[i].width, resolutions[i].height)));

        dropResolution.options = od;
    }

    public void SetResolutionDropDown() {
        int val = 0;
        for (int i = 0; i < resolutions.Length; ++i) {
            int width = Mathf.RoundToInt(GameSettings.GS_RESOLUTION.x);
            int height = Mathf.RoundToInt(GameSettings.GS_RESOLUTION.y);

            if (resolutions[i].width == width && resolutions[i].height == height)
                val = i;
        }
        dropResolution.value = val;
    }

    public void UpdateFramerate() {
        GameSettings.GS_FRAMECAP = Mathf.RoundToInt(sliderFramerate.value);
        txtFramerate.text = GameSettings.GS_FRAMECAP.ToString();

        UpdateGraphicsSettings();
    }

    public void UpdateDrawDistanceValue() {
        GameSettings.GS_DRAWDISTANCE = Mathf.RoundToInt(sliderDrawDistance.value);
        txtDrawDistance.text = GameSettings.GS_DRAWDISTANCE.ToString();

        UpdateGraphicsSettings();
    }

    public void UpdateVolumes() {
        txtMasterVolume.text = System.Math.Round(sliderMasterVolume.value, 2).ToString();
        txtSFXVolume.text = System.Math.Round(sliderSFXVolume.value, 2).ToString();
        txtMusicVolume.text = System.Math.Round(sliderMusicVolume.value, 2).ToString();
        txtVoiceVolume.text = System.Math.Round(sliderVoiceVolume.value, 2).ToString();

        UpdateAudioSettings();
    }

    private string ToTime(float time) {
        string newString = (Mathf.Floor(time / 60)).ToString("00") + ":" +
                    (Mathf.Floor(time) % 60).ToString("00") + "." +
                        Mathf.Floor((time * 1000) % 1000).ToString("000");
        return newString;
    }
}
