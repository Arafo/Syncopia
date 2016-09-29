using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ShipReferer : NetworkBehaviour {

    // IA
    public bool isAI;
    public bool isNetworked;
    public bool autopilot;
    private float aiSoundMinDistance = 0.1f;
    private float aiSoundMaxDistance = 1.0f;

    // Clases de una nave
    public ShipConfig config;
    public ShipController control;
    public ShipTrailEffects effects;
    public ShipInput input;
    public ShipSimulation sim;
    public ShipPosition position;
    public ShipAI ai;
    public ShipAudio music;

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

    public TrackSegment currentSection;
    public TrackSegment midSection;


    public int currentPosition;
    public int finalPosition;

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

        if (!isNetworked) {
            input.OnStart();
            effects.OnStart();
            music.OnStart();
            ai.OnStart();
            control.OnStart();
        }
        position.OnStart();


        secondSector = true;
        midSection = RaceSettings.raceManager.trackData.trackData.sections[RaceSettings.raceManager.trackData.trackData.sections.Count / 2];
    }

    void Update() {
        if (!isNetworked)
            input.OnUpdate();
    }

    private void FixedUpdate() {

        position.OnUpdate();

        if (isAI || autopilot)
            ai.OnUpdate();

        if (!isNetworked) {
            control.OnUpdate();
            sim.OnUpdate();
            effects.OnUpdate();
            music.OnUpdate();
        }

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
        Debug.Log(other.tag);
        if (other.tag == "Checkpoint1") {
            if (secondSector || currentLap == 0) {
                UpdateLap();
                secondSector = false;
            }
        }

        if (other.tag == "Checkpoint2") {
            secondSector = true;
            midSection = currentSection;
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
                finalPosition = currentPosition;
                if (!isAI && !ServerSettings.isNetworked) {
                    autopilot = true;
                    // Carrera acabada 

                    // Habilitar los resultados
                    RaceSettings.raceManager.results.gameObject.SetActive(true);
                    RaceSettings.raceManager.results.Results();

                    // Desactivar HUD
                    RaceSettings.raceManager.ui.gameObject.SetActive(false);
                }
            }

            // send finish to clients on network
            //if (ServerSettings.isNetworked)
                //RaceSettings.raceManager.mpmanager.RpcRaceEnding();
        }

        currentTime = 0;
        perfectLap = true;
        currentLap++;
    }

    public void PlayClip(AudioClip clip) {
        if (isAI || isNetworked) {
            ClipManager.CreateClip(clip, transform, AudioSettings.VOLUME_MAIN, 1.0f, aiSoundMinDistance, aiSoundMaxDistance);
        }
        else {
            ClipManager.CreateClip(clip, AudioSettings.VOLUME_MAIN, 1.0f);
        }
    }

    public void PlayClip(AudioClip clip, float volume, float pitch) {
        if (isAI || isNetworked) {
            ClipManager.CreateClip(clip, transform, volume * AudioSettings.VOLUME_MAIN, pitch, aiSoundMinDistance, aiSoundMaxDistance);
        }
        else {
            ClipManager.CreateClip(clip, volume * AudioSettings.VOLUME_MAIN, pitch);
        }
    }
}
