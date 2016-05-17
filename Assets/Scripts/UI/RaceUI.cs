using System;
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

    // Vuelta actual
    public GameObject uiCurrentLap;
    private Text uiCurrentLapText;

    // Maximo de vueltas
    public GameObject uiMaxLap;
    private Text uiMaxLapText;

    // Velocidad actual
    public GameObject uiCurrentSpeed;
    private Text uiCurrentSpeedText;

    //
    public GameObject uiSpeedMeter;
    private RectTransform uiSpeedMeterRect;

    //
    public GameObject uiLaps;
    private Text uiLapsText;

    private float uiSize = 1f;
    private float uiOpacity = 1f;

    void Start()
    {
        uiBestTimeText = uiBestTime.GetComponent<Text>();
        uiCurrentTimeText = uiCurrentTime.GetComponent<Text>();
        //uiTotalTimeText = uiTotalTime.GetComponent<Text>();
        uiCurrentLapText = uiCurrentLap.GetComponent<Text>();
        //uiMaxLapText = uiMaxLap.GetComponent<Text>();
        uiCurrentSpeedText = uiCurrentSpeed.GetComponent<Text>();

        //ship.control = ship.GetComponent<ShipController>();
        //ship.sim = ship.GetComponent<ShipSimulation>();
    }

    void Update()
    {
        // TODO: Iniciar en Start, no ejecutar esto cada frame
        //ship.control = ship.GetComponent<ShipController>();
        //ship.sim = ship.GetComponent<ShipSimulation>();
        uiCurrentSpeedText= uiCurrentSpeed.GetComponent<Text>();
        uiCurrentLapText = uiCurrentLap.GetComponent<Text>();
        uiSpeedMeterRect = uiSpeedMeter.GetComponent<RectTransform>();
        uiBestTimeText = uiBestTime.GetComponent<Text>();
        uiCurrentTimeText = uiCurrentTime.GetComponent<Text>();
        uiLapsText = uiLaps.GetComponent<Text>();


        if (!ship.finished)  {
            if (ship.sim.isBoosting) {
                uiSize = Mathf.Lerp(uiSize, 1.17f, Time.deltaTime * 7f);
                uiOpacity = Mathf.Lerp(uiOpacity, 0.3f, Time.deltaTime * 10f);
            }
            else {
                uiSize = Mathf.Lerp(uiSize, 1f, Time.deltaTime * 2f);
                uiOpacity = Mathf.Lerp(uiOpacity, 1f, Time.deltaTime * 10f);
            }

            uiRoot.transform.localScale = new Vector3(uiSize, uiSize, 1f);

            float x2 = ship.sim.gameObject.transform.InverseTransformDirection(ship.body.velocity).z * 0.8f;
            Vector2 rectSize = new Vector2(x2, uiSpeedMeterRect.sizeDelta.y);
            uiSpeedMeterRect.sizeDelta = rectSize;

            // Velocidad
            //uiCurrentTimeText.text = ship.control.totalS.ToString();
            int speed = Mathf.RoundToInt(ship.control.gameObject.GetComponent<ShipSimulation>().engineSpeed * 2f);
            uiCurrentSpeedText.text = speed.ToString() + " km/h";

            // Numero de vueltas
            uiCurrentLapText.text = "Lap: " + ship.currentLap.ToString() + "/" + RaceSettings.laps;

            // Tiempo total de la vuelta
            float timer = ship.currentTime;
            string minutes = Mathf.Floor(timer / 60).ToString("00");
            string seconds = Mathf.RoundToInt(timer % 60).ToString("00");
            string miliseconds = ((timer * 1000) % 1000).ToString("000");

            string total = string.Concat(new string[] { minutes, ":", seconds, ":", miliseconds });
            uiCurrentTimeText.text = "Current Lap\n" + total;

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
            uiBestTimeText.text = "Best Lap\n" + total;


            // Tiempos de las ultimas 5 vueltas
            int min = 1;
            int max = 5;
            String laps = "";
            for (int i = min; i <= max; i++) {
                if (ship.laps.Length >= max) {
                    laps = "";
                    min += 1;
                    max += 1;
                }
                if (ship.laps.Length >= i)
                    laps += "Lap " + i + ": " + ship.laps[i - 1].ToString() + "\n";
            }
            uiLapsText.text = laps;

        }
    }
}