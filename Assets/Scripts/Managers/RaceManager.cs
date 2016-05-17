using UnityEngine;
using System.Collections;

public class RaceManager : MonoBehaviour {

    [Header("[ TRACK DATA ]")]
    public TrackData trackData;


    public RaceUI ui;
    public PauseManager pause;


    // Use this for initialization
    void Start () {
        RaceSettings.trackData = trackData;
        trackData.FindSpawnTiles();

        RaceSettings.raceManager = this;
        RaceSettings.ships.Clear();

        SetRaceSettings();
        SetRaceShips();

        // HUD
        GameObject hudUI = Instantiate(Resources.Load("HUD") as GameObject) as GameObject;
        ui = hudUI.GetComponent<RaceUI>();
        ui.ship = RaceSettings.ships[0];

        // Menu de pausa
        GameObject pauseUI = Instantiate(Resources.Load("Pause") as GameObject) as GameObject;
        pause = pauseUI.GetComponent<PauseManager>();
        pauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        // Pausa
        if (Input.GetButtonDown("Pause") || Input.GetKey(KeyCode.Escape) || (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Tab)) ||
                (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))) {
            Pause();
        }
    }

    private void SetRaceSettings() {
        RaceSettings.racers = 5;
        RaceSettings.laps = 5;
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

            loader.SpawnShip("Ship1", isAI);
        }

    }

    public void Pause() {
        GameSettings.PauseToggle();
        ui.gameObject.SetActive(!GameSettings.isPaused);

        pause.gameObject.SetActive(GameSettings.isPaused);

        Cursor.visible = GameSettings.isPaused;
    }
}
