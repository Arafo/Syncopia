using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class RacingLine : MonoBehaviour {

    public List<Point> racingLine = new List<Point>();
    public List<Point> bestLine = new List<Point>();
    private TrackTile currentTile;
    private CheckpointHit cpHit;
    private ShipController control;
    public int lap;
    public float bestLap;

	// Use this for initialization
	void Start () {
        cpHit = gameObject.GetComponent<CheckpointHit>();
        control = gameObject.GetComponent<ShipController>();
        lap = control.currentLap;
        bestLap = int.MaxValue;
        //racingLine = new List<TrackTile>();
    }
	
	// Update is called once per frame
	void Update () {

        //if (racingLine == null)
            //racingLine = new TrackTile[RaceSettings.trackData.trackData.tilesMapped.Count];

        if (cpHit.crash)
            ClearRacingLine();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0f, 1 << LayerMask.NameToLayer("Track"))) {
            TrackTile newTile = TileFromTriangleIndex(hit.triangleIndex, RaceSettings.trackData.trackData.tilesMapped);

            if (newTile != currentTile) {
                TrackTile start = new TrackTile();
                start.position = transform.position;
                //racingLine[newTile.index] = start;
                Point p = new Point();
                p.index = start.index;
                p.point = start.position;
                racingLine.Add(p);            
                //Debug.DrawLine(racingLine[currentTile.index].position, racingLine[newTile.index].position, Color.cyan);
                currentTile = newTile;
                //Debug.Log(currentTile.index);
            }

            // Pintar la linea de carrea actual
            for (int i=0; i < racingLine.Count - 1; i++) {
                Point next = FindNextTile(racingLine, i + 1);
                if (racingLine[i] != null && next != null)
                    Debug.DrawLine(racingLine[i].point, next.point, Color.magenta);
            }

            // Pintar la mejor linea de carrera
            for (int i = 0; i < bestLine.Count - 1; i++) {
                    Debug.DrawLine(bestLine[i].point, bestLine[i + 1].point, Color.black);
            }

            // Guardar linea de carrera
            if (control.currentLap != lap) {
                // Si el tiempo de la vuelta actual es el mejor
                if (control.bestLap < bestLap) {
                    XmlSerializer serializer = new XmlSerializer(typeof(PointContainer));
                    Debug.Log(Application.dataPath);
                    FileStream stream = new FileStream(Application.dataPath + "/racinline.xml", FileMode.Create);
                    PointContainer pc = new PointContainer();
                    pc.Points = racingLine;
                    serializer.Serialize(stream, pc);
                    stream.Close();
                    Debug.Log("FIN!" + racingLine.Count);
                    bestLine = racingLine;
                    bestLap = control.bestLap;
                }
                lap = control.currentLap;
                ClearRacingLine();
            }
        }

    }

    private Point FindNextTile(List<Point> tiles, int index) {
        Point tile = new Point();
        bool found = false;

        while (!found) {
            if (index >= tiles.Count)
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
        //for (int i = 0; i < racingLine.Length; i++) {
        //racingLine[i] = null;
        //}
        racingLine.Clear();
        racingLine = new List<Point>();
    }

    private TrackTile TileFromTriangleIndex(int index, List<TrackTile> mappedTiles) {
        if (mappedTiles[index * 3] != null)
            return mappedTiles[index * 3];
        else
            return null;
    }
}

[XmlRoot("PointCollection")]
public class PointContainer {
    [XmlArray("Points")]
    [XmlArrayItem("Point")]
    public List<Point> Points = new List<Point>();
}

public class Point {
    [XmlAttribute("index")]
    public int index;

    public Vector3 point;
}
