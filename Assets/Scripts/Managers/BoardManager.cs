using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {

    [Header("[ REFERENCIAS ]")]
    public Text txtCountDown;
    public Text txtInformation;
    public GameObject checkersParent;

    private bool gotRaceInfo;
    public string gameMode;

    public void UpdateCountdown(string text) {
        txtCountDown.text = text;
    }

    void FixedUpdate() {
        // Obteber la informacion de la carrera
        if (!gotRaceInfo) {

            gotRaceInfo = true;
        }

        // Actualizar la informacion
        txtInformation.text = gameMode;

        if (RaceSettings.ships[0].currentLap > 0 && RaceSettings.ships[0].currentLap < RaceSettings.laps - 1)
            txtCountDown.text = "LAP " + (RaceSettings.ships[0].currentLap + 1);

        if (RaceSettings.ships[0].currentLap == RaceSettings.laps - 1)
            txtCountDown.text = "FINAL!";

        if (RaceSettings.ships[0].currentLap == RaceSettings.laps)
            txtCountDown.text = "FINISH!";
    }
}
