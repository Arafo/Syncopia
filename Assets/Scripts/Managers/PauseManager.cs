using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Continue() {
        RaceSettings.raceManager.Pause();
    }

    public void Restart() {
    }

    public void Options() {
    }

    public void Quit() {
        Time.timeScale = 1.0f;
        GameSettings.isPaused = false;
        SceneManager.LoadScene("Menu");
    }
}
