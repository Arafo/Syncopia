using UnityEngine;
using System.Collections;

public class RaceManager : MonoBehaviour {

    [Header("[ TRACK DATA ]")]
    public TrackData trackData;

    // Use this for initialization
    void Start () {
        RaceSettings.trackData = trackData;
        RaceSettings.raceManager = this;
        RaceSettings.ships.Clear();

        SetRaceSettings();
        SetRaceShips();
    }

    // Update is called once per frame
    void Update () {
	
	}

    private void SetRaceSettings() {
        //RaceSettings.racers = 1;
        RaceSettings.laps = 5;
    }

    private void SetRaceShips() {
        GameObject newShip = new GameObject("PLAYER SHIP");
        newShip.transform.position = new Vector3(0, 4, 0);
        //newShip.transform.rotation =;
        ShipLoader loader = newShip.AddComponent<ShipLoader>();

        float rot = newShip.transform.eulerAngles.y;
        newShip.transform.rotation = Quaternion.Euler(0.0f, rot, 0.0f);

        loader.SpawnShip("Ship1", false);

    }
}
