using UnityEngine;
using System.Collections;

public class GameSettings : MonoBehaviour {

    // Pausa
    public static bool isPaused;

    public static void PauseToggle() {
        if (isPaused) {
            isPaused = false;
            Time.timeScale = 1.0f;
        }
        else if (!isPaused) {
            isPaused = true;
            Time.timeScale = 0.0f;
        }
    }

    public static void PauseToggle(bool state) {
        isPaused = state;
        if (isPaused)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }
}
