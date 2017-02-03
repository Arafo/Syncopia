using UnityEngine;
using UnityEngine.UI;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Se ocupa de controlar el contenido de los letreros 
/// que indican las vueltas que faltan en una carrera
/// </summary>
public class BoardManager : MonoBehaviour {

    [Header("[ REFERENCIAS ]")]
    public Text txtCountDown;
    public Text txtInformation;
    public GameObject checkersParent;

    private bool gotRaceInfo;
    public string gameMode;

    /// <summary>
    /// Actualiza el texto del panel
    /// </summary>
    /// <param name="text"></param>
    public void UpdateCountdown(string text) {
        txtCountDown.text = text;
    }

    /// <summary>
    /// Comprueba y actualiza la información del panel
    /// </summary>
    void FixedUpdate() {
        // Obteber la informacion de la carrera
        if (!gotRaceInfo) {

            gotRaceInfo = true;
        }

        // Actualizar la informacion
        txtInformation.text = gameMode;

        if (RaceSettings.ships.Count <= 0)
            return;

        if (RaceSettings.ships[0].currentLap > 0 && RaceSettings.ships[0].currentLap < RaceSettings.laps - 1)
            txtCountDown.text = "LAP " + (RaceSettings.ships[0].currentLap + 1);

        if (RaceSettings.ships[0].currentLap == RaceSettings.laps - 1)
            txtCountDown.text = "FINAL!";

        if (RaceSettings.ships[0].currentLap == RaceSettings.laps)
            txtCountDown.text = "FINISH!";
    }
}
