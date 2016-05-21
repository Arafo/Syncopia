using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {

    [Header("[ RESULTADOS ]")]
    public Text positions;
    public Text drivers;
    public Text ships;
    public Text bestTimes;
    public Text totalTimes;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Results() {
        int position = 1;

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

    public void Options() {
    }

    public void Quit() {
        Time.timeScale = 1.0f;
        GameSettings.isPaused = false;
        SceneManager.LoadScene("Menu");
    }

    private string ToTime(float time) {
        string newString = (Mathf.Floor(time / 60)).ToString("00") + ":" +
                    (Mathf.Floor(time) % 60).ToString("00") + "." +
                        Mathf.Floor((time * 1000) % 1000).ToString("000");
        return newString;
    }
}
