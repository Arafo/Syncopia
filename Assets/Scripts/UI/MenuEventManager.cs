﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuEventManager : MonoBehaviour {

    public MenuAnimationManager menuAnimation;

    // Use this for initialization
    void Start() {
        GameOptions.LoadGameSettings();
    }

    // Update is called once per frame
    void Update() {

    }

    public void StartRace() {
        ServerSettings.isNetworked = false;
        menuAnimation.SetLeaveScene("LoadingScreen");
    }

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
        RaceSettings.racers = players;
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