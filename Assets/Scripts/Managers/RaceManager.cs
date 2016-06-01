using UnityEngine;
using System.Collections;

public class RaceManager : MonoBehaviour {

    [Header("[ TRACK DATA ]")]
    public TrackData trackData;

    [Header("[ RACE SETTINGS ] ")]
    [Range(1, 8)]
    public int racerCount = 1;
    [Range(1, 10)]
    public int lapCount = 1;

    [Header("[ CUENTA ATRAS ]")]
    public float countdownStartDelay;
    public float countdownStageDelay;
    public float countdownTimer;
    public int countDownStage;

    public RaceUI ui;
    public PauseManager pause;
    public PauseManager results;

    public TrackSegment endSection;

    public bool musicManagerEnabled;
    public MusicManager musicManager;
    public AudioClip clipThree;
    public AudioClip clipTwo;
    public AudioClip clipOne;
    public AudioClip clipGo;

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
        GameObject hudUI = Instantiate(Resources.Load("HUD") as GameObject) as GameObject;
        ui = hudUI.GetComponent<RaceUI>();
        ui.ship = RaceSettings.ships[0];

        // Menu de pausa
        GameObject pauseUI = Instantiate(Resources.Load("Pause") as GameObject) as GameObject;
        pause = pauseUI.GetComponent<PauseManager>();
        pauseUI.SetActive(false);

        // Resultados
        GameObject ResultsUI = Instantiate(Resources.Load("Results") as GameObject) as GameObject;
        results = ResultsUI.GetComponent<PauseManager>();
        ResultsUI.SetActive(false);

        SetManagers();
    }

    // Update is called once per frame
    void Update() {
        // Pausa
        if (Input.GetButtonDown("Pause") || Input.GetKey(KeyCode.Escape) || (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Tab)) ||
                (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))) {
            if (!RaceSettings.ships[0].finished) {
                Pause();
            }
        }
        CalculatePosition();
    }

    void FixedUpdate() {
        if (!RaceSettings.countdownFinished) {
            if (RaceSettings.countdownReady) {
                if (countDownStage == 0) {
                    countdownTimer += Time.deltaTime;
                    if (countdownTimer > countdownStartDelay) {
                        ui.uiCountDownText.text = "3";
                        ClipManager.CreateOneShot(clipThree, AudioSettings.VOLUME_VOICES);
                        countdownTimer = 0.0f;
                        countDownStage++;
                    }
                }
                else if (countDownStage < 4) {
                    countdownTimer += Time.deltaTime;
                    if (countdownTimer > countdownStageDelay) {
                        countdownTimer = 0.0f;
                        countDownStage++;

                        if (countDownStage == 2) {
                            ui.uiCountDownText.text = "2";
                            ClipManager.CreateOneShot(clipTwo, AudioSettings.VOLUME_VOICES);
                        }

                        if (countDownStage == 3) {
                            ui.uiCountDownText.text = "1";
                            ClipManager.CreateOneShot(clipOne, AudioSettings.VOLUME_VOICES);
                        }

                    }
                }
                if (countDownStage == 4) {
                    ui.uiCountDownText.text = "GO!";
                    ClipManager.CreateOneShot(clipGo, AudioSettings.VOLUME_VOICES);
                    countdownTimer = 0.0f;
                    RaceSettings.countdownFinished = true;
                    RaceSettings.shipsRestrained = false;
                }
            }
        }
        else {
            countdownTimer += Time.deltaTime;
            if (countdownTimer > countdownStageDelay) {
                ui.uiCountDownText.text = "";
            }
        }
    }

    private void SetRaceSettings() {
        RaceSettings.raceManager = this;

        RaceSettings.shipsRestrained = true;
        RaceSettings.countdownFinished = false;
        RaceSettings.countdownReady = true;

        RaceSettings.ships.Clear();
        RaceSettings.racers = racerCount;
        RaceSettings.laps = lapCount;
    }

    private void SetRaceShips() {
        //GameObject newShip = new GameObject("PLAYER SHIP");
        //newShip.transform.position = new Vector3(0, 4, 0);
        //newShip.transform.rotation =;
        //ShipLoader loader = newShip.AddComponent<ShipLoader>();

        //float rot = newShip.transform.eulerAngles.y;
        //newShip.transform.rotation = Quaternion.Euler(0.0f, rot, 0.0f);

        //loader.SpawnShip("Ship1", false);

        for (int i = 0; i < RaceSettings.racers; i++) {
            bool isAI = (i != 0);

            GameObject newShip = new GameObject("PLAYER SHIP");
            //newShip.transform.position = new Vector3(0, 4, 0);
            //newShip.transform.rotation =;
            ShipLoader loader = newShip.AddComponent<ShipLoader>();

            float rot = newShip.transform.eulerAngles.y;
            newShip.transform.rotation = Quaternion.Euler(0.0f, rot, 0.0f);

            newShip.transform.position = RaceSettings.trackData.spawnPositions[i];
            newShip.transform.rotation = RaceSettings.trackData.spawnRotations[i];

            loader.SpawnShip("Flyer", isAI);
        }

    }

    public void Pause() {
        GameSettings.PauseToggle();
        ui.gameObject.SetActive(!GameSettings.isPaused);

        pause.gameObject.SetActive(GameSettings.isPaused);

        Cursor.visible = GameSettings.isPaused;
    }

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

    private void SetManagers() {

        // create music manager
        if (musicManagerEnabled) {
            GameObject newObj = new GameObject("MANAGER_MUSIC");
            musicManager = newObj.AddComponent<MusicManager>();
            musicManager.ship = RaceSettings.ships[0];
        }
    }

    private void CalculatePosition() {

        int position;
        int mid = RaceSettings.raceManager.trackData.trackData.sections[RaceSettings.raceManager.trackData.trackData.sections.Count / 2].index;

        for (int i = 0; i < RaceSettings.ships.Count; i++) {
            position = 1;
            ShipReferer ship1 = RaceSettings.ships[i];
            for (int j = 0; j < RaceSettings.ships.Count; j++) {
                ShipReferer ship2 = RaceSettings.ships[j];
                if (i != j) {
                    if (ship1.currentLap < ship2.currentLap) {
                        position += 1;
                    }
                    else {
                        int currentSection1 = ship1.currentSection.index;
                        int currentSection2 = ship2.currentSection.index;

                        if (ship1.secondSector)
                            if (ship1.currentSection.index < mid - 1)
                                currentSection1 = endSection.index + ship1.currentSection.index;

                        if (ship2.secondSector)
                            if (ship2.currentSection.index < mid - 1)
                                currentSection2 = endSection.index + ship2.currentSection.index;

                        if (currentSection1 < currentSection2) {
                            position += 1;
                        }
                        else if (currentSection1 == currentSection2) {
                            Vector3 pos1 = ship1.transform.position;
                            pos1.y = 0.0f;
                            Vector3 pos2 = ship1.currentSection.position;
                            pos2.y = 0.0f;
                            Vector3 pos3 = ship2.transform.position;
                            pos3.y = 0.0f;
                            if (Vector3.Distance(pos1, pos2) < Vector3.Distance(pos3, pos2))
                                position += 1;
                        }
                    }
                }
            }
            ship1.currentPosition = position;
        }
    }
}
