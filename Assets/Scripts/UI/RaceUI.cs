using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceUI : ShipCore
{
    //public GameObject ship;
    //public ShipController ship.control;
    //public ShipSimulation ship.sim;

    // Raiz
    public GameObject uiRoot;

    // Mejor tiempo
    public GameObject uiBestTime;
    private Text uiBestTimeText;

    // Tiempo Actual
    public GameObject uiCurrentTime;
    private Text uiCurrentTimeText;

    // Tiempo total
    public GameObject uiTotalTime;
    private Text uiTotalTimeText;

    // Sector actuañ
    public GameObject uiCurrentSector;
    private Text uiCurrentSectorText;
    private bool displayCurrentSector;
    private float diff = 0f;

    // Vuelta actual
    public GameObject uiCurrentLap;
    private Text uiCurrentLapText;

    // Velocidad actual
    public GameObject uiCurrentSpeed;
    private Text uiCurrentSpeedText;

    // Posicion actual
    public GameObject uiCurrentPosition;
    private Text uiCurrentPositionText;

    //
    public GameObject uiSpeedMeter;
    private RectTransform uiSpeedMeterRect;

    // PowerBoost
    public GameObject uiPowerBoost;
    private RectTransform uiPowerBoostRect;
    private float powerBoostRectMax;

    //
    public GameObject uiLaps;
    private Text uiLapsText;

    // Cuenta atras
    public GameObject uiCountDown;
    public Text uiCountDownText;

    private float uiSize = 1f;
    private float uiOpacity = 1f;

    public delegate void OnVariableChangeDelegate(bool newVal);
    public event OnVariableChangeDelegate OnVariableChange;

    void Start() {
        uiBestTimeText = uiBestTime.GetComponent<Text>();
        uiCurrentTimeText = uiCurrentTime.GetComponent<Text>();
        uiCurrentSectorText = uiCurrentSector.GetComponent<Text>();
        //uiTotalTimeText = uiTotalTime.GetComponent<Text>();
        uiCurrentLapText = uiCurrentLap.GetComponent<Text>();
        uiCurrentSpeedText = uiCurrentSpeed.GetComponent<Text>();
        uiCurrentPositionText = uiCurrentPosition.GetComponent<Text>();
        uiSpeedMeterRect = uiSpeedMeter.GetComponent<RectTransform>();
        uiPowerBoostRect = uiPowerBoost.GetComponent<RectTransform>();
        powerBoostRectMax = uiPowerBoostRect.sizeDelta.x;
        uiCurrentSpeedText = uiCurrentSpeed.GetComponent<Text>();
        OnVariableChange += VariableChangeHandler;
        uiCurrentSectorText.enabled = false;
    }

    void Update() {
        // TODO: Iniciar en Start, no ejecutar esto cada frame
        uiCurrentSpeedText= uiCurrentSpeed.GetComponent<Text>();
        uiCurrentLapText = uiCurrentLap.GetComponent<Text>();
        uiSpeedMeterRect = uiSpeedMeter.GetComponent<RectTransform>();
        uiBestTimeText = uiBestTime.GetComponent<Text>();
        uiCurrentTimeText = uiCurrentTime.GetComponent<Text>();
        uiLapsText = uiLaps.GetComponent<Text>();
        uiCountDownText = uiCountDown.GetComponent<Text>();

        if (ship == null)
            return;

        if (!ship.finished)  {
            if (!ship.isNetworked) {
                if (ship.sim.isBoosting) {
                    uiSize = Mathf.Lerp(uiSize, 1.17f, Time.deltaTime * 7f);
                    uiOpacity = Mathf.Lerp(uiOpacity, 0.3f, Time.deltaTime * 10f);
                }
                else {
                    uiSize = Mathf.Lerp(uiSize, 1f, Time.deltaTime * 2f);
                    uiOpacity = Mathf.Lerp(uiOpacity, 1f, Time.deltaTime * 10f);
                }
            }

            uiRoot.transform.localScale = new Vector3(uiSize, uiSize, 1f);

            if (!ship.isNetworked) {
                float x2 = ship.sim.gameObject.transform.InverseTransformDirection(ship.body.velocity).z * 0.8f;
                Vector2 rectSize = new Vector2(x2, uiSpeedMeterRect.sizeDelta.y);
                uiSpeedMeterRect.sizeDelta = rectSize;
            }

            // PowerBoost
            float powerBoostX = ship.powerBoost * powerBoostRectMax / 100f;
            Vector2 powerBoostSize = new Vector2(powerBoostX, uiPowerBoostRect.sizeDelta.y);
            uiPowerBoostRect.sizeDelta = powerBoostSize;

            // Velocidad
            if (!ship.isNetworked) {
                int speed = Mathf.RoundToInt(ship.control.gameObject.GetComponent<ShipSimulation>().engineSpeed * 2f);
                uiCurrentSpeedText.text = speed.ToString() + " km/h";
            }

            // Numero de vueltas
            uiCurrentLapText.text = ship.currentLap.ToString() + "/" + RaceSettings.laps;

            // Tiempo total de la vuelta
            float timer = ship.currentTime;
            string minutes = Mathf.Floor(timer / 60).ToString("00");
            string seconds = Mathf.RoundToInt(timer % 60).ToString("00");
            string miliseconds = ((timer * 1000) % 1000).ToString("000");

            string total = string.Concat(new string[] { minutes, ":", seconds, ":", miliseconds });
            uiCurrentTimeText.text = "Current \t\t" + total;

            // Mejor tiempo de vuelta
            timer = ship.bestLap;
            if (timer != float.MaxValue) {
                minutes = Mathf.Floor(timer / 60).ToString("00");
                seconds = Mathf.RoundToInt(timer % 60).ToString("00");
                miliseconds = ((timer * 1000) % 1000).ToString("000");
            }
            else {
                minutes = "00";
                seconds = "00";
                miliseconds = "000";
            }
            total = string.Concat(new string[] { minutes, ":", seconds, ":", miliseconds });
            uiBestTimeText.text = "Best\t\t\t\t" + total;

            uiCurrentPositionText.text = ship.currentPosition + "/" + (!ServerSettings.isNetworked ? RaceSettings.ships.Count : ServerSettings.players.Count);

            // Sector
            if (ship.currentLap > 1 && ship.currentLap <= RaceSettings.laps) {
                if (ship.secondSector) {
                    diff = ship.sector1[ship.currentLap - 1] - ship.sector1[ship.bestLapIndex];
                }
                else {
                    diff = ship.laps[ship.currentLap - 2] - ship.bestLap;
                }

                if (ship.secondSector != displayCurrentSector && OnVariableChange != null) {
                    displayCurrentSector = ship.secondSector;
                    OnVariableChange(displayCurrentSector);
                }
                
            }

            // Tiempos de las ultimas 5 vueltas
            int min = 1;
            int max = 5;
            String laps = "";
            /*for (int i = min; i <= max; i++) {
                if (ship.laps.Length >= max) {
                    laps = "";
                    min += 1;
                    max += 1;
                }
                if (ship.laps.Length >= i)
                    laps += "Lap " + i + ": " + ship.laps[i - 1].ToString() + "\n";
            }*/
            uiLapsText.text = laps;

        }
    }



    private void VariableChangeHandler(bool newVal) {
        StartCoroutine(ShowSectorTime(diff, 2));
    }

    IEnumerator ShowSectorTime(float time, float delay) {

        //
        string text = "";
        if (diff > 0)
            text = "<color=red>+" + diff.ToString("0.000")+ "</color>";
        else if (diff < 0)
            text = "<color=green>"+ diff.ToString("0.000") + "</color>";
        else
            text = diff.ToString("0.000");

        uiCurrentSectorText.text = text;
        uiCurrentSectorText.enabled = true;
        yield return new WaitForSeconds(delay);
        uiCurrentSectorText.enabled = false;
    }
}