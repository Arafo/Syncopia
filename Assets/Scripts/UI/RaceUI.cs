using System;
using UnityEngine;
using UnityEngine.UI;

public class RaceUI : MonoBehaviour
{
    public GameObject ship;
    public ShipController m_control;
    public ShipSimulation m_sim;

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

    private float uiSize = 1f;
    private float uiOpacity = 1f;

    void Start()
    {
        uiBestTimeText = uiBestTime.GetComponent<Text>();
        uiCurrentTimeText = uiCurrentTime.GetComponent<Text>();
        uiTotalTimeText = uiTotalTime.GetComponent<Text>();
        uiCurrentLapText = uiCurrentLap.GetComponent<Text>();
        uiMaxLapText = uiMaxLap.GetComponent<Text>();
        uiCurrentSpeedText = uiCurrentSpeed.GetComponent<Text>();

        m_control = ship.GetComponent<ShipController>();
        m_sim = ship.GetComponent<ShipSimulation>();
    }

    void Update()
    {
        // TODO: Iniciar en Start, no ejecutar esto cada frame
        m_control = ship.GetComponent<ShipController>();
        m_sim = ship.GetComponent<ShipSimulation>();
        uiCurrentSpeedText= uiCurrentSpeed.GetComponent<Text>();
        uiCurrentLapText = uiCurrentLap.GetComponent<Text>();


        if (!m_control.finishedRace)  {
            if (m_sim.isBoosting) {
                uiSize = Mathf.Lerp(uiSize, 1.17f, Time.deltaTime * 7f);
                uiOpacity = Mathf.Lerp(uiOpacity, 0.3f, Time.deltaTime * 10f);
            }
            else {
                uiSize = Mathf.Lerp(uiSize, 1f, Time.deltaTime * 2f);
                uiOpacity = Mathf.Lerp(uiOpacity, 1f, Time.deltaTime * 10f);
            }

            uiRoot.transform.localScale = new Vector3(uiSize, uiSize, 1f);

            //uiCurrentTimeText.text = m_control.totalS.ToString();
            int speed = Mathf.RoundToInt(m_control.gameObject.GetComponent<ShipSimulation>().engineSpeed * 2f);
            uiCurrentSpeedText.text = speed.ToString() + " km/h";

            uiCurrentLapText.text = "Lap: " + m_control.currentLap.ToString();
        }
    }
}