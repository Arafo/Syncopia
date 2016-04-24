using UnityEngine;
using System.Collections;

public class ShipReferer : MonoBehaviour {

    // IA
    public bool isAI;
    public bool autopilot;

    // Clases de una nave
    public ShipConfig config;
    public ShipController control;
    public ShipTrailEffects effects;
    public ShipInput input;
    public ShipSimulation sim;
    public ShipAI ai;

    // Componentes de una nave
    public GameObject axis;
    public GameObject mesh;
    public Camera cam;
    public Rigidbody body;

    public bool isRespawning;
    public bool facingFoward;

    public int boostState;
    public float boostAccel;
    public float boostTimer;

    // Tiempos
    public int place;
    public float[] laps;
    public bool[] perfects;
    public float bestLap;
    public int currentLap;

    public bool secondSector;
    public bool perfectLap;
    public float currentTime;
    public float totalTime;
    public float checkpoint;

    public bool hasBestTime;
    public bool loadedBestTime;
    public bool finished;
   
    void Start() {
        laps = new float[RaceSettings.laps];
        perfects = new bool[RaceSettings.laps];

        input.OnStart();
        effects.OnStart();
        ai.OnStart();
        control.OnStart();
    }

    void Update() {
        input.OnUpdate();
    }

    private void FixedUpdate() {
        control.OnUpdate();
        sim.OnUpdate();
        effects.OnUpdate();

        if (currentLap > 0)
            RaceTimers();


        if (boostTimer > 0) {
            boostTimer -= Time.deltaTime;
        }
        else {
            boostState = 0;
            boostAccel = 0;
        }
    }

    private void RaceTimers() {
        if (!finished) {
            totalTime += Time.deltaTime;
            currentTime += Time.deltaTime;
        }
    }

    public void HitSpeedPad() {
        if (boostState < 3) {
            boostTimer += 1.5f;
            //boostAccel += sim.m_engineThrust * 0.1f;
            boostState++;
        }
        else {
            boostTimer += 0.5f;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Checkpoint1") {
            if (secondSector || currentLap == 0) {
                UpdateLap();
                secondSector = false;
            }
        }

        if (other.tag == "Checkpoint2") {
            secondSector = true;
        }
    }


    private void UpdateLap() {
        if (currentLap > 0 && currentLap <= RaceSettings.laps) {
            // Tiempo de vuelta
            laps[currentLap - 1] = currentTime;

            // Vuelta perfecta
            perfects[currentLap - 1] = perfectLap;

            // Mejor tiempo
            if ((currentTime < bestLap || !hasBestTime) && !loadedBestTime) {
                bestLap = currentTime;
                hasBestTime = true;
            }

        }

        if (currentLap >= RaceSettings.laps) {
            if (!finished) {
                finished = true;
                if (!isAI) {
                    // Carrera acabada
                }
            }
        }

        currentTime = 0;
        perfectLap = true;
        currentLap++;
    }
}
