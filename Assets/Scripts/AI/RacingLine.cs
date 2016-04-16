using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacingLine : MonoBehaviour {

    private TrackTile[] racingLine;
    private TrackTile currentTile;
    private Agent agent;

	// Use this for initialization
	void Start () {
        agent = gameObject.GetComponent<Agent>();
        racingLine = new TrackTile[RaceSettings.trackData.trackData.tilesMapped.Count];
	}
	
	// Update is called once per frame
	void Update () {

        if (racingLine == null)
            racingLine = new TrackTile[RaceSettings.trackData.trackData.tilesMapped.Count];

        if (agent.hasFailed)
            ClearRacingLine();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0f, 1 << LayerMask.NameToLayer("Track"))) {
            TrackTile newTile = TileFromTriangleIndex(hit.triangleIndex, RaceSettings.trackData.trackData.tilesMapped);

            if (newTile != currentTile) {
                TrackTile start = new TrackTile();
                start.position = transform.position;
                racingLine[newTile.index] = start;
                //Debug.DrawLine(racingLine[currentTile.index].position, racingLine[newTile.index].position, Color.cyan);
                currentTile = newTile;
                Debug.Log(currentTile.index);
            }

            for (int i=0; i < racingLine.Length - 1; i++) {
                TrackTile next = FindNextTile(racingLine, i + 1);
                if (racingLine[i] != null && next != null)
                    Debug.DrawLine(racingLine[i].position, next.position, Color.magenta);
            }

            // Guardar linea de carrera
            if (RaceSettings.trackData.trackData.tilesMapped.Count == racingLine.Length) {
                Debug.Log("FIN!");
            }
        }

    }

    private TrackTile FindNextTile(TrackTile[] tiles, int index) {
        TrackTile tile = new TrackTile();
        bool found = false;

        while (!found) {
            if (index >= tiles.Length)
                index = 0;

            if (tiles[index] != null) {
                tile = tiles[index];
                found = true;
            }
            index++;
        }
        return tile;
    }

    private void ClearRacingLine() {
        for (int i = 0; i < racingLine.Length; i++) {
            racingLine[i] = null;
        }
    }

    private TrackTile TileFromTriangleIndex(int index, List<TrackTile> mappedTiles) {
        if (mappedTiles[index * 3] != null)
            return mappedTiles[index * 3];
        else
            return null;
    }
}
