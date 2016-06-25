using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuEventManager : MonoBehaviour {

    // Use this for initialization
    void Start() {
        GameOptions.LoadGameSettings();
    }

    // Update is called once per frame
    void Update() {

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
    /// Establce la dificultad de la carrera
    /// </summary>
    /// <param name="difficulty"></param>
    public void SetDifficulty(int difficulty) {
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
