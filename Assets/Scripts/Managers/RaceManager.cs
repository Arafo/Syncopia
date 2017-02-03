using UnityEngine;
using UnityStandardAssets.CinematicEffects;
using System.Collections.Generic;
using UnityEngine.Networking;
using Rewired;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona todas las acciones que ocurren durante una partida.
/// Desde el inicio donde se carga todo lo necesario para empezar
/// una carrera, hasta la ejecución donde se deben actualizar 
/// las posiciones de las naves.
/// </summary>
public class RaceManager : MonoBehaviour {

    [Header("[ TRACK DATA ]")]
    public TrackData trackData;

    [Header("[ RACE SETTINGS ] ")]
    [Range(1, 8)]
    public int racerCount = 1;
    [Range(1, 10)]
    public int lapCount = 1;
    public Enumerations.E_GAMEMODE mode;

    [Header("[ CUENTA ATRAS ]")]
    public float countdownStartDelay;
    public float countdownStageDelay;
    public float countdownTimer;
    public int countDownStage;

    public RaceUI ui;
    public PauseManager pause;
    public PauseManager results;

    public NetworkGameManager mpmanager;


    public TrackSegment endSection;

    public bool musicManagerEnabled;
    public MusicManager musicManager;
    public AudioClip clipThree;
    public AudioClip clipTwo;
    public AudioClip clipOne;
    public AudioClip clipGo;

    // Efectos
    private List<UnityStandardAssets.ImageEffects.Bloom> ppBlooms = new List<UnityStandardAssets.ImageEffects.Bloom>();
    private List<AntiAliasingManager> ppAA = new List<AntiAliasingManager>();
    private List<AmbientOcclusion> ppAO = new List<AmbientOcclusion>();
    private List<TonemappingColorGrading> ppTonemapping = new List<TonemappingColorGrading>();
    private List<DynamicResolution> ppDynamicResolution = new List<DynamicResolution>();

    // Use this for initialization
    void Start () {
        GameOptions.LoadGameSettings();
        GameSettings.CapFPS(GameSettings.GS_FRAMECAP);

        RaceSettings.trackData = trackData;

        trackData.FindSpawnTiles();

        SetRaceSettings();
        IndexSections();
        SetRaceShips();

        // HUD
        GameObject hudUI = Instantiate(Resources.Load("UI/HUD") as GameObject) as GameObject;
        ui = hudUI.GetComponent<RaceUI>();
        //ui.ship = RaceSettings.ships[0];

        // Menu de pausa
        //GameObject pauseUI = Instantiate(Resources.Load("UI/OPTIONS") as GameObject) as GameObject;
        //pause = pauseUI.GetComponent<PauseManager>();
        //pauseUI.SetActive(false);

        // Resultados
        GameObject ResultsUI = Instantiate(Resources.Load("UI/Results") as GameObject) as GameObject;
        results = ResultsUI.GetComponent<PauseManager>();
        ResultsUI.SetActive(false);

        SetManagers();
        AddImageEffects();
    }

