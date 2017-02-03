using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona las distintas opciones que se pueden elegir en los menús
/// </summary>
public class MenuEventManager : MonoBehaviour {

    public MenuAnimationManager menuAnimation;

    // Use this for initialization
    void Start() {
        GameOptions.LoadGameSettings();
        GameSettings.SetScreen();

        // Carga del idioma
        LanguageSingleton.InstanceLanguage();
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// Carga la escena del menú principal
    /// </summary>
    public void LoadMenu() {
        menuAnimation.SetLeaveScene("Menu");
    }

    /// <summary>
    /// Carga la escena del menú multijugador
    /// </summary>
    public void LoadOnline() {
        menuAnimation.SetLeaveScene("Online");
    }

    /// <summary>
    /// Carga la escena de la pantalla de carga
    /// </summary>
    public void StartRace() {
        ServerSettings.isNetworked = false;
        menuAnimation.SetLeaveScene("LoadingScreen");
    }

    /// <summary>
    /// Inicia una carrera rápida con valores aleatorios
    /// </summary>
    public void StartQuickRace() {
        RaceSettings.playerShip = (Enumerations.E_SHIPS)UnityEngine.Random.Range(0, (float)Enum.GetValues(typeof(Enumerations.E_SHIPS)).Cast<Enumerations.E_SHIPS>().Max());
        int track = UnityEngine.Random.Range(0, 3);
        switch (track) {
            case 0:
                RaceSettings.trackToLoad = "Test";
                break;
            case 1:
                RaceSettings.trackToLoad = "Blood Dragon";
                break;
            case 2:
                RaceSettings.trackToLoad = "Volcano";
                break;
        }
        StartRace();
    }

    /// <summary>
    /// Inicia una carrera multijugador
    /// </summary>
    public void StartRaceMP() {
        ServerSettings.isNetworked = true;
    }

    /// <summary>
    /// Cierra el juego
    /// </summary>
    public void Quit() {
        if (Application.isEditor) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else {
            Application.Quit();
        }
    }

    /// <summary>
    /// Establece el circuito a cargar
    /// </summary>
    /// <param name="sceneName"></param>
    public void SetTrackToLoad(string sceneName) {
        RaceSettings.trackToLoad = sceneName;
    }

    /// <summary>
    /// Establece la dificultad de la carrera
    /// </summary>
    /// <param name="difficulty"></param>
    public void SetDifficulty(int difficulty) {
    }

    /// <summary>
    /// Establece la dificultad de la carrera
    /// </summary>
    /// <param name="difficulty"></param>
    public void SetLaps(int laps) {
        RaceSettings.laps = laps;
    }

    /// <summary>
    /// Establece la dificultad de la carrera
    /// </summary>
    /// <param name="difficulty"></param>
    public void SetPlayers(int players) {
        if (RaceSettings.gamemode != Enumerations.E_GAMEMODE.TIMETRIAL)
            RaceSettings.racers = players;
    }

    /// <summary>
    /// Establece el modo de juego
    /// </summary>
    /// <param name="mode"></param>
    public void SetGameMode(int mode) {
        RaceSettings.gamemode = (Enumerations.E_GAMEMODE)mode;
        if (RaceSettings.gamemode == Enumerations.E_GAMEMODE.ARCADE)
            RaceSettings.racers = 8;
        else
            RaceSettings.racers = 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    public void SetOverrideRaceSettings(bool state) {
        RaceSettings.overrideRaceSettings = state;
    }


    /// <summary>
    /// Establece la nave a cargar
    /// </summary>
    /// <param name="ship"></param>
    /// <param name="updatePlayerReference"></param>
    public void SetShip(Enumerations.E_SHIPS ship, bool updatePlayerReference) {
        if (updatePlayerReference)
            RaceSettings.playerShip = ship;
    }
}