    // Update is called once per frame
    void Update() {

        if (ui.ship == null && RaceSettings.ships.Count > 0)
            ui.ship = RaceSettings.ships[0];

        // Pausa
        if (ReInput.players.GetPlayer(0).GetButtonDown("Pause") || (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Tab)) ||
                (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))) {
            if (!RaceSettings.ships[0].finished) {               
                Pause();
            }
        }
        UpdateImageEffects();
        if (RaceSettings.gamemode == Enumerations.E_GAMEMODE.ARCADE || RaceSettings.gamemode == Enumerations.E_GAMEMODE.SEASSON)
            CalculatePosition();
    }

    /// <summary>
    /// Gestiona el contador de tiempo al inicio de la carrera
    /// </summary>
    void FixedUpdate() {
        if (!RaceSettings.countdownFinished) {
            if (RaceSettings.countdownReady) {

                // No se hace nada si es una partida en red y algún jugador no esta listo
                if (ServerSettings.isNetworked && !mpmanager.allPlayersSpawned) 
                    return;
                
                // No se hace nada si es una partida es red y no es el servidor
                if (ServerSettings.isNetworked && !NetworkServer.active) 
                    return;

                if (countDownStage == 0) {
                    countdownTimer += Time.deltaTime;
                    if (countdownTimer > countdownStartDelay) {
                        UpdateCounter("3");
                        ClipManager.CreateClip(clipThree, AudioSettings.VOLUME_VOICES);
                        countdownTimer = 0.0f;
                        countDownStage++;

                        if (NetworkServer.active)
                            mpmanager.RpcSetCountdown("3");
                    }
                }
                else if (countDownStage < 4) {
                    countdownTimer += Time.deltaTime;
                    if (countdownTimer > countdownStageDelay) {
                        countdownTimer = 0.0f;
                        countDownStage++;

                        if (countDownStage == 2) {
                            UpdateCounter("2");
                            ClipManager.CreateClip(clipTwo, AudioSettings.VOLUME_VOICES);

                            if (NetworkServer.active)
                                mpmanager.RpcSetCountdown("2");
                        }

                        if (countDownStage == 3) {
                            UpdateCounter("1");
                            ClipManager.CreateClip(clipOne, AudioSettings.VOLUME_VOICES);

                            if (NetworkServer.active)
                                mpmanager.RpcSetCountdown("1");
                        }

                    }
                }
                if (countDownStage == 4) {
                    UpdateCounter("GO!");
                    ClipManager.CreateClip(clipGo, AudioSettings.VOLUME_VOICES);

                    if (NetworkServer.active)
                        mpmanager.RpcSetCountdown("GO!");

                    countdownTimer = 0.0f;
                    RaceSettings.countdownFinished = true;
                    RaceSettings.shipsRestrained = false;
                }
            }
        }
        else {
            countdownTimer += Time.deltaTime;
            if (countdownTimer > countdownStageDelay && ServerSettings.raceCountdown <= 0) {
                UpdateCounter("");

                if (NetworkServer.active)
                    mpmanager.RpcSetCountdown("");
            }      
        }
    }

    /// <summary>
    /// Actualiza el texto del contador de tiempo
    /// </summary>
    /// <param name="text"></param>
    public void UpdateCounter(string text) {
        ui.uiCountDownText.text = text;
    }

    /// <summary>
    /// Actualiza el contador de resultados
    /// </summary>
    /// <param name="text"></param>
    public void UpdateCounterResults(string text) {
        results.countdown.text = text;
    }

    /// <summary>
    /// Actualiza los efectos gráficos en tiempo de ejecución
    /// </summary>
    private void UpdateImageEffects() {
        int i = 0;
        for (i = 0; i < ppBlooms.Count; ++i)
            ppBlooms[i].enabled = GameSettings.GS_BLOOM == 1;
        for (i = 0; i < ppAA.Count; ++i)
            ppAA[i].SelectAA(GameSettings.GS_AA, RaceSettings.ships[i].cam);//.enabled = GameSettings.GS_AA == 1;
        for (i = 0; i < ppAO.Count; ++i)
            ppAO[i].enabled = GameSettings.GS_AO;
        for (i = 0; i < ppTonemapping.Count; ++i)
            if (SceneIndexManager.SceneIndexFromName("Blood Dragon") == SceneManager.GetActiveScene().buildIndex)
                ppTonemapping[i].enabled = false;
            else
                ppTonemapping[i].enabled = GameSettings.GS_TONEMAPPING;
        for (i = 0; i < ppDynamicResolution.Count; ++i) {
            ppDynamicResolution[i].enabled = GameSettings.GS_DYNAMICRESOLUTION == 1;
            if (ppDynamicResolution[i].superSampling != null)
                ppDynamicResolution[i].superSampling.enabled = GameSettings.GS_DYNAMICRESOLUTION == 1 || 
                    GameSettings.GS_AA == (int)Enumerations.E_AA.SSAAx2 ||
                    GameSettings.GS_AA == (int)Enumerations.E_AA.SSAAx4 ||
                    GameSettings.GS_AA == (int)Enumerations.E_AA.SSAAx8 ;
        }

        for (i = 0; i < RaceSettings.ships.Count; ++i)
            if (RaceSettings.ships[i].cam != null)
                RaceSettings.ships[i].cam.hdr = GameSettings.GS_TONEMAPPING;
    }

    /// <summary>
    /// Establece las opciones de una carrera
    /// </summary>
    private void SetRaceSettings() {
        RaceSettings.raceManager = this;

        RaceSettings.shipsRestrained = true;
        RaceSettings.countdownFinished = false;
        RaceSettings.countdownReady = true;

        RaceSettings.ships.Clear();


        if (!RaceSettings.overrideRaceSettings) {
            RaceSettings.racers = racerCount;
            RaceSettings.laps = lapCount;
            //RaceSettings.gamemode = mode;
        }
    }

    /// <summary>
    /// Carga las naves de una carrera
    /// </summary>
    private void SetRaceShips() {

        if (ServerSettings.isNetworked)
            return;

        for (int i = 0; i < RaceSettings.racers; i++) {
            Enumerations.E_SHIPS ship = RaceSettings.playerShip;
            bool isAI = (i != 0);

            // TEST
            /*if (i > 2)
                return;*/

            GameObject newShip = new GameObject("PLAYER SHIP" + i);
            ShipLoader loader = newShip.AddComponent<ShipLoader>();
            //ShipLoaderTest loader = newShip.AddComponent<ShipLoaderTest>();

            float rot = newShip.transform.eulerAngles.y;
            newShip.transform.rotation = Quaternion.Euler(0.0f, rot, 0.0f);

            newShip.transform.position = RaceSettings.trackData.spawnPositions[i];
            newShip.transform.rotation = RaceSettings.trackData.spawnRotations[i];

            if (isAI) {
                int shipCount = System.Enum.GetNames(typeof(Enumerations.E_SHIPS)).Length;
                int rand = Random.Range(0, shipCount);
                ship = (Enumerations.E_SHIPS)rand;
            }

            // TEST
            /*if (i == 1) ship = Enumerations.E_SHIPS.TESTSHIP;
            if (i == 2) ship = Enumerations.E_SHIPS.FLYER;*/

            newShip.name = ship.ToString();
            int type = (isAI) ? 1 : 0;
            loader.SpawnShip(ship, type);
        }

    }

    /// <summary>
    /// Pausa el juego
    /// </summary>
    public void Pause() {
        GameSettings.PauseToggle();
        ui.gameObject.SetActive(!GameSettings.isPaused);
        pause.gameObject.transform.GetChild(0).gameObject.SetActive(GameSettings.isPaused);

        RaceSettings.ships[0].cam.GetComponent<Blur>().enabled = GameSettings.isPaused;
        //if (GameSettings.isPaused)
        //pause.InitParcialResults();

        // Reiniciar menu al salir de la pausa
        if (!GameSettings.isPaused)
            pause.ResetMenu();

        Cursor.visible = GameSettings.isPaused;
    }

    /// <summary>
    /// Indexa los segmentos de un circuito
    /// </summary>
    private void IndexSections() {
        TrackSegment[] sections = trackData.trackData.sections.ToArray();
        TrackSegment startSection = sections[trackData.trackData.sectionStart];
        startSection.index = 0;

        int index = 1;
        int maxIteration = 100000;
        int iteration = 0;

        TrackSegment currentSection = startSection.next;
        bool ended = false;
        while (!ended && iteration < maxIteration) {
            currentSection.index = index;
            currentSection = currentSection.next;
            if (currentSection.next == startSection)
                endSection = currentSection;

            if (currentSection == startSection)
                ended = true;

            ++index;
            ++iteration;
        }
    }

    /// <summary>
    /// Establece los managers de una carrera
    /// </summary>
    private void SetManagers() {

        // create music manager
        if (musicManagerEnabled) {
            GameObject newObj = new GameObject("MANAGER_MUSIC");
            musicManager = newObj.AddComponent<MusicManager>();
            //musicManager.ship = RaceSettings.ships[0];
        }
    }

    /// <summary>
    /// Añade los efectos gráficos 
    /// </summary>
    private void AddImageEffects() {
        for (int i = 0; i < RaceSettings.ships.Count; ++i) {
            if (RaceSettings.ships[i].cam != null) {

                // bloom
                //Kino.Bloom bloom = RaceSettings.ships[i].cam.gameObject.AddComponent<Kino.Bloom>();
                //bloom.radius = 1.0f;

                UnityStandardAssets.ImageEffects.Bloom bloom = RaceSettings.ships[i].cam.gameObject.AddComponent<UnityStandardAssets.ImageEffects.Bloom>();
                bloom.tweakMode = UnityStandardAssets.ImageEffects.Bloom.TweakMode.Complex;
                bloom.bloomIntensity = 0.1f;
                bloom.bloomThreshold = 4;
                bloom.bloomThresholdColor = new Color(0, 255, 202, 255);
                bloom.bloomBlurIterations = 1;
                bloom.sepBlurSpread = 4.09f;
                bloom.lensFlareShader = Shader.Find("Hidden/LensFlareCreate");
                bloom.screenBlendShader = Shader.Find("Hidden/BlendForBloom");
                bloom.blurAndFlaresShader = Shader.Find("Hidden/BlurAndFlares");
                bloom.brightPassFilterShader = Shader.Find("Hidden/BrightPassFilter2");
                ppBlooms.Add(bloom);

                // fxaa
                AntiAliasingManager aa = RaceSettings.ships[i].cam.gameObject.AddComponent<AntiAliasingManager>();
                ppAA.Add(aa);

                // ao
                AmbientOcclusion ao = RaceSettings.ships[i].cam.gameObject.AddComponent<AmbientOcclusion>();
                ao.settings.intensity = 1f;
                ao.settings.radius = 0.3f;
                ppAO.Add(ao);

                // tonemapping
                TonemappingColorGrading tm = RaceSettings.ships[i].cam.gameObject.AddComponent<TonemappingColorGrading>();
                TonemappingColorGrading.ColorGradingSettings gs = new TonemappingColorGrading.ColorGradingSettings {
                        enabled = true,
                        useDithering = false,
                        showDebug = false,
                        precision = TonemappingColorGrading.ColorGradingPrecision.Normal,
                        colorWheels = TonemappingColorGrading.ColorWheelsSettings.defaultSettings,
                        basics = new TonemappingColorGrading.BasicsSettings {
                            temperatureShift = 0.3f,
                            tint = 0f,
                            contrast = 1f,
                            hue = 0f,
                            saturation = 1f,
                            value = 1f,
                            vibrance = 0f,
                            gain = 1.5f,
                            gamma = 1f
                        },  
                        channelMixer = TonemappingColorGrading.ChannelMixerSettings.defaultSettings,
                        curves = TonemappingColorGrading.CurvesSettings.defaultSettings
                };
                tm.colorGrading = gs;
                ppTonemapping.Add(tm);

                // Dynamic Resolution
                DynamicResolution dr = RaceSettings.ships[i].cam.gameObject.AddComponent<DynamicResolution>();
                ppDynamicResolution.Add(dr);

                //CameraMotionBlur cmb = RaceSettings.ships[i].cam.gameObject.AddComponent<CameraMotionBlur>();
                //cmb.
            }
        }
    }

    /// <summary>
    /// Calcula la posición de una nave en el circuito
    /// </summary>
    private void CalculatePosition() {

        int position;
        //int mid = RaceSettings.raceManager.trackData.trackData.sections[RaceSettings.raceManager.trackData.trackData.sections.Count / 2].index;

        for (int i = 0; i < RaceSettings.ships.Count; i++) {
            position = 0;
            ShipReferer ship1 = RaceSettings.ships[i];
            for (int j = 0; j < RaceSettings.ships.Count; j++) {
                if (!(RaceSettings.ships[j] == ship1)) {
                    ShipReferer ship2 = RaceSettings.ships[j];
                    int num2 = ship1.currentSection.index;
                    int num3 = ship2.currentSection.index;

                    if (ship1.secondSector && ship1.currentSection.index < ship1.midSection.index - 1) {
                        num2 = endSection.index + ship1.currentSection.index;
                    }
                    if (ship2.secondSector && ship2.currentSection.index < ship2.midSection.index - 1) {
                        num3 = endSection.index + ship2.currentSection.index;
                    }
                    if (ship1.currentLap < ship2.currentLap) {
                        position++;
                    }
                    else if (ship1.currentLap == ship2.currentLap) {
                        if (num2 < num3) {
                            position++;
                        }
                        else if (num2 == num3) {
                            Vector3 pos = ship1.transform.position;
                            pos.y = 0f;
                            Vector3 position2 = ship1.currentSection.position;
                            position2.y = 0f;
                            Vector3 position3 = ship2.transform.position;
                            position3.y = 0f;
                            if (Vector3.Distance(pos, position2) < Vector3.Distance(position3, position2)) {
                                position++;
                            }
                        }
                    }                   
                }
            }
            ship1.currentPosition = position + 1;
        }
    }
}
